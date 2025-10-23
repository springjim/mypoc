package com.mypoc.ptt.activity;

import androidx.appcompat.app.AppCompatActivity;

import android.graphics.Color;
import android.hardware.lights.LightState;
import android.os.Build;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.Switch;
import android.widget.TextView;

import com.mypoc.ptt.R;
import com.mypoc.ptt.model.SettingItem;
import com.mypoc.ptt.pref.LoginPrefereces;
import com.mypoc.ptt.webrtc.base.BaseActivity;

import java.util.ArrayList;
import java.util.List;

import butterknife.BindView;
import butterknife.ButterKnife;

public class AppConfigActivity extends BaseActivity {


    @BindView(R.id.settings_container)
    LinearLayout settingsContainer;

    @BindView(R.id.tv_title)
    TextView title;

    @BindView(R.id.btn_back)
    Button btnBack;

    // 配置项定义
    private List<SettingItem> settingItems = new ArrayList<>();


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
        setContentView(R.layout.activity_app_config);
        //
        ButterKnife.bind(this);

        title.setText("系统参数");
        initItems();  //填充配置项
        addListener();
    }

    private void addListener() {

        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                finish();
            }
        });

    }

    private void initItems() {

        String serverIp= LoginPrefereces.getData_String(this,LoginPrefereces.serverAddrKey);
        if (TextUtils.isEmpty(serverIp))
            serverIp= "159.75.230.229";

        //服务IP地址
        settingItems.add(new SettingItem(LoginPrefereces.serverAddrKey, "服务IP地址", SettingItem.Type.EDIT_TEXT, serverIp));

        //单呼
        settingItems.add(new SettingItem(LoginPrefereces.singleTalkKey,"单呼权限", SettingItem.Type.SWITCH,
                LoginPrefereces.getDefualtState(this,LoginPrefereces.singleTalkKey,false)));
        //切换群组
        settingItems.add(new SettingItem(LoginPrefereces.switchGroupKey,"切换群组权限", SettingItem.Type.SWITCH,
                LoginPrefereces.getDefualtState(this,LoginPrefereces.switchGroupKey,false)));

        //创建群组权限
        settingItems.add(new SettingItem(LoginPrefereces.createGroupKey,"创建群组权限", SettingItem.Type.SWITCH,
                LoginPrefereces.getDefualtState(this,LoginPrefereces.createGroupKey,false)));

        // 为每个配置项创建UI
        for (SettingItem item : settingItems) {
            addSettingItem(item);
        }

    }

    private void addSettingItem(SettingItem item) {
        // 创建行容器
        LinearLayout rowLayout = new LinearLayout(this);
        rowLayout.setLayoutParams(new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MATCH_PARENT,
                LinearLayout.LayoutParams.WRAP_CONTENT));
        rowLayout.setOrientation(LinearLayout.HORIZONTAL);
        rowLayout.setPadding(0, 12, 0, 12);

        // 添加标题
        TextView titleView = new TextView(this);
        titleView.setLayoutParams(new LinearLayout.LayoutParams(
                0,
                LinearLayout.LayoutParams.WRAP_CONTENT,
                1)); // 权重为1，占据剩余空间
        titleView.setText(item.getTitle());
        titleView.setTextSize(16);
        rowLayout.addView(titleView);

        // 根据类型添加不同的控件
        switch (item.getType()) {
            case SWITCH:
                addSwitchItem(rowLayout, item);
                break;
            case EDIT_TEXT:
                addEditTextItem(rowLayout, item);
                break;
        }

        settingsContainer.addView(rowLayout);
        
    }

    private void addSwitchItem(LinearLayout rowLayout, SettingItem item) {
        Switch switchView = new Switch(this);
        switchView.setLayoutParams(new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WRAP_CONTENT,
                LinearLayout.LayoutParams.WRAP_CONTENT));


        switchView.setChecked(Boolean.parseBoolean(item.getDefaultValue().toString()));

        // 设置监听器
        switchView.setOnCheckedChangeListener((buttonView, isChecked) -> {
             LoginPrefereces.setState(AppConfigActivity.this, item.getKey(),isChecked);
        });

        rowLayout.addView(switchView);
    }

    private void addEditTextItem(LinearLayout rowLayout, SettingItem item) {
        EditText editText = new EditText(this);
        editText.setLayoutParams(new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WRAP_CONTENT,
                LinearLayout.LayoutParams.WRAP_CONTENT));
        editText.setMinEms(10); // 设置最小宽度

        editText.setText(item.getDefaultValue().toString());

        // 设置监听器
        editText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {}

            @Override
            public void afterTextChanged(Editable s) {
                 LoginPrefereces.setData_String(AppConfigActivity.this, item.getKey(),s.toString().trim());
            }
        });

        rowLayout.addView(editText);
    }


}