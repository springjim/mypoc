package com.mypoc.ptt.activity;

import androidx.appcompat.app.AlertDialog;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.viewpager.widget.ViewPager;

import android.Manifest;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.ServiceConnection;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.IBinder;
import android.os.Looper;
import android.os.Message;
import android.provider.Settings;
import android.speech.tts.TextToSpeech;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.LoginActivity;
import com.mypoc.ptt.activity.backgroud.RtpService;
import com.mypoc.ptt.activity.keypad.MainMenuActivity;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.R;
import com.mypoc.ptt.adapter.MyPocPageAdapter;
import com.mypoc.ptt.consts.ErrorCodeMessage;
import com.mypoc.ptt.enums.PocSessionStatusEnum;
import com.mypoc.ptt.enums.PocTalkModeEnum;
import com.mypoc.ptt.enums.SingleCallSignalEnum;
import com.mypoc.ptt.event.AvChatEvent;
import com.mypoc.ptt.event.EnterPrevGroupEvent;
import com.mypoc.ptt.event.ExitDeleteGroupEvent;
import com.mypoc.ptt.event.MeetInviteEvent;
import com.mypoc.ptt.event.SingleCallSignalEvent;
import com.mypoc.ptt.fragment.ProfileFragment;
import com.mypoc.ptt.pref.LoginPrefereces;
import com.mypoc.ptt.service.BaiduLocationService;
import com.mypoc.ptt.service.FloatingTalkService;
import com.mypoc.ptt.utils.POCBroadCast;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.ptt.webrtc.room.CallMultiActivity;
import com.mypoc.pttlibrary.callback.CommonCallback;
import com.mypoc.pttlibrary.callback.ReleaseMicCallback;
import com.mypoc.pttlibrary.callback.RequestMicCallback;
import com.mypoc.pttlibrary.enums.TalkStatusEnum;
import com.mypoc.pttlibrary.event.TalkStatusMessageEvent;
import com.mypoc.ptt.event.TtsSpeakEvent;
import com.mypoc.ptt.event.UpdateGroupMemberStausEvent;
import com.mypoc.ptt.event.UpdateWorkingGroupEvent;
import com.mypoc.ptt.fragment.ContactFragment;
import com.mypoc.ptt.fragment.GroupFragment;
import com.mypoc.ptt.fragment.TalkFragment;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.ptt.widget.MyPocViewPager;
import com.mypoc.ptt.widget.SoundDialog;
import com.mypoc.pttlibrary.api.IPTTEventListener;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.api.PTTState;
import com.mypoc.pttlibrary.callback.CmpAllUserCallback;
import com.mypoc.pttlibrary.callback.LogoutCallback;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;
import com.mypoc.pttlibrary.internal.tcp.message.SingleCallSignalMessage;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTUser;

import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.greenrobot.eventbus.ThreadMode;

import java.lang.ref.WeakReference;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Locale;
import java.util.concurrent.TimeoutException;

import butterknife.BindView;
import butterknife.ButterKnife;
import io.reactivex.Single;
import io.reactivex.android.schedulers.AndroidSchedulers;
import io.reactivex.schedulers.Schedulers;

public class MainActivity extends BaseActivity {

    private String TAG = "MainActivity";

    @BindView(R.id.group_name)
    TextView tvGroupName;  //当前组名称

    @BindView(R.id.main_media_img)
    ImageView ivMicStatus;  //麦状态图标

    @BindView(R.id.main_media_text)
    TextView tvMicDesc;   //麦状态描述

    @BindView(R.id.account_login)
    TextView tvAccount;

    @BindView(R.id.main_viewpager)
    MyPocViewPager vp;

    private MyPocPageAdapter adapter;
    private List<Fragment> fglist = new ArrayList<Fragment>();

    @BindView(R.id.re_mypoc_talk)
    RelativeLayout rlTalk;
    @BindView(R.id.iv_mypoc_talk)
    ImageView ivTalk;
    @BindView(R.id.tv_mypoc_talk)
    TextView tvTalk;

    @BindView(R.id.re_mypoc_member)
    RelativeLayout rlMember;
    @BindView(R.id.iv_mypoc_member)
    ImageView ivMember;
    @BindView(R.id.tv_mypoc_member)
    TextView tvMember;

    @BindView(R.id.re_mypoc_contact)
    RelativeLayout rlContact;
    @BindView(R.id.iv_mypoc_contact)
    ImageView ivContact;
    @BindView(R.id.tv_mypoc_contact)
    TextView tvContact;

    @BindView(R.id.re_mypoc_profile)
    RelativeLayout rlProfile;
    @BindView(R.id.iv_mypoc_profile)
    ImageView ivProfile;
    @BindView(R.id.tv_mypoc_profile)
    TextView tvProfile;

    private ImageView[] imagebuttons;
    private TextView[] textviews;

    private IPTTSDK pttSDK;

    //tts start
    private TextToSpeech textToSpeech;  //tts
    /**
     * tts 初始化完成状态
     */
    private boolean isTTSInitialized = false;
    private final Handler mHandler = new TTSHandler(this);
    //tts end

    //fragment
    private TalkFragment talkFragment;
    private GroupFragment groupFragment;
    private ContactFragment contactFragment;
    private ProfileFragment profileFragment;
    //点击选中的
    private int index;
    //当前滑动选的tab index
    private int currentTabIndex;

    //收到组邀请是否弹窗，还是自动进入, 这里设置为弹窗提示
    private boolean showGroupInviteDialog = true;
    SoundDialog groupInvitesoundDialog;
    //指示是否在被组邀请处理中..., 如果是: 随后的组同步指令要过滤掉
    private boolean groupInviteProcessing = false;

    ////////////////////////////////////////////////////////
    //单呼的邀请
    private boolean showSingleCallInviteDialog = true;  //弹窗
    SoundDialog singleCallInvitesoundDialog;
    private boolean singleCallInviteProcessing = false;

    //分享视频流对话框
    SoundDialog videoSharesoundDialog;


    ////////////////////////////////////////////////////////
    //收到webrtc视频会议邀请
    SoundDialog meetInvitesSoundDialog;

    ////////////

    //收到强制同步组，是否弹窗，还是自动进入，这里设置为自动进入
    private boolean showGroupSyncDialog = false;
    SoundDialog groupSyncsoundDialog;

    private int applyMicOwnerTimeoutMs = 5000;
    private Runnable applyMicOwnerTimeoutTask;

    //国标对讲的广播接收器,适配誉龙执法仪
    private BroadcastReceiver gbTalkBroadcastReceiver;

    //以下用于: 后台视频推流服务
    private RtpService rtpService;
    private boolean isServiceBound = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            Window window = getWindow();
            window.clearFlags(WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS
                    | WindowManager.LayoutParams.FLAG_TRANSLUCENT_NAVIGATION);
            window.getDecorView().setSystemUiVisibility(View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                    | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                    | View.SYSTEM_UI_FLAG_LAYOUT_STABLE);
            window.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS);
            window.setStatusBarColor(Color.TRANSPARENT);
            window.setNavigationBarColor(Color.TRANSPARENT);
        } else if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
            Window window = getWindow();
            window.setFlags(WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS,
                    WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS);
        }

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        EventBus.getDefault().register(this);
        ButterKnife.bind(this); //先绑定，再做 initListen事件

        initViews();
        addListener();
        initGroupPager();

        pttSDK = MyPOCApplication.getInstance().getPttSDK();
        addPttEventListener(pttSDK);  //监听各种SDK事件

        // 设置悬浮窗：用于显示组和麦状态
        checkOverlayPermissionAndFloating();

        //注册gb28181的对讲广播
        registerGBTalkBroadcastReceiver();

        getCmpAllUsers();  //获取cmp下所有用户，后面当作 "通讯录"fragment中的数据

        reportInitGroup();  //上报初始工作组，开启对讲或监听

        //最后获取一些固定组、临时组
        getFixAndTempGroups();

        // 绑定后台rtsp推流服务
        bindRtpService();

        //这里只简单写，实际情况下，请把 textToSpeech 放到service中写，可以在后台播放tts
        Log.d(TAG, "create TTS");
        textToSpeech = new TextToSpeech(this, new TextToSpeech.OnInitListener() {
            @Override
            public void onInit(int status) {
                if (status == TextToSpeech.SUCCESS) {
                    int result = textToSpeech.setLanguage(Locale.SIMPLIFIED_CHINESE);  //简体中文
                    if (result == TextToSpeech.LANG_MISSING_DATA || result == TextToSpeech.LANG_NOT_SUPPORTED) {
                        Log.e(TAG, "TTS中文简体语言不支持,缺少数据");

                    } else {
                        Log.e(TAG, "TextToSpeech init Success");
                        isTTSInitialized = true;
                        ttsSpeak("登录成功");
                    }
                } else {
                    Log.e(TAG, "TTS引擎初始化失败");

                }
            }
        });

        applyMicOwnerTimeoutTask = new Runnable() {
            @Override
            public void run() {

                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ApplyTimeout));

                Log.e(TAG, "申请麦权的超时时间");
            }
        };

    }

    // 服务连接回调
    private final ServiceConnection serviceConnection = new ServiceConnection() {
        @Override
        public void onServiceConnected(ComponentName className, IBinder service) {
            RtpService.RtpServiceBinder binder = (RtpService.RtpServiceBinder) service;
            rtpService = binder.getService();
            isServiceBound = true;
            Log.i(TAG, "RTP service connected");
            //updateStatus("Service connected");

            // 初始化RTSP URL（但不开始推流）
            initializeStreamSettings();
        }

        @Override
        public void onServiceDisconnected(ComponentName arg0) {
            rtpService = null;
            isServiceBound = false;
            Log.w(TAG, "RTP service disconnected");
            //updateStatus("Service disconnected");
        }
    };

    /**
     * 初始化推流设置（不开始推流）
     */
    private void initializeStreamSettings() {
        if (isServiceBound && rtpService != null) {
            // 设置RTSP URL
            rtpService.setRtspUrl("rtsp://192.168.101.140:554/345/100668_moni?callId=3663778888&sign=36673788888");

            // 设置默认分辨率（720p）
            rtpService.setVideoResolution(720, 1280);
            rtpService.setVideoBitrate(2500 * 1024);

            Log.i(TAG, "Stream settings initialized");
            //updateStatus("Ready to stream");
            //updateButtonStates(true);
        }
    }

    /**
     * 绑定RTP服务
     */
    private void bindRtpService() {

        Intent intent = new Intent(this, RtpService.class);

        // 预先设置RTSP URL（可选，也可以在initializeStreamSettings中设置）
        intent.putExtra("rtsp_url", "rtsp://192.168.101.140:554/345/100668_moni?callId=3663778888&sign=36673788888");
        intent.putExtra("video_width", 720);
        intent.putExtra("video_height", 1280);
        intent.putExtra("video_bitrate", 2500 * 1024);

        bindService(intent, serviceConnection, Context.BIND_AUTO_CREATE);
        startService(intent); // 确保服务启动
        //updateStatus("Binding service...");

    }

    private void registerGBTalkBroadcastReceiver() {

        // 创建广播接收器
        gbTalkBroadcastReceiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                if (POCBroadCast.BROADCAST_YDT_GBTALK.equalsIgnoreCase(intent.getAction())) {
                    Boolean value = intent.getBooleanExtra("talk", false);
                    // 处理接收到的数据
                    if (value) {
                        //国标对讲开始
                        MyPOCApplication.getInstance().setPttKeyValid(false);
                        pttSDK.setPttValid(false);

                        //如果这时，当前人正在对讲中,要中断并播报语音
                        if (MyPOCApplication.getInstance().getPocSession() == PocSessionStatusEnum.Speaking) {

                            //发出释放麦
                            pttSDK.releaseMicrophone(new ReleaseMicCallback() {
                                @Override
                                public void onSuccess() {
                                    //发送等待事件
                                    EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                                    MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                                }

                                @Override
                                public void onFailure(String error) {
                                    Log.e(TAG, "发送释放麦报错:" + error);
                                    MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                                }
                            });

                            //发送tts播报
                            EventBus.getDefault().post(new TtsSpeakEvent("平台对讲插入，麦已释放"));


                        }

                    } else {
                        MyPOCApplication.getInstance().setPttKeyValid(true);
                        pttSDK.setPttValid(true);
                    }

                    Toast.makeText(context, "收到国标对讲广播: " + value, Toast.LENGTH_LONG).show();
                }
            }
        };

        // 创建IntentFilter
        IntentFilter filter = new IntentFilter();
        filter.addAction(POCBroadCast.BROADCAST_YDT_GBTALK);
        // 注册接收器
        registerReceiver(gbTalkBroadcastReceiver, filter);

    }


    private void checkOverlayPermissionAndFloating() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M && !Settings.canDrawOverlays(this)) {
            Intent intent = new Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION,
                    Uri.parse("package:" + getPackageName()));
            //startActivityForResult(intent, OVERLAY_PERMISSION_CODE);
            //已经在LoginActivity检查过了

        } else {
            Intent serviceIntent = new Intent(this, FloatingTalkService.class);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                startForegroundService(serviceIntent);
            } else {
                startService(serviceIntent);
            }
        }

    }


    //////////////tts

    private static class TTSHandler extends Handler {
        private WeakReference<MainActivity> mActivity;

        public TTSHandler(MainActivity activity) {
            mActivity = new WeakReference<MainActivity>(activity);
        }

        @Override
        public void handleMessage(Message msg) {
            switch (msg.what) {
                default:
                    break;
            }
        }
    }

    private void ttsSpeak(String text) {
        if (textToSpeech != null && isTTSInitialized) {
            Log.e(TAG, "ttsSpeak,开始播报:" + text);
            //要放到UI线程去播报
            mHandler.postDelayed(new Runnable() {
                @Override
                public void run() {
                    textToSpeech.speak(text, TextToSpeech.QUEUE_FLUSH, null, null);
                }
            }, 1800);


        } else {
            //未创建或初始化失败
        }
    }

    //以下是对pttsdk的事件回调，一定要写
    private void addPttEventListener(IPTTSDK pttSDK) {
        pttSDK.setEventListener(new IPTTEventListener() {
            @Override
            public void onApplyMicSuccess() {
                Log.i(TAG, "收到抢麦成功事件");
                MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Speaking);
                //eventbus 事件发送
                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ApplySuccess));
            }

            @Override
            public void onApplyMicFailed(String reason) {
                Log.i(TAG, "收到抢麦失败事件");
                PocSessionStatusEnum currPocSessionStatus = MyPOCApplication.getInstance().getPocSession();
                if (currPocSessionStatus != PocSessionStatusEnum.Listening) {
                    //不是收听前提下的抢麦失败才能设为idel
                    MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                }

                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ApplyFail));

            }

            @Override
            public void onMicTimeout(String reason) {
                Log.i(TAG, "收到麦权超时事件");
                //如果是被麦权高的人打断了，这里就不能播报了
                ttsSpeak("讲话已超时");  //看情况，也可以不播报
            }

            @Override
            public void onMicOccupied() {
                ttsSpeak("麦克风被占用，麦已释放");
                Log.i(TAG, "收到麦克风被占用事件");

                //下面要延迟下，因为收到方，有个500ms显示收听的UI处理，所以这里要延迟大于这个值
                mHandler.postDelayed(() -> {
                    pttSDK.releaseMicrophone(new ReleaseMicCallback() {
                        @Override
                        public void onSuccess() {
                            Log.i(TAG, "收到麦克风被占用事件,麦已释放");
                            EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                            MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                        }

                        @Override
                        public void onFailure(String error) {
                            Log.i(TAG, "收到麦克风被占用事件,麦释放报错:" + error);
                            EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                            MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                        }
                    });
                }, 1000);

            }

            @Override
            public void onUserStartSpeaking(int groupId, int userId) {
                //某人正在某组讲话
                Log.i(TAG, String.format("收到 groupId=%d, userId=%d 正在讲话 ", groupId, userId));

                //todo 在非监听模式下，只有与当前组一致的才处理
                if (MyPOCApplication.getInstance().getCurrGroupId() != null &&
                        MyPOCApplication.getInstance().getCurrGroupId().equals(groupId)
                ) {

                    //再延迟500ms发送
                    //2025.2.11 这里要注意，在低麦权讲话被打断的用户，会立马发送释放麦报文，其它端会收到无人讲话(在先收到)，随后高麦权人又发送了讲话报文（这个在后），所以要延迟下执行，
                    //要不然会显示错乱的顺序，即正确的顺序是： 1: 先收到被打断的人的42停止报文(这时显示空闲)  2：后收到打断人的讲话42报文，所以第2步要延迟执行下
                    new Handler(getMainLooper()).postDelayed(
                            () -> {
                                MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Listening);
                                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ListenStart, groupId, userId));
                            }
                            , 200);


                } else {
                    //是监听别的组情况

                    // 同时messageId=100 的语音包要解码出来
                }


            }

            @Override
            public void onUserStopSpeaking(int groupId, int userId) {
                //某人正在某组停止讲话
                Log.i(TAG, String.format("收到 groupId=%d, userId=%d 正在停止讲话 ", groupId, userId));

                if (MyPOCApplication.getInstance().getCurrGroupId() != null &&
                        MyPOCApplication.getInstance().getCurrGroupId().equals(groupId)
                ) {

                    //也要延迟执行，不然停止会在开始前面，(当另一方在很快0.5之内开始又停止时), 否则会出顺序错乱
                    new Handler(getMainLooper()).postDelayed(
                            () -> {
                                MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ListenStop, groupId, userId));
                            }
                            , 200);


                } else {
                    //是监听别的组情况

                }

            }

            @Override
            public void onUserJoinedGroup(int groupId, int userId) {
                //某人进入某组,UI上要显示在线在组的图标
                //更新livedata, 以便其它订阅来更新相应的UI
                PttHelper.updateAllUserStatus(userId, 1);

                EventBus.getDefault().post(new UpdateGroupMemberStausEvent(groupId, userId,
                        UpdateGroupMemberStausEvent.Status_EnterGroup, null));
            }

            @Override
            public void onUserLeftGroup(int groupId, int userId) {
                //某人离开某组，UI上要显示在线，但不在组
                //更新livedata, 以便其它订阅来更新相应的UI
                EventBus.getDefault().post(new UpdateGroupMemberStausEvent(groupId, userId,
                        UpdateGroupMemberStausEvent.Status_ExitGroup, null));

            }

            @Override
            public void onUserOffline(int userId) {
                //某人下线了或掉线了,UI上要显示所有组离线图标
                //更新livedata, 以便其它订阅来更新相应的UI
                PttHelper.updateAllUserStatus(userId, 0);
                EventBus.getDefault().post(new UpdateGroupMemberStausEvent(-1, userId,
                        UpdateGroupMemberStausEvent.Status_Offline, null));
            }

            @Override
            public void onGroupInviteTwoWay(int groupId, String groupName, int userId, int inviteUserId, int inviteMode) {
                //双向对讲技术不成熟，废弃掉 2025.7
            }

            @Override
            public void onSingleCallSignal(int groupId, int fromUserId, int toUserId, byte signalVal) {
                //单呼信令处理
                Log.i(TAG, String.format("收到单呼信令（2个人）, groupId=%d, fromUserId=%d, toUserId=%d, signalVal=%d",
                        groupId, fromUserId, toUserId, signalVal));

                if (toUserId != MyPOCApplication.getInstance().getUserId()) {
                    Log.i(TAG, "不是给自己的，不用处理");
                    return;
                }

                SingleCallSignalEnum signalEnum= SingleCallSignalEnum.fromByte(signalVal);

                if (signalEnum.equals(SingleCallSignalEnum.INVITE)){

                    //判断当前是正在有单呼中...
                    if (MyPOCApplication.getInstance().getPocTalkMode()== PocTalkModeEnum.PTT_SINGLE &&
                            MyPOCApplication.getInstance().getPeerUserId()!= fromUserId
                    ){
                        //正在与别人在单呼中
                        pttSDK.sendSingleCallSignal(groupId, MyPOCApplication.getInstance().getUserId(), fromUserId,
                                SingleCallSignalEnum.BUSING.getValue(), new CommonCallback() {
                                    @Override
                                    public void onSuccess() {
                                        Log.i(TAG, "发送单呼信令成功");
                                    }

                                    @Override
                                    public void onFailure(String error) {
                                        Log.e(TAG, "发送单呼信令失败,"+ error);
                                    }
                                }
                        );
                        return;
                    }

                    //有否国标对讲中, todo... 也在这里处理，然后回发  SingleCallSignalEnum.BUSING，再return

                    //先回发响铃
                    pttSDK.sendSingleCallSignal(groupId, MyPOCApplication.getInstance().getUserId(), fromUserId,
                            SingleCallSignalEnum.RING.getValue(), new CommonCallback() {
                                @Override
                                public void onSuccess() {
                                    Log.i(TAG, "发送单呼信令成功");
                                }

                                @Override
                                public void onFailure(String error) {
                                    Log.e(TAG, "发送单呼信令失败,"+ error);
                                }
                            }
                    );

                    //只是呼叫邀请
                    MyPOCApplication.getInstance().setSingleCaller(false); //被叫
                    singleCallInviteProcessing =true;
                    String inviteUserName = PttHelper.findUserName(fromUserId);
                    int oldGroupId= MyPOCApplication.getInstance().getCurrGroupId();
                    //
                    singleCallInvitesoundDialog = new SoundDialog(MainActivity.this,
                            "提示",
                            "收到【" + inviteUserName + "】单呼邀请，是否接受",
                            new SoundDialog.DialogListener() {
                                @Override
                                public void onConfirm() {
                                    //用户点击确认的处理
                                    //发送给groupfragment进入组事件，并播报tts语音
                                    PTTGroup pttGroupExt = new PTTGroup(groupId, "单呼:"+inviteUserName,  fromUserId, System.currentTimeMillis() / 1000);
                                    MyPOCApplication.getInstance().addToTempGroups(pttGroupExt);  //让groupfragment 会感知更新，在被叫退出时，还要删除它

                                    pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                        @Override
                                        public void onSuccess() {
                                            ttsSpeak("进入单呼" );
                                            MyPOCApplication.getInstance().setPeerUserId(fromUserId);
                                            MyPOCApplication.getInstance().setPocTalkMode(PocTalkModeEnum.PTT_SINGLE);

                                            singleCallInvitesoundDialog.dismiss(); //关闭
                                            singleCallInviteProcessing = false;
                                            //进入单呼页面
                                            Intent intent = new Intent(MainActivity.this, PttSingleCallActivity.class);
                                            intent.putExtra(PttSingleCallActivity.KEY_PREV_GROUP_ID,oldGroupId);
                                            intent.putExtra(PttSingleCallActivity.KEY_GROUP_ID, groupId);
                                            intent.putExtra(PttSingleCallActivity.KEY_PEER_NAME, inviteUserName);
                                            intent.putExtra(PttSingleCallActivity.KEY_PEER_USERID, fromUserId);
                                            intent.putExtra(PttSingleCallActivity.KEY_IS_CALLER,false);
                                            startActivity(intent);

                                        }

                                        @Override
                                        public void onFailure(String error) {
                                            ttsSpeak("进入失败");
                                            Log.e(TAG, error);
                                            singleCallInvitesoundDialog.dismiss(); //关闭
                                            singleCallInviteProcessing = false;
                                        }
                                    });

                                }

                                @Override
                                public void onCancel() {
                                    // 用户点击取消的处理
                                    //Toast.makeText(MainActivity.this, "取消点击", Toast.LENGTH_SHORT).show();
                                    pttSDK.sendSingleCallSignal(groupId, MyPOCApplication.getInstance().getUserId(), fromUserId,
                                            SingleCallSignalEnum.REFUSE.getValue(), new CommonCallback() {
                                                @Override
                                                public void onSuccess() {
                                                    Log.i(TAG, "发送单呼信令成功");
                                                }

                                                @Override
                                                public void onFailure(String error) {
                                                    Log.e(TAG, "发送单呼信令失败,"+ error);
                                                }
                                            }
                                    );

                                    singleCallInvitesoundDialog.dismiss(); //关闭
                                    singleCallInviteProcessing = false;
                                }

                                @Override
                                public void onAutoDismiss() {
                                    //弹窗60秒没操作自动关闭的回调
                                    pttSDK.sendSingleCallSignal(groupId, MyPOCApplication.getInstance().getUserId(), fromUserId,
                                            SingleCallSignalEnum.TIMEOUT.getValue(), new CommonCallback() {
                                                @Override
                                                public void onSuccess() {
                                                    Log.i(TAG, "发送单呼信令成功");
                                                }

                                                @Override
                                                public void onFailure(String error) {
                                                    Log.e(TAG, "发送单呼信令失败,"+ error);
                                                }
                                            }
                                    );

                                }
                            });

                    singleCallInvitesoundDialog.show();


                } else if (signalEnum.equals(SingleCallSignalEnum.LEAVE)){
                    //对方离开，如果是主叫方离开，被叫方还未接的话
                    if (singleCallInviteProcessing){
                        if (singleCallInvitesoundDialog!=null)
                            singleCallInvitesoundDialog.dismiss();
                        //
                        MyPOCApplication.getInstance().removeFromTempGroups(groupId);
                    } else {
                        //发出事件
                        EventBus.getDefault().post(new SingleCallSignalEvent(groupId,fromUserId,toUserId,signalVal));

                    }

                } else {

                    //发出事件
                    EventBus.getDefault().post(new SingleCallSignalEvent(groupId,fromUserId,toUserId,signalVal));

                }

            }

            @Override
            public void onGroupInvite(int groupId, String groupName, int userId, int inviteUserId) {
                //收到建临组邀请对讲模式
                Log.i(TAG, String.format("收到建临组邀请（大于2个人）, groupId=%d, groupName=%s, userId=%d,inviteUserId=%d",
                        groupId, groupName, userId, inviteUserId));

                //如果邀请人是自己，则不用处理，因为是自己发出去的
                if (inviteUserId == MyPOCApplication.getInstance().getUserId()) {
                    Log.i(TAG, "自己发出去的创建临时组，不用处理");
                    return;
                }

                groupInviteProcessing = true;

                PTTGroup pttGroupExt = new PTTGroup(groupId, groupName, inviteUserId, System.currentTimeMillis() / 1000);
                MyPOCApplication.getInstance().addToTempGroups(pttGroupExt);  //让groupfragment 会感知更新

                String inviteUserName = PttHelper.findUserName(inviteUserId);

                //一般这里是要弹出提醒用户， 是手动确认进入这个邀请临时组，还是自动进入
                if (showGroupInviteDialog) {
                    groupInvitesoundDialog = new SoundDialog(MainActivity.this,
                            "提示",
                            "收到【" + inviteUserName + "】组邀请，是否进入",
                            new SoundDialog.DialogListener() {
                                @Override
                                public void onConfirm() {
                                    //用户点击确认的处理
                                    //发送给groupfragment进入组事件，并播报tts语音
                                    pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                        @Override
                                        public void onSuccess() {
                                            ttsSpeak("进入" + groupName);
                                            EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId, groupName));
                                            groupInvitesoundDialog.dismiss(); //关闭
                                            groupInviteProcessing = false;
                                        }

                                        @Override
                                        public void onFailure(String error) {
                                            ttsSpeak("进入失败");
                                            Log.e(TAG, error);
                                            groupInvitesoundDialog.dismiss(); //关闭
                                            groupInviteProcessing = false;
                                        }
                                    });


                                }

                                @Override
                                public void onCancel() {
                                    // 用户点击取消的处理
                                    //Toast.makeText(MainActivity.this, "取消点击", Toast.LENGTH_SHORT).show();
                                    groupInvitesoundDialog.dismiss(); //关闭
                                    groupInviteProcessing = false;
                                }

                                @Override
                                public void onAutoDismiss() {
                                    //60秒超时没操作关闭的回调
                                }
                            });

                    groupInvitesoundDialog.show();

                } else {
                    //直接进入, 因为随后会有组同步 groupSync, 所以这里务必要延迟下，以便后面收到的groupSync不用重复处理
                    new Handler(Looper.getMainLooper()).postDelayed(
                            () -> {
                                pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                    @Override
                                    public void onSuccess() {
                                        ttsSpeak("进入" + groupName);
                                        EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId, groupName));
                                        groupInviteProcessing = false;
                                    }

                                    @Override
                                    public void onFailure(String error) {
                                        ttsSpeak("进入失败");
                                        Log.e(TAG, error);
                                        groupInviteProcessing = false;
                                    }
                                });
                            }
                            , 500);

                }

            }

            @Override
            public void onGroupDelete(int groupId) {
                //收到组解散的通知，一般是临时组或广播组
                Log.i(TAG, "收到组解散:" + groupId);
                if (MyPOCApplication.getInstance().getCurrGroupId().equals(groupId)) {
                    MyPOCApplication.getInstance().removeFromTempGroups(groupId);
                    //todo... 这里不管是对讲模式或 双向模式的临时组，都执行下，关闭双向模式，恢复状态
                    pttSDK.closeTwoWay();

                    //
                    enterPrevGroup(null);
                } else {
                    MyPOCApplication.getInstance().removeFromTempGroups(groupId);
                }
                //发送事件，让临时组的窗口处理
                EventBus.getDefault().post(new ExitDeleteGroupEvent(groupId));

            }

            @Override
            public void onGroupUserChange(int groupId, int groupTypeId, int changeType, String userStr) {
                //发送事件, 以便其它页面订阅同步状态用
                if (changeType == 0) {
                    //强踢的用户
                    EventBus.getDefault().post(new UpdateGroupMemberStausEvent(groupId, -1, UpdateGroupMemberStausEvent.Status_KickOutGroup, userStr));
                } else if (changeType == 1) {
                    //强拉的用户
                    EventBus.getDefault().post(new UpdateGroupMemberStausEvent(groupId, -1, UpdateGroupMemberStausEvent.Status_PullInGroup, userStr));
                }
                //
                if (changeType == 0) {
                    //有人被强踢
                    Log.i(TAG, "收到强踢: groupId=" + groupId + ",人员有:" + userStr);

                    //查看人员名单中是否有自己
                    //如果是自己被强踢了，要退出组
                    String[] userArr = userStr.split(",");
                    if (Arrays.asList(userArr).contains(MyPOCApplication.getInstance().getUserId() + "")) {

                        if (groupTypeId == 1)
                            MyPOCApplication.getInstance().removeFromTempGroups(groupId);
                        else if (groupTypeId == 0)
                            MyPOCApplication.getInstance().removeFromFixGroups(groupId);

                        //如果是当前组，还要退到上一个组
                        if (MyPOCApplication.getInstance().getCurrGroupId().equals(groupId))
                            enterPrevGroup(null);
                        else {
                            //不是当前组不用处理
                        }

                    }


                } else {
                    //强插，查询这个组
                    //查强插人员名单有没有自己
                    String[] userArr = userStr.split(",");
                    if (!Arrays.asList(userArr).contains(MyPOCApplication.getInstance().getUserId() + ""))
                        return;

                    Single.fromCallable(() -> {
                                if (groupTypeId == 1)
                                    return pttSDK.queryTempGroupInfo(groupId);
                                else
                                    return pttSDK.queryGroupInfo(groupId);
                            }).subscribeOn(Schedulers.io())
                            .observeOn(AndroidSchedulers.mainThread())
                            .subscribe(
                                    pttGroup -> {
                                        // 处理成功的结果
                                        PTTGroup pttGroupExt = new PTTGroup(groupId, pttGroup.getGroupName(), -1, System.currentTimeMillis() / 1000);
                                        MyPOCApplication.getInstance().addToTempGroups(pttGroupExt);  //让groupfragment 会感知更新

                                        //自动进入该组
                                        pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                            @Override
                                            public void onSuccess() {
                                                ttsSpeak("进入" + pttGroup.getGroupName());
                                                EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId, pttGroup.getGroupName()));
                                            }

                                            @Override
                                            public void onFailure(String error) {
                                                ttsSpeak("进入失败");
                                                Log.e(TAG, error);
                                            }
                                        });
                                    },
                                    throwable -> {
                                        // 处理错误
                                        Log.e(TAG, "获取组信息失败:" + throwable);
                                    }
                            );

                }

            }

            @Override
            public void onAVChatMessage(short videoType, short videoCommand, int fromUserId, int toUserId, String fromUserName, String toUserName, String desc) {
                //收到avchat消息
                Log.i(TAG, "收到onAVChatMessage");
                //如果是调取监控的请求, 则就地处理
                if (videoType==4){

                    switch (videoCommand){
                        case 1:
                            //请求监控
                            if (isServiceBound && rtpService != null) {
                                //要动态解析,监控端要求的分辨率来设置最大码率, 这时都按16:9 来显示横屏,好看点
                                if (!TextUtils.isEmpty(desc)){
                                    if (desc.equalsIgnoreCase("240p")){
                                        rtpService.setVideoResolution(240,426);
                                        rtpService.setVideoBitrate(500*1024);

                                    } else if (desc.equalsIgnoreCase("360p")){
                                        rtpService.setVideoResolution(360,640);
                                        rtpService.setVideoBitrate(800*1024);

                                    } else if (desc.equalsIgnoreCase("480p")){
                                        rtpService.setVideoResolution(480,640);
                                        rtpService.setVideoBitrate(1500*1024);

                                    } else if (desc.equalsIgnoreCase("720p")){
                                        rtpService.setVideoResolution(720,1280);
                                        rtpService.setVideoBitrate(3000*1024);

                                    } else if (desc.equalsIgnoreCase("1080p")){
                                        rtpService.setVideoResolution(1080,1920);
                                        rtpService.setVideoBitrate(6000*1024);
                                    }

                                }

                                rtpService.setRtspUrl(MyPOCApplication.getInstance().getRtspUrl("moni"));

                                boolean success = rtpService.startStreaming();
                                if (success) {
                                    Log.i(TAG,"后台推流成功");
                                } else {
                                    Log.e(TAG,"后台推流失败");
                                }
                            }
                            break;
                        case 4:
                            //挂断,停止监控
                            if (isServiceBound && rtpService != null) {
                                rtpService.stopStream();
                                Log.i(TAG,"后台停止推流");
                            }
                            break;
                        case 6:
                            //切换摄像头
                            if (isServiceBound && rtpService != null) {
                                rtpService.switchCamera();
                                Log.i(TAG, "后台Camera switched");
                            }
                            break;
                        default:
                            break;
                    }

                    return;
                } else if (videoType==5 ){
                    //分享视频流
                    videoSharesoundDialog = new SoundDialog(MainActivity.this,
                            "提示",
                            "收到【" + fromUserName + "】发来的在线视频分享，是否打开",
                            new SoundDialog.DialogListener() {
                                @Override
                                public void onConfirm() {
                                    //用户点击确认的处理
                                    Intent intent=new Intent(MainActivity.this,RtspPlayActivity.class);
                                    intent.putExtra(RtspPlayActivity.KEY_RTSP_URL, desc);
                                    intent.putExtra(RtspPlayActivity.KEY_USER_ID, fromUserId);
                                    intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT |
                                            Intent.FLAG_ACTIVITY_SINGLE_TOP);
                                    startActivity(intent);
                                }

                                @Override
                                public void onCancel() {
                                    // 用户点击取消的处理
                                    //Toast.makeText(MainActivity.this, "取消点击", Toast.LENGTH_SHORT).show();
                                    videoSharesoundDialog.dismiss(); //关闭

                                }

                                @Override
                                public void onAutoDismiss() {
                                    //60秒超时没操作关闭的回调
                                }
                            });

                    videoSharesoundDialog.show();
                    return;
                }
                //出出事件通知，在rtspActivity中处理
                EventBus.getDefault().post(new AvChatEvent(videoType,videoCommand,
                        fromUserId,toUserId,fromUserName,toUserName,desc));

            }

            @Override
            public void onGroupSync(int groupId, String groupName, int userId, int inviteId) {
                //
                Log.i(TAG, "收到组同步通知: groupId=" + groupId + ",groupName=" + groupName);

                String syncUserName = PttHelper.findUserName(inviteId); //出现同步命令的用户名

                ////注意: 一般邀请方可能随后会发一个同步组的信令，所以下面如果是手动确认的话，还要处理:
                //即： 在邀请还没有确认时，这里的同步就不能处理了
                if (groupInviteProcessing) {
                    //邀请弹窗还在处理中, 这里跳过了, 这里不考虑并发，双向的通话的同步，应该在发送端不要发同步组消息，切记
                    Log.i(TAG, "邀请弹窗还在处理中, 这里跳过了");
                    return;
                }

                //一般这里是要弹出提醒用户， 是手动确认进入这个组，还是自动进入
                if (showGroupSyncDialog) {

                    groupSyncsoundDialog = new SoundDialog(MainActivity.this,
                            "提示",
                            "收到【" + syncUserName + "】同步对讲组请求，是否进入",
                            new SoundDialog.DialogListener() {
                                @Override
                                public void onConfirm() {
                                    // 用户点击确认的处理
                                    //发送给groupfragment进入组事件，并播报tts语音
                                    pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                        @Override
                                        public void onSuccess() {
                                            ttsSpeak("进入" + groupName);
                                            EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId, groupName));
                                            groupSyncsoundDialog.dismiss();
                                        }

                                        @Override
                                        public void onFailure(String error) {
                                            ttsSpeak("进入失败");
                                            Log.e(TAG, error);
                                            groupSyncsoundDialog.dismiss();
                                        }
                                    });


                                }

                                @Override
                                public void onCancel() {
                                    // 用户点击取消的处理
                                    //Toast.makeText(MainActivity.this, "取消点击", Toast.LENGTH_SHORT).show();
                                }

                                @Override
                                public void onAutoDismiss() {

                                }
                            });

                    groupSyncsoundDialog.show();

                } else {
                    //直接进入
                    pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                        @Override
                        public void onSuccess() {
                            ttsSpeak("进入" + groupName);
                            EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId, groupName));
                        }

                        @Override
                        public void onFailure(String error) {
                            ttsSpeak("进入失败");
                            Log.e(TAG, error);
                        }
                    });

                }

            }

            @Override
            public void onSpeakingBreaked(int groupId, int userId) {
                //UI同步显示某人说话，并播报打断的tts语音
                Log.e(TAG, "被麦权高的人打断");
                ttsSpeak("讲话被打断");
                if (MyPOCApplication.getInstance().getCurrGroupId() != null &&
                        MyPOCApplication.getInstance().getCurrGroupId().equals(groupId)
                ) {
                    //切换为收听UI，要延后500 ms, 因为
                    new Handler(getMainLooper()).postDelayed(
                            () -> {
                                MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Listening);
                                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ListenStart, groupId, userId));
                            }
                            , 500);

                }

            }

            @Override
            public void onAudioDataReceived(String userId, byte[] audioData) {
                //一般用于前端展示音频波形用，这里略
            }

            @Override
            public void onConnectionStateChanged(PTTState newState) {
                Log.i(TAG, "SDK 状态:" + newState.toString());

                if (newState.equals(PTTState.TCP_CONNECTED)) {
                    EventBus.getDefault().post(new TtsSpeakEvent("网络连接成功，已上线"));
                } else if (newState.equals(PTTState.TCP_DISCONNECTED)) {
                    EventBus.getDefault().post(new TtsSpeakEvent("网络断开，已下线"));
                }

            }

            @Override
            public void onRemoteKill() {
                //
                Log.e(TAG, "收到帐号被遥毙");
                ttsSpeak("你的帐号被遥毙 即将退出");
                //延时执行退出任务
                mHandler.postDelayed(
                        () -> {

                            //设为false, 下次开机不会自动登录
                            LoginPrefereces.setState( MainActivity.this,LoginPrefereces.isLoggedInKey,false);
                            Intent intent= new Intent(MainActivity.this, FloatingTalkService.class);
                            stopService(intent);

                            //要先关闭服务，要不然是Service.START_STICKY,在退出时又自动创建
                            if (pttSDK != null)
                                pttSDK.release();

                            finish();

                            //清空任务栈，返回登录页面
                            Intent intentLogin = new Intent(MainActivity.this, LoginActivity.class);
                            intentLogin.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                            startActivity(intentLogin);

                            //android.os.Process.killProcess(android.os.Process.myPid());
                            //System.exit(0);

                        }, 6000
                );

            }

            @Override
            public void onKickOff() {
                Log.e(TAG, "收到帐号被踢");
                ttsSpeak("你的帐号被踢 即将退出应用");
                //延时执行退出任务
                mHandler.postDelayed(
                        () -> {

                            //设为false, 下次开机不会自动登录
                            LoginPrefereces.setState( MainActivity.this,LoginPrefereces.isLoggedInKey,false);

                            Intent intent= new Intent(MainActivity.this, FloatingTalkService.class);
                            stopService(intent);

                            //要先关闭服务，要不然是Service.START_STICKY,在退出时又自动创建
                            if (pttSDK != null)
                                pttSDK.release();

                            finish();

                            //清空任务栈，返回登录页面
                            Intent intentLogin = new Intent(MainActivity.this, LoginActivity.class);
                            intentLogin.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                            startActivity(intentLogin);

                            //android.os.Process.killProcess(android.os.Process.myPid());
                            //System.exit(0);

                        }, 6000
                );

            }

            @Override
            public void onErrorCode(int errorCode) {
                Log.e(TAG, "收到SDK的错误码: "+ errorCode);

                switch (errorCode){

                    case ErrorCodeMessage.WORKGROUP_NOFOUND:
                    case ErrorCodeMessage.WORKGROUP_INVALID:
                        Log.e(TAG,"收到工作组错误: "+errorCode);
                        //要重新上报工作组
                        if (MainActivity.this.pttSDK==null)
                            return;

                        MainActivity.this.pttSDK.joinGroup(MyPOCApplication.getInstance().getCurrGroupId(),
                                new ReportWorkGroupCallback() {
                                    @Override
                                    public void onSuccess() {
                                        Log.i(TAG,"上报工作组成功 ");
                                    }

                                    @Override
                                    public void onFailure(String error) {
                                        Log.e(TAG,"上报工作组失败: "+error);
                                    }
                                }
                        );

                        break;

                    case ErrorCodeMessage.PROHIBIT_TALK:
                        //被遥晕，禁止讲话，仅收听
                        break;

                    case ErrorCodeMessage.OTHER_TALKING:
                        //同话权的人正在讲话，无权打断

                        break;

                    default:
                        Log.e(TAG,"未处理的errorCode: "+errorCode);
                        break;
                }

            }

            @Override
            public void onError(String error) {
                Log.e(TAG, "收到SDK的异常信息: "+ error);
            }
        });
    }


    //解散或被强踢后，进入上一个组
    private void enterPrevGroup(Integer prevGroupId) {
        //自动回到默认组，如果没有回到上一个组
        Integer defaultGroupId = MyPOCApplication.getInstance().getDefaultGroupId();
        Log.i(TAG, "defaultGroupId=" + defaultGroupId);

        int enterGroupId = -1;

        if (prevGroupId != null) {
            //指定了
            Log.i(TAG, "指定了:" + prevGroupId);
            enterGroupId = prevGroupId;

        } else {

            if (defaultGroupId == null) {
                //进入上一个组
                if (MyPOCApplication.getInstance().getPrevGroupId() == null) {
                    //为空
                    enterGroupId = PttHelper.getFirstGroupIdFromApplication(-1);
                    Log.i(TAG, "MyPOCApplication.getInstance().getPrevGroupId()=" + enterGroupId);

                } else {
                    //不为空，但有可能被删除了，也要去找一遍
                    enterGroupId = MyPOCApplication.getInstance().getPrevGroupId();
                    Log.i(TAG, "PrevGroupId不为空,=" + enterGroupId);
                }
            } else {
                //进入默认组
                enterGroupId = defaultGroupId;
            }
        }


        //开始切换组
        if (enterGroupId != -1) {

            String groupName = PttHelper.findGroupName(enterGroupId);
            Log.i(TAG, "组名:" + groupName);

            if (groupName.equalsIgnoreCase(PttHelper.NO_FOUND_GROUP)) {
                //没有找到，可能是已经删除的，或是被强踢出了，这时要重新找groupId,
                enterGroupId = PttHelper.getFirstGroupIdFromApplication(-1);
                groupName = PttHelper.findGroupName(enterGroupId);
            }
            //发送给groupfragment进入组事件，并播报tts语音
            int finalEnterGroupId = enterGroupId;
            String finalGroupName = groupName;

            pttSDK.joinGroup(finalEnterGroupId, new ReportWorkGroupCallback() {
                @Override
                public void onSuccess() {
                    ttsSpeak("进入" + finalGroupName);
                    EventBus.getDefault().post(new UpdateWorkingGroupEvent(finalEnterGroupId, finalGroupName));
                }

                @Override
                public void onFailure(String error) {
                    ttsSpeak("切换组失败");
                    Log.e(TAG, error);
                }
            });

        } else {
            Log.e(TAG, "无法进入组, 因为未找到或没有组了");
        }
    }

    private void getFixAndTempGroups() {

        new Thread(() -> {

            try {

                List<PTTGroup> fixGroups = pttSDK.queryFixGroups();
                if (fixGroups != null && fixGroups.size() > 0) {
                    Log.i(TAG, "存储固定组信息");
                    MyPOCApplication.getInstance().setFixGroups(fixGroups);
                }

                List<PTTGroup> tempGroups = pttSDK.queryTempGroups();
                if (tempGroups != null && tempGroups.size() > 0) {
                    Log.i(TAG, "存储临时组信息");
                    MyPOCApplication.getInstance().setTempGroups(tempGroups);
                }

            } catch (InterruptedException | TimeoutException e) {
                runOnUiThread(() -> {
                    Log.e("PTT", "Error fetching data", e);
                    Toast.makeText(this, "Error: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                });
            }
        }).start();

    }

    private void getCmpAllUsers() {

        pttSDK.queryCmpAllUsers(new CmpAllUserCallback() {
            @Override
            public void onSuccess(List<PTTUser> pttCmpUsers) {
                MyPOCApplication.getInstance().setAllUsers(pttCmpUsers);
                Log.i(TAG, "getCmpAllUsers成功");
            }

            @Override
            public void onFailure(String error) {
                Log.e(TAG, "getCmpAllUsers失败:" + error);
            }
        });

    }

    /**
     * 存储一些user信息, 包含一些定位和权限等设置
     * @param pttUser
     */
    private void saveUserInfo(PTTUser pttUser){

        //单呼权限
        String privSingleCall= pttUser.getPrivSinglecall();
        if (TextUtils.isEmpty(privSingleCall) || privSingleCall.equalsIgnoreCase("N"))
            LoginPrefereces.setData_String(MainActivity.this,
                    LoginPrefereces.privSingleCallKey, "N");
        else
            LoginPrefereces.setData_String(MainActivity.this,
                    LoginPrefereces.privSingleCallKey, "Y");

        //上报定位
        String flagAutoLocation = pttUser.getFlagAutoLocation();
        if (TextUtils.isEmpty(flagAutoLocation) || flagAutoLocation.equalsIgnoreCase("N"))
            LoginPrefereces.setData_String(MainActivity.this,
                    LoginPrefereces.flagAutoLocationKey, "N");
        else
            LoginPrefereces.setData_String(MainActivity.this,
                    LoginPrefereces.flagAutoLocationKey, "Y");

        //定位精度 locationMode
        Integer locationMode = pttUser.getLocationMode();
        if (locationMode==null)
            LoginPrefereces.setData_Int(MainActivity.this, LoginPrefereces.locationModeKey,0);
        else
            LoginPrefereces.setData_Int(MainActivity.this, LoginPrefereces.locationModeKey,locationMode);

        //定位问隔 单位:秒
        Integer locationInterval= pttUser.getLocationInterval();
        if (locationInterval==null ||  locationInterval.equals(0))
            LoginPrefereces.setData_Int(MainActivity.this, LoginPrefereces.locationIntervalKey,60);
        else
            LoginPrefereces.setData_Int(MainActivity.this, LoginPrefereces.locationIntervalKey,locationInterval);


        if ( !TextUtils.isEmpty(flagAutoLocation)  && flagAutoLocation.equalsIgnoreCase("Y")){
            Log.i(TAG,"开启定位服务...");
            mHandler.post( () -> {
                startLocationService();
            });

        }

    }

    /**
     * 启动GPS定位服务
     */
    private void startLocationService(){
        // 检查权限
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION)
                == PackageManager.PERMISSION_GRANTED) {
            // 启动定位服务
            Intent serviceIntent = new Intent(this, BaiduLocationService.class);
            startService(serviceIntent);
        } else  {
            Log.e(TAG, "app定位权限未开通");
        }
    }

    /**
     * 上报初始工作组，开启对讲或监听
     */
    private void reportInitGroup() {

        new Thread(() -> {
            try {

                PTTUser pttUser = pttSDK.userinfo();
                if (pttUser == null) {
                    Log.e(TAG, "获取userinfo失败");
                    //触发eventbus消息
                    return;
                }

                //存储一些user信息
                saveUserInfo(pttUser);

                if (pttUser.getDefaultAppconfigPwd() != null)
                    MyPOCApplication.getInstance().setDefaultAppconfigPwd(pttUser.getDefaultAppconfigPwd());

                Log.i(TAG,"USERID="+ pttUser.getUserId());

                MyPOCApplication.getInstance().setUserId(pttUser.getUserId());
                MyPOCApplication.getInstance().setCmpid(pttUser.getCmpid());

                MyPOCApplication.getInstance().setPhone(pttUser.getPhone());

                MyPOCApplication.getInstance().setUserName(pttUser.getUserName());

                if (pttUser.getDefaultGroupId() != null) {
                    //有默认组，则优先进入
                    MyPOCApplication.getInstance().setDefaultGroupId(pttUser.getDefaultGroupId()); //保存默认组ID
                    MyPOCApplication.getInstance().setCurrGroupId(pttUser.getDefaultGroupId()); //记录当前登录组

                } else {
                    Integer groupId = getFirstGroupId();
                    if (groupId.equals(0)) {
                        Log.e(TAG, "无法获取组 ");
                        //触发eventbus消息
                    } else {

                        MyPOCApplication.getInstance().setCurrGroupId(groupId); //记录当前登录组

                    }
                }
                //进入这个组
                String groupName = "";
                if (MyPOCApplication.getInstance().getCurrGroupId() != null) {
                    //
                    PTTGroup pttGroup = pttSDK.queryGroupInfo(MyPOCApplication.getInstance().getCurrGroupId());
                    if (pttGroup != null) {
                        groupName = pttGroup.getGroupName();
                    } else {
                        pttGroup = pttSDK.queryTempGroupInfo(MyPOCApplication.getInstance().getCurrGroupId());
                        if (pttGroup != null)
                            groupName = pttGroup.getGroupName();
                    }

                    //上报
                    String finalGroupName_ = groupName;
                    pttSDK.joinGroup(MyPOCApplication.getInstance().getCurrGroupId(), new ReportWorkGroupCallback() {
                        @Override
                        public void onSuccess() {
                            Log.i(TAG, "上报工作组ID: " + MyPOCApplication.getInstance().getCurrGroupId());
                            EventBus.getDefault().post(new UpdateWorkingGroupEvent(MyPOCApplication.getInstance().getCurrGroupId(),
                                    finalGroupName_));

                        }

                        @Override
                        public void onFailure(String error) {
                            Log.e(TAG, "上报工作组失败: " + error);
                        }
                    });

                    String finalGroupName = groupName;
                    runOnUiThread(() -> {
                        Log.d("PTT", "UserInfo: " + pttUser);
                        // 更新 UI
                        if (!TextUtils.isEmpty(finalGroupName)) {
                            tvGroupName.setText(finalGroupName);
                            tvAccount.setText(MyPOCApplication.getInstance().getUserId()+"");
                            Intent updateIntent = new Intent(FloatingTalkService.BROADCAST_UPDATE_FLOATING_WINDOW);
                            updateIntent.putExtra("group", finalGroupName);
                            sendBroadcast(updateIntent);
                        }

                    });

                }

            } catch (InterruptedException | TimeoutException e) {
                runOnUiThread(() -> {
                    Log.e("PTT", "Error fetching data", e);
                    Toast.makeText(this, "Error: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                });
            }

        }).start();

    }

    /**
     * 当用户没有设定默认组时，要从参与的固定组或临时组查找，优先顺序时固定，再找临时组
     * 这个方法要在子线程同步执行
     */
    private Integer getFirstGroupId() throws InterruptedException, TimeoutException {
        //
        List<PTTGroup> pttGroups = pttSDK.queryFixGroups();
        if (pttGroups != null && pttGroups.size() > 0) {
            return pttGroups.get(0).getGroupId();
        }

        pttGroups = pttSDK.queryTempGroups();
        if (pttGroups != null && pttGroups.size() > 0) {
            return pttGroups.get(0).getGroupId();
        }

        return 0;
    }

    @Override
    public void onBackPressed() {
        //按键对讲机，是不能放到后台的
        moveTaskToBack(true);
    }

    private void completeQuitDialog(final Activity context) {
        AlertDialog.Builder builder = new AlertDialog.Builder((Context) context);
        builder.setMessage("确认要退出应用吗?");
        builder.setTitle("提示");
        builder.setPositiveButton(R.string.exitComplete, new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface param1DialogInterface, int param1Int) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.append("completeQuitDialog() - 当前要销毁的activity：");
                stringBuilder.append(context);
                Log.e("MainActivity", stringBuilder.toString());

                //先主动退出socket服务
                pttSDK.logout(new LogoutCallback() {
                    @Override
                    public void onSuccess() {
                        Log.i(TAG, "发送socket logout成功");
                        exitApp();
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG, "发送socket logout失败" + error);
                        exitApp();
                    }
                });


            }
        });
        builder.setNegativeButton(R.string.cancel, new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface param1DialogInterface, int param1Int) {
                param1DialogInterface.dismiss();
            }
        });
        builder.create().show();
    }

    private void exitApp() {
        MainActivity.this.finish();
        //要先关闭服务，要不然是Service.START_STICKY,在退出时又自动创建
        if (pttSDK != null)
            pttSDK.release();

        android.os.Process.killProcess(android.os.Process.myPid());
        System.exit(0);
    }

    public void onTabClicked(View view) {

        switch (view.getId()) {
            case R.id.re_mypoc_talk:
                index = 0;
                break;
            case R.id.re_mypoc_member:
                index = 1;
                break;
            case R.id.re_mypoc_contact:
                index = 2;
                break;
            case R.id.re_mypoc_profile:
                index = 3;
                break;
        }

        imagebuttons[currentTabIndex].setSelected(false);

        // 把当前tab设为选中状态
        imagebuttons[index].setSelected(true);
        textviews[currentTabIndex].setTextColor(getResources().getColor(R.color.mypoc_bottomtab_txtcolor));
        textviews[index].setTextColor(getResources().getColor(R.color.white));

        if (currentTabIndex != index) {
            if (vp != null && index < adapter.getCount()) {
                vp.setCurrentItem(index);
            }
        }

        if (index == 0) {

            rlTalk.setBackgroundResource(R.color.mypoc_bg_color);

            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.transparent);

        } else if (index == 1) {

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.mypoc_bg_color);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.transparent);

        } else if (index == 2) {

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.mypoc_bg_color);
            rlProfile.setBackgroundResource(R.color.transparent);
        } else if (index == 3) {

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.mypoc_bg_color);
        }

        currentTabIndex = index;
    }

    //webrtc的会议邀请
    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onMeetInvite(MeetInviteEvent event){

        if (event==null) return;

        String inviteUserName= PttHelper.findUserName(Integer.valueOf(event.getFromUserId()));
        meetInvitesSoundDialog = new SoundDialog(MyPOCApplication.getInstance(),
                "提示",
                "收到【" + inviteUserName + "】会议邀请，是否进入",
                new SoundDialog.DialogListener() {
                    @Override
                    public void onConfirm() {
                        //用户点击确认的处理
                        CallMultiActivity.openActivity(MyPOCApplication.getInstance(), event.getRoom(),
                                false,"");

                    }

                    @Override
                    public void onCancel() {
                        // 用户点击取消的处理
                        //Toast.makeText(MainActivity.this, "取消点击", Toast.LENGTH_SHORT).show();
                        meetInvitesSoundDialog.dismiss(); //关闭

                    }

                    @Override
                    public void onAutoDismiss() {

                    }
                });

        meetInvitesSoundDialog.show();

    }


    //这里更新顶部当前组名称
    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onUpdateWorkingGroup(UpdateWorkingGroupEvent event) {
        tvGroupName.setText(event.getGroupName());
        //再通知悬浮窗
        Intent updateIntent = new Intent(FloatingTalkService.BROADCAST_UPDATE_FLOATING_WINDOW);
        updateIntent.putExtra("group", event.getGroupName());
        sendBroadcast(updateIntent);

    }

    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onEnterPrevGroup(EnterPrevGroupEvent event) {
        enterPrevGroup(event.getPrevGroupId());
    }

    /**
     * 麦权申请状态，订阅它更新麦权UI和麦权描述
     */
    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onTalkStatusMessage(TalkStatusMessageEvent messageEvent) {
        Log.i(TAG, "messageEvent=" + messageEvent);
        TalkStatusEnum talkStatus = messageEvent.getStatus();
        //
        boolean needNotifyFloatWindow = false; //是否要通知悬浮窗
        String notifyFloatMicDesc = "";       //通知悬浮窗的麦权内容
        String listenUserName = "";

        switch (talkStatus) {

            case ListenStart:
                ivMicStatus.setBackgroundResource(R.mipmap.media_talk);
                listenUserName = PttHelper.findUserName(messageEvent.getUserId());
                tvMicDesc.setText("[" + listenUserName + "] " +
                        getResources().getString(R.string.mic_state_listening));

                needNotifyFloatWindow = true;
                notifyFloatMicDesc = "[" + listenUserName + "] " +
                        getResources().getString(R.string.mic_state_listening);


                break;

            case ListenStop:
                ivMicStatus.setBackgroundResource(R.mipmap.media_idle);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_idle));

                needNotifyFloatWindow = true;
                notifyFloatMicDesc = getResources().getString(R.string.mic_state_mic_idle);

                break;

            case Applying:
                ivMicStatus.setBackgroundResource(R.mipmap.media_talk_wait);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_read));

                needNotifyFloatWindow = true;
                notifyFloatMicDesc = getResources().getString(R.string.mic_state_mic_read);

                break;

            case ApplySuccess:
                ivMicStatus.setBackgroundResource(R.mipmap.media_talk);
                tvMicDesc.setText("我方说话中...");
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);

                needNotifyFloatWindow = true;
                notifyFloatMicDesc = "我方说话中...";

                break;

            case Idel:
                ivMicStatus.setBackgroundResource(R.mipmap.media_idle);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_idle));
                needNotifyFloatWindow = true;
                notifyFloatMicDesc = getResources().getString(R.string.mic_state_mic_idle);

                break;

            case ApplyFail:
                ivMicStatus.setBackgroundResource(R.mipmap.media_idle);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_fail));
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);

                needNotifyFloatWindow = true;
                notifyFloatMicDesc = getResources().getString(R.string.mic_state_mic_fail);

                break;

            case ApplyTimeout:

                break;

            default:
                break;
        }

        if (needNotifyFloatWindow) {
            //通知悬浮窗，显示麦权状态
            Intent updateIntent = new Intent(FloatingTalkService.BROADCAST_UPDATE_FLOATING_WINDOW);
            updateIntent.putExtra("mic", notifyFloatMicDesc);
            sendBroadcast(updateIntent);

        }

    }

    @Override
    public boolean dispatchKeyEvent(KeyEvent event) {
        int keyCode = event.getKeyCode();
        Log.d(TAG, "KeyCode: " + keyCode);
        return super.dispatchKeyEvent(event);
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        Log.i(TAG, "KEYCODE=" + keyCode);
        //String keyName = KeyEvent.keyCodeToString(keyCode);
        Log.d("KeyEvent", "按键按下 - 键码: " + keyCode);
        //Toast.makeText(this, "按下: " + keyName, Toast.LENGTH_SHORT).show();
        // 返回true表示已处理该事件，不再传递
        // 返回false表示未处理，继续传递
        if (MyPOCApplication.getInstance().getPttKeyVal() == keyCode &&
                !MyPOCApplication.getInstance().isPttUseBroadCastMode()
        ) {

            PocSessionStatusEnum currPocSessionStatus = MyPOCApplication.getInstance().getPocSession();
            if (currPocSessionStatus == PocSessionStatusEnum.Idel ||
                    currPocSessionStatus == PocSessionStatusEnum.Listening
            ) {

                //允许抢麦
                pttSDK.requestMicrophone(new RequestMicCallback() {
                    @Override
                    public void onSuccess() {
                        //超时执行的任务
                        mHandler.postDelayed(applyMicOwnerTimeoutTask, applyMicOwnerTimeoutMs);
                        //发送等待事件
                        EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Applying));
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG, "发送抢麦报错:" + error);
                    }
                });

            }

        } else if (keyCode==KeyEvent.KEYCODE_DPAD_CENTER){
            //这是键盘上的确认键或OK键的键值: 23
            //进入dpad键盘键导航的菜单页面
            Toast.makeText(this,"按了OK键",Toast.LENGTH_SHORT).show();

            Intent intent = new Intent(MainActivity.this, MainMenuActivity.class);
            startActivity(intent);

            return true;
        }

        return super.onKeyDown(keyCode, event);
    }

    @Override
    public boolean onKeyUp(int keyCode, KeyEvent event) {
        //String keyName = KeyEvent.keyCodeToString(keyCode);
        Log.d("KeyEvent", "按键松开 - 键码: " + keyCode);
        //Toast.makeText(this, "松开: " + keyName, Toast.LENGTH_SHORT).show();
        if (MyPOCApplication.getInstance().getPttKeyVal() == keyCode &&
                !MyPOCApplication.getInstance().isPttUseBroadCastMode()
        ) {
            //
            PocSessionStatusEnum currPocSessionStatus = MyPOCApplication.getInstance().getPocSession();
            if (currPocSessionStatus == PocSessionStatusEnum.Speaking) {

                pttSDK.releaseMicrophone(new ReleaseMicCallback() {
                    @Override
                    public void onSuccess() {
                        //发送等待事件
                        EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                        MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG, "发送释放麦报错:" + error);
                    }
                });

            }

        }
        return super.onKeyUp(keyCode, event);
    }

    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onTtsSpeak(TtsSpeakEvent messageEvent) {

        if (messageEvent.getSpeakText() != null)
            ttsSpeak(messageEvent.getSpeakText());

    }


    private void initGroupPager() {
        Log.i(TAG, "initGroupPager执行了... ");
        try {
            vp.setOffscreenPageLimit(3);  //当前fragment的前后各3个缓存，一共7个够了
            fglist.clear();
            if (adapter != null) {
                adapter.CleanAll();
                adapter.notifyDataSetChanged();
            }
        } catch (Exception e) {
            Log.e(TAG, "initGroupPager Exception");
            Log.e(TAG, Log.getStackTraceString(e));
        }

        talkFragment = new TalkFragment();
        groupFragment = new GroupFragment();
        contactFragment = new ContactFragment();
        profileFragment = new ProfileFragment();
        fglist.add(talkFragment);
        fglist.add(groupFragment);
        fglist.add(contactFragment);
        fglist.add(profileFragment);

        adapter = new MyPocPageAdapter(getSupportFragmentManager(), fglist);
        vp.setAdapter(adapter);

        vp.requestDisallowInterceptTouchEvent(true);
        vp.setOnPageChangeListener(new ViewPager.OnPageChangeListener() {
            @Override
            public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
                Log.i(TAG, "onPageScrolled-arg0 :" + position);
            }

            @Override
            public void onPageSelected(int position) {
                //
                currentTabIndex = position;
                setScroll(position);
            }

            @Override
            public void onPageScrollStateChanged(int state) {
                Log.i(TAG, "onPageScrollStateChanged-arg0 :" + state);
            }
        });

    }

    private void setScroll(int arg) {

        imagebuttons[index].setSelected(false);
        imagebuttons[arg].setSelected(true);
        textviews[index].setTextColor(getResources().getColor(R.color.mypoc_bottomtab_txtcolor));
        textviews[arg].setTextColor(getResources().getColor(R.color.white));
        index = arg;

        if (index == 0) {

            rlTalk.setBackgroundResource(R.color.mypoc_bg_color);

            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.transparent);


        } else if (index == 1) {

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.mypoc_bg_color);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.transparent);

        } else if (index == 2) {

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.mypoc_bg_color);
            rlProfile.setBackgroundResource(R.color.transparent);

        } else if (index == 3) {

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.mypoc_bg_color);

        }

    }

    private void addListener() {
    }

    private void initViews() {

        imagebuttons = new ImageView[4];
        imagebuttons[0] = ivTalk;
        imagebuttons[1] = ivMember;
        imagebuttons[2] = ivContact;
        imagebuttons[3] = ivProfile;
        imagebuttons[0].setSelected(true);

        textviews = new TextView[4];
        textviews[0] = tvTalk;
        textviews[1] = tvMember;
        textviews[2] = tvContact;
        textviews[3] = tvProfile;
        textviews[0].setTextColor(getResources().getColor(R.color.white));

        rlTalk.setBackgroundResource(R.color.mypoc_bg_color);
        currentTabIndex = 0;

    }


    @Override
    protected void onDestroy() {
        if (EventBus.getDefault().isRegistered(this))
            EventBus.getDefault().unregister(this);

        // 移除事件监听避免内存泄漏
        if (pttSDK != null)
            pttSDK.setEventListener(null);

        if (textToSpeech != null) {
            textToSpeech.stop();
            textToSpeech.shutdown();
        }

        if (groupInvitesoundDialog != null) {
            groupInvitesoundDialog.dismiss();
        }

        if (groupSyncsoundDialog != null) {
            groupSyncsoundDialog.dismiss();
        }

        // 解绑服务
        if (isServiceBound) {
            unbindService(serviceConnection);
            isServiceBound = false;
        }

        // 取消注册
        if (gbTalkBroadcastReceiver != null)
            unregisterReceiver(gbTalkBroadcastReceiver);

        super.onDestroy();
    }
}