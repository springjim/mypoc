package com.mypoc.ptt.activity.backgroud;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.Binder;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.util.Log;

import androidx.annotation.Nullable;
import androidx.annotation.RequiresApi;
import androidx.core.app.NotificationCompat;
import androidx.lifecycle.MutableLiveData;

import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.utils.CodecUtils;
import com.pedro.encoder.input.gl.render.filters.object.TextObjectFilterRender;
import com.pedro.encoder.input.video.CameraHelper;
import com.pedro.encoder.utils.CodecUtil;
import com.pedro.encoder.utils.gl.TranslateTo;
import com.pedro.rtplibrary.base.Camera2Base;
import com.pedro.rtplibrary.rtsp.RtspCamera2;
import com.pedro.rtplibrary.util.BitrateAdapter;
import com.pedro.rtplibrary.view.OpenGlView;
import com.pedro.rtsp.rtsp.VideoCodec;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

/**
 * 用于 后台推流rtsp流的服务
 */
@RequiresApi(api = Build.VERSION_CODES.LOLLIPOP)
public class RtpService extends Service {

    private static final String TAG = "RtpService";
    private static final String CHANNEL_ID = "rtpStreamChannel";
    private static final int NOTIFY_ID = 123456;
    private static NotificationManager notificationManager;
    private static final MutableLiveData<RtpService> observer = new MutableLiveData<>();

    // 服务绑定相关
    private final IBinder binder = new RtpServiceBinder();
    private String rtspUrl;
    private int videoWidth = 720 ;  // 默认分辨率，横屏显示，如果要竖屏，要将videoWidth，videoHeight 对调
    private int videoHeight = 1280;
    private int videoBitrate = 3000 * 1024; // 默认码率，也是最大动态适配码率

    // 自适应码率相关
    private BitrateAdapter bitrateAdapter;
    SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.getDefault());

    public static MutableLiveData<RtpService> getObserver() {
        return observer;
    }

    private Camera2Base camera2Base;

    //水印相关
    private TextObjectFilterRender textObjectFilterRender;
    private TextObjectFilterRender textObjectFilterRender_line2;  //第二行显示
    private Handler watermarkHandler = new Handler();
    private Runnable watermarkUpdateRunnable;

    public class RtpServiceBinder extends Binder {
        public RtpService getService() {
            return RtpService.this;
        }
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        Log.i(TAG, "RTP service bound");
        return binder;
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        Log.i(TAG, "RTP service started");
        // 从Intent中获取参数
        if (intent != null) {
            rtspUrl = intent.getStringExtra("rtsp_url");
            videoWidth = intent.getIntExtra("video_width", 720);
            videoHeight = intent.getIntExtra("video_height", 1280);
            videoBitrate = intent.getIntExtra("video_bitrate", 2500 * 1024);
        }
        return START_STICKY;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        Log.i(TAG, TAG + " create");
        notificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel(CHANNEL_ID, CHANNEL_ID, NotificationManager.IMPORTANCE_HIGH);
            notificationManager.createNotificationChannel(channel);
        }
        keepAliveTrick();

        camera2Base = new RtspCamera2(this, true, connectCheckerRtp);
        initWatermarkSystem();

        observer.postValue(this);
    }

    private void initWatermarkSystem() {
        // 初始化文本过滤器
        textObjectFilterRender = new TextObjectFilterRender();
        textObjectFilterRender_line2 = new TextObjectFilterRender();

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

        if (textObjectFilterRender == null || !isStreaming()) return;

        // 获取当前时间和推流时长
        String currentTime = sdf.format(new Date());

        // 构建水印文本
        String watermarkText = String.format(Locale.getDefault(),
                "%s", currentTime);

            try {
                // 检查 GLInterface 是否可用
                if (camera2Base.getGlInterface() != null) {
                    // 更新时间文本过滤器
                    textObjectFilterRender.setText(watermarkText, 26, Color.RED);

                    //竖屏
                    //textObjectFilterRender.setDefaultScale(rtspCamera1.getStreamWidth(),rtspCamera1.getStreamHeight());

                    //横屏
                    textObjectFilterRender.setDefaultScale(camera2Base.getStreamHeight(), camera2Base.getStreamWidth());

                    textObjectFilterRender.setPosition(TranslateTo.TOP_LEFT);

                    //

                    textObjectFilterRender_line2.setText(MyPOCApplication.getInstance().getUserId()+"["+
                            MyPOCApplication.getInstance().getUserName()
                            +"]", 24, Color.BLUE);
                    //竖屏
                    //textObjectFilterRender_line2.setDefaultScale(rtspCamera1.getStreamWidth(), rtspCamera1.getStreamHeight());
                    //横屏
                    textObjectFilterRender_line2.setDefaultScale(camera2Base.getStreamHeight(),camera2Base.getStreamWidth());
                    textObjectFilterRender_line2.setPosition(TranslateTo.BOTTOM_RIGHT);

                }

            } catch (Exception e) {
                Log.e("Watermark", "更新水印失败", e);
            }

    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        Log.i(TAG, "RTP service destroy");
        stopRecord();
        stopStream();
        stopPreview();
        observer.postValue(null);
    }

    private void keepAliveTrick() {
        if (Build.VERSION.SDK_INT > Build.VERSION_CODES.O) {
            Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                    .setOngoing(true)
                    .setContentTitle("RTSP Stream Service")
                    .setContentText("Streaming service is running").build();
            startForeground(1, notification);
        } else {
            startForeground(1, new Notification());
        }
    }

    private void showNotification(String text) {
        Notification notification = new NotificationCompat.Builder(getApplicationContext(), CHANNEL_ID)
                .setSmallIcon(R.mipmap.ic_launcher)
                .setContentTitle("RTSP Stream")
                .setContentText(text).build();
        notificationManager.notify(NOTIFY_ID, notification);
    }

    /**
     * 设置推流URL
     */
    public void setRtspUrl(String url) {
        this.rtspUrl = url;
        Log.i(TAG, "RTSP URL set to: " + url);
    }

    /**
     * 设置视频分辨率
     */
    public void setVideoResolution(int width, int height) {
        this.videoWidth = width;
        this.videoHeight = height;
        Log.i(TAG, "Video resolution set to: " + width + "x" + height);
    }

    /**
     * 设置视频码率
     */
    public void setVideoBitrate(int bitrate) {
        this.videoBitrate = bitrate;
        Log.i(TAG, "Video bitrate set to: " + bitrate);
    }

    public boolean prepare() {

        if (camera2Base != null) {

            //可以用H265, 但要检测硬件支不支持
            if (CodecUtils.isH265EncoderSupported())
              ((RtspCamera2)camera2Base).setVideoCodec(VideoCodec.H265);
            return (camera2Base.prepareAudio(64 * 1024, 32000, true, true, true)
              && camera2Base.prepareVideo(videoWidth, videoHeight, 25, videoBitrate, 2,
                    CameraHelper.getCameraOrientation(RtpService.this))
            );
        }
        return false;

    }

    /**
     * 开始推流
     */
    public boolean startStreaming() {
        if (camera2Base != null && rtspUrl != null && !rtspUrl.isEmpty()) {
            if (prepare()) {
                startStream(rtspUrl);
                return true;
            }
        } else {
            Log.e(TAG, "Cannot start streaming: camera2Base is null or RTSP URL is not set");
        }
        return false;
    }

    /**
     * 开始推流（带自定义URL）
     */
    public boolean startStreaming(String url, int width, int height, int bitrate) {
        this.rtspUrl = url;
        this.videoWidth = width;
        this.videoHeight = height;
        this.videoBitrate = bitrate;
        return startStreaming();
    }

    public void startPreview() {
        if (camera2Base != null) {
            camera2Base.startPreview();
        }
    }

    public void stopPreview() {
        if (camera2Base != null) {
            camera2Base.stopPreview();
        }
    }

    public void switchCamera() {
        if (camera2Base != null) {
            camera2Base.switchCamera();
        }
    }

    public boolean isStreaming() {
        return camera2Base != null && camera2Base.isStreaming();
    }

    public boolean isRecording() {
        return camera2Base != null && camera2Base.isRecording();
    }

    public boolean isOnPreview() {
        return camera2Base != null && camera2Base.isOnPreview();
    }

    public void startStream(String endpoint) {
        if (camera2Base != null) {
            camera2Base.startStream(endpoint);
        }
    }

    public void stopStream() {
        if (camera2Base != null) {
            camera2Base.stopStream();
        }
        stopWatermarkUpdates();

    }

    public void startRecord(String path) {
        if (camera2Base != null) {
            try {
                camera2Base.startRecord(path, result -> {
                    Log.i(TAG, "record state: " + result.name());
                });
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

    public void stopRecord() {
        if (camera2Base != null) {
            camera2Base.stopRecord();
        }
    }

    public void setView(OpenGlView openGlView) {
        if (camera2Base != null) {
            camera2Base.replaceView(openGlView);
        }
    }

    public void setView(Context context) {
        if (camera2Base != null) {
            camera2Base.replaceView(context);
        }
    }

    // Getters
    public String getRtspUrl() {
        return rtspUrl;
    }

    public int getVideoWidth() {
        return videoWidth;
    }

    public int getVideoHeight() {
        return videoHeight;
    }

    public int getVideoBitrate() {
        return videoBitrate;
    }

    private void setupTextFilter() {

        //固定的第二行
        if (camera2Base.getGlInterface() != null && textObjectFilterRender_line2 != null)
        {
            // 设置默认缩放
            textObjectFilterRender_line2.setDefaultScale(camera2Base.getStreamWidth(),
                    camera2Base.getStreamHeight());

            // 设置边距
            textObjectFilterRender_line2.setPosition(TranslateTo.BOTTOM_RIGHT);

            // 初始文本
            textObjectFilterRender_line2.setText(MyPOCApplication.getInstance().getUserId()+"["+
                    MyPOCApplication.getInstance().getUserName()
                    +"]", 24, Color.BLUE);
            //textObjectFilterRender.setBackgroundColor(Color.argb(128, 0, 0, 0));
            //textObjectFilterRender.setBackgroundRound(15);
            // 设置文本过滤器
            camera2Base.getGlInterface().addFilter(textObjectFilterRender_line2);
        }

        if (camera2Base.getGlInterface() != null && textObjectFilterRender != null) {

            // 设置默认缩放
            textObjectFilterRender.setDefaultScale(camera2Base.getStreamWidth(),
                    camera2Base.getStreamHeight());

            // 设置位置 - 左上角
            //textObjectFilterRender.setPosition(TranslateTo.LEFT);

            // 设置边距
            textObjectFilterRender.setPosition(TranslateTo.TOP_LEFT);

            // 初始文本
            textObjectFilterRender.setText("准备推流...", 24, Color.WHITE);
            //textObjectFilterRender.setBackgroundColor(Color.argb(128, 0, 0, 0));
            //textObjectFilterRender.setBackgroundRound(15);
            // 设置文本过滤器
            camera2Base.getGlInterface().addFilter(textObjectFilterRender);

        }
    }

    private final ConnectCheckerRtp  connectCheckerRtp = new ConnectCheckerRtp () {
        @Override
        public void onConnectionStartedRtp(String rtpUrl) {
            //showNotification("Stream connection started");

            Log.i(TAG, "Connection started: " + rtpUrl);
        }

        @Override
        public void onConnectionSuccessRtp() {
            //showNotification("Stream started successfully");

            Log.i(TAG, "Stream started successfully");
            bitrateAdapter = new BitrateAdapter(new BitrateAdapter.Listener() {

                @Override
                public void onBitrateAdapted(int bitrate) {
                    camera2Base.setVideoBitrateOnFly(bitrate);
                }
            });
            bitrateAdapter.setMaxBitrate( videoBitrate);
            //
            // 确保 OpenGL 已初始化后再设置过滤器
            if (camera2Base.getGlInterface() != null) {
                camera2Base.getGlInterface().enableAA(false); // 禁用抗锯齿以获得更好性能
                setupTextFilter();
            }

            startWatermarkUpdates();

        }


        @Override
        public void onNewBitrateRtp(long bitrate) {
            Log.d(TAG, "New bitrate: " + bitrate);
        }

        @Override
        public void onConnectionFailedRtp(String reason) {
            //showNotification("Stream connection failed: " + reason);

            Log.e(TAG, "Connection failed: " + reason);
            stopWatermarkUpdates();
        }

        @Override
        public void onDisconnectRtp() {
            //showNotification("Stream stopped");

            Log.i(TAG, "Stream disconnected");
            if (camera2Base.getGlInterface() != null) {

                camera2Base.getGlInterface().clearFilters();
            }

            stopWatermarkUpdates();
        }

        @Override
        public void onAuthErrorRtp() {
            //showNotification("Stream auth error");

            Log.e(TAG, "Auth error");
        }

        @Override
        public void onAuthSuccessRtp() {
            //showNotification("Stream auth success");

            Log.i(TAG, "Auth success");
        }
    };
}