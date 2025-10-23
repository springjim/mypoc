package com.mypoc.ptt.activity;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.enums.PocSessionStatusEnum;
import com.mypoc.ptt.enums.PocTalkModeEnum;
import com.mypoc.ptt.enums.SingleCallSignalEnum;
import com.mypoc.ptt.event.EnterPrevGroupEvent;
import com.mypoc.ptt.event.ExitDeleteGroupEvent;
import com.mypoc.ptt.event.SingleCallSignalEvent;
import com.mypoc.ptt.event.TtsSpeakEvent;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.CommonCallback;
import com.mypoc.pttlibrary.callback.ReleaseMicCallback;
import com.mypoc.pttlibrary.callback.RequestMicCallback;
import com.mypoc.pttlibrary.callback.TempGroupDeleteCallback;
import com.mypoc.pttlibrary.enums.TalkStatusEnum;
import com.mypoc.pttlibrary.event.TalkStatusMessageEvent;

import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.greenrobot.eventbus.ThreadMode;

import butterknife.BindView;
import butterknife.ButterKnife;

/**
 * 专用于ptt单呼
 */
public class PttSingleCallActivity extends BaseActivity {

    public static final String KEY_GROUP_ID = "groupId";
    public static final String KEY_PREV_GROUP_ID= "prevGroupId";  //进入单呼之前的组ID
    public static final String KEY_PEER_NAME = "peerUserName";  //对方名称
    public static final String KEY_PEER_USERID= "peerUserId";   //对方的userId
    public static final String KEY_IS_CALLER= "isCaller";       //是否主叫

    private String TAG = "PttSingleCallActivity";
    private IPTTSDK pttSDK;
    private Handler mHandler;

    @BindView(R.id.btn_back)
    Button btnBack;

    @BindView(R.id.tv_group_name)
    TextView tvGroupName;

    @BindView(R.id.signal_status_text_frame)
    FrameLayout signalStatusTextFrame;

    @BindView(R.id.signal_status_text)
    TextView signalStatusText;

    @BindView(R.id.talk_img_frame)
    FrameLayout talkImgFrame;

    @BindView(R.id.talk_img)
    ImageView ivTalkImg;

    @BindView(R.id.main_media_img)
    ImageView ivMicStatus;  //麦状态图标

    @BindView(R.id.main_media_text)
    TextView tvMicDesc;   //麦状态描述

    Integer currGroupId;  //当前组ID
    Integer prevGroupId;  //之前的组ID
    Integer peerUserId;   //对方的userId
    boolean isCaller=false;     //是否主叫

    //mic owner apply 申请麦权的超时时间
    private int applyMicOwnerTimeoutMs = 5000;
    private Runnable applyMicOwnerTimeoutTask;

    //单呼邀请超时
    private int signalCallTimeoutMs= 60*1000;  //信令超时 60秒, 如果对方一直没接听的话
    private Runnable signalCallTimeoutTask;

    //当前单呼状态
    private boolean signalConnect = false;

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
        setContentView(R.layout.activity_ptt_single_call);

        ButterKnife.bind(this); //先绑定，再做 initListen事件
        EventBus.getDefault().register(this);
        pttSDK = MyPOCApplication.getInstance().getPttSDK();
        mHandler = new Handler(Looper.getMainLooper());
        // 获取Intent
        Intent intent = getIntent();

        peerUserId = intent.getIntExtra(KEY_PEER_USERID,-1);
        isCaller = intent.getBooleanExtra(KEY_IS_CALLER,false);
        currGroupId= intent.getIntExtra(KEY_GROUP_ID, -1);
        prevGroupId=  intent.getIntExtra(KEY_PREV_GROUP_ID, -1);

        MyPOCApplication.getInstance().setSingleCaller(this.isCaller);
        MyPOCApplication.getInstance().setCurrGroupId(currGroupId);
        tvGroupName.setText("单呼:"+intent.getStringExtra(KEY_PEER_NAME));

        initTasks();
        addListener();

        if (this.isCaller) {
            //是主叫方
            signalStatusTextFrame.setVisibility(View.VISIBLE);
            signalStatusText.setText("呼叫中,请稍候...");
            talkImgFrame.setVisibility(View.INVISIBLE);
            EventBus.getDefault().post(new TtsSpeakEvent("呼叫中,请稍候"));
            //启动超时
            mHandler.postDelayed(signalCallTimeoutTask, signalCallTimeoutMs);

        } else {

            signalStatusTextFrame.setVisibility(View.INVISIBLE);
            talkImgFrame.setVisibility(View.VISIBLE);

            //发送接受信令
            pttSDK.sendSingleCallSignal(currGroupId, MyPOCApplication.getInstance().getUserId(), this.peerUserId,
                    SingleCallSignalEnum.ACCEPTED.getValue(), new CommonCallback() {
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

    }

    private void initTasks() {
        //下面任务，定义麦权申请超时任务，给后面麦权申请时用
        applyMicOwnerTimeoutTask = new Runnable() {
            @Override
            public void run() {

                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ApplyTimeout));

                Log.e(TAG, "申请麦权的超时时间");
            }
        };

        signalCallTimeoutTask = new Runnable() {
            @Override
            public void run() {
                mHandler.post(new Runnable() {
                    @Override
                    public void run() {
                        //在关闭前要,删除组
                        exitHandle();
                    }
                });
            }
        };

    }

    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onSingleCallSignalMessage(SingleCallSignalEvent messageEvent){

        int toUserId= messageEvent.getToUserId();
        if (toUserId!= MyPOCApplication.getInstance().getUserId()){
            return;
        }

        SingleCallSignalEnum signalEnum= SingleCallSignalEnum.fromByte(messageEvent.getSignalVal());

        switch (signalEnum){

            case RING:
                //被叫方在响铃
                signalStatusText.setText("对方响铃中...");
                break;
            case BUSING:
                signalStatusText.setText("对方忙线中...");
                //2秒后关闭，退出
                mHandler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        exitHandle();
                    }
                }  ,2000);
                mHandler.removeCallbacks(signalCallTimeoutTask);
                break;
            case REFUSE:
                signalStatusText.setText("对方拒绝接听");
                //2秒后关闭，退出
                mHandler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        exitHandle();
                    }
                }  ,2000);
                mHandler.removeCallbacks(signalCallTimeoutTask);
                break;
            case ACCEPTED:
                signalStatusTextFrame.setVisibility(View.INVISIBLE);
                talkImgFrame.setVisibility(View.VISIBLE);
                signalConnect= true;
                mHandler.removeCallbacks(signalCallTimeoutTask);
                break;
            case LEAVE:
                //2秒后关闭，退出
                mHandler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        exitHandle();
                    }
                }  ,2000);
                mHandler.removeCallbacks(signalCallTimeoutTask);
                break;
            case TIMEOUT:
                signalStatusText.setText("对方接听超时");
                //2秒后关闭，退出
                mHandler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        exitHandle();
                    }
                }  ,2000);
                break;

            default:
                break;
        }


    }


    private void exitHandle(){

        MyPOCApplication.getInstance().setPeerUserId(-1);
        MyPOCApplication.getInstance().setPocTalkMode(PocTalkModeEnum.PTT);

        if (PttSingleCallActivity.this.isCaller){

            pttSDK.tempGroupDelete(currGroupId, 7, 0, new TempGroupDeleteCallback() {
                @Override
                public void onSuccess() {
                    //要关闭和退出前一个组
                    Log.i(TAG, "主叫退出单呼:"+currGroupId);
                    MyPOCApplication.getInstance().removeFromTempGroups(currGroupId);
                }

                @Override
                public void onFailure(String error) {
                    mHandler.post(()-> {
                        Toast.makeText(PttSingleCallActivity.this,error,Toast.LENGTH_LONG).show();
                    });
                }
            });
            //不用处理其它, 本帐号也会收到解散组消息，在那里也会处理和同步各种事件
            sendLeaveSignal();
            EventBus.getDefault().post(new EnterPrevGroupEvent(prevGroupId));
            finish();

        } else {
            //非主叫
            Log.i(TAG, "非主叫退出单呼:"+currGroupId);
            MyPOCApplication.getInstance().removeFromTempGroups(currGroupId);
            sendLeaveSignal();
            EventBus.getDefault().post(new EnterPrevGroupEvent(prevGroupId));
            finish();
        }
    }

    private void sendLeaveSignal(){

        pttSDK.sendSingleCallSignal(this.currGroupId, MyPOCApplication.getInstance().getUserId(), this.peerUserId,
                SingleCallSignalEnum.LEAVE.getValue(), new CommonCallback() {
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

    private void addListener() {

        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                exitHandle();
            }
        });

        ivTalkImg.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                PocSessionStatusEnum currStatus= MyPOCApplication.getInstance().getPocSession();

                switch (currStatus){

                    case Idel:
                        //可以抢麦
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

                        break;
                    case Appling:
                        //已经在抢麦中了，等待超时，或等服务返回结果
                        Log.i(TAG,"已经在抢麦中了，等待超时，或等服务返回结果");
                        break;
                    case Speaking:
                        //我方说话中, 可以释放麦
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
                                MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                            }
                        });


                        break;
                    case Listening:
                        //别人在讲, 麦权高的人可以打断
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

                        break;
                    case Breaking:
                        //正在被打断中
                        break;

                    default:
                        break;
                }

            }
        });

    }

    @Override
    public void onBackPressed() {
        //与退出按钮效果一样
        exitHandle();
    }

    //这里只对ivtalk图标进行同步
    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onTalkStatusMessage(TalkStatusMessageEvent messageEvent) {
        //
        Log.i(TAG, "messageEvent=" + messageEvent);
        TalkStatusEnum talkStatus= messageEvent.getStatus();
        switch (talkStatus){

            case ListenStart:
                ivTalkImg.setImageResource(R.mipmap.btn_talk_listening);
                ivMicStatus.setBackgroundResource(R.mipmap.media_talk);
                tvMicDesc.setText("["+ PttHelper.findUserName(messageEvent.getUserId())+"] "+
                        getResources().getString(R.string.mic_state_listening));

                break;

            case Applying:
                if (MyPOCApplication.getInstance().getPocSession().equals(PocSessionStatusEnum.Listening)){
                    //当前正在监听中
                } else {
                    ivTalkImg.setImageResource(R.mipmap.btn_talk_press);
                    ivMicStatus.setBackgroundResource(R.mipmap.media_talk_wait);
                    tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_read));
                }

                break;

            case ListenStop:
            case Idel:
                ivTalkImg.setImageResource(R.mipmap.btn_talk_idle);
                ivMicStatus.setBackgroundResource(R.mipmap.media_idle);
                tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_idle));

                break;


            case ApplyFail:
                PocSessionStatusEnum currPocSessionStatus= MyPOCApplication.getInstance().getPocSession();
                Log.e(TAG,"当前currPocSessionStatus="+currPocSessionStatus);
                if (MyPOCApplication.getInstance().getPocSession()!=PocSessionStatusEnum.Listening){
                    //不是收听下的抢麦失败，要显示
                    ivTalkImg.setImageResource(R.mipmap.btn_talk_idle);
                    ivMicStatus.setBackgroundResource(R.mipmap.media_idle);
                    tvMicDesc.setText(getResources().getString(R.string.mic_state_mic_fail));
                }
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);
                break;

            case ApplyTimeout:
                //申请超时
                Log.e(TAG,"申请麦权超时...");
                ivTalkImg.setImageResource(R.mipmap.btn_talk_idle);
                break;

            case ApplySuccess:
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);  //当申请成功后, 要取消掉
                ivTalkImg.setImageResource(R.mipmap.btn_talk_speak);
                ivMicStatus.setBackgroundResource(R.mipmap.media_talk);
                tvMicDesc.setText("我方说话中...");
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);

                break;

            default:
                break;
        }

    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        EventBus.getDefault().unregister(this); // 反注册 EventBus
    }


}