package com.mypoc.ptt.activity.keypad;

import androidx.appcompat.app.AppCompatActivity;

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
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.R;
import com.mypoc.ptt.activity.keypad.adapter.GroupAdapterPad;
import com.mypoc.ptt.adapter.GroupAdapter;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.event.TtsSpeakEvent;
import com.mypoc.ptt.event.UpdateWorkingGroupEvent;
import com.mypoc.ptt.model.PTTGroupExt;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;
import com.mypoc.pttlibrary.model.PTTGroup;

import org.greenrobot.eventbus.EventBus;

import java.util.ArrayList;
import java.util.List;

import butterknife.BindView;
import butterknife.ButterKnife;

public class GroupSelectActivity extends BaseActivity {
    private String TAG = "GroupSelectActivity";

    private IPTTSDK pttSDK;
    private Handler mHandler;

    @BindView(R.id.allgroup_list)
    ListView lvGroup;

    @BindView(R.id.tv_title)
    TextView tvTitle;

    private GroupAdapterPad  groupAdapterPad;

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
        setContentView(R.layout.activity_group_select);

        ButterKnife.bind(this); //先绑定，再做 initListen事件
        mHandler = new Handler(Looper.getMainLooper());
        pttSDK = MyPOCApplication.getInstance().getPttSDK();

        tvTitle.setText("切换对讲组");
        addListener();

        initGroups();

    }

    private void addListener() {
        lvGroup.setOnFocusChangeListener((v, hasFocus) -> {
            Log.d(TAG, "ListView has focus: " + hasFocus);
        });
    }

    @Override
    protected void onResume() {
        super.onResume();

    }

    private void initGroups() {

        List<PTTGroupExt>  groupList = new ArrayList<>();
        List<PTTGroup> fixGroups= MyPOCApplication.getInstance().getFixGroups().getValue();
        for (PTTGroup group:
        fixGroups) {
            groupList.add(new PTTGroupExt(group.getGroupId(),group.getGroupName(),group.getOwnerId(),
                    group.getCreateDate(),1));
        }
        List<PTTGroup> tempGroups= MyPOCApplication.getInstance().getTempGroups().getValue();
        for (PTTGroup group:
                tempGroups) {
            groupList.add(new PTTGroupExt(group.getGroupId(),group.getGroupName(),group.getOwnerId(),
                    group.getCreateDate(),0));
        }

        groupAdapterPad = new GroupAdapterPad(this, groupList);
        groupAdapterPad.setCurrentGroupPosition(MyPOCApplication.getInstance().getCurrGroupId());
        lvGroup.setAdapter(groupAdapterPad);

        // 设置 ListView 的点击事件
        lvGroup.setOnItemClickListener((parent, view, position, id) -> {
            Toast.makeText(this, "Clicked: " + groupList.get(position).getGroupName(), Toast.LENGTH_SHORT).show();
            enterGroup(groupList.get(position).getGroupId());

        });

        // 确保 ListView 可获取焦点（关键！）
        lvGroup.setFocusable(true);
        lvGroup.requestFocus();

    }

    private void enterGroup(Integer groupId){
        pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
            @Override
            public void onSuccess() {
                //获得组名称
                String groupName= PttHelper.findGroupName(groupId);
                EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId,groupName));

                //发送播报tts语音事件
                EventBus.getDefault().post(new TtsSpeakEvent("进入"+groupName));
                //
                if (groupAdapterPad!=null){
                    groupAdapterPad.setCurrentGroupPosition(groupId);  //更新listview中当前组图标
                }
            }

            @Override
            public void onFailure(String error) {
                new Handler(Looper.getMainLooper()).post(new Runnable() {
                    @Override
                    public void run() {
                        Toast.makeText( GroupSelectActivity.this ,"进入失败,原因:"+error,Toast.LENGTH_LONG).show();
                    }
                });
            }
        });
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

                if (lvGroup.getSelectedItemPosition()>=0){
                    Log.i(TAG,"lvGroup.getSelectedItemPosition()="+lvGroup.getSelectedItemPosition());
                    lvGroup.performItemClick(
                            lvGroup.getChildAt(lvGroup.getSelectedItemPosition()),
                            lvGroup.getSelectedItemPosition(),0
                    );
                }
                return true;
        }

        return super.onKeyDown(keyCode, event);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
    }
}