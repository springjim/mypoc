package com.mypoc.ptt.activity.keypad;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ListView;
import android.widget.Toast;

import com.mypoc.ptt.R;
import com.mypoc.ptt.activity.keypad.adapter.GroupMemberAdapterPad;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTGroupMember;

import java.util.ArrayList;
import java.util.List;

import butterknife.BindView;
import butterknife.ButterKnife;
import io.reactivex.Single;
import io.reactivex.android.schedulers.AndroidSchedulers;
import io.reactivex.schedulers.Schedulers;

/**
 * 适配keypad按键的组成员页面
 */
public class PadGroupMemberActivity extends BaseActivity {

    private String TAG = "PadGroupMemberActivity";

    @BindView(R.id.allmember_list)
    RecyclerView rvGroupMembers;

    private IPTTSDK pttSDK;
    private Handler mHandler;

    GroupMemberAdapterPad groupMemberAdapterPad;

    /**
     * 当前的成员，有可能会动态的增减
     */
    private List<PTTGroupMember> currGroupMembers = new ArrayList<>();

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
        setContentView(R.layout.activity_pad_group_member);

        ButterKnife.bind(this); //先绑定，再做 initListen事件
        mHandler = new Handler(Looper.getMainLooper());
        pttSDK = MyPOCApplication.getInstance().getPttSDK();

        addListener();
        initMembers();
        
    }

    private void initMembers() {
        int groupId= MyPOCApplication.getInstance().getCurrGroupId();
        int groupType =1;  //1: 表示固定组  0：表示临时组
        //检查组类型
        List<PTTGroup> tempGroups=  MyPOCApplication.getInstance().getTempGroups().getValue();
        for (PTTGroup group:
                tempGroups) {

            if (group.getGroupId().equals(groupId)){
                groupType=0;
                break;
            }
        }

        //查询组成员
        int finalGroupType = groupType;
        Single.fromCallable(() -> {
                    if (finalGroupType == 1)
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

    private void addListener() {
        //

    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        switch (keyCode) {
            case KeyEvent.KEYCODE_BACK:
                //返回上一页
                Log.i(TAG,"back键");
                finish();
                return  true;

            case  KeyEvent.KEYCODE_DPAD_CENTER:
                Log.i(TAG,"ok键");

                return true;
        }
        return super.onKeyDown(keyCode, event);
    }

    private void handleGroupMembers(List<PTTGroupMember> groupMembers) {

        if (groupMemberAdapterPad == null) {
            // 首次初始化Adapter
            groupMemberAdapterPad = new GroupMemberAdapterPad(this, groupMembers);

            rvGroupMembers.setLayoutManager(new LinearLayoutManager(this));
            rvGroupMembers.setAdapter(groupMemberAdapterPad);

        } else {
            // 后续只更新数据

        }

    }


    private void handleError(Throwable throwable) {
        Log.e(TAG, Log.getStackTraceString(throwable));
        Toast.makeText(this, "获取数据异常:" + throwable.getMessage(), Toast.LENGTH_LONG).show();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
    }
}