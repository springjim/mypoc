package com.mypoc.ptt.webrtc;


import android.content.Context;
import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.TextUtils;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.Toast;

import androidx.annotation.RequiresApi;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.textfield.TextInputEditText;
import com.mypoc.ptt.R;
import com.mypoc.ptt.activity.CreateGroupActivity;
import com.mypoc.ptt.adapter.ContactCheckAdapter;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.model.PTTUserExt;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.ptt.webrtc.room.CallMultiActivity;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.model.PTTUser;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import butterknife.BindView;
import butterknife.ButterKnife;

//视频会议邀请,MESH方式
public class InviteMeetActivity  extends BaseActivity {

    private String TAG = "InviteMeetActivity";
    private IPTTSDK pttSDK;

    @BindView(R.id.btn_back)
    Button btnBack;

    @BindView(R.id.et_meet_name)
    TextInputEditText etRoomName;

    @BindView(R.id.btn_create_meet)
    Button btnCreateMeet;
    @BindView(R.id.rv_contact_list)
    RecyclerView rvContactList;

    private ContactCheckAdapter contactCheckAdapter;
    List<PTTUserExt> currUsers;
    private String   currUserIdJoin;

    private Handler mHandler;

    @RequiresApi(api = Build.VERSION_CODES.N)
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
        setContentView(R.layout.activity_invite_meet);
        ButterKnife.bind(this); //先绑定，再做 initListen事件
        pttSDK = MyPOCApplication.getInstance().getPttSDK();
        mHandler = new Handler(Looper.getMainLooper());

        addListener();
        
        //会议名称改用创建人userId作为前缀
        String timeStr = String.valueOf(System.currentTimeMillis());
        String meetName = "room-"+MyPOCApplication.getInstance().getUserId() + "0" + timeStr.substring(timeStr.length() - 3);
        etRoomName.setText(meetName);

        //加载通讯录
        loadContactsAndInitListView();
        // 延迟隐藏键盘（确保布局已加载完成）
        etRoomName.postDelayed(() -> {
            InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(etRoomName.getWindowToken(), 0);
        }, 500);
    }

    @RequiresApi(api = Build.VERSION_CODES.N)
    private void loadContactsAndInitListView() {

        List<PTTUser> currVal = MyPOCApplication.getInstance().getAllUsers().getValue();
        if (currVal != null && !currVal.isEmpty()) {
            initListViewWithData(currVal);
            return;
        }

    }

    @RequiresApi(api = Build.VERSION_CODES.N)
    private void initListViewWithData(List<PTTUser> currVal) {
        currUsers = new ArrayList<>();
        for (PTTUser user : currVal) {
            PTTUserExt ext = new PTTUserExt();
            ext.setSelected(false);
            ext.setUserName(user.getUserName());
            ext.setUserId(user.getUserId());
            ext.setPhone(user.getPhone());
            ext.setLogon(user.getLogon());
            currUsers.add(ext);
        }
        if (contactCheckAdapter == null) {
            contactCheckAdapter = new ContactCheckAdapter(InviteMeetActivity.this, currUsers, item -> {
                //回调处理
                updateCurrUsers(item);
            });

        }
        rvContactList.setLayoutManager(new LinearLayoutManager(InviteMeetActivity.this));
        rvContactList.setAdapter(contactCheckAdapter);

    }

    @RequiresApi(api = Build.VERSION_CODES.N)
    private void updateCurrUsers(PTTUserExt userExt) {
        if (currUsers == null || currUsers.isEmpty()) return;
        currUsers.stream().filter(item ->
                        item.getUserId().equals(userExt.getUserId())
                ).findFirst()
                .ifPresent(item -> item.setSelected(userExt.isSelected()));
    }

    private void addListener() {

        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                InviteMeetActivity.this.finish();
            }
        });

        btnCreateMeet.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //创建room
                String roomName = etRoomName.getText().toString();
                if (!check(roomName))
                    return;

                // 创建一个房间并进入
                CallMultiActivity.openActivity(InviteMeetActivity.this,
                        roomName, true, currUserIdJoin);

                InviteMeetActivity.this.finish();
            }
        });

    }

    private boolean check(String groupName) {
        if (TextUtils.isEmpty(groupName)) {
            Toast.makeText(InviteMeetActivity.this, "组名称不允许为空", Toast.LENGTH_LONG).show();
            return false;
        }
        List<String> seledUser = new ArrayList<>();
        for (PTTUserExt ext :
                currUsers) {
            if (ext.isSelected())
                seledUser.add(ext.getUserId() + "");
        }
        if (seledUser.size()==0){
            Toast.makeText(InviteMeetActivity.this, "没有选择成员", Toast.LENGTH_LONG).show();
            return false;
        }

        currUserIdJoin=  String.join(",", seledUser);
        return true;
    }

}