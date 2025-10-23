package com.mypoc.ptt.activity;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.Toast;

import com.google.android.material.textfield.TextInputEditText;
import com.mypoc.ptt.R;
import com.mypoc.ptt.adapter.ContactAdapter;
import com.mypoc.ptt.adapter.ContactCheckAdapter;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.event.TtsSpeakEvent;
import com.mypoc.ptt.event.UpdateWorkingGroupEvent;
import com.mypoc.ptt.model.PTTUserExt;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;
import com.mypoc.pttlibrary.callback.TempGroupCreateCallback;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTUser;

import org.greenrobot.eventbus.EventBus;

import java.util.ArrayList;
import java.util.List;

import butterknife.BindView;
import butterknife.ButterKnife;

//创建临时组
public class CreateGroupActivity extends BaseActivity {

    private String TAG = "CreateGroupActivity";
    private IPTTSDK pttSDK;

    @BindView(R.id.btn_back)
    Button btnBack;

    @BindView(R.id.et_group_name)
    TextInputEditText etGroupName;

    @BindView(R.id.btn_create_group)
    Button btnCreateGroup;
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
        setContentView(R.layout.activity_create_group);
        ButterKnife.bind(this); //先绑定，再做 initListen事件
        pttSDK = MyPOCApplication.getInstance().getPttSDK();
        mHandler = new Handler(Looper.getMainLooper());

        addListener();

        //初始化组名称
        //临时组名称改用创建人名称作为前缀
        String timeStr = String.valueOf(System.currentTimeMillis());
        String groupName = MyPOCApplication.getInstance().getUserName() + "0" + timeStr.substring(timeStr.length() - 2);
        etGroupName.setText(groupName);

        //加载通讯录
        loadContactsAndInitListView();
        // 延迟隐藏键盘（确保布局已加载完成）
        etGroupName.postDelayed(() -> {
            InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(etGroupName.getWindowToken(), 0);
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
            contactCheckAdapter = new ContactCheckAdapter(CreateGroupActivity.this, currUsers, item -> {
                //回调处理
                updateCurrUsers(item);
            });

        }
        rvContactList.setLayoutManager(new LinearLayoutManager(CreateGroupActivity.this));
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
                CreateGroupActivity.this.finish();
            }
        });

        btnCreateGroup.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //创建临时群组
                String groupName = etGroupName.getText().toString();
                if (!check(groupName))
                    return;

                //调用创建临时组
                pttSDK.tempGroupCreate(groupName, MyPOCApplication.getInstance().getUserId(), currUserIdJoin,
                        new TempGroupCreateCallback() {
                            @Override
                            public void onSuccess(PTTGroup group) {

                                //加入组
                                PTTGroup pttGroupExt= new PTTGroup(group.getGroupId(),group.getGroupName(),
                                        MyPOCApplication.getInstance().getUserId(),System.currentTimeMillis()/1000);
                                MyPOCApplication.getInstance().addToTempGroups(pttGroupExt);  //让groupfragment 会感知更新

                                pttSDK.joinGroup(group.getGroupId(), new ReportWorkGroupCallback() {
                                    @Override
                                    public void onSuccess() {

                                        MyPOCApplication.getInstance().setCurrGroupId(group.getGroupId());

                                        EventBus.getDefault().post(new TtsSpeakEvent("进入"+groupName));
                                        //创建成功,跳转到一个专用的对讲页面（与单呼一样）
                                        //创建的组也会发给自己，在邀请时，要判断下邀请人是不是自己，是的话，则不要处理
                                        Intent intent = new Intent(CreateGroupActivity.this, TempTalkActivity.class);
                                        intent.putExtra(TempTalkActivity.KEY_GROUP_ID,group.getGroupId());
                                        intent.putExtra(TempTalkActivity.KEY_GROUP_NAME,group.getGroupName());
                                        startActivity(intent);
                                        finish();
                                    }

                                    @Override
                                    public void onFailure(String error) {
                                        Log.e(TAG,error);
                                    }
                                });



                            }

                            @Override
                            public void onFailure(String error) {
                                mHandler.post(() -> {
                                   Toast.makeText(CreateGroupActivity.this,"创建失败:"+error,Toast.LENGTH_LONG).show();
                                });
                            }
                        });

            }
        });

    }


    private boolean check(String groupName) {
        if (TextUtils.isEmpty(groupName)) {
            Toast.makeText(CreateGroupActivity.this, "组名称不允许为空", Toast.LENGTH_LONG).show();
            return false;
        }
        List<String> seledUser = new ArrayList<>();
        for (PTTUserExt ext :
                currUsers) {
            if (ext.isSelected())
                seledUser.add(ext.getUserId() + "");
        }
        if (seledUser.size()==0){
            Toast.makeText(CreateGroupActivity.this, "没有选择成员", Toast.LENGTH_LONG).show();
            return false;
        }

        StringBuffer fuids = new StringBuffer();
        for (String str : seledUser) {
            fuids.append(str).append(',');
        }
        currUserIdJoin= MyPOCApplication.getInstance().getUserId()+","+fuids.toString();
        return true;
    }

}