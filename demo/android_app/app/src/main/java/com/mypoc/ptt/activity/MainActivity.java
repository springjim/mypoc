package com.mypoc.ptt.activity;

import static android.content.pm.PackageManager.PERMISSION_GRANTED;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.viewpager.widget.ViewPager;

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Color;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
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

import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.R;
import com.mypoc.ptt.adapter.MyPocPageAdapter;
import com.mypoc.ptt.enums.PocSessionStatusEnum;
import com.mypoc.ptt.event.EnterPrevGroupEvent;
import com.mypoc.ptt.event.ExitDeleteGroupEvent;
import com.mypoc.ptt.fragment.ProfileFragment;
import com.mypoc.ptt.service.FloatingTalkService;
import com.mypoc.ptt.service.KeyMonitorService;
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
import com.mypoc.pttlibrary.internal.PTTService;
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

public class MainActivity extends AppCompatActivity {

    private String TAG = "MainActivity";

    @BindView(R.id.group_name)
    TextView tvGroupName;  //当前组名称

    @BindView(R.id.main_media_img)
    ImageView ivMicStatus;  //麦状态图标

    @BindView(R.id.main_media_text)
    TextView tvMicDesc;   //麦状态描述

    @BindView(R.id.main_viewpager)
    MyPocViewPager vp;

    private MyPocPageAdapter adapter;
    private List<Fragment> fglist = new ArrayList<Fragment>();

    @BindView(R.id.re_mypoc_talk)
    RelativeLayout rlTalk;
    @BindView(R.id.iv_mypoc_talk)
    ImageView ivTalk;
    @BindView(R.id.tv_mypoc_talk)
    TextView  tvTalk;

    @BindView(R.id.re_mypoc_member)
    RelativeLayout rlMember;
    @BindView(R.id.iv_mypoc_member)
    ImageView ivMember;
    @BindView(R.id.tv_mypoc_member)
    TextView  tvMember;

    @BindView(R.id.re_mypoc_contact)
    RelativeLayout rlContact;
    @BindView(R.id.iv_mypoc_contact)
    ImageView ivContact;
    @BindView(R.id.tv_mypoc_contact)
    TextView  tvContact;

    @BindView(R.id.re_mypoc_profile)
    RelativeLayout rlProfile;
    @BindView(R.id.iv_mypoc_profile)
    ImageView ivProfile;
    @BindView(R.id.tv_mypoc_profile)
    TextView tvProfile;

    private ImageView[] imagebuttons;
    private TextView[]  textviews;

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
    private boolean showGroupInviteDialog=true;
    SoundDialog groupInvitesoundDialog;
    //指示是否在被组邀请处理中..., 如果是: 随后的组同步指令要过滤掉
    private boolean groupInviteProcessing =false;

    //收到强制同步组，是否弹窗，还是自动进入，这里设置为自动进入
    private boolean showGroupSyncDialog=false;
    SoundDialog groupSyncsoundDialog;

    private int applyMicOwnerTimeoutMs = 5000;
    private Runnable applyMicOwnerTimeoutTask;

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

        getCmpAllUsers();  //获取cmp下所有用户，后面当作 "通讯录"fragment中的数据

        reportInitGroup();  //上报初始工作组，开启对讲或监听

        //最后获取一些固定组、临时组
        getFixAndTempGroups();

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

        public TTSHandler(MainActivity activity)
        {
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
        if (textToSpeech!=null && isTTSInitialized) {
            Log.e(TAG,"ttsSpeak,开始播报:"+text);
            //要放到UI线程去播报
            mHandler.postDelayed(new Runnable() {
                @Override
                public void run() {
                    textToSpeech.speak(text, TextToSpeech.QUEUE_FLUSH, null, null);
                }
            },1800);


        } else {
            //未创建或初始化失败
        }
    }

    //以下是对pttsdk的事件回调，一定要写
    private void addPttEventListener(IPTTSDK pttSDK) {
        pttSDK.setEventListener(new IPTTEventListener() {
            @Override
            public void onApplyMicSuccess() {
                Log.i(TAG,"收到抢麦成功事件");
                MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Speaking);
                //eventbus 事件发送
                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ApplySuccess));
            }

            @Override
            public void onApplyMicFailed(String reason) {
                Log.i(TAG,"收到抢麦失败事件");
                PocSessionStatusEnum currPocSessionStatus= MyPOCApplication.getInstance().getPocSession();
                if (currPocSessionStatus!=PocSessionStatusEnum.Listening){
                    //不是收听前提下的抢麦失败才能设为idel
                    MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                }

                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ApplyFail));

            }

            @Override
            public void onMicTimeout(String reason) {
                Log.i(TAG,"收到麦权超时事件");
                //如果是被麦权高的人打断了，这里就不能播报了
                ttsSpeak("讲话已超时");  //看情况，也可以不播报
            }

            @Override
            public void onMicOccupied() {
                ttsSpeak("麦克风被占用，麦已释放");
                Log.i(TAG,"收到麦克风被占用事件");

                //下面要延迟下，因为收到方，有个500ms显示收听的UI处理，所以这里要延迟大于这个值
                mHandler.postDelayed( ()-> {
                    pttSDK.releaseMicrophone(new ReleaseMicCallback() {
                        @Override
                        public void onSuccess() {
                            Log.i(TAG,"收到麦克风被占用事件,麦已释放");
                            EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                            MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                        }

                        @Override
                        public void onFailure(String error) {
                            Log.i(TAG,"收到麦克风被占用事件,麦释放报错:"+error);
                            EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                            MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                        }
                    });
                },1000);

            }

            @Override
            public void onUserStartSpeaking(int groupId, int userId) {
                //某人正在某组讲话
                Log.i(TAG, String.format("收到 groupId=%d, userId=%d 正在讲话 ", groupId,userId) );

                //todo 在非监听模式下，只有与当前组一致的才处理
                if (MyPOCApplication.getInstance().getCurrGroupId()!=null &&
                 MyPOCApplication.getInstance().getCurrGroupId().equals(groupId)
                ) {

                    //再延迟500ms发送
                    //2025.2.11 这里要注意，在低麦权讲话被打断的用户，会立马发送释放麦报文，其它端会收到无人讲话(在先收到)，随后高麦权人又发送了讲话报文（这个在后），所以要延迟下执行，
                    //要不然会显示错乱的顺序，即正确的顺序是： 1: 先收到被打断的人的42停止报文(这时显示空闲)  2：后收到打断人的讲话42报文，所以第2步要延迟执行下
                    new Handler(getMainLooper()).postDelayed(
                            ()-> {
                                MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Listening);
                                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ListenStart, groupId,userId));
                            }
                            ,500);


                } else {
                     //是监听别的组情况

                    // 同时messageId=100 的语音包要解码出来
                }


            }

            @Override
            public void onUserStopSpeaking(int groupId, int userId) {
                //某人正在某组停止讲话
                Log.i(TAG, String.format("收到 groupId=%d, userId=%d 正在停止讲话 ", groupId,userId) );

                if (MyPOCApplication.getInstance().getCurrGroupId()!=null &&
                        MyPOCApplication.getInstance().getCurrGroupId().equals(groupId)
                ) {
                    MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                    EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ListenStop, groupId,userId));
                } else {
                    //是监听别的组情况

                }

            }

            @Override
            public void onUserJoinedGroup(int groupId, int userId) {
                //某人进入某组,UI上要显示在线在组的图标
                //更新livedata, 以便其它订阅来更新相应的UI
                PttHelper.updateAllUserStatus(userId,1);

                EventBus.getDefault().post(new UpdateGroupMemberStausEvent(groupId,userId,
                        UpdateGroupMemberStausEvent.Status_EnterGroup,null));
            }

            @Override
            public void onUserLeftGroup(int groupId, int userId) {
                //某人离开某组，UI上要显示在线，但不在组
                //更新livedata, 以便其它订阅来更新相应的UI
                EventBus.getDefault().post(new UpdateGroupMemberStausEvent(groupId,userId,
                        UpdateGroupMemberStausEvent.Status_ExitGroup,null));

            }

            @Override
            public void onUserOffline(int userId) {
                //某人下线了或掉线了,UI上要显示所有组离线图标
                //更新livedata, 以便其它订阅来更新相应的UI
                PttHelper.updateAllUserStatus(userId,0);
                EventBus.getDefault().post(new UpdateGroupMemberStausEvent(-1,userId,
                        UpdateGroupMemberStausEvent.Status_Offline,null));
            }


            @Override
            public void onGroupInvite(int groupId, String groupName, int userId, int inviteUserId) {
                //收到建临组邀请（包括单呼）
                Log.i(TAG,String.format("收到建临组邀请（包括单呼）, groupId=%d, groupName=%s, userId=%d,inviteUserId=%d",
                        groupId,groupName,userId,inviteUserId));

                //如果邀请人是自己，则不用处理，因为是自己发出去的
                if (inviteUserId==MyPOCApplication.getInstance().getUserId()){
                    Log.i(TAG, "自己发出去的创建临时组或单呼，不用处理");
                    return;
                }

                groupInviteProcessing =true;

                PTTGroup pttGroupExt= new PTTGroup(groupId,groupName,inviteUserId,System.currentTimeMillis()/1000);
                MyPOCApplication.getInstance().addToTempGroups(pttGroupExt);  //让groupfragment 会感知更新

                String inviteUserName= PttHelper.findUserName(inviteUserId);

                //一般这里是要弹出提醒用户， 是手动确认进入这个邀请临时组，还是自动进入
                if (showGroupInviteDialog){
                    groupInvitesoundDialog = new SoundDialog(MainActivity.this,
                            "提示",
                            "收到【"+inviteUserName+"】组邀请，是否进入",
                            new SoundDialog.DialogListener() {
                                @Override
                                public void onConfirm() {
                                    //用户点击确认的处理
                                    //发送给groupfragment进入组事件，并播报tts语音
                                    pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                        @Override
                                        public void onSuccess() {
                                            ttsSpeak("进入"+groupName);
                                            EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId,groupName));
                                            groupInvitesoundDialog.dismiss(); //关闭
                                            groupInviteProcessing =false;
                                        }

                                        @Override
                                        public void onFailure(String error) {
                                            ttsSpeak("进入失败");
                                            Log.e(TAG,error);
                                            groupInvitesoundDialog.dismiss(); //关闭
                                            groupInviteProcessing =false;
                                        }
                                    });


                                }

                                @Override
                                public void onCancel() {
                                    // 用户点击取消的处理
                                    //Toast.makeText(MainActivity.this, "取消点击", Toast.LENGTH_SHORT).show();
                                    groupInvitesoundDialog.dismiss(); //关闭
                                    groupInviteProcessing =false;
                                }
                            });

                    groupInvitesoundDialog.show();

                } else {
                    //直接进入, 因为随后会有组同步 groupSync, 所以这里务必要延迟下，以便后面收到的groupSync不用重复处理
                    new Handler(Looper.getMainLooper()).postDelayed(
                            () ->{
                                pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                    @Override
                                    public void onSuccess() {
                                        ttsSpeak("进入"+groupName);
                                        EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId,groupName));
                                        groupInviteProcessing =false;
                                    }

                                    @Override
                                    public void onFailure(String error) {
                                        ttsSpeak("进入失败");
                                        Log.e(TAG,error);
                                        groupInviteProcessing =false;
                                    }
                                });
                            }
                            , 500);

                }

            }

            @Override
            public void onGroupDelete(int groupId) {
                //收到组解散的通知，一般是临时组或广播组
                Log.i(TAG, "收到组解散:"+groupId);
                if (MyPOCApplication.getInstance().getCurrGroupId().equals(groupId)){
                    MyPOCApplication.getInstance().removeFromTempGroups(groupId);
                    //
                    enterPrevGroup();
                }
                //发送事件，让临时组的窗口处理
                //EventBus.getDefault().post(new ExitDeleteGroupEvent(groupId));

            }

            @Override
            public void onGroupUserChange(int groupId, int groupTypeId, int changeType,String userStr ) {
                //发送事件, 以便其它页面订阅同步状态用
                if (changeType==0) {
                    //强踢的用户
                    EventBus.getDefault().post(new UpdateGroupMemberStausEvent(groupId, -1, UpdateGroupMemberStausEvent.Status_KickOutGroup, userStr));
                } else if (changeType==1){
                    //强拉的用户
                    EventBus.getDefault().post(new UpdateGroupMemberStausEvent(groupId, -1, UpdateGroupMemberStausEvent.Status_PullInGroup, userStr));
                }
                //
                if (changeType==0){
                    //有人被强踢
                    Log.i(TAG, "收到强踢: groupId="+groupId+ ",人员有:"+userStr);

                    //查看人员名单中是否有自己
                    //如果是自己被强踢了，要退出组
                    String[] userArr= userStr.split(",");
                    if (Arrays.asList(userArr).contains(MyPOCApplication.getInstance().getUserId()+"")){

                        if (groupTypeId==1)
                            MyPOCApplication.getInstance().removeFromTempGroups(groupId);
                        else if (groupTypeId==0)
                            MyPOCApplication.getInstance().removeFromFixGroups(groupId);

                        //如果是当前组，还要退到上一个组
                        if (MyPOCApplication.getInstance().getCurrGroupId().equals(groupId))
                            enterPrevGroup();
                        else{
                            //不是当前组不用处理
                        }

                    }


                } else {
                    //强插，查询这个组
                    //查强插人员名单有没有自己
                    String[] userArr= userStr.split(",");
                    if (!Arrays.asList(userArr).contains(MyPOCApplication.getInstance().getUserId()+""))
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
                                        PTTGroup pttGroupExt= new PTTGroup(groupId,pttGroup.getGroupName(),-1,System.currentTimeMillis()/1000);
                                        MyPOCApplication.getInstance().addToTempGroups(pttGroupExt);  //让groupfragment 会感知更新

                                        //自动进入该组
                                        pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                            @Override
                                            public void onSuccess() {
                                                ttsSpeak("进入"+pttGroup.getGroupName());
                                                EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId,pttGroup.getGroupName()));
                                            }

                                            @Override
                                            public void onFailure(String error) {
                                                ttsSpeak("进入失败");
                                                Log.e(TAG,error);
                                            }
                                        });
                                    },
                                    throwable -> {
                                        // 处理错误
                                        Log.e(TAG,"获取组信息失败:"+throwable);
                                    }
                            );

                }

            }

            @Override
            public void onGroupSync(int groupId, String groupName, int userId, int inviteId) {
                //
                Log.i(TAG,"收到组同步通知: groupId="+groupId+",groupName="+groupName);

                String syncUserName= PttHelper.findUserName(inviteId); //出现同步命令的用户名

                ////注意: 一般邀请方可能随后会发一个同步组的信令，所以下面如果是手动确认的话，还要处理:
                //即： 在邀请还没有确认时，这里的同步就不能处理了
                if (groupInviteProcessing) {
                    //邀请弹窗还在处理中, 这里跳过了
                    Log.i(TAG,"邀请弹窗还在处理中, 这里跳过了");
                    return;
                }

                //一般这里是要弹出提醒用户， 是手动确认进入这个组，还是自动进入
                if (showGroupSyncDialog){

                    groupSyncsoundDialog = new SoundDialog(MainActivity.this,
                            "提示",
                            "收到【"+syncUserName+"】同步对讲组请求，是否进入",
                            new SoundDialog.DialogListener() {
                                @Override
                                public void onConfirm() {
                                    // 用户点击确认的处理
                                    //发送给groupfragment进入组事件，并播报tts语音
                                    pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                                        @Override
                                        public void onSuccess() {
                                            ttsSpeak("进入"+groupName);
                                            EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId,groupName));
                                            groupSyncsoundDialog.dismiss();
                                        }

                                        @Override
                                        public void onFailure(String error) {
                                            ttsSpeak("进入失败");
                                            Log.e(TAG,error);
                                            groupSyncsoundDialog.dismiss();
                                        }
                                    });


                                }

                                @Override
                                public void onCancel() {
                                    // 用户点击取消的处理
                                    //Toast.makeText(MainActivity.this, "取消点击", Toast.LENGTH_SHORT).show();
                                }
                            });

                    groupSyncsoundDialog.show();

                } else {
                    //直接进入
                    pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
                        @Override
                        public void onSuccess() {
                            ttsSpeak("进入"+groupName);
                            EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId,groupName));
                        }

                        @Override
                        public void onFailure(String error) {
                            ttsSpeak("进入失败");
                            Log.e(TAG,error);
                        }
                    });

                }

            }

            @Override
            public void onSpeakingBreaked(int groupId,  int userId) {
                //UI同步显示某人说话，并播报打断的tts语音
                Log.e(TAG,"被麦权高的人打断");
                ttsSpeak("讲话被打断");
                if (MyPOCApplication.getInstance().getCurrGroupId()!=null &&
                        MyPOCApplication.getInstance().getCurrGroupId().equals(groupId)
                ) {
                    //切换为收听UI，要延后500 ms, 因为
                    new Handler(getMainLooper()).postDelayed(
                            ()-> {
                                MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Listening);
                                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ListenStart, groupId,userId));
                            }
                            ,500);

                }

            }

            @Override
            public void onAudioDataReceived(String userId, byte[] audioData) {
                //一般用于前端展示音频波形用，这里略
            }

            @Override
            public void onConnectionStateChanged(PTTState newState) {
                Log.i(TAG,"SDK 状态:"+newState.toString());
            }

            @Override
            public void onKickOff() {
                Log.e(TAG,"收到帐号被踢");
                ttsSpeak("你的帐号被踢 即将退出应用");
                //延时执行退出任务
                mHandler.postDelayed(
                        ()-> {
                            //要先关闭服务，要不然是Service.START_STICKY,在退出时又自动创建
                            if (pttSDK!=null)
                                pttSDK.release();

                            android.os.Process.killProcess(android.os.Process.myPid());
                            System.exit(0);
                        }, 6000
                );

            }

            @Override
            public void onError(String error) {

            }
        });
    }


    //解散或被强踢后，进入上一个组
    private void enterPrevGroup(){
        //自动回到默认组，如果没有回到上一个组
        Integer defaultGroupId= MyPOCApplication.getInstance().getDefaultGroupId();
        Log.i(TAG,"defaultGroupId="+defaultGroupId);

        int enterGroupId=-1;

        if (defaultGroupId==null){
            //进入上一个组
            if (MyPOCApplication.getInstance().getPrevGroupId()==null){
                //为空
                enterGroupId= PttHelper.getFirstGroupIdFromApplication(-1);
            } else {
                enterGroupId= MyPOCApplication.getInstance().getPrevGroupId();
            }
        } else {
            //进入默认组
            enterGroupId= defaultGroupId;
        }

        //开始切换组
        if (enterGroupId!=-1){

            String groupName= PttHelper.findGroupName(enterGroupId);
            if (groupName.equalsIgnoreCase(PttHelper.NO_FOUND_GROUP)){
                //没有找到，可能是已经删除的，或是被强踢出了，这时要重新找groupId,
                enterGroupId= PttHelper.getFirstGroupIdFromApplication(-1);
                groupName= PttHelper.findGroupName(enterGroupId);
            }
            //发送给groupfragment进入组事件，并播报tts语音
            int finalEnterGroupId = enterGroupId;
            String finalGroupName = groupName;

            pttSDK.joinGroup(finalEnterGroupId, new ReportWorkGroupCallback() {
                @Override
                public void onSuccess() {
                    ttsSpeak("进入"+ finalGroupName);
                    EventBus.getDefault().post(new UpdateWorkingGroupEvent(finalEnterGroupId, finalGroupName));
                }

                @Override
                public void onFailure(String error) {
                    ttsSpeak("切换组失败");
                    Log.e(TAG,error);
                }
            });

        } else {
            Log.e(TAG,"无法进入组, 因为未找到或没有组了");
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

                MyPOCApplication.getInstance().setUserId(pttUser.getUserId());
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
                    pttSDK.joinGroup(MyPOCApplication.getInstance().getCurrGroupId(), new ReportWorkGroupCallback() {
                        @Override
                        public void onSuccess() {
                            Log.i(TAG, "上报工作组ID: " + MyPOCApplication.getInstance().getCurrGroupId());
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
        //completeQuitDialog((Activity)this);
        //放到后台
        moveTaskToBack(true);
    }

    private  void completeQuitDialog(final Activity context) {
        AlertDialog.Builder builder = new AlertDialog.Builder((Context)context);
        builder.setMessage("确认要退出应用吗?");
        builder.setTitle("提示");
        builder.setPositiveButton(R.string.exitComplete, new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface param1DialogInterface, int param1Int) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.append("completeQuitDialog() - 当前要销毁的activity：" );
                stringBuilder.append(context);
                Log.e("MainActivity", stringBuilder.toString());

                //先主动退出socket服务
                pttSDK.logout(new LogoutCallback() {
                    @Override
                    public void onSuccess() {
                        Log.i(TAG,"发送socket logout成功");
                        exitApp();
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"发送socket logout失败"+error);
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

    private void exitApp(){
        MainActivity.this.finish();
        //要先关闭服务，要不然是Service.START_STICKY,在退出时又自动创建
        if (pttSDK!=null)
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

        if (index==0){

            rlTalk.setBackgroundResource(R.color.mypoc_bg_color);

            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.transparent);

        } else if (index==1){

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.mypoc_bg_color);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.transparent);

        } else if (index==2){

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.mypoc_bg_color);
            rlProfile.setBackgroundResource(R.color.transparent);
        }  else if (index==3){

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.mypoc_bg_color);
        }

        currentTabIndex = index;
    }

    //这里更新顶部当前组名称
    @Subscribe(threadMode =  ThreadMode.MAIN)
    public void onUpdateWorkingGroup(UpdateWorkingGroupEvent event){
        tvGroupName.setText(event.getGroupName());
        //再通知悬浮窗
        Intent updateIntent = new Intent(FloatingTalkService.BROADCAST_UPDATE_FLOATING_WINDOW);
        updateIntent.putExtra("group", event.getGroupName());
        sendBroadcast(updateIntent);

    }

    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onEnterPrevGroup(EnterPrevGroupEvent event ){
        enterPrevGroup();
    }

    /**
     * 麦权申请状态，订阅它更新麦权UI和麦权描述
     */
    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onTalkStatusMessage(TalkStatusMessageEvent messageEvent) {
        Log.i(TAG, "messageEvent=" + messageEvent);
        TalkStatusEnum talkStatus= messageEvent.getStatus();
        //
        boolean needNotifyFloatWindow=false; //是否要通知悬浮窗
        String  notifyFloatMicDesc="";       //通知悬浮窗的麦权内容
        String  listenUserName="";

        switch (talkStatus){

            case ListenStart:
                ivMicStatus.setBackgroundResource(R.mipmap.media_talk);
                listenUserName= PttHelper.findUserName(messageEvent.getUserId());
                tvMicDesc.setText("["+ listenUserName+"] "+
                        getResources().getString(R.string.mic_state_listening));

                needNotifyFloatWindow=true;
                notifyFloatMicDesc= "["+ listenUserName+"] "+
                        getResources().getString(R.string.mic_state_listening);


                break;

            case ListenStop:
                ivMicStatus.setBackgroundResource(R.mipmap.media_idle);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_idle));

                needNotifyFloatWindow=true;
                notifyFloatMicDesc=getResources().getString(R.string.mic_state_mic_idle);

                break;

            case Applying:
                ivMicStatus.setBackgroundResource(R.mipmap.media_talk_wait);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_read));

                needNotifyFloatWindow=true;
                notifyFloatMicDesc=getResources().getString(R.string.mic_state_mic_read);

                break;

            case ApplySuccess:
                ivMicStatus.setBackgroundResource(R.mipmap.media_talk);
                tvMicDesc.setText("我方说话中...");
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);

                needNotifyFloatWindow=true;
                notifyFloatMicDesc= "我方说话中...";

                break;

            case Idel:
                ivMicStatus.setBackgroundResource(R.mipmap.media_idle);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_idle));
                needNotifyFloatWindow=true;
                notifyFloatMicDesc= getResources().getString(R.string.mic_state_mic_idle);

                break;

            case ApplyFail:
                ivMicStatus.setBackgroundResource(R.mipmap.media_idle);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_fail));
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);

                needNotifyFloatWindow=true;
                notifyFloatMicDesc= getResources().getString(R.string.mic_state_mic_fail);

                break;

            case ApplyTimeout:

                break;

            default:
                break;
        }

        if (needNotifyFloatWindow){
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
        Log.i(TAG,"KEYCODE="+keyCode);
        //String keyName = KeyEvent.keyCodeToString(keyCode);
        Log.d("KeyEvent", "按键按下 - 键码: " + keyCode);
        //Toast.makeText(this, "按下: " + keyName, Toast.LENGTH_SHORT).show();
        // 返回true表示已处理该事件，不再传递
        // 返回false表示未处理，继续传递
        if (MyPOCApplication.getInstance().getPttKeyVal()==keyCode &&
         !MyPOCApplication.getInstance().isPttUseBroadCastMode()
        ) {

            PocSessionStatusEnum currPocSessionStatus=MyPOCApplication.getInstance().getPocSession();
            if (currPocSessionStatus==PocSessionStatusEnum.Idel ||
                    currPocSessionStatus== PocSessionStatusEnum.Listening
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
                        Log.e(TAG,"发送抢麦报错:"+error);
                    }
                });

            }

        }

        return super.onKeyDown(keyCode, event);
    }

    @Override
    public boolean onKeyUp(int keyCode, KeyEvent event) {
        //String keyName = KeyEvent.keyCodeToString(keyCode);
        Log.d("KeyEvent", "按键松开 - 键码: " + keyCode);
        //Toast.makeText(this, "松开: " + keyName, Toast.LENGTH_SHORT).show();
        if (MyPOCApplication.getInstance().getPttKeyVal()==keyCode &&
                !MyPOCApplication.getInstance().isPttUseBroadCastMode()
        ) {
            //
            PocSessionStatusEnum currPocSessionStatus=MyPOCApplication.getInstance().getPocSession();
            if (currPocSessionStatus==PocSessionStatusEnum.Speaking) {

                pttSDK.releaseMicrophone(new ReleaseMicCallback() {
                    @Override
                    public void onSuccess() {
                        //发送等待事件
                        EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                        MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"发送释放麦报错:"+error);
                    }
                });

            }

        }
        return super.onKeyUp(keyCode, event);
    }

    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onTtsSpeak(TtsSpeakEvent messageEvent) {

        if (messageEvent.getSpeakText()!=null)
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

        talkFragment= new TalkFragment();
        groupFragment =new GroupFragment();
        contactFragment = new ContactFragment();
        profileFragment = new ProfileFragment();
        fglist.add(talkFragment);
        fglist.add(groupFragment);
        fglist.add(contactFragment);
        fglist.add(profileFragment);

        adapter = new MyPocPageAdapter(getSupportFragmentManager(),fglist);
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
                currentTabIndex  = position;
                setScroll(position);
            }

            @Override
            public void onPageScrollStateChanged(int state) {
                Log.i(TAG, "onPageScrollStateChanged-arg0 :" + state);
            }
        });

    }

    private   void  setScroll(int arg){

        imagebuttons[index].setSelected(false);
        imagebuttons[arg].setSelected(true);
        textviews[index].setTextColor(getResources().getColor(R.color.mypoc_bottomtab_txtcolor));
        textviews[arg].setTextColor(getResources().getColor(R.color.white));
        index = arg;

        if (index==0){

            rlTalk.setBackgroundResource(R.color.mypoc_bg_color);

            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.transparent);


        } else if (index==1){

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.mypoc_bg_color);
            rlContact.setBackgroundResource(R.color.transparent);
            rlProfile.setBackgroundResource(R.color.transparent);

        } else if (index==2){

            rlTalk.setBackgroundResource(R.color.transparent);
            rlMember.setBackgroundResource(R.color.transparent);
            rlContact.setBackgroundResource(R.color.mypoc_bg_color);
            rlProfile.setBackgroundResource(R.color.transparent);

        } else if (index==3){

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
        currentTabIndex=0;

    }



    @Override
    protected void onDestroy() {
        if (EventBus.getDefault().isRegistered(this))
            EventBus.getDefault().unregister(this);

        // 移除事件监听避免内存泄漏
        if (pttSDK!=null)
            pttSDK.setEventListener(null);

        if (textToSpeech != null) {
            textToSpeech.stop();
            textToSpeech.shutdown();
        }

        if (groupInvitesoundDialog!=null){
            groupInvitesoundDialog.dismiss();
        }

        if (groupSyncsoundDialog!=null){
            groupSyncsoundDialog.dismiss();
        }



        super.onDestroy();
    }
}