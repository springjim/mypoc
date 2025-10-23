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

import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.event.EnterPrevGroupEvent;
import com.mypoc.ptt.event.ExitDeleteGroupEvent;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.pttlibrary.api.IPTTSDK;

import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.greenrobot.eventbus.ThreadMode;

import butterknife.BindView;
import butterknife.ButterKnife;

/**
 * 收到双向通话邀请，并进入的页面
 */
public class IncomeTwoWayGroupTalkActivity extends BaseActivity {

    private String TAG = "IncomeTwoWayGroupTalkActivity";
    private IPTTSDK pttSDK;
    /**
     * 当前组ID
     */
    int currGroupId = -1;
    int currInviteMode = -1;
    int currInviteUserId = -1;
    int prevGroupId=-1;

    public static final String KEY_GROUP_ID = "groupId";
    public static final String KEY_INVITE_MODE="inviteMode";
    public static final String KEY_INVITE_USERID="inviteUserId";  //发出邀请的人Id
    public static final String KEY_PREV_GROUP_ID="prevGroupId";

    private Handler mHandler;

    @BindView(R.id.btn_back)
    Button btnBack;

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
        setContentView(R.layout.activity_income_twoway_group_talk);

        ButterKnife.bind(this); //先绑定，再做 initListen事件
        EventBus.getDefault().register(this);

        // 获取Intent
        Intent intent = getIntent();
        int groupId = intent.getIntExtra(KEY_GROUP_ID, -1);
        int inviteMode = intent.getIntExtra(KEY_INVITE_MODE, -1);
        int inviteUserId = intent.getIntExtra(KEY_INVITE_USERID, -1);

        prevGroupId=  intent.getIntExtra(KEY_PREV_GROUP_ID, -1);
        currGroupId = groupId;
        currInviteMode= inviteMode;
        currInviteUserId=inviteUserId;
        Log.i(TAG,"currGroupId="+currGroupId+",prevGroupId="+prevGroupId);



        mHandler = new Handler(Looper.getMainLooper());
        pttSDK = MyPOCApplication.getInstance().getPttSDK();

        addListener();

    }

    @Override
    public void onBackPressed() {
        exitGroup();
    }

    private void addListener() {
        //
        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                exitGroup();
            }
        });

    }


    private void exitGroup(){

        if (MyPOCApplication.getInstance().getUserId()!= currInviteUserId) {
            //是被邀请的, 调用rest退出群组接口

            MyPOCApplication.getInstance().removeFromTempGroups(currGroupId);
            //todo... 这里不管是对讲模式或 双向模式的临时组，都执行下，关闭双向模式，恢复状态
            pttSDK.closeTwoWay();

            Log.i(TAG,"发送进入上个组消息, 当前组删除："+currGroupId);
            EventBus.getDefault().post(new EnterPrevGroupEvent(prevGroupId));

        } else {
            //主叫方退出, 要调用删除群组
        }

        IncomeTwoWayGroupTalkActivity.this.finish();
    }


    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onExitDeleteGroupMessage(ExitDeleteGroupEvent messageEvent){

        if (messageEvent.getGroupId()==this.currGroupId){
            //主叫方删除了该组, 被叫方要退出
            finish();
        }

    }


    @Override
    protected void onDestroy() {
        if (EventBus.getDefault().isRegistered(this))
            EventBus.getDefault().unregister(this);
        super.onDestroy();
    }
}