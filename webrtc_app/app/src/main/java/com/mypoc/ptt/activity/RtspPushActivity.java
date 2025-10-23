package com.mypoc.ptt.activity;

import androidx.annotation.NonNull;

import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.view.MotionEvent;
import android.view.SurfaceHolder;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.event.AvChatEvent;
import com.mypoc.ptt.utils.CodecUtils;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.pedro.encoder.input.gl.SpriteGestureController;
import com.pedro.encoder.input.gl.render.filters.object.TextObjectFilterRender;
import com.pedro.encoder.input.video.CameraHelper;
import com.pedro.encoder.utils.CodecUtil;
import com.pedro.encoder.utils.gl.TranslateTo;
import com.pedro.rtplibrary.rtsp.RtspCamera1;
import com.pedro.rtplibrary.util.BitrateAdapter;
import com.pedro.rtplibrary.view.OpenGlView;
import com.pedro.rtsp.rtsp.Protocol;
import com.pedro.rtsp.rtsp.VideoCodec;
import com.pedro.rtsp.utils.ConnectCheckerRtsp;

import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.greenrobot.eventbus.ThreadMode;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

import butterknife.ButterKnife;

public class RtspPushActivity extends BaseActivity implements SurfaceHolder.Callback, View.OnTouchListener {

    private String TAG = "RtspActivity";

    private RtspCamera1 rtspCamera1;
    private OpenGlView openGlView;
    private Button startStopButton,switchCameraButton;
    private EditText etRtspUrl;
    private TextView tvStatus, tvBitrate, tvResolution;
    private Handler watermarkHandler = new Handler();
    private Runnable watermarkUpdateRunnable;

    private TextObjectFilterRender textObjectFilterRender;
    private TextObjectFilterRender textObjectFilterRender_line2;  //第二行显示

    private boolean streaming = false;
    private long streamStartTime = 0;
    private SpriteGestureController spriteGestureController = new SpriteGestureController();


    SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.getDefault());
    // 自适应码率相关
    private BitrateAdapter bitrateAdapter;
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
        setContentView(R.layout.activity_rtsp);
        ButterKnife.bind(this); //先绑定，再做 initListen事件
        EventBus.getDefault().register(this);
        initViews();
        initRtspCamera();
        initWatermarkSystem();
        //initAdaptiveBitrate();

    }

    private void initViews() {
        openGlView = findViewById(R.id.surfaceView);
        startStopButton = findViewById(R.id.b_start_stop);
        switchCameraButton= findViewById(R.id.btn_switch_camera);

        tvStatus = findViewById(R.id.tv_status);
        tvBitrate = findViewById(R.id.tv_bitrate);
        tvResolution = findViewById(R.id.tv_resolution);

        etRtspUrl = findViewById(R.id.et_server_url);

        openGlView.getHolder().addCallback(this);
        openGlView.setOnTouchListener(this);

        switchCameraButton.setOnClickListener(view -> {
            if (rtspCamera1!=null)
                rtspCamera1.switchCamera();
        });

        startStopButton.setOnClickListener(v -> {
            if (!streaming) {
                startStreaming();
            } else {
                stopStreaming();
            }
        });
    }

    @Override
    public void surfaceCreated(@NonNull SurfaceHolder surfaceHolder) {

    }

    @Override
    public void surfaceChanged(@NonNull SurfaceHolder surfaceHolder, int i, int i1, int i2) {
        rtspCamera1.startPreview();
    }

    @Override
    public void surfaceDestroyed(@NonNull SurfaceHolder surfaceHolder) {
        if (rtspCamera1.isRecording()) {
            rtspCamera1.stopRecord();

        }
        if (rtspCamera1.isStreaming()) {
            rtspCamera1.stopStream();
            //button.setText(getResources().getString(R.string.start_button));
        }
        rtspCamera1.stopPreview();
    }

    @Override
    public boolean onTouch(View view, MotionEvent motionEvent) {
        if (spriteGestureController.spriteTouched(view, motionEvent)) {
            spriteGestureController.moveSprite(view, motionEvent);
            spriteGestureController.scaleSprite(motionEvent);
            return true;
        }
        return false;
    }

    private void initRtspCamera() {
        rtspCamera1 = new RtspCamera1(openGlView, new ConnectCheckerRtsp() {

            @Override
            public void onConnectionStartedRtsp(@NonNull String s) {
                Log.i("RTSP", "onConnectionStartedRtsp:"+s);
            }

            @Override
            public void onNewBitrateRtsp(long bitrate) {
                if (bitrateAdapter != null) bitrateAdapter.adaptBitrate(bitrate);
                // 码率变化回调
                runOnUiThread(() -> {
                    tvBitrate.setText("码率: " + (bitrate / 1024) + " kbps");
                });
            }

            @Override
            public void onConnectionSuccessRtsp() {

                bitrateAdapter = new BitrateAdapter(new BitrateAdapter.Listener() {

                    @Override
                    public void onBitrateAdapted(int bitrate) {
                        rtspCamera1.setVideoBitrateOnFly(bitrate);
                    }
                });

                //todo 后续这个根据分辨率来设置一个最大值, 480p 用 1200 kb, 720p 用3000 kb , 1080p用 5000kb
                bitrateAdapter.setMaxBitrate( 3000*1024);

                runOnUiThread(() -> {
                    Toast.makeText(RtspPushActivity.this, "RTSP连接成功", Toast.LENGTH_SHORT).show();
                    tvStatus.setText("推流中");
                    tvStatus.setTextColor(Color.GREEN);
                    streamStartTime = System.currentTimeMillis();
                    // 确保 OpenGL 已初始化后再设置过滤器
                    if (rtspCamera1.getGlInterface() != null) {
                        rtspCamera1.getGlInterface().enableAA(false); // 禁用抗锯齿以获得更好性能
                        setupTextFilter();
                    }

                    startWatermarkUpdates();
                });

            }

            @Override
            public void onConnectionFailedRtsp(String reason) {
                runOnUiThread(() -> {
                    Toast.makeText(RtspPushActivity.this, "连接失败: " + reason, Toast.LENGTH_SHORT).show();
                    tvStatus.setText("连接失败");
                    tvStatus.setTextColor(Color.RED);
                    stopWatermarkUpdates();
                });
            }

            @Override
            public void onDisconnectRtsp() {
                runOnUiThread(() -> {
                    tvStatus.setText("已断开");
                    tvStatus.setTextColor(Color.RED);

                    if (rtspCamera1.getGlInterface() != null) {

                        clearTextFilter();
                    }

                    stopWatermarkUpdates();
                });
            }

            @Override
            public void onAuthErrorRtsp() {
                Log.e("RTSP", "认证错误");
            }

            @Override
            public void onAuthSuccessRtsp() {
                Log.i("RTSP", "认证成功");
            }
        });

        rtspCamera1.setProtocol(Protocol.UDP);  //默认用udp

    }

    private void initWatermarkSystem() {
        // 初始化文本过滤器
        textObjectFilterRender = new TextObjectFilterRender();
        textObjectFilterRender_line2 = new TextObjectFilterRender();
        //rtspCamera1.getGlInterface().setFilter(textObjectFilterRender);
        // 设置水印更新任务
        watermarkUpdateRunnable = new Runnable() {
            @Override
            public void run() {
                updateTimeWatermark();
                watermarkHandler.postDelayed(this, 1000); // 每秒更新一次
            }
        };
    }

    private void startWatermarkUpdates() {
        watermarkHandler.post(watermarkUpdateRunnable);
    }

    private void stopWatermarkUpdates() {
        watermarkHandler.removeCallbacks(watermarkUpdateRunnable);
    }

    private void updateTimeWatermark() {

        if (textObjectFilterRender == null || !streaming) return;

        // 获取当前时间和推流时长
        String currentTime = sdf.format(new Date());

        // 构建水印文本
        String watermarkText = String.format(Locale.getDefault(),
                "%s", currentTime);

        runOnUiThread(() -> {
            try {
                // 检查 GLInterface 是否可用
                if (rtspCamera1.getGlInterface() != null) {
                    // 更新时间文本过滤器
                    textObjectFilterRender.setText(watermarkText, 26, Color.RED);

                    //竖屏
                    //textObjectFilterRender.setDefaultScale(rtspCamera1.getStreamWidth(),rtspCamera1.getStreamHeight());

                    //横屏
                    textObjectFilterRender.setDefaultScale(rtspCamera1.getStreamHeight(), rtspCamera1.getStreamWidth());

                    textObjectFilterRender.setPosition(TranslateTo.TOP_LEFT);

                    //

                    textObjectFilterRender_line2.setText(MyPOCApplication.getInstance().getUserId()+"["+
                            MyPOCApplication.getInstance().getUserName()
                            +"]", 24, Color.BLUE);
                    //竖屏
                    //textObjectFilterRender_line2.setDefaultScale(rtspCamera1.getStreamWidth(), rtspCamera1.getStreamHeight());
                    //横屏
                    textObjectFilterRender_line2.setDefaultScale(rtspCamera1.getStreamHeight(),rtspCamera1.getStreamWidth());
                    textObjectFilterRender_line2.setPosition(TranslateTo.BOTTOM_RIGHT);

                    // 更新分辨率显示
                    if (rtspCamera1 != null) {
                        tvResolution.setText(String.format("分辨率: %dx%d",
                                rtspCamera1.getStreamWidth(), rtspCamera1.getStreamHeight()));
                    }
                }

            } catch (Exception e) {
                Log.e("Watermark", "更新水印失败", e);
            }
        });
    }

    //
    // 推流控制相关方法
    private void startStreaming() {
        try {
            // 配置视频参数
            //setupVideoConfig();

            //String rtspUrl = "rtsp://192.168.101.140:554/345/100668_live?callId=3663778888&sign=36673788888"; // 替换为你的RTSP服务器地址

            String rtspUrl= MyPOCApplication.getInstance().getRtspUrl("live");
            etRtspUrl.setText(rtspUrl);

            int rotation = CameraHelper.getCameraOrientation(RtspPushActivity.this);
            //可以推H265的， 但播放端有点卡顿，且pc端的vlc无法播放在线播放 h265的录像
            if (CodecUtils.isH265EncoderSupported()){
               rtspCamera1.setVideoCodec(VideoCodec.H265);
            //rtspCamera1.setForce(CodecUtil.Force.FIRST_COMPATIBLE_FOUND, CodecUtil.Force.FIRST_COMPATIBLE_FOUND);
            }

            //横屏: 720,1280, 竖屏时要写成 1280,720
            if (rtspCamera1.prepareVideo(720, 1280, 25, 2500 * 1024, 1, rotation)
             && rtspCamera1.prepareAudio(64 * 1024, 32000, true, true, true)) {
                rtspCamera1.startStream(rtspUrl);
                // 更新UI状态
                streaming = true;
                startStopButton.setText("停止推流");
                startStopButton.setBackgroundColor(Color.RED);
                tvStatus.setText("准备推流...");
                tvStatus.setTextColor(Color.YELLOW);

            } else {
                Toast.makeText(this, "准备流失败", Toast.LENGTH_SHORT).show();
            }

        } catch (Exception e) {
            Log.e("Streaming", "推流启动失败", e);
            Toast.makeText(this, "推流启动失败: " + e.getMessage(), Toast.LENGTH_SHORT).show();
        }
    }

    private void clearTextFilter(){
        if (rtspCamera1.getGlInterface() != null ) {
            rtspCamera1.getGlInterface().clearFilters();
        }
    }

    private void setupTextFilter() {

        //固定的第二行
        if (rtspCamera1.getGlInterface() != null && textObjectFilterRender_line2 != null)
        {
            // 设置默认缩放
            textObjectFilterRender_line2.setDefaultScale(rtspCamera1.getStreamWidth(),
                    rtspCamera1.getStreamHeight());

            // 设置边距
            textObjectFilterRender_line2.setPosition(TranslateTo.BOTTOM_RIGHT);

            // 初始文本
            textObjectFilterRender_line2.setText(MyPOCApplication.getInstance().getUserId()+"["+
                    MyPOCApplication.getInstance().getUserName()
                    +"]", 24, Color.BLUE);
            //textObjectFilterRender.setBackgroundColor(Color.argb(128, 0, 0, 0));
            //textObjectFilterRender.setBackgroundRound(15);
            // 设置文本过滤器
            rtspCamera1.getGlInterface().addFilter(textObjectFilterRender_line2);
        }

        if (rtspCamera1.getGlInterface() != null && textObjectFilterRender != null) {

            // 设置默认缩放
            textObjectFilterRender.setDefaultScale(rtspCamera1.getStreamWidth(),
                    rtspCamera1.getStreamHeight());

            // 设置位置 - 左上角
            //textObjectFilterRender.setPosition(TranslateTo.LEFT);

            // 设置边距
            textObjectFilterRender.setPosition(TranslateTo.TOP_LEFT);

            // 初始文本
            textObjectFilterRender.setText("准备推流...", 24, Color.WHITE);
            //textObjectFilterRender.setBackgroundColor(Color.argb(128, 0, 0, 0));
            //textObjectFilterRender.setBackgroundRound(15);
            // 设置文本过滤器
            rtspCamera1.getGlInterface().addFilter(textObjectFilterRender);

        }
    }

    private void adjustVideoQualityBasedOnBitrate(int newBitrate) {
        runOnUiThread(() -> {
            try {
                int newWidth, newHeight;
                int newBitrateKbps = newBitrate / 1024;

                if (newBitrateKbps < 800) {
                    // 低码率 - 480p
                    newWidth = 854;
                    newHeight = 480;
                    updateWatermarkStyle(20, Color.YELLOW); // 网络差时改变水印颜色
                } else if (newBitrateKbps < 1500) {
                    // 中码率 - 720p
                    newWidth = 1280;
                    newHeight = 720;
                    updateWatermarkStyle(24, Color.WHITE);
                } else {
                    // 高码率 - 1080p
                    newWidth = 1920;
                    newHeight = 1080;
                    updateWatermarkStyle(28, Color.WHITE);
                }

                // 更新分辨率
                //rtspCamera1.setv(newWidth, newHeight);

                // 更新文本过滤器缩放
                if (textObjectFilterRender != null) {
                    textObjectFilterRender.setDefaultScale(newWidth, newHeight);
                }

                Log.i("Adaptive", String.format("调整分辨率: %dx%d, 码率: %d kbps",
                        newWidth, newHeight, newBitrateKbps));

            } catch (Exception e) {
                Log.e("Adaptive", "调整视频质量失败", e);
            }
        });
    }

    private void updateWatermarkStyle(int textSize, int textColor) {
        if (textObjectFilterRender != null) {
            // 获取当前文本
            String currentText = "调整中...";

            // 临时更新文本样式
            textObjectFilterRender.setText(currentText, textSize, textColor);
        }
    }

    private void stopStreaming() {
        if (rtspCamera1 != null) {
            rtspCamera1.stopStream();
        }

        stopWatermarkUpdates();

        // 更新UI状态
        streaming = false;
        startStopButton.setText("开始推流");
        startStopButton.setBackgroundColor(Color.GREEN);
        tvStatus.setText("已停止");
        tvStatus.setTextColor(Color.GRAY);
        tvBitrate.setText("码率: 0 kbps");
        tvResolution.setText("分辨率: 0x0");

        streamStartTime = 0;
    }

    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onAvChatMessage(AvChatEvent event){
        //
        Log.i(TAG, "event="+event.toString());
        if (event.getVideoType()==3){
            //直播时，pc远端下达的指令
            switch (event.getVideoCommand()){
                case 4:
                    //挂断直播
                    stopStreaming();
                    break;
                case 6:
                    //切换前后摄像头
                    if (rtspCamera1!=null){
                        rtspCamera1.switchCamera();
                    }
                    break;
                default:
                    break;
            }

        }

    }

    @Override
    protected void onDestroy() {
        EventBus.getDefault().unregister(this); // 反注册 EventBus
        super.onDestroy();
    }
}