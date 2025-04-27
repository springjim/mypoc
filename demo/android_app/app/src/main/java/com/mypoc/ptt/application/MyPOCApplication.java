package com.mypoc.ptt.application;

import android.app.Application;
import android.os.Build;
import android.os.StrictMode;
import android.util.Log;

import androidx.lifecycle.MutableLiveData;

import com.mypoc.ptt.enums.PocSessionStatusEnum;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.internal.PTTSDKImpl;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTUser;

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
     * 当前登录者的userId
     */
    private Integer userId;

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

    public Integer getUserId() {
        return userId;
    }

    public void setUserId(Integer userId) {
        this.userId = userId;
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
}
