package com.mypoc.ptt.application;

import android.app.Application;
import android.os.Build;
import android.os.StrictMode;
import android.util.Log;

import androidx.lifecycle.MutableLiveData;

import com.baidu.mapapi.CoordType;
import com.baidu.mapapi.SDKInitializer;
import com.dds.skywebrtc.SkyEngineKit;
import com.mypoc.ptt.LoginActivity;
import com.mypoc.ptt.enums.PocSessionStatusEnum;
import com.mypoc.ptt.enums.PocTalkModeEnum;
import com.mypoc.ptt.pref.LoginPrefereces;
import com.mypoc.ptt.utils.Md5Utils;
import com.mypoc.ptt.webrtc.voip.VoipEvent;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.internal.PTTSDKImpl;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTUser;
import com.tencent.bugly.crashreport.CrashReport;

import java.util.ArrayList;
import java.util.List;


public class MyPOCApplication extends Application {

    private static String TAG = MyPOCApplication.class.getName();

    private IPTTSDK pttSDK;
    private static MyPOCApplication instance;

    private final Object lock = new Object(); // 专用锁对象
    /**
     * 记录整个会话的实时状态
     */
    private PocSessionStatusEnum pocSession = PocSessionStatusEnum.Idel;

    private PocTalkModeEnum pocTalkMode= PocTalkModeEnum.PTT;  //默认对讲模式

    private boolean isSingleCaller=false;  //单呼对讲下，是主叫，用于谁是主叫在退出时要释放(删除)单户临时组

    private int     peerUserId;            //单呼对讲下，对方ID

    private String  webrtcPeerUserName;    //webrtc呼叫下，主叫对方的名称或被叫时收到对方的名称

    /**
     * 用户默认登录的组ID，用于初始登录或从临时组退出时进入
     */
    private Integer defaultGroupId;


    /**
     * 上一个组ID，每次进入新组，要记忆上一个组ID
     */
    private Integer prevGroupId;

    /**
     * 当前进入的组
     */
    private Integer currGroupId;

    /**
     * 企业id, 在推流时要用到
     */
    private Integer cmpid;

    /**
     * 当前登录者的userId
     */
    private Integer userId;

    /**
     * 当前登录者的userId
     */
    private String phone;

    /**
     * 当前登录者的userName
     */
    private String  userName;

    /**
     * ptt键值
     */
    private int pttKeyVal=131;

    /**
     * ptt键的监听模式, true: 用广播，在pttservice完成  false: 用键值，在mainactivity的onkeyDown和onkeyUp中完成
     */
    private boolean pttUseBroadCastMode=true;

    /**
     * 所有用户
     */
    private MutableLiveData<List<PTTUser>>  allUsers = new MutableLiveData<>();

    /**
     * 固定组列表
     */
    private MutableLiveData<List<PTTGroup>> fixGroups = new MutableLiveData<>();

    /**
     * 临时组列表
     */
    private MutableLiveData<List<PTTGroup>> tempGroups = new MutableLiveData<>();

    /**
     * ptt键有效吗，当gb28181的讲话开始时，是无效的，当其结束时才有效
     */
    private boolean pttKeyValid=true;

    /**
     * 用户在app上修改配置的密码,默认为666666
     */
    private String  defaultAppconfigPwd = "666666";

    //以下是webrtc变量
    /**
     * 房间id
     */
    private String roomId = "";
    /**
     * 别人邀请的用户id
     */
    private String otherUserId = "";

    /**
     * 登录后的token, 在推拉rtsp流时，要参考鉴权编码
     */
    private String token="";

    @Override
    public void onCreate() {
        super.onCreate();
        instance = this;
        pttSDK = PTTSDKImpl.getInstance(this);
        pocSession= PocSessionStatusEnum.Idel;
        //初始化
        fixGroups.setValue(new ArrayList<>());  //主线程用 setValue, 子线程用postValue
        tempGroups.setValue(new ArrayList<>());  //主线程用 setValue, 子线程用postValue

        /*StrictMode.setVmPolicy(new StrictMode.VmPolicy.Builder()
                .detectLeakedClosableObjects()
                .penaltyLog()
                .build());*/

        if (Build.VERSION.SDK_INT >= 28) {
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.setVmPolicy(builder.build());
            builder.detectFileUriExposure();
        }

        Log.i(TAG,"PTTSDK CREATE OK");
        //
        // 初始化百度地图SDK
        initBaiduMapSDK();

        //建议在测试阶段建议设置成true，发布时设置为false。
        CrashReport.initCrashReport(getApplicationContext(), "e7c6d09ad5", true);

        //webrtc
        // init SkyEngineKit
        SkyEngineKit.init(new VoipEvent());

    }

    private void initBaiduMapSDK() {
        try {
            // 在使用SDK各组件之前初始化context信息，传入ApplicationContext
            SDKInitializer.setAgreePrivacy(getApplicationContext(), true); // 必须设置，否则定位失败
            SDKInitializer.setHttpsEnable(false);
            SDKInitializer.initialize(getApplicationContext());

            // 自4.3.0起，百度地图SDK所有接口均支持百度坐标和国测局坐标，用此方法设置您使用的坐标类型.
            // 包括BD09LL和GCJ02两种坐标，默认是BD09LL坐标。
            SDKInitializer.setCoordType(CoordType.BD09LL);
            Log.d(TAG, "百度地图SDK初始化成功");
        } catch (Exception e) {
            Log.e(TAG, "百度地图SDK初始化失败", e);
            e.printStackTrace();
        }

    }

    /**
     * 获取rtsp推流地址，这里假设流媒体服务器与rest服务器是同一台
     * rtsp://ip:port/cmpid/userid_{videoType}?callId=时间戳秒级&sign=md5("用户token的最后8个字符:mypoc:callid")
     * 注: videoType指： live 直播， moni: 指监控, 用于录像时区分,  sos 终端SOS一键推流: 用于sos推流区分
     * @return
     */
    public String getRtspUrl(String videoType){

        String serverIp= LoginPrefereces.getData_String(this, LoginPrefereces.serverAddrKey);
        String callId= String.valueOf(System.currentTimeMillis()/1000);
        String last8chars= this.token.substring(token.length()-8);

        try {
            return new StringBuilder()
                    .append("rtsp://")
                    .append(serverIp)
                    .append(":554/")
                    .append(cmpid)
                    .append("/")
                    .append(userId)
                    .append("_")
                    .append(videoType)
                    .append("?callId=")
                    .append(callId)
                    .append("&sign=")
                    .append(Md5Utils.getMd5(last8chars+":mypoc:"+callId).toLowerCase())
                    .toString();
        } catch (Exception e) {
            e.printStackTrace();
        }

        return "";

    }

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public String getWebrtcPeerUserName() {
        return webrtcPeerUserName;
    }

    public void setWebrtcPeerUserName(String webrtcPeerUserName) {
        this.webrtcPeerUserName = webrtcPeerUserName;
    }

    public int getPeerUserId() {
        return peerUserId;
    }

    public void setPeerUserId(int peerUserId) {
        this.peerUserId = peerUserId;
    }

    public boolean isSingleCaller() {
        return isSingleCaller;
    }

    public void setSingleCaller(boolean singleCaller) {
        isSingleCaller = singleCaller;
    }

    public String getDefaultAppconfigPwd() {
        return defaultAppconfigPwd;
    }

    public void setDefaultAppconfigPwd(String defaultAppconfigPwd) {
        this.defaultAppconfigPwd = defaultAppconfigPwd;
    }

    public boolean isPttKeyValid() {
        return pttKeyValid;
    }

    public void setPttKeyValid(boolean pttKeyValid) {
        this.pttKeyValid = pttKeyValid;
    }

    ///////对固定组列表的操作
    /**
     * 读取固定组列表
     * @return
     */
    public MutableLiveData<List<PTTGroup>> getFixGroups() {
        return fixGroups;
    }

    /**
     * 整体更新固定组列表
     * @param fixGroups
     */
    public void setFixGroups(List<PTTGroup> fixGroups) {
        this.fixGroups.postValue(fixGroups);  // 使用postValue确保线程安全
    }

    /**
     * 添加单个项目到固定组列表的辅助方法
     * @param item
     */
    public void addToFixGroups(PTTGroup item) {
        List<PTTGroup> currentList = this.fixGroups.getValue() != null
                ? new ArrayList<>( this.fixGroups.getValue())
                : new ArrayList<>();
        currentList.add(item);
        this.fixGroups.postValue(currentList);
    }

    /**
     * 从固定组列表中删除指定组ID的项
     * @param groupId
     * @return
     */
    public boolean removeFromFixGroups(int groupId){
        List<PTTGroup> currentList = this.fixGroups.getValue();
        if (currentList == null || currentList.isEmpty()) {
            return false;
        }
        List<PTTGroup> newList = new ArrayList<>();
        boolean removed = false;

        for (PTTGroup item : currentList) {
            if (item.getGroupId().equals(groupId)) {
                removed = true;
            } else {
                newList.add(item);
            }
        }

        if (removed) {
            this.fixGroups.postValue(newList);
        }
        return removed;
    }

    //////对临时组列表的操作
    /**
     * 读取临时组列表
     * @return
     */
    public MutableLiveData<List<PTTGroup>> getTempGroups() {
        return tempGroups;
    }

    /**
     * 整体更新临时组列表
     * @param tempGroups
     */
    public void setTempGroups(List<PTTGroup> tempGroups) {
        this.tempGroups.postValue(tempGroups);  // 使用postValue确保线程安全
    }

    /**
     * 添加单个项目到临时组列表的辅助方法
     * @param item
     */
    public void addToTempGroups(PTTGroup item) {
        List<PTTGroup> currentList = this.tempGroups.getValue() != null
                ? new ArrayList<>( this.tempGroups.getValue())
                : new ArrayList<>();
        currentList.add(item);
        this.tempGroups.postValue(currentList);
    }

    /**
     * 从临时组列表中删除指定组ID的项
     * @param groupId
     * @return
     */
    public boolean removeFromTempGroups(int groupId){
        List<PTTGroup> currentList = this.tempGroups.getValue();
        if (currentList == null || currentList.isEmpty()) {
            return false;
        }
        List<PTTGroup> newList = new ArrayList<>();
        boolean removed = false;

        for (PTTGroup item : currentList) {
            if (item.getGroupId().equals(groupId)) {
                removed = true;
            } else {
                newList.add(item);
            }
        }

        if (removed) {
            this.tempGroups.postValue(newList);
        }
        return removed;
    }

    /////////////////////////////////
    @Override
    public void onTerminate() {
        if (pttSDK!=null)
            pttSDK.release();
        super.onTerminate();
    }

    public static MyPOCApplication getInstance() {
        return instance;
    }

    public IPTTSDK getPttSDK() {
        return pttSDK;
    }

    public Integer getDefaultGroupId() {
        return defaultGroupId;
    }

    public void setDefaultGroupId(Integer defaultGroupId) {
        this.defaultGroupId = defaultGroupId;
    }

    public MutableLiveData<List<PTTUser>>  getAllUsers() {
        return allUsers;
    }

    public void setAllUsers(List<PTTUser> allUsers) {
        this.allUsers.postValue(allUsers);  // 主线程安全更新
    }

    public Integer getCmpid() {
        return cmpid;
    }

    public void setCmpid(Integer cmpid) {
        this.cmpid = cmpid;
    }

    public Integer getUserId() {
        return userId;
    }

    public void setUserId(Integer userId) {
        this.userId = userId;
    }

    public String getPhone() {
        return phone;
    }

    public void setPhone(String phone) {
        this.phone = phone;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public PocSessionStatusEnum getPocSession() {
        synchronized (lock) {
            return pocSession;
        }
    }

    public void setPocSession(PocSessionStatusEnum pocSession) {
        synchronized (lock) {
            this.pocSession = pocSession;
        }
    }

    public PocTalkModeEnum getPocTalkMode() {
        return pocTalkMode;
    }

    public void setPocTalkMode(PocTalkModeEnum pocTalkMode) {
        this.pocTalkMode = pocTalkMode;
    }


    public Integer getCurrGroupId() {
        return currGroupId;
    }

    //上一个组，也在这里处理
    public void setCurrGroupId(Integer currGroupId) {
        //这里记忆上一个组
        if (this.prevGroupId==null){
            //初始为空
            this.prevGroupId= currGroupId;
        } else {
            if (!this.prevGroupId.equals(currGroupId)){
                //不一样才处理
                this.prevGroupId= this.currGroupId;
            }
        }
        //然后再更新  this.currGroupId
        this.currGroupId = currGroupId;
    }

    /**
     * 强制指定
     * @param prevGroupId
     */
    public void setPrevGroupId(Integer prevGroupId) {
        this.prevGroupId = prevGroupId;
    }

    //获取上一个组ID
    public Integer getPrevGroupId() {
        return prevGroupId;
    }

    public int getPttKeyVal() {
        return pttKeyVal;
    }

    public void setPttKeyVal(int pttKeyVal) {
        this.pttKeyVal = pttKeyVal;
    }

    public boolean isPttUseBroadCastMode() {
        return pttUseBroadCastMode;
    }

    public void setPttUseBroadCastMode(boolean pttUseBroadCastMode) {
        this.pttUseBroadCastMode = pttUseBroadCastMode;
    }

    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }

    public String getOtherUserId() {
        return otherUserId;
    }

    public void setOtherUserId(String otherUserId) {
        this.otherUserId = otherUserId;
    }
}
