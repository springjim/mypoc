package com.mypoc.ptt.activity.keypad;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.BuildConfig;
import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.pref.LoginPrefereces;
import com.mypoc.ptt.webrtc.base.BaseActivity;

import butterknife.BindView;
import butterknife.ButterKnife;

/**
 * 应用设置
 */
public class UserConfigActivity extends BaseActivity {

    private String TAG = "UserConfigActivity";

    @BindView(R.id.account_content)
    TextView tvAccount;

    @BindView(R.id.ll_account)
    LinearLayout llAccount;

    @BindView(R.id.username_content)
    TextView tvUsername;

    @BindView(R.id.ttsplay_content)
    Switch switchTtsplay;

    @BindView(R.id.location_content)
    Switch switchLocation;

    @BindView(R.id.singlecall_content)
    Switch switchSingleCall;

    @BindView(R.id.versionName_content)
    TextView tvVersionName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_user_config);

        ButterKnife.bind(this); //先绑定，再做 initListen事件

        tvAccount.setText(MyPOCApplication.getInstance().getUserId()+"");
        tvUsername.setText(MyPOCApplication.getInstance().getUserName());
        switchTtsplay.setChecked(true);
        switchLocation.setChecked(LoginPrefereces.getData_String(
                UserConfigActivity.this,LoginPrefereces.flagAutoLocationKey).equalsIgnoreCase("Y"));

        switchSingleCall.setChecked(LoginPrefereces.getData_String(
                UserConfigActivity.this,LoginPrefereces.privSingleCallKey).equalsIgnoreCase("Y"));

        tvVersionName.setText(BuildConfig.VERSION_NAME);
        llAccount.requestFocus();
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

                        /*case R.id.menuItem_switchgroup:
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

                            break;*/
                        default:
                            break;
                    }

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