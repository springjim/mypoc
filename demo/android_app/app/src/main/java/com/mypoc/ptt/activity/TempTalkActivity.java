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
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.enums.PocSessionStatusEnum;
import com.mypoc.ptt.event.ExitDeleteGroupEvent;
import com.mypoc.ptt.event.TtsSpeakEvent;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.pttlibrary.api.IPTTSDK;
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
 * 临时组(多个人)或单呼(2个人)形成的临时对讲，退出后要求解散组
 */
public class TempTalkActivity extends AppCompatActivity {

    public static final String KEY_GROUP_ID = "groupId";
    public static final String KEY_GROUP_NAME = "groupName";
    private String TAG = "TempTalkActivity";

    private IPTTSDK pttSDK;
    private Handler mHandler;
    @BindView(R.id.btn_back)
    Button btnBack;

    @BindView(R.id.tv_group_name)
    TextView tvGroupName;

    Integer currGroupId;  //当前组ID

    @BindView(R.id.talk_img)
    ImageView ivTalkImg;

    @BindView(R.id.main_media_img)
    ImageView ivMicStatus;  //麦状态图标

    @BindView(R.id.main_media_text)
    TextView tvMicDesc;   //麦状态描述

    //mic owner apply 申请麦权的超时时间
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
        setContentView(R.layout.activity_temp_talk);

        ButterKnife.bind(this); //先绑定，再做 initListen事件
        EventBus.getDefault().register(this);
        // 获取Intent
        Intent intent = getIntent();
        int groupId = intent.getIntExtra(KEY_GROUP_ID, -1);
        String groupName = intent.getStringExtra(KEY_GROUP_NAME);

        pttSDK = MyPOCApplication.getInstance().getPttSDK();
        mHandler = new Handler(Looper.getMainLooper());

        currGroupId= groupId;
        tvGroupName.setText(groupName);

        initTasks();
        addListener();

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

    }

    private void addListener() {
        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                pttSDK.tempGroupDelete(currGroupId, 7, 0, new TempGroupDeleteCallback() {
                    @Override
                    public void onSuccess() {
                        //要关闭和退出前一个组
                        Log.i(TAG, "主动解散组:"+currGroupId);
                        //不用处理其它, 本帐号也会收到解散组消息，在那里也会处理和同步各种事件
                        finish();
                    }

                    @Override
                    public void onFailure(String error) {
                        mHandler.post(()-> {
                            Toast.makeText(TempTalkActivity.this,error,Toast.LENGTH_LONG).show();
                        });
                    }
                });


            }
        });

        //

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
        pttSDK.tempGroupDelete(currGroupId, 7, 0, new TempGroupDeleteCallback() {
            @Override
            public void onSuccess() {
                //要关闭和退出前一个组
                Log.i(TAG, "主动解散组:"+currGroupId);
                //不用处理其它, 本帐号也会收到解散组消息，在那里也会处理和同步各种事件
                finish();
            }

            @Override
            public void onFailure(String error) {
                mHandler.post(()-> {
                    Toast.makeText(TempTalkActivity.this,error,Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onExitDeleteGroupMessage(ExitDeleteGroupEvent messageEvent){

        if (messageEvent.getGroupId()==this.currGroupId){
            //主叫方或被叫方正在退出, todo...
            //EventBus.getDefault().post(new TtsSpeakEvent("对"));

        }

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