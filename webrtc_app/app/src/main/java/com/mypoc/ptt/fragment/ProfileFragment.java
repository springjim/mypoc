package com.mypoc.ptt.fragment;

import android.app.Activity;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.Fragment;

import android.os.Handler;
import android.os.Looper;
import android.text.TextUtils;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.GridLayout;
import android.widget.RelativeLayout;
import android.widget.Toast;

import com.mypoc.ptt.LoginActivity;
import com.mypoc.ptt.R;
import com.mypoc.ptt.activity.AppConfigActivity;
import com.mypoc.ptt.activity.CreateGroupActivity;
import com.mypoc.ptt.activity.RtspPushActivity;
import com.mypoc.ptt.activity.SosCallActivity;
import com.mypoc.ptt.activity.backgroud.BackgroundActivity;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.pref.LoginPrefereces;
import com.mypoc.ptt.service.FloatingTalkService;
import com.mypoc.ptt.utils.InputDialogUtil;
import com.mypoc.ptt.webrtc.InviteMeetActivity;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.LogoutCallback;

import butterknife.BindView;
import butterknife.ButterKnife;


/**
 * 放一些业务功能，系统设置及退出功能等
 */
public class ProfileFragment extends Fragment implements View.OnClickListener {

    private final  String TAG = "ProfileFragment";
    private IPTTSDK pttSDK;
    private Handler mHandler;

    @BindView(R.id.fun_grid)
    GridLayout gridLayout;

    @BindView(R.id.re_setting)
    RelativeLayout reSetting;
    @BindView(R.id.re_exitapp)
    RelativeLayout reExitapp;
    @BindView(R.id.re_creategroup)
    RelativeLayout reCreateGroup;

    @BindView(R.id.re_createmeet)
    RelativeLayout reCreateMeeting;

    @BindView(R.id.re_rtsp_push)
    RelativeLayout reRtspPush;

    @BindView(R.id.re_rtsp_push_back)
    RelativeLayout retReRtspPushBack;

    @BindView(R.id.re_sos)
    RelativeLayout reSos;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        pttSDK = MyPOCApplication.getInstance().getPttSDK();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {


        return inflater.inflate(R.layout.fragment_profile, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        //EventBus.getDefault().register(this);
        ButterKnife.bind(this, view); // 在 onViewCreated 中绑定

        // 绑定View的生命周期
        mHandler = new Handler(Looper.getMainLooper());
        addListener();
        updateGridLayoutColumns();
    }

    private void addListener() {
        reSetting.setOnClickListener(this);
        reExitapp.setOnClickListener(this);
        reCreateGroup.setOnClickListener(this);
        reCreateMeeting.setOnClickListener(this);
        reRtspPush.setOnClickListener(this);
        retReRtspPushBack.setOnClickListener(this);
        reSos.setOnClickListener(this);
    }


    private void updateGridLayoutColumns() {
        DisplayMetrics displayMetrics = new DisplayMetrics();
        getActivity().getWindowManager().getDefaultDisplay().getMetrics(displayMetrics);
        int screenWidth = displayMetrics.widthPixels;
        Log.i(TAG,"screenWidth="+screenWidth);
        int columns;
        if (screenWidth >= 1080) {
            columns = 4; // 大屏幕显示4列
        } else if (screenWidth >= 480) {
            columns = 3; // 中等屏幕显示3列
        } else {
            columns = 2; // 小屏幕显示2列
        }

        gridLayout.setColumnCount(columns);
    }

    @Override
    public void onClick(View v) {
        //
        switch (v.getId()){

            case R.id.re_sos:
                startActivity(new Intent(requireActivity(), SosCallActivity.class));
                break;

            case R.id.re_rtsp_push_back:
                startActivity(new Intent(requireActivity(), BackgroundActivity.class));

                break;

            case R.id.re_rtsp_push:
                startActivity(new Intent(requireActivity(), RtspPushActivity.class));

                break;

            case R.id.re_createmeet:
                //视频会议
                //创建一个会议室，并选中人员，确认后通过信令发出邀请与会人员
                startActivity(new Intent(getActivity(), InviteMeetActivity.class));

                break;

            case R.id.re_setting:
                //弹出密码窗口，输入对了才能修改系统参数, 客户业务定制化
                InputDialogUtil.showInputDialog(
                        requireActivity(),
                        "密码验证",
                        "修改系统参数需要输入密码，默认是666666",
                        "请输入密码",
                        new InputDialogUtil.InputDialogListener() {
                            @Override
                            public void onConfirm(String inputText) {
                                // 处理确认操作
                                //验证输入密码是否正确
                                if (TextUtils.isEmpty(inputText)) {
                                    Toast.makeText(requireActivity(),
                                            "请输入密码",
                                            Toast.LENGTH_SHORT).show();
                                } else {

                                    if (inputText.equalsIgnoreCase(MyPOCApplication.getInstance().getDefaultAppconfigPwd())){
                                        //进入系统设置页面
                                        startActivity(new Intent(requireActivity(), AppConfigActivity.class));

                                    } else {
                                        Toast.makeText(requireActivity(),
                                                "密码不对",
                                                Toast.LENGTH_SHORT).show();
                                    }

                                }

                            }

                            @Override
                            public void onCancel() {
                                // 处理取消操作
                                Toast.makeText(requireActivity(),
                                        "用户取消了输入",
                                        Toast.LENGTH_SHORT).show();
                            }
                        }
                );


                break;

            case R.id.re_exitapp:
                completeQuitDialog( requireActivity() );
                break;

            case R.id.re_creategroup:
                //创建临时群组
                //检查权限,默认有权限，为了测试
                boolean createGroupPriv =
                        LoginPrefereces.getDefualtState(requireActivity(),LoginPrefereces.createGroupKey,true);

                if (!createGroupPriv){
                    mHandler.post(() -> {
                        Toast.makeText(requireActivity(),"创建群组权限没有打开，请进入系统参数修改设置",Toast.LENGTH_LONG).show();
                    });

                    return;
                }

                startActivity(new Intent(getActivity(), CreateGroupActivity.class));
                break;

            default:
                break;
        }
    }

    private  void completeQuitDialog(final Activity context) {
        AlertDialog.Builder builder = new AlertDialog.Builder((Context)context);
        builder.setMessage("退出后，重开机不会自动登录，确认要退出应用吗?");
        builder.setTitle("提示");
        builder.setPositiveButton(R.string.exitComplete, new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface param1DialogInterface, int param1Int) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.append("completeQuitDialog() - 当前要销毁的activity：" );
                stringBuilder.append(context);
                Log.e("MainActivity", stringBuilder.toString());

                //设为false, 下次开机不会自动登录
                LoginPrefereces.setState( requireActivity(),LoginPrefereces.isLoggedInKey,false);

                //先主动退出socket服务
                pttSDK.logout(new LogoutCallback() {
                    @Override
                    public void onSuccess() {
                        Log.i(TAG,"发送socket logout成功");
                        exitApp();
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"发送socket logout失败"+error);
                        exitApp();
                    }
                });

            }
        });
        builder.setNegativeButton(R.string.cancel, new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface param1DialogInterface, int param1Int) {
                param1DialogInterface.dismiss();
            }
        });
        builder.create().show();
    }

    private void exitApp(){
        requireActivity().finish();

        Intent intent= new Intent(requireActivity(), FloatingTalkService.class);
        requireActivity().stopService(intent);

        //要先关闭服务，要不然是Service.START_STICKY,在退出时又自动创建
        if (pttSDK!=null)
            pttSDK.release();

        //2025.10 不退出整个应用，
        //android.os.Process.killProcess(android.os.Process.myPid());
        //System.exit(0);

        //清空任务栈，返回登录页面
        Intent intentLogin = new Intent(requireActivity(), LoginActivity.class);
        intentLogin.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        requireActivity().startActivity(intentLogin);

    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
    }


}