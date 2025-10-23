package com.mypoc.ptt.webrtc.room;

import android.Manifest;
import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.View;
import android.view.WindowManager;
import android.widget.ImageView;

import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;

import com.dds.base.permission.Permissions;
import com.dds.skywebrtc.CallSession;
import com.dds.skywebrtc.EnumType;
import com.dds.skywebrtc.SkyEngineKit;
import com.dds.skywebrtc.exception.NotInitializedException;
import com.mypoc.ptt.R;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.ptt.webrtc.voip.VoipEvent;


import org.webrtc.EglBase14;

import java.util.UUID;

/**
 * Created by dds on 2018/7/26.
 * 多人通话界面
 */
public class CallMultiActivity extends BaseActivity implements CallSession.CallSessionCallback, View.OnClickListener {

    private String TAG = "CallMultiActivity";

    private SkyEngineKit gEngineKit;
    private final Handler handler = new Handler(Looper.getMainLooper());
    private ImageView meetingHangupImageView;
    private CallSession.CallSessionCallback currentFragment;

    public static final String EXTRA_MO = "isOutGoing";
    public static final String EXTRA_ROOM_OTHERUSER = "roomOtherUserIds";

    private boolean isOutgoing;


    public static void openActivity(Context activity, String room, boolean isOutgoing, String roomOtherUserIds) {
        Intent intent = new Intent(activity, CallMultiActivity.class);
        intent.putExtra("room", room);
        intent.putExtra(EXTRA_MO, isOutgoing);
        intent.putExtra(EXTRA_ROOM_OTHERUSER,roomOtherUserIds);
        activity.startActivity(intent);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN |
                WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON |
                WindowManager.LayoutParams.FLAG_SHOW_WHEN_LOCKED |
                WindowManager.LayoutParams.FLAG_TURN_SCREEN_ON);
        getWindow().getDecorView().setSystemUiVisibility(getSystemUiVisibility());
        setContentView(R.layout.activity_multi_call);
        initView();
        initListener();
        initData();
    }

    @Override
    public void onBackPressed() {

    }

    private void initView() {
        meetingHangupImageView = findViewById(R.id.meetingHangupImageView);
        Fragment fragment = new FragmentMeeting();
        FragmentManager fragmentManager = getSupportFragmentManager();
        fragmentManager.beginTransaction()
                .add(R.id.meeting_container, fragment)
                .commit();
        currentFragment = (CallSession.CallSessionCallback) fragment;
    }

    private void initListener() {
        meetingHangupImageView.setOnClickListener(this);
    }

    private void initData() {
        Intent intent = getIntent();
        String room = intent.getStringExtra("room");
        isOutgoing = intent.getBooleanExtra(EXTRA_MO, false);
        String roomOtherUserIds= intent.getStringExtra(EXTRA_ROOM_OTHERUSER);

        try {
            gEngineKit = SkyEngineKit.Instance();
        } catch (NotInitializedException e) {
            finish();
        }
        String[] per = new String[]{Manifest.permission.RECORD_AUDIO, Manifest.permission.CAMERA};
        Permissions.request(this, per, integer -> {
            if (integer == 0) {
                // 权限同意
                init(room, isOutgoing,roomOtherUserIds);
            } else {
                // 权限拒绝
                CallMultiActivity.this.finish();
            }
        });


    }

    private void init(String room, boolean isOutgoing, String roomOtherUserIds) {
        //SkyEngineKit.init(new VoipEvent());
        if (isOutgoing) {
            // 创建一个房间并进入
            Log.i(TAG, "room="+room+", roomOtherUserIds="+roomOtherUserIds);
            //gEngineKit.createAndJoinRoom(this, "room-" + UUID.randomUUID().toString().substring(0, 16));
            gEngineKit.createAndJoinRoom(this, room,roomOtherUserIds );

        } else {
            //被邀请时，加入房间
            gEngineKit.joinRoom(getApplicationContext(), room);
        }

        CallSession session = gEngineKit.getCurrentSession();
        if (session == null) {
            this.finish();
        } else {
            session.setSessionCallback(this);

        }


    }


    public SkyEngineKit getEngineKit() {
        return gEngineKit;
    }


    @TargetApi(19)
    private static int getSystemUiVisibility() {
        int flags = View.SYSTEM_UI_FLAG_HIDE_NAVIGATION | View.SYSTEM_UI_FLAG_FULLSCREEN;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
            flags |= View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
        }
        return flags;
    }


    //-------------------------------------------------回调相关------------------------------------
    @Override
    public void didCallEndWithReason(EnumType.CallEndReason var1) {
        finish();
    }

    @Override
    public void didChangeState(EnumType.CallState callState) {
        handler.post(() -> currentFragment.didChangeState(callState));
    }

    @Override
    public void didChangeMode(boolean var1) {
        handler.post(() -> currentFragment.didChangeMode(var1));
    }

    @Override
    public void didCreateLocalVideoTrack() {
        handler.post(() -> currentFragment.didCreateLocalVideoTrack());
    }

    @Override
    public void didReceiveRemoteVideoTrack(String userId) {
        handler.post(() -> currentFragment.didReceiveRemoteVideoTrack(userId));
    }

    @Override
    public void didUserLeave(String userId) {
        handler.post(() -> currentFragment.didUserLeave(userId));
    }

    @Override
    public void didError(String var1) {
        finish();
    }

    @Override
    public void didDisconnected(String userId) {

    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.meetingHangupImageView:
                handleHangup();
                break;
        }

    }

    // 处理挂断事件
    private void handleHangup() {
        SkyEngineKit.Instance().leaveRoom();
        this.finish();
    }
}
