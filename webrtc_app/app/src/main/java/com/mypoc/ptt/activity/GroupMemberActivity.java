package com.mypoc.ptt.activity;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.app.AlertDialog;
import android.content.DialogInterface;
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
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.R;
import com.mypoc.ptt.adapter.GroupAdapter;
import com.mypoc.ptt.adapter.GroupMemberAdapter;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.event.EnterPrevGroupEvent;
import com.mypoc.ptt.event.UpdateGroupMemberStausEvent;
import com.mypoc.ptt.event.UpdateWorkingGroupEvent;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.TempGroupDeleteCallback;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTGroupMember;
import com.mypoc.pttlibrary.model.PTTUser;

import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.greenrobot.eventbus.ThreadMode;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import butterknife.BindView;
import butterknife.ButterKnife;
import io.reactivex.Single;
import io.reactivex.android.schedulers.AndroidSchedulers;
import io.reactivex.disposables.CompositeDisposable;
import io.reactivex.schedulers.Schedulers;

public class GroupMemberActivity extends BaseActivity {

    private String TAG = "GroupMemberActivity";

    private IPTTSDK pttSDK;

    @BindView(R.id.btn_back)
    Button btnBack;

    @BindView(R.id.tv_group_name)
    TextView tvGroupName;

    @BindView(R.id.tv_group_type)
    TextView tvGroupType;

    @BindView(R.id.tv_member_count)
    TextView tvMemberCount;

    @BindView(R.id.tv_create_time)
    TextView tvCreateTime;

    @BindView(R.id.tv_creator)
    TextView tvCreator;

    @BindView(R.id.rv_member_list)
    RecyclerView rvMember;

    @BindView(R.id.deletegroup)
    Button btnDeleteGroup;

    @BindView(R.id.ll_group_oper)
    LinearLayout llGroupOper;

    GroupMemberAdapter groupMemberAdapter;
    /**
     * 当前的成员，有可能会动态的增减
     */
    private List<PTTGroupMember> currGroupMembers = new ArrayList<>();

    /**
     * 当前组ID
     */
    int currGroupId = -1;
    final Object lock = new Object(); // 专门的锁对象

    public static final String KEY_GROUP_ID = "groupId";
    public static final String KEY_GROUP_TYPE = "groupType";
    private Handler mHandler;

    private CompositeDisposable disposables = new CompositeDisposable();

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
        setContentView(R.layout.activity_group_member);

        EventBus.getDefault().register(this);
        ButterKnife.bind(this); //先绑定，再做 initListen事件

        // 获取Intent
        Intent intent = getIntent();
        int groupId = intent.getIntExtra(KEY_GROUP_ID, -1);
        int groupType = intent.getIntExtra(KEY_GROUP_TYPE, -1);

        currGroupId = groupId;
        mHandler = new Handler(Looper.getMainLooper());
        pttSDK = MyPOCApplication.getInstance().getPttSDK();

        addListener();
        //获取指定组信息和其中的成员
        getGroupAndMembers(groupId, groupType);
    }

    private void addListener() {
        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                GroupMemberActivity.this.finish();
            }
        });

        btnDeleteGroup.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //
                deleteTempGroup(currGroupId,7,0);

            }
        });

    }

    //
    private void deleteTempGroup( int group_id, int type, int priv) {
        AlertDialog.Builder dialog = new AlertDialog.Builder(GroupMemberActivity.this);
        dialog.setTitle(getResources().getString(R.string.confirm))
                .setIcon(android.R.drawable.ic_dialog_info)
                .setMessage(
                        getResources().getString(R.string.prompt_delete_groups)+": 【"+ tvGroupName.getText().toString()+"】")
                .setPositiveButton(
                        getResources().getString(R.string.yes),
                        new DialogInterface.OnClickListener() {

                            @Override
                            public void onClick(DialogInterface dialog,
                                                int which) {
                                //
                                pttSDK.tempGroupDelete(group_id, type, priv, new TempGroupDeleteCallback() {
                                    @Override
                                    public void onSuccess() {
                                        //要关闭和退出前一个组
                                        Log.i(TAG, "主动解散组:"+group_id);
                                        //不用处理其它, 本帐号也会收到解散组消息，在那里也会处理和同步各种事件
                                        finish();
                                    }

                                    @Override
                                    public void onFailure(String error) {
                                        mHandler.post(()-> {
                                            Toast.makeText(GroupMemberActivity.this,error,Toast.LENGTH_LONG).show();
                                        });
                                    }
                                });

                            }
                        })
                .setNegativeButton(getResources().getString(R.string.no),
                        new DialogInterface.OnClickListener() {

                            public void onClick(DialogInterface dialog,
                                                int which) {
                                dialog.cancel();// 取消弹出框
                            }
                        }).create().show();
    }


    /**
     * @param groupId
     * @param groupType 1: 表示固定组  0：表示临时组
     */
    private void getGroupAndMembers(int groupId, int groupType) {

        btnDeleteGroup.setVisibility(View.INVISIBLE);
        llGroupOper.setVisibility(View.GONE);

        List<PTTGroup> groups;
        if (groupType == 1) {
            groups = MyPOCApplication.getInstance().getFixGroups().getValue();
            for (PTTGroup item : groups
            ) {
                if (item.getGroupId().equals(groupId)) {
                    tvGroupName.setText(item.getGroupName());
                    tvGroupType.setText("固定组");
                    tvCreateTime.setText("");  //固定组没有创建时间，由web管理后台创建
                    tvCreator.setText("管理后台");
                    break;
                }
            }
            //固定组，不允许在客户端删除


        } else {

            groups = MyPOCApplication.getInstance().getTempGroups().getValue();
            for (PTTGroup item : groups
            ) {
                if (item.getGroupId().equals(groupId)) {
                    tvGroupName.setText(item.getGroupName());
                    tvGroupType.setText("临时组");
                    if (item.getCreateDate() != null) {
                        tvCreateTime.setText(PttHelper.convertUnixTime(item.getCreateDate()));
                    } else
                        tvCreateTime.setText("");  //固定组没有创建时间，由web管理后台创建
                    //查找创建人
                    if (item.getOwnerId() != null) {
                        tvCreator.setText(PttHelper.findUserName(item.getOwnerId()));
                        //如果是本人创建的，允许显示删除按钮
                        if (item.getOwnerId().equals(MyPOCApplication.getInstance().getUserId())){
                            //
                            llGroupOper.setVisibility(View.VISIBLE);
                            btnDeleteGroup.setVisibility(View.VISIBLE);
                        }

                    } else
                        tvCreator.setText("");
                    break;
                }
            }

        }

        //
        Single.fromCallable(() -> {
                    if (groupType == 1)
                        return pttSDK.queryFixGroupMembers(groupId);
                    else
                        return pttSDK.queryTempGroupMembers(groupId);
                }).subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(

                        groupMembers -> {
                            // 处理成功的结果

                            currGroupMembers= new ArrayList<>(groupMembers);
                            Log.i(TAG, "初始化时currGroupMembers=" + currGroupMembers.size());
                            handleGroupMembers(currGroupMembers);
                        },
                        throwable -> {
                            // 处理错误
                            handleError(throwable);
                        }
                );

    }

    private void handleError(Throwable throwable) {
        Log.e(TAG, Log.getStackTraceString(throwable));
        Toast.makeText(this, "获取数据异常:" + throwable.getMessage(), Toast.LENGTH_LONG).show();
    }

    private void handleGroupMembers(List<PTTGroupMember> groupMembers) {

        if (groupMemberAdapter == null) {
            // 首次初始化Adapter
            groupMemberAdapter = new GroupMemberAdapter(this, groupMembers);

            rvMember.setLayoutManager(new LinearLayoutManager(this));
            rvMember.setAdapter(groupMemberAdapter);
        } else {
            // 后续只更新数据
            Log.i(TAG, "加入" + groupMembers.size());
            groupMemberAdapter.updateData(groupMembers);
        }

        //成员数量
        int memberCount = (groupMembers == null ? 0 : groupMembers.size());
        tvMemberCount.setText(memberCount + "");

    }


    //改变当前组成员的在组状态
    private void changeCurrGroupMembers(int userId, int status) {
        synchronized (lock){
        if (this.currGroupMembers == null || this.currGroupMembers.isEmpty())
            return;
        for (PTTGroupMember member : this.currGroupMembers
        ) {
            if (member.getUserId().equals(userId)) {
                if (status == UpdateGroupMemberStausEvent.Status_EnterGroup) {
                    member.setLogon(1);
                    member.setListen("y");
                } else if (status == UpdateGroupMemberStausEvent.Status_ExitGroup) {
                    member.setLogon(1);
                    member.setListen("n");
                } else if (status == UpdateGroupMemberStausEvent.Status_Offline) {
                    member.setLogon(0);
                    member.setListen("n");
                }

                break;
            }
        }
        }
    }

    //新增或删除当前组的成员
    @RequiresApi(api = Build.VERSION_CODES.N)
    private void addOrDeleteCurrGroupMember(String userStr, int status) {
        synchronized (lock){

        String[] userArr = userStr.split(",");
        for (int i = 0; i < userArr.length; i++) {

            if (status == UpdateGroupMemberStausEvent.Status_PullInGroup) {
                PTTUser user = PttHelper.findPttUser(Integer.valueOf(userArr[i]));
                if (user == null) continue;
                this.currGroupMembers.add(new PTTGroupMember(user.getUserId(), user.getPhone(),
                        user.getUserName(), user.getLogon(), user.getMyclass(), "y"));
            } else if (status == UpdateGroupMemberStausEvent.Status_KickOutGroup) {
                //被踢出
                int finalI = i;
                this.currGroupMembers.removeIf(member -> member.getUserId().equals(Integer.valueOf(userArr[finalI])));

            }

        }

        }
    }

    //这里只更新adapter的当前组同步
    @RequiresApi(api = Build.VERSION_CODES.N)
    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onUpdateGroupMemberStaus(UpdateGroupMemberStausEvent event) {

        int status = event.getStatus();

        switch (status) {

            case UpdateGroupMemberStausEvent.Status_Offline:
                //用户离线了
                changeCurrGroupMembers(event.getUserId(), event.getStatus());
                handleGroupMembers(this.currGroupMembers);
                break;

            case UpdateGroupMemberStausEvent.Status_EnterGroup:
                //进入的不是当前组也要处理，当作离开了当前组处理：重要 （因为服务端没有发送离开消息）
                if (this.currGroupId != event.getGroupId()){
                    //当作离开
                    Log.i(TAG, "离开onUpdateGroupMemberStaus");
                    changeCurrGroupMembers(event.getUserId(), UpdateGroupMemberStausEvent.Status_ExitGroup );
                    handleGroupMembers(this.currGroupMembers);
                    return;
                }

                Log.i(TAG, "进入onUpdateGroupMemberStaus");
                changeCurrGroupMembers(event.getUserId(), event.getStatus());
                handleGroupMembers(this.currGroupMembers);
                break;

            case UpdateGroupMemberStausEvent.Status_ExitGroup:
                //其它人离开当前组，先判断是否为当前组
                if (this.currGroupId != event.getGroupId()){
                    //当作离开
                    return;
                }
                //
                Log.i(TAG, "离开onUpdateGroupMemberStaus");
                changeCurrGroupMembers(event.getUserId(), event.getStatus());
                handleGroupMembers(this.currGroupMembers);

                break;

            case UpdateGroupMemberStausEvent.Status_PullInGroup:
            case UpdateGroupMemberStausEvent.Status_KickOutGroup:
                Log.i(TAG,"收到强拉强踢:"+event.getStatus());

                //其它人(可能有多人)被踢出组
                if (event.getChangeUserStr() == null || event.getChangeUserStr().isEmpty())
                    return;

                //如果是自己被强踢了，还要自动退出当前页面
                String[] userArr= event.getChangeUserStr().split(",");
                if (event.getStatus()==UpdateGroupMemberStausEvent.Status_KickOutGroup &&
                        Arrays.asList(userArr).contains(MyPOCApplication.getInstance().getUserId()+"")
                ) {
                    //含本人被踢出了
                    Toast.makeText(this,"被踢出了",Toast.LENGTH_LONG).show();
                    finish();
                    return;
                }

                if (this.currGroupId != event.getGroupId())
                    return;

                addOrDeleteCurrGroupMember(event.getChangeUserStr(), event.getStatus());
                handleGroupMembers(this.currGroupMembers);

                break;
            default:
                break;
        }

    }


    @Override
    protected void onDestroy() {
        // 取消所有订阅，防止内存泄漏
        disposables.clear();
        if (EventBus.getDefault().isRegistered(this))
            EventBus.getDefault().unregister(this);

        super.onDestroy();
    }
}