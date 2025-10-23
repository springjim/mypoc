package com.mypoc.ptt.activity;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.TextView;

import com.dds.skywebrtc.SkyEngineKit;
import com.mypoc.ptt.R;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.ptt.webrtc.voip.CallSingleActivity;
import com.mypoc.ptt.webrtc.voip.VoipEvent;
import com.mypoc.pttlibrary.api.IPTTSDK;

import butterknife.BindView;
import butterknife.ButterKnife;

public class RtcTalkActivity extends BaseActivity {

    private String TAG = "RtcTalkActivity";

    private IPTTSDK pttSDK;

    @BindView(R.id.talk_audio)
    Button talkAudio;

    @BindView(R.id.talk_video)
    Button talkVideo;

    @BindView(R.id.btn_back)
    Button btnBack;

    @BindView(R.id.tv_call_info)
    TextView tvCallInfo;

    public static final String KEY_USER_ID = "userId";
    public static final String KEY_USER_NAME = "userName";

    String userId;
    String userName;

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
        setContentView(R.layout.activity_rtc_talk);
        ButterKnife.bind(this); //先绑定，再做 initListen事件

        // 获取Intent
        Intent intent = getIntent();
        userId = intent.getStringExtra(KEY_USER_ID);
        userName = intent.getStringExtra(KEY_USER_NAME);

        //
        tvCallInfo.setText("对方信息："+ userName+"("+userId+")");

        addListener();

    }

    private void addListener() {

        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });

        //
        talkAudio.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                SkyEngineKit.init(new VoipEvent());
                CallSingleActivity.openActivity(RtcTalkActivity.this, userId, true, userName, true, false);
            }
        });

        talkVideo.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                CallSingleActivity.openActivity(RtcTalkActivity.this, userId, true, userName, false, false);
            }
        });

    }

    @Override
    public void onBackPressed() {
        finish();
    }


}