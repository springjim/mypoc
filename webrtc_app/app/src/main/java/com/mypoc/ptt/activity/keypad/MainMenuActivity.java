package com.mypoc.ptt.activity.keypad;

import androidx.appcompat.app.AppCompatActivity;

import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.os.Bundle;
import android.provider.Settings;
import android.view.KeyEvent;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.R;
import com.mypoc.ptt.webrtc.base.BaseActivity;

public class MainMenuActivity extends BaseActivity {
    private TextView tv_menu_witchgroup, tv_menu_groupmember,tv_menu_appconfig, tv_menu_sysconfig;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main_menu);
        tv_menu_witchgroup=findViewById(R.id.menuItem_switchgroup);
        tv_menu_groupmember=findViewById(R.id.menuItem_groupmember);
        tv_menu_appconfig=findViewById(R.id.menuItem_appconfig);
        tv_menu_sysconfig=findViewById(R.id.menuItem_sysconfig);
        tv_menu_witchgroup.requestFocus();
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {

        View currentFocusView = getCurrentFocus(); // 获取当前焦点的 View
        switch (keyCode) {
            case KeyEvent.KEYCODE_BACK:
                //返回上一页
                finish();
                return  true;
            case  KeyEvent.KEYCODE_DPAD_CENTER:

                if (currentFocusView!=null){

                    switch (currentFocusView.getId()){

                        case R.id.menuItem_switchgroup:
                            Toast.makeText(this,"按了切换组",Toast.LENGTH_SHORT).show();

                            Intent intent= new Intent(getBaseContext(), GroupSelectActivity.class);
                            startActivity(intent);

                            break;
                        case R.id.menuItem_groupmember:
                            Toast.makeText(this,"按了群成员",Toast.LENGTH_SHORT).show();
                            Intent intentGroupMember= new Intent(getBaseContext(), PadGroupMemberActivity.class);
                            startActivity(intentGroupMember);

                            break;
                        case R.id.menuItem_appconfig:
                            Toast.makeText(this,"按了应用设置",Toast.LENGTH_SHORT).show();
                            Intent intentUserConfig= new Intent(getBaseContext(), UserConfigActivity.class);
                            startActivity(intentUserConfig);
                            break;
                        case R.id.menuItem_sysconfig:
                            try {
                                Intent intentSysConfig = new Intent(Settings.ACTION_SETTINGS);
                                startActivity(intentSysConfig);
                            } catch (ActivityNotFoundException e) {
                                // 处理无法找到设置应用的情况
                                Toast.makeText(this, "无法打开系统设置", Toast.LENGTH_SHORT).show();
                            }
                            break;
                        default:
                            break;
                    }

                }
                return true;

        }


        return super.onKeyDown(keyCode, event);
    }


}