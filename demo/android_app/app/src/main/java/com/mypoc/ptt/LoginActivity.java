package com.mypoc.ptt;

import static android.content.pm.PackageManager.PERMISSION_GRANTED;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.core.content.FileProvider;

import android.Manifest;
import android.app.Activity;
import android.content.ActivityNotFoundException;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.provider.Settings;
import android.speech.tts.TextToSpeech;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.Toast;

import com.google.android.material.textfield.TextInputEditText;
import com.mypoc.ptt.activity.MainActivity;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.pref.LoginPrefereces;
import com.mypoc.ptt.utils.AESHelper;
import com.mypoc.ptt.utils.FileUtil;
import com.mypoc.pttlibrary.api.PTTConfig;
import com.mypoc.pttlibrary.callback.InitializeCallback;
import com.mypoc.pttlibrary.callback.LoginCallback;

import java.io.File;
import java.util.List;

import butterknife.BindView;
import butterknife.ButterKnife;
public class LoginActivity extends AppCompatActivity {

    @BindView(R.id.et_account)
    TextInputEditText etAccount;

    @BindView(R.id.et_password)
    TextInputEditText etPassword;

    @BindView(R.id.et_serverip)
    TextInputEditText etServerIp;

    @BindView(R.id.loginButton)
    Button loginButton;

    private static String TAG = LoginActivity.class.getName();
    private boolean pttSDKStatus;

    private  static final int REQUEST_INSTALL_PACKAGES = 1;
    private  static final int OVERLAY_PERMISSION_REQ_CODE = 1234; //悬浮窗权限请求
    private  static final int PERMISSION_REQUEST_CODE_MANAGE_ALL_FILES = 1001; //ANDROID 11以上的存储权限

    MyPOCApplication application = null;
    //监听按键服务
    private Intent intent_keymoniservice;
    private String masterPasswordKey = "abc@jimy!!7876@";  //用于aes加解密

    private static final int ALL_NEED_PERMISSION = 100;       //系列权限请求
    static final String[] LOCATIONGPS = new String[]{
            Manifest.permission.READ_EXTERNAL_STORAGE,
            Manifest.permission.WRITE_EXTERNAL_STORAGE,
            Manifest.permission.RECORD_AUDIO,
            Manifest.permission.MODIFY_AUDIO_SETTINGS,
            Manifest.permission.INTERNET,
            //Manifest.permission.FOREGROUND_SERVICE,
            Manifest.permission.ACCESS_LOCATION_EXTRA_COMMANDS,
            Manifest.permission.READ_PHONE_STATE};

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        //以下是系统状态栏透明效果的处理
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
        }
        else if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
            Window window = getWindow();
            window.setFlags(WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS,
                    WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS);
        }

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        Log.i(TAG,"进入oncreate");

        ButterKnife.bind(this);
        application= MyPOCApplication.getInstance();
        openQuanXian(this);  //动态申请权限，主要是麦克风采集权限，一定要给

        //android 10.0需要手动开启 悬浮窗权限 Settings.ACTION_MANAGE_OVERLAY_PERMISSION
        if (Build.VERSION.SDK_INT >=  Build.VERSION_CODES.M &&
                !Settings.canDrawOverlays(this) ){

            if (!Settings.canDrawOverlays(application.getApplicationContext())){

                Toast.makeText(LoginActivity.this, "请授权允许悬浮设置，否则直播无法使用！", Toast.LENGTH_SHORT).show();
                //Intent intent = new Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION,  Uri.parse("package:" + getPackageName()) );
                //没有悬浮窗权限m,去开启悬浮窗权限
                try{

                    Intent intent = new Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION);
                    //intent.addCategory("android.intent.category.DEFAULT");
                    intent.setData(Uri.parse(String.format("package:%s",getApplicationContext().getPackageName())));

                    startActivityForResult(intent, OVERLAY_PERMISSION_REQ_CODE);


                } catch (Exception e)
                {
                    // 如果无法打开，则跳转到应用详情页
                    Intent intent = new Intent(Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
                    intent.setData(Uri.parse("package:" + getPackageName()));
                    startActivity(intent);
                    //e.printStackTrace();
                }

            }else {
                // 这里说明6.0系统已经有权限了，可以把要操作的代码在这里
            }

        }else {
            //这里android6.0以下的系统，不用权限检查，直接操作
        }

        init();
        addListener();

        //tts engine, 废除掉 espeak tts ,虽然是开源的离线tts, 但中文声音很机械化
        //国内科大讯飞,百度,腾讯等tts都收费，开发者可自行接入
        //以下以系统自带的tts引擎为例
        //由于 android 各种版本对 应用内安装 apk的权限限制不同
        checkGoogleTTSEngine();

        //不用启用KeyMonitorService，按键ptt事件放到mainactivity中
        /*if (!isAccessibilityServiceEnabled(this, KeyMonitorService.class)){
            Intent intent = new Intent(Settings.ACTION_ACCESSIBILITY_SETTINGS);
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            startActivity(intent);
            Toast.makeText(this, "请找到并启用" + getString(R.string.app_name) + "的无障碍服务",
                    Toast.LENGTH_LONG).show();
        } else {
            //key监听服务
            intent_keymoniservice = new Intent(LoginActivity.this, KeyMonitorService.class);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                startForegroundService(intent_keymoniservice);
            } else {
                startService(intent_keymoniservice);
            }

        }*/

        //在上一次明确退出后 isLoggedIn=false时，是不能自动登录的，所以要确保上一次是登录过的，然后关机重启
        autoLogin();

    }

    private void autoLogin(){
        //尝试自动登录
        String account= LoginPrefereces.getData_String(LoginActivity.this, LoginPrefereces.accountKey);

        String password= LoginPrefereces.getData_String(LoginActivity.this, LoginPrefereces.pswKey);
        String serverIp= LoginPrefereces.getData_String(LoginActivity.this, LoginPrefereces.serverAddrKey);
        boolean isLoggedIn= LoginPrefereces.getDefualtState(LoginActivity.this,LoginPrefereces.isLoggedInKey,false);

        if (isLoggedIn && !TextUtils.isEmpty(account) && !TextUtils.isEmpty(password) && !TextUtils.isEmpty(serverIp)
         ) {

            Log.i(TAG,"自动登录");
            //尝试登录
            //初始化PTT管理器， 因为有服务IP配置
            String restServerUrl= "http://"+ serverIp+":17003/ptt/";

            PTTConfig config = new PTTConfig.Builder()
                    .setRestServerUrl(restServerUrl)   //rest服务地址
                    .setTcpServerHost(serverIp)                     //tcp服务的ip
                    .setTcpServerPort(17001)                                //tcp服务的port
                    .setHeartbeatIntervalSec(15)                             //心跳间隔，单位:秒，这个值不能超过服务器设定的心跳间隔，否则会有问题
                    .setMicOwnerTimeoutSec(60)                               //抢到麦后讲话的最长时间，到时没释放麦，会自动强制释放，单位:秒
                    .setPttKeyVal(131)                                       //ptt键值,如 131, 这个因厂家而异，要咨询各厂家的技术人员
                    .setPttDownBroadCastVal("android.intent.action.DOWN_PTT_KEY")       //ptt键按下的广播字符串,如 qmstar.keyflag.ptt.down, 这个因厂家而异，要咨询各厂家的技术人员
                    .setPttUpBroadCastVal("android.intent.action.UP_PTT_KEY")           //ptt键抬起的广播字符串,如 qmstar.keyflag.ptt.up, 这个因厂家而异，要咨询各厂家的技术人员
                    .setPttUseBroadCastMode(true)                            //ptt键用键值监听，还是用广播方式监听， true: 用广播， false: 用键值
                    .build();


            MyPOCApplication.getInstance().setPttKeyVal(config.getPttKeyVal());  //记住
            MyPOCApplication.getInstance().setPttUseBroadCastMode(config.isPttUseBroadCastMode()); //记住

            MyPOCApplication.getInstance().getPttSDK().initialize(config, new InitializeCallback() {
                @Override
                public void onSuccess() {
                    pttSDKStatus=true;
                    Log.d(TAG, "PTTSDK初始化成功");
                }

                @Override
                public void onFailure(String error) {
                    pttSDKStatus=false;
                    Log.e(TAG, "PTTSDK初始化失败: " + error);
                }
            });

            if (!pttSDKStatus){
                Toast.makeText(LoginActivity.this,"PTTSDK初始化失败",Toast.LENGTH_SHORT).show();
                return;
            }

            password= AESHelper.decrypt(password,masterPasswordKey); //先解密

            MyPOCApplication.getInstance().getPttSDK().login(account,
                    password,
                    new LoginCallback() {
                        @Override
                        public void onSuccess(String token, long expiresIn, String refreshToken) {
                            runOnUiThread(() -> {
                                // 登录成功跳转到主页面
                                Log.i(TAG,"TOKEN="+token);
                                //成功登录后写配置文件

                                startActivity(new Intent(LoginActivity.this, MainActivity.class));
                                finish();
                            });
                        }

                        @Override
                        public void onFailure(String error) {
                            runOnUiThread(() -> {
                                Toast.makeText(LoginActivity.this, error, Toast.LENGTH_SHORT).show();
                            });
                        }
                    }
            );

        }

    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == OVERLAY_PERMISSION_REQ_CODE) {
            if (Build.VERSION.SDK_INT >= 23) {
                if (!Settings.canDrawOverlays(application.getApplicationContext())) {
                    Toast.makeText(LoginActivity.this, "授权APP允许悬浮设置失败，功能会受影响", Toast.LENGTH_SHORT).show();
                } else {
                    Toast.makeText(LoginActivity.this, "权限授予成功！", Toast.LENGTH_SHORT).show();

                }
            }

        } else if (requestCode == LoginActivity.PERMISSION_REQUEST_CODE_MANAGE_ALL_FILES) {

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
                if (Environment.isExternalStorageManager()) {

                    //已开通
                    Log.e("LoginActivity", "android11以上已开通存储权限");

                } else {
                    Toast.makeText(this, "Allow permission for storage access!", Toast.LENGTH_SHORT).show();
                }
            }

        }
    }

    //登录前的检查
    private boolean preCheckForLogin(){
        //再检查一次麦克风权限
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {

            //判断是否为android6.0系统版本，如果是，需要动态添加权限
            if (ContextCompat.checkSelfPermission(LoginActivity.this, Manifest.permission.RECORD_AUDIO)
                    != PERMISSION_GRANTED) {// 没有权限，申请权限。
                Toast.makeText(LoginActivity.this,"麦克风权限没有授权,请先授权才能登录",Toast.LENGTH_LONG).show();


                ActivityCompat.requestPermissions(LoginActivity.this, LOCATIONGPS, ALL_NEED_PERMISSION);
                return false;

            }

        }

        //
        if (etAccount.getText().toString().isEmpty()){
            Toast.makeText(LoginActivity.this,"帐号不能为空",Toast.LENGTH_SHORT).show();
            return false;
        }
        if (etPassword.getText().toString().isEmpty()){
            Toast.makeText(LoginActivity.this,"密码不能为空",Toast.LENGTH_SHORT).show();
            return false;
        }

        if (etServerIp.getText().toString().isEmpty()){
            Toast.makeText(LoginActivity.this,"服务地址不能为空",Toast.LENGTH_SHORT).show();
            return false;
        }


        return true;
    }

    private void init(){
        //加载pref
        String account= LoginPrefereces.getData_String(LoginActivity.this, LoginPrefereces.accountKey);
        if (!TextUtils.isEmpty(account))
            etAccount.setText(account);

        String serverIp= LoginPrefereces.getData_String(LoginActivity.this, LoginPrefereces.serverAddrKey);
        if (!TextUtils.isEmpty(serverIp))
            etServerIp.setText(serverIp);

        String password= LoginPrefereces.getData_String(LoginActivity.this, LoginPrefereces.pswKey);
        if (!TextUtils.isEmpty(password)){
            password= AESHelper.decrypt(password,masterPasswordKey);
            etPassword.setText(password);
        }



    }

    private void addListener() {

        loginButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                //预登录检查
                if (!preCheckForLogin())
                    return;

                // 初始化PTT管理器， 因为有服务IP配置
                //http://159.75.230.229:17003/ptt/
                String serverIp= etServerIp.getText().toString().trim();
                String restServerUrl= "http://"+ serverIp+":17003/ptt/";

                PTTConfig config = new PTTConfig.Builder()
                        .setRestServerUrl(restServerUrl)   //rest服务地址
                        .setTcpServerHost(serverIp)                     //tcp服务的ip
                        .setTcpServerPort(17001)                                //tcp服务的port
                        .setHeartbeatIntervalSec(15)                             //心跳间隔，单位:秒，这个值不能超过服务器设定的心跳间隔，否则会有问题
                        .setMicOwnerTimeoutSec(60)                               //抢到麦后讲话的最长时间，到时没释放麦，会自动强制释放，单位:秒
                        .setPttKeyVal(131)                                       //ptt键值,如 131, 这个因厂家而异，要咨询各厂家的技术人员
                        .setPttDownBroadCastVal("android.intent.action.DOWN_PTT_KEY")       //ptt键按下的广播字符串,如 qmstar.keyflag.ptt.down, 这个因厂家而异，要咨询各厂家的技术人员
                        .setPttUpBroadCastVal("android.intent.action.UP_PTT_KEY")           //ptt键抬起的广播字符串,如 qmstar.keyflag.ptt.up, 这个因厂家而异，要咨询各厂家的技术人员
                        .setPttUseBroadCastMode(true)                            //ptt键用键值监听，还是用广播方式监听， true: 用广播， false: 用键值
                        .build();


                MyPOCApplication.getInstance().setPttKeyVal(config.getPttKeyVal());  //记住
                MyPOCApplication.getInstance().setPttUseBroadCastMode(config.isPttUseBroadCastMode()); //记住

                MyPOCApplication.getInstance().getPttSDK().initialize(config, new InitializeCallback() {
                    @Override
                    public void onSuccess() {
                        pttSDKStatus=true;
                        Log.d(TAG, "PTTSDK初始化成功");
                    }

                    @Override
                    public void onFailure(String error) {
                        pttSDKStatus=false;
                        Log.e(TAG, "PTTSDK初始化失败: " + error);
                    }
                });

                Log.i(TAG,etAccount.getText().toString());
                Log.i(TAG,etPassword.getText().toString());
                Log.i(TAG,etServerIp.getText().toString());

                if (!pttSDKStatus){
                    Toast.makeText(LoginActivity.this,"PTTSDK初始化失败",Toast.LENGTH_SHORT).show();
                    return;
                }

                MyPOCApplication.getInstance().getPttSDK().login(etAccount.getText().toString().trim(),
                        etPassword.getText().toString().trim(),
                        new LoginCallback() {
                            @Override
                            public void onSuccess(String token, long expiresIn, String refreshToken) {
                                runOnUiThread(() -> {
                                    // 登录成功跳转到主页面
                                    Log.i(TAG,"TOKEN="+token);
                                    //成功登录后写配置文件
                                    LoginPrefereces.setData_String(LoginActivity.this,
                                            LoginPrefereces.serverAddrKey,etServerIp.getText().toString().trim());

                                    LoginPrefereces.setData_String(LoginActivity.this,
                                            LoginPrefereces.accountKey,etAccount.getText().toString().trim());

                                    LoginPrefereces.setData_String(LoginActivity.this,
                                            LoginPrefereces.pswKey, AESHelper.encrypt(etPassword.getText().toString().trim(),masterPasswordKey));

                                    //在退出时要写false
                                    LoginPrefereces.setState(LoginActivity.this,
                                            LoginPrefereces.isLoggedInKey,true);

                                    startActivity(new Intent(LoginActivity.this, MainActivity.class));
                                    finish();
                                });
                            }

                            @Override
                            public void onFailure(String error) {
                                runOnUiThread(() -> {
                                    Toast.makeText(LoginActivity.this, error, Toast.LENGTH_SHORT).show();
                                });
                            }
                        }
                );
            }
        });

    }

    public  void openQuanXian(final Activity activity) {

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {

            //判断是否为android6.0系统版本，如果是，需要动态添加权限
            if (ContextCompat.checkSelfPermission(activity, Manifest.permission.RECORD_AUDIO)
                    != PERMISSION_GRANTED) {// 没有权限，申请权限。
                ActivityCompat.requestPermissions(activity, LOCATIONGPS, ALL_NEED_PERMISSION);

            }

        }

    }



    private boolean isAccessibilityServiceEnabled(Context context, Class<?> serviceClass) {
        String serviceName = new ComponentName(context, serviceClass).flattenToString();
        Log.i(TAG,"SERVICENAME="+serviceName);

        String enabledServices = Settings.Secure.getString(
                context.getContentResolver(),
                Settings.Secure.ENABLED_ACCESSIBILITY_SERVICES
        );
        return enabledServices != null && enabledServices.contains(serviceName);
    }

    private void checkGoogleTTSEngine(){

        // 获取所有已安装的 TTS 引擎
        List<TextToSpeech.EngineInfo> engines = new TextToSpeech(this, null)
                .getEngines(); // 注意：此方法需要在 TTS 初始化后调用

        if (engines==null || engines.size()==0){
            Log.e(TAG,"没有发现tts引擎");

            //没有tts引擎
            boolean exists= FileUtil.isFileExistInExternalStorage("tts-google.apk");
            if (!exists){
                //拷贝
                FileUtil.copyAssetToExternalStorage(this,"tts-google.apk","tts-google.apk");

            }
            File ttsApkFile = new File(Environment.getExternalStorageDirectory(), "tts-google.apk");
            if (ttsApkFile.exists()){
                Toast.makeText(this, "tts apk 存在，引导安装", Toast.LENGTH_LONG).show();
            } else {
                Toast.makeText(this, "TTS包复制失败", Toast.LENGTH_LONG).show();
                return;
            }

            //先申请安装权限
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                // 对于Android 8.0及以上版本，不能直接请求安装未知来源应用的权限
                // 需要引导用户到系统设置中手动启用
                showInstallPackagesPermissionDialog();

            } else {
                // 对于Android 8.0以下版本，尝试请求安装未知来源应用的权限（尽管这通常不是通过权限请求对话框完成的）
                // 但由于这个权限在大多数设备上是通过系统设置控制的，所以这里的代码可能不会有实际效果
                if (ContextCompat.checkSelfPermission(this, Manifest.permission.INSTALL_PACKAGES)
                        != PackageManager.PERMISSION_GRANTED) {
                    ActivityCompat.requestPermissions(this,
                            new String[]{Manifest.permission.INSTALL_PACKAGES},
                            REQUEST_INSTALL_PACKAGES);
                } else {
                    // 权限已经启用（尽管这通常不是通过这种方式检查的），执行你的逻辑
                    doInstallTTSApk(ttsApkFile);
                }
            }

        }

    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        switch (requestCode) {

            case REQUEST_INSTALL_PACKAGES:
                if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    // 权限被授予（尽管这通常不会发生，因为INSTALL_PACKAGES不是运行时权限）
                    File  ttsApkFile = new File(Environment.getExternalStorageDirectory(), "tts-google.apk");
                    doInstallTTSApk(ttsApkFile);

                } else {
                    // 权限被拒绝
                    Toast.makeText(LoginActivity.this,"取消后,语音播报将无效",Toast.LENGTH_LONG).show();
                }

                break;
            case ALL_NEED_PERMISSION:
                //应用需要的麦克风等权限
                if (grantResults.length > 0 && grantResults[0] == PERMISSION_GRANTED) {
                    // 权限被授予，可以获取IMSI

                } else {
                    // 权限被拒绝，无法获取IMSI
                }

                break;
            default:
                break;
        }
    }

    private void  doInstallTTSApk(File  ttsApkFile) {
        Uri apkUri;
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.N){
            apkUri= Uri.fromFile(ttsApkFile);
        } else {
            apkUri = FileProvider.getUriForFile(this, "com.mypoc.ptt.fileProvider", ttsApkFile);
        }

        Intent installIntent = new Intent(Intent.ACTION_VIEW);
        installIntent.setDataAndType(apkUri, "application/vnd.android.package-archive");
        installIntent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        installIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        try {
            startActivity(installIntent);
        } catch (ActivityNotFoundException e) {
            e.printStackTrace();
            Toast.makeText(this, "Cannot install TTS engine. Please manually install it.", Toast.LENGTH_LONG).show();
        }
    }

    private void showInstallPackagesPermissionDialog() {
        // 显示一个对话框，引导用户到系统设置中手动启用安装未知来源应用的权限
        new android.app.AlertDialog.Builder(this)
                .setTitle("POC对讲: 需要安装TTS语音合成引擎")
                .setMessage("为了安装此应用，您需要在系统设置中允许安装未知来源的应用。")
                .setPositiveButton("去设置", (dialog, which) -> {
                    Intent intent = new Intent(Settings.ACTION_MANAGE_UNKNOWN_APP_SOURCES);
                    Uri uri = Uri.fromParts("package", "com.weiding.poc", null);
                    intent.setData(uri);
                    startActivity(intent);
                    //startActivityForResult(intent, REQUEST_INSTALL_PACKAGES); // 注意：这里使用startActivityForResult可能不会有回调，因为这是一个系统设置页面
                    File  ttsApkFile = new File(Environment.getExternalStorageDirectory(), "tts-google.apk");
                    //因为上面可能得不到用户的回调，下面不妨直接安装
                    doInstallTTSApk(ttsApkFile);

                })
                .setNegativeButton("取消", (dialog, which) -> {
                    // 用户取消操作
                    Toast.makeText(LoginActivity.this,"取消后,语音播报将无效",Toast.LENGTH_LONG).show();
                })
                .show();
    }

}