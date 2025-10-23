package com.mypoc.ptt.activity;

import androidx.appcompat.app.AppCompatActivity;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.IBinder;
import android.os.Looper;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;

import com.mypoc.ptt.R;
import com.mypoc.ptt.activity.backgroud.RtpService;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.location.BaiduLocUtils;
import com.mypoc.ptt.location.LocCallback;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.CommonCallback;

import java.util.UUID;

import butterknife.BindView;
import butterknife.ButterKnife;

public class SosCallActivity extends AppCompatActivity {

    private String TAG = "SosCallActivity";
    private IPTTSDK pttSDK;

    @BindView(R.id.btn_back)
    Button btnBack;

    @BindView(R.id.talk_img)
    ImageView ivTalkImg;

    private Handler mHandler;
    private long     sosTimeout=  10*60*1000;    //正式环境下，超时设为10分钟
    private boolean isLongPressed = false;
    private final long longPressTimeout = 300L; // 长按时间阈值(毫秒)
    private String  sosSessionId;
    private Runnable sosStopTask;

    //
    //以下用于: 后台视频推流服务
    private RtpService rtpService;
    private boolean isServiceBound = false;
    //以下用于：定位
    private BaiduLocUtils locUtils;

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
        setContentView(R.layout.activity_sos_call);
        ButterKnife.bind(this); //先绑定，再做 initListen事件
        pttSDK = MyPOCApplication.getInstance().getPttSDK();
        mHandler = new Handler(Looper.getMainLooper());

        addListener();

        sosStopTask= new Runnable() {
            @Override
            public void run() {
                Log.e(TAG,"SOS呼叫超时，强制退出");
                isLongPressed=false;
                stopSosSession();
            }
        };

        // 绑定后台rtsp推流服务
        bindRtpService();
        // 初始化定位工具类及回调
        initLocationUtils();

    }

    private void initLocationUtils() {

        locUtils=new BaiduLocUtils(SosCallActivity.this, 5, new LocCallback() {
            @Override
            public void onLocationSuccess(String provider, double latitude, double longitude) {
                //定位成功
                Log.i(TAG, "定位成功,provider="+provider+",latitude="+latitude+",longitude="+longitude);
                pttSDK.sendSOSLocation(MyPOCApplication.getInstance().getCurrGroupId(),
                        MyPOCApplication.getInstance().getUserId(),
                        sosSessionId, (byte) 1, provider, longitude, latitude,
                        new CommonCallback() {
                            @Override
                            public void onSuccess() {
                                Log.i(TAG, "上报成功");
                            }

                            @Override
                            public void onFailure(String error) {
                                Log.e(TAG, "上报失败:"+error);
                            }
                        }
                );
            }

            @Override
            public void onLocationFailed(int errorCode, String errorMsg) {
                //定位失败
                Log.e(TAG, "定位失败:errorCode="+errorCode+",errorMsg="+errorMsg);
            }

            @Override
            public void onLocationTimeout() {
                //定位超时
                Log.e(TAG, "定位超时");

            }
        });

        // 设置定位超时时间
        locUtils.setOvertime(30);

    }

    /**
     * 绑定RTP服务
     */
    private void bindRtpService() {

        Intent intent = new Intent(this, RtpService.class);

        // 预先设置RTSP URL（可选，也可以在initializeStreamSettings中设置）
        intent.putExtra("rtsp_url", "rtsp://192.168.101.140:554/345/100668_sos?callId=3663778888&sign=36673788888&sessionid=fddcc8922ca640b493e8ea3bf3cf5920");
        intent.putExtra("video_width", 720);
        intent.putExtra("video_height", 1280);
        intent.putExtra("video_bitrate", 2500 * 1024);

        bindService(intent, serviceConnection, Context.BIND_AUTO_CREATE);
        startService(intent); // 确保服务启动

    }

    // 服务连接回调
    private final ServiceConnection serviceConnection = new ServiceConnection() {
        @Override
        public void onServiceConnected(ComponentName className, IBinder service) {
            RtpService.RtpServiceBinder binder = (RtpService.RtpServiceBinder) service;
            rtpService = binder.getService();
            isServiceBound = true;
            Log.i(TAG, "RTP service connected");

            // 初始化RTSP URL（但不开始推流）
            // initializeStreamSettings();
        }

        @Override
        public void onServiceDisconnected(ComponentName arg0) {
            rtpService = null;
            isServiceBound = false;
            Log.w(TAG, "RTP service disconnected");

        }
    };

    private Runnable longPressRunnable = new Runnable() {
        @Override
        public void run() {
            isLongPressed = true;
            doTouchHandler();
        }
    };

    private void addListener() {
        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                SosCallActivity.this.finish();
            }
        });
        ivTalkImg.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                        // 按下时开始计时
                        isLongPressed = false;
                        mHandler.postDelayed(longPressRunnable, longPressTimeout);
                        return true;
                    case MotionEvent.ACTION_UP:
                        // 抬起时移除回调并触发结束事件
                        mHandler.removeCallbacks(longPressRunnable);
                        if (isLongPressed) {
                            isLongPressed = false;
                            doTouchHandler();
                        }
                        return true;
                    case MotionEvent.ACTION_CANCEL:
                        // 取消时移除回调
                        mHandler.removeCallbacks(longPressRunnable);
                        isLongPressed = false;
                        return true;
                    default:
                        return false;
                }
            }
        });
    }

    private void doTouchHandler(){
        if (isLongPressed){
            //开始SOS
            mHandler.postDelayed(sosStopTask, sosTimeout);
            startSosSession();

        } else {
            //退出SOS
            mHandler.removeCallbacks(sosStopTask);
            stopSosSession();
        }

    }

    private void startSosSession(){

        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                ivTalkImg.setImageResource(R.mipmap.btn_talk_speak);
                //1. 发送SOS会话报文：开始
                String sessionId =  UUID.randomUUID().toString().replace("-", "").toLowerCase();
                sosSessionId=sessionId;
                pttSDK.sendSOSSession(
                        MyPOCApplication.getInstance().getCurrGroupId(),
                        MyPOCApplication.getInstance().getUserId(),
                        sosSessionId, (byte) 1,
                        new CommonCallback() {
                            @Override
                            public void onSuccess() {
                                //同步调用定位和推流
                                //1.推流
                                if (isServiceBound && rtpService != null) {
                                    rtpService.setVideoResolution(480,640);
                                    rtpService.setVideoBitrate(1500*1024);

                                    rtpService.setRtspUrl(MyPOCApplication.getInstance().getRtspUrl("sos")+
                                            "&sessionid="+sosSessionId);

                                    boolean success = rtpService.startStreaming();
                                    if (success) {
                                        Log.i(TAG,"后台推流成功");
                                    } else {
                                        Log.e(TAG,"后台推流失败");
                                    }
                                }
                                //2.定位
                                Log.d("LocationActivity", "开始定位");
                                locUtils.startBaiduLoc();

                            }

                            @Override
                            public void onFailure(String error) {
                                mHandler.post(new Runnable() {
                                    @Override
                                    public void run() {
                                        Toast.makeText(SosCallActivity.this, "发送sos失败"+error, Toast.LENGTH_SHORT).show();
                                    }
                                });
                            }
                        }
                );

            }
        });
    }

    private void stopSosSession(){

        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                ivTalkImg.setImageResource(R.mipmap.btn_talk_idle);
                //结束sos
                pttSDK.sendSOSSession(
                        MyPOCApplication.getInstance().getCurrGroupId(),
                        MyPOCApplication.getInstance().getUserId(),
                        sosSessionId, (byte) 2,
                        new CommonCallback() {
                            @Override
                            public void onSuccess() {
                                //同步调用停止定位和推流
                                //停止推流
                                if (isServiceBound && rtpService != null) {
                                    rtpService.stopStream();
                                    Log.i(TAG,"后台停止推流");
                                }
                                //停止定位
                                stopLocation();
                            }

                            @Override
                            public void onFailure(String error) {
                                mHandler.post(new Runnable() {
                                    @Override
                                    public void run() {
                                        Toast.makeText(SosCallActivity.this, "停止sos失败"+error, Toast.LENGTH_SHORT).show();
                                    }
                                });
                            }
                        }
                );

            }
        });
    }

    private void stopLocation() {

        Log.d("LocationActivity", "停止定位");

        if (locUtils != null && locUtils.isLocating()) {
            locUtils.stopBaiduLoc();
        }
    }

    @Override
    protected void onPause() {
        super.onPause();
        // 页面不可见时自动停止定位
        stopLocation();
    }

    @Override
    protected void onDestroy() {
        // 解绑服务
        if (isServiceBound) {
            unbindService(serviceConnection);
            isServiceBound = false;
            Log.i(TAG, "unbind rtpservice");
        }
        mHandler.removeCallbacksAndMessages(null); // 清理所有消息

        // 彻底释放定位资源
        if (locUtils != null) {
            locUtils.stopBaiduLoc();
            // 如果有release方法的话调用
            // locUtils.release();
        }

        super.onDestroy();
    }
}