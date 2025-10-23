package com.mypoc.ptt.player.views;

import android.annotation.SuppressLint;
import android.content.Context;
import android.graphics.Matrix;
import android.graphics.SurfaceTexture;
import android.text.TextUtils;
import android.util.AttributeSet;
import android.util.Log;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.Surface;
import android.view.TextureView;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.GridLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.mypoc.ptt.R;
import com.mypoc.ptt.player.listener.OnTextureViewDoubleClickListener;
import com.mypoc.ptt.player.listener.VideoFocusEvent;

import org.greenrobot.eventbus.EventBus;

import java.io.IOException;

import tv.danmaku.ijk.media.player.IMediaPlayer;
import tv.danmaku.ijk.media.player.IjkMediaPlayer;

public class VideoTextureView extends FrameLayout implements
        TextureView.SurfaceTextureListener,
        IjkMediaPlayer.OnPreparedListener,
        IjkMediaPlayer.OnCompletionListener,
        IjkMediaPlayer.OnErrorListener,
        IjkMediaPlayer.OnInfoListener,
        IjkMediaPlayer.OnVideoSizeChangedListener {

    private String TAG = "VideoSurfaceView";
    private IjkMediaPlayer mediaPlayer;


    private TextView statusTextView; //显示运行状态的提示

    private PlayState playState = PlayState.NOT_PLAYING;
    private String rtspUrl="";
    String  channelId; //通道ID，是唯一的
    private String noplayPrompt="";  //未播放时的textview文字，如通道1，通道2等提示


    //记住它自身的parent的parent, 用于还原排版
    private FrameLayout containerView;
    //记住原来的排版, 用于还原
    GridLayout.LayoutParams originalParams;
    Surface surface;
    private TextureView textureView;
    private int textureViewHeight=0;
    private int textureViewWidth=0;

    private Context mContext;

    //以下用于显示播放器的控制view
    private GridLayout gridLayout;
    private boolean isGridLayoutVisible = false;
    private VideoTextureView mSelf;
    private long lastClickTime = 0;
    private OnTextureViewDoubleClickListener textureViewDoubleClickListener; //抛出事件，让调用窗口监听处理

    public VideoTextureView(@NonNull Context context) {
        super(context);
        initVideoView(context);
    }
    public VideoTextureView(@NonNull Context context, AttributeSet attrs) {
        super(context, attrs);
        initVideoView(context);
    }

    public VideoTextureView(@NonNull Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        initVideoView(context);
    }

    @SuppressLint("NewApi")
    public VideoTextureView(@NonNull Context context, @Nullable AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        super(context, attrs, defStyleAttr, defStyleRes);
        initVideoView(context);
    }

    private void initVideoView(Context context) {

        mSelf= this;
        mContext=context;
        // 设置 CustomFrameLayout 自身为可点击
        setClickable(true);
        //
        LayoutInflater inflater= (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        gridLayout= (GridLayout) inflater.inflate(R.layout.video_view_control,this,false);

    }

    /**
     * 完全释放资源，在Activity销毁时调用
     */
    public void releaseResources() {
        Log.d(TAG, "释放VideoTextureView资源");

        // 1. 停止并释放媒体播放器
        if (mediaPlayer != null) {
            try {
                mediaPlayer.stop();
                mediaPlayer.setDisplay(null);
                mediaPlayer.setSurface(null);
                mediaPlayer.release();
                mediaPlayer = null;
            } catch (Exception e) {
                Log.e(TAG, "释放mediaPlayer异常: " + e.getMessage());
            }
        }

        // 2. 释放Surface
        if (surface != null) {
            try {
                surface.release();
                surface = null;
            } catch (Exception e) {
                Log.e(TAG, "释放surface异常: " + e.getMessage());
            }
        }

        // 3. 清理TextureView
        if (textureView != null) {
            try {
                textureView.setSurfaceTextureListener(null);
                removeView(textureView);
                textureView = null;
            } catch (Exception e) {
                Log.e(TAG, "清理textureView异常: " + e.getMessage());
            }
        }

        // 4. 清理控制视图
        if (gridLayout != null) {
            try {
                removeView(gridLayout);
                gridLayout = null;
            } catch (Exception e) {
                Log.e(TAG, "清理gridLayout异常: " + e.getMessage());
            }
        }

        // 5. 重置状态
        playState = PlayState.NOT_PLAYING;
        rtspUrl = "";

        // 6. 移除所有子视图
        removeAllViews();

        Log.d(TAG, "VideoTextureView资源释放完成");
    }

    public OnTextureViewDoubleClickListener getTextureViewDoubleClickListener() {
        return textureViewDoubleClickListener;
    }

    public void setTextureViewDoubleClickListener(OnTextureViewDoubleClickListener textureViewDoubleClickListener) {
        this.textureViewDoubleClickListener = textureViewDoubleClickListener;
    }

    /**
     * 关闭不是选中的view中的子gridview
     */
    public void closeGridView(VideoTextureView videoTextureView){
        if (videoTextureView!=this){
            gridLayout.setVisibility(INVISIBLE);
            isGridLayoutVisible = false;
        }
    }


    public void setStatusTextView(TextView statusTextView, String noplayPrompt, FrameLayout containerView, GridLayout.LayoutParams originalParams) {
        this.statusTextView = statusTextView;
        this.noplayPrompt= noplayPrompt;
        this.containerView=containerView;
        this.originalParams=originalParams;
        updateStatusText();
    }

    public void setRtspUrl(String rtspUrl,String channelId) {
        if (TextUtils.equals("",this.rtspUrl)){
            //如果是第一次播放视频，那就创建一个新的TextureView
            this.rtspUrl=rtspUrl;
            this.channelId=channelId;
            createTextureView();
        } else {
            this.rtspUrl = rtspUrl;
            this.channelId=channelId;
            load();
        }

    }

    /**
     * grid下的video控制的按钮
     */
    private void  addGridSubviewListener(){
        //
        findViewById(R.id.iv_refresh).setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                Log.e(TAG,"R.id.iv_refresh");
                load();
            }
        });
        findViewById(R.id.iv_close).setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                Log.e(TAG,"R.id.iv_close");
                close();
            }
        });

    }

    //关闭流


    /**
     * 新建一个TextureView
     */
    private void createTextureView() {

        textureView = null;
        textureView = new TextureView(getContext());

        //
        Log.e(TAG,"textureView.setSurfaceTextureListener(this)");
        textureView.setSurfaceTextureListener(this);

        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.MATCH_PARENT);
        layoutParams.gravity = Gravity.CENTER;
        textureView.setLayoutParams(layoutParams);
        addView(textureView);
        //
        addView(gridLayout, new FrameLayout.LayoutParams(
                LayoutParams.WRAP_CONTENT,
                FrameLayout.LayoutParams.WRAP_CONTENT,
                Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL
        ));
        gridLayout.setVisibility(INVISIBLE);
        addGridSubviewListener();  //事件监听

        //
        textureView.setOnTouchListener(new OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                Log.i(TAG,"子控件中textureview的点击事件");
                if (event.getAction() == MotionEvent.ACTION_DOWN ) {
                    long currentTime = System.currentTimeMillis();
                    if (currentTime - lastClickTime < 300) { // 300毫秒内两次点击视为双击
                        // 处理双击事件
                        Log.i(TAG,"子控件中textureview的点击事件,双击");
                        // 如果是双击事件，不消费事件，返回 false, 让其父控件在mainactivity中处理
                        // 双击事件，通知父控件
                        if (textureViewDoubleClickListener != null) {
                            textureViewDoubleClickListener.onDoubleClick(mSelf);
                        }
                        return true;

                    } else {

                        lastClickTime = currentTime;
                        //以下是单击事件处理

                        if (playState==PlayState.NOT_PLAYING) {

                            return true;
                        }

                        if (isGridLayoutVisible) {

                            gridLayout.setVisibility(INVISIBLE);
                            isGridLayoutVisible = false;

                        } else {

                            gridLayout.setVisibility(VISIBLE);
                            isGridLayoutVisible = true;
                            EventBus.getDefault().post(new VideoFocusEvent(mSelf));
                        }

                        return true; //子控件已处理，防止其继续向上传递
                    }

                }

                // 其他事件正常处理
                return VideoTextureView.super.onTouchEvent(event);
            }
        });


    }


    public FrameLayout getContainerView() {
        return containerView;
    }

    public void setContainerView(FrameLayout containerView) {
        this.containerView = containerView;
    }

    public String getChannelId() {
        return channelId;
    }

    public void setChannelId(String channelId) {
        this.channelId = channelId;
    }

    public PlayState getPlayState() {
        return playState;
    }

    public void setPlayState(PlayState playState) {
        this.playState = playState;
    }

    ///////////////
    // 获取Matrix
    private Matrix getMatrix(float textureViewWidth, float textureViewHeight, float videoWidth, float videoHeight) {
        float sx = textureViewWidth / videoWidth;
        float sy = textureViewHeight / videoHeight;
        Matrix matrix = new Matrix();
        // 第1步: 把视频区移动到View区, 使两者中心点重合
        matrix.preTranslate((textureViewWidth - videoWidth) / 2, (textureViewHeight - videoHeight) / 2);
        // 第2步: 因为默认视频是fitXY的形式显示的, 所以首先要缩放还原回来
        matrix.preScale(videoWidth / textureViewWidth, videoHeight / textureViewHeight);
        // 第3步, 等比例放大或缩小, 直到视频区的一边和View一边相等. 如果另一边和view的一边不相等, 则留下空隙
        if (sx >= sy) {
            matrix.postScale(sy, sy, textureViewWidth / 2, textureViewHeight / 2);
        } else {
            matrix.postScale(sx, sx, textureViewWidth / 2, textureViewHeight / 2);
        }
        return matrix;
    }

    // 视频缩放，调整比例
    private void resolveStretching(float textureViewWidth, float textureViewHeight) {
        if (textureView == null || mediaPlayer == null) {
            return;
        }
        try {

            float videoWidth = mediaPlayer.getVideoWidth();
            float videoHeight = mediaPlayer.getVideoHeight();
            Log.d(TAG, "videoWidth="+videoWidth+",videoHeight="+videoHeight);

            if (textureViewWidth == 0 || textureViewHeight == 0 || videoWidth == 0 || videoHeight == 0) {
                return;
            }
            Matrix matrix = getMatrix(textureViewWidth, textureViewHeight, videoWidth, videoHeight);
            textureView.setTransform(matrix);
            textureView.postInvalidate();
        } catch (Exception e) {
            // 处理异常
            Log.d(TAG, "resolveStretching: error"+Log.getStackTraceString(e));
        }
    }

    @Override
    public void onSurfaceTextureAvailable(@NonNull SurfaceTexture surface, int width, int height) {
        Log.e(TAG,"获得surface,width="+width+",height="+height);
        this.surface= new Surface(surface);
        this.textureViewHeight= height;
        this.textureViewWidth=width;
        // surfaceTexture数据通道准备就绪，打开播放器
        load();

    }

    @Override
    public void onSurfaceTextureSizeChanged(@NonNull SurfaceTexture surface, int width, int height) {
        this.textureViewHeight= height;
        this.textureViewWidth=width;
        resolveStretching(width, height);
    }

    @Override
    public boolean onSurfaceTextureDestroyed(@NonNull SurfaceTexture surface) {
        //releasePlayer();
        surface.release();
        return false;
    }

    @Override
    public void onSurfaceTextureUpdated(@NonNull SurfaceTexture surface) {

    }

    /**
     * 关闭当前播放的流
     */
    private void close(){

        if (mediaPlayer != null) {
            mediaPlayer.stop();
            mediaPlayer.setDisplay(null);
            mediaPlayer.setSurface(null);
            mediaPlayer.release();
            mediaPlayer=null;
        }
        // 释放 Surface 和 SurfaceTexture
        if (surface != null) {
            surface.release();
            surface = null;
        }

        removeView(textureView);
        removeView(gridLayout);
        // 确保 TextureView 的表面被重置
        textureView.setSurfaceTextureListener(null);
        textureView=null;

        this.rtspUrl="";
        updatePlayState(PlayState.NOT_PLAYING);

    }

    private void load() {

        //每次都要重新创建IMediaPlayer
        if (mediaPlayer != null) {
            mediaPlayer.stop();
            mediaPlayer.setDisplay(null);
            mediaPlayer.setSurface(null);
            mediaPlayer.release();
        }

        try {
            mediaPlayer = new IjkMediaPlayer();

            mediaPlayer.native_setLogLevel(IjkMediaPlayer.IJK_LOG_VERBOSE);

            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "protocol_whitelist",
                    "crypto,file,http,https,tcp,tls,udp,rtp,rtsp,rtmp");

            //建议添加：启用 H264 硬件解码
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "mediacodec", 1);
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "mediacodec-auto-rotate", 1);

            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "mediacodec-hevc", 1); //h265的硬解码
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "analyzeduration", 1); //设置播放前的探测时间 1,达到首屏秒开效果

            //播放延时的解决方案
            // 如果是rtsp协议，可以优先用tcp(默认是用udp)
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "rtsp_transport", "tcp");

            // 设置播放前的最大探测时间 （100未测试是否是最佳值）
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "analyzemaxduration", 100L);

            // 每处理一个packet之后刷新io上下文
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "flush_packets", 1L);

            // 需要准备好后自动播放
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "start-on-prepared", 1);

            // 不额外优化（使能非规范兼容优化，默认值0 ）
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "fast", 1);

            // 是否开启预缓冲，一般直播项目会开启，达到秒开的效果，不过带来了播放丢帧卡顿的体验
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "packet-buffering", 0);

            // 自动旋屏
            //mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "mediacodec-auto-rotate", 0);
            // 处理分辨率变化
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "mediacodec-handle-resolution-change", 0);
            // 最大缓冲大小,单位kb
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "max-buffer-size", 0);
            // 默认最小帧数2
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "min-frames", 2);
            // 最大缓存时长
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "max_cached_duration", 3); //300
            // 是否限制输入缓存数
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "infbuf", 1);
            // 缩短播放的rtmp视频延迟在1s内
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "fflags", "nobuffer");
            // 播放前的探测Size，默认是1M, 改小一点会出画面更快
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "probesize", 200); //1024L)
            // 播放重连次数
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "reconnect", 5);
            // TODO:
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_FORMAT, "http-detect-range-support", 0);
            // 设置是否开启环路过滤: 0开启，画面质量高，解码开销大，48关闭，画面质量差点，解码开销小
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_CODEC, "skip_loop_filter", 48L);
            // 跳过帧 ？？
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_CODEC, "skip_frame", 0);
            // 视频帧处理不过来的时候丢弃一些帧达到同步的效果
            mediaPlayer.setOption(IjkMediaPlayer.OPT_CATEGORY_PLAYER, "framedrop", 5);

            mediaPlayer.setDataSource(rtspUrl);

            mediaPlayer.setOnPreparedListener(this);
            mediaPlayer.setOnCompletionListener(this);
            mediaPlayer.setOnErrorListener(this);
            mediaPlayer.setOnInfoListener(this);
            mediaPlayer.setOnVideoSizeChangedListener(this);

            //给mediaPlayer设置视图
            Log.e(TAG, "SURFACE=" + surface);

            mediaPlayer.setSurface(surface);
            mediaPlayer.prepareAsync();
            Log.e(TAG, "SURFACE2=" + surface);

        } catch (Exception e) {
            Log.e(TAG, "报错了,"+e.getMessage());
            e.printStackTrace();
        }


    }

    // Player release

    // Update play state and status text
    private void updatePlayState(PlayState newState) {
        if (playState != newState) {
            playState = newState;
            updateStatusText();
        }
    }

    // Update status text in TextView
    private void updateStatusText() {
        if (statusTextView != null) {
            switch (playState) {
                case NOT_PLAYING:
                    statusTextView.setVisibility(VISIBLE);
                    statusTextView.setText(noplayPrompt);
                    break;
                case PLAYING:
                    statusTextView.setVisibility(INVISIBLE);
                    statusTextView.setText("正在播放");
                    break;
                case PLAYBACK_FAILED:
                    statusTextView.setVisibility(VISIBLE);
                    statusTextView.setText("播放失败");
                    break;
            }
        }
    }

    // IjkMediaPlayer.OnPreparedListener method

    @Override
    public void onPrepared(IMediaPlayer mp) {
        mediaPlayer.start();
        Log.i(TAG,"开始播放:"+rtspUrl);
        updatePlayState(PlayState.PLAYING);
    }

    // IjkMediaPlayer.OnCompletionListener method
    @Override
    public void onCompletion(IMediaPlayer mp) {
        // Handle playback completion if needed
        Log.i(TAG,"结束播放或结束拉流:"+rtspUrl);
        close();
    }

    // IjkMediaPlayer.OnErrorListener method
    @Override
    public boolean onError(IMediaPlayer mp, int what, int extra) {
        Log.e(TAG, "失败!!!");
        updatePlayState(PlayState.PLAYBACK_FAILED);
        return true; // Consume the error
    }

    // IjkMediaPlayer.OnInfoListener method
    @Override
    public boolean onInfo(IMediaPlayer mp, int what, int extra) {
        // Handle info messages if needed
        Log.e("IjkPlayer", "Info event: what = " + what + ", extra = " + extra);
        return false; // Do not consume the info
    }

    // IjkMediaPlayer.OnVideoSizeChangedListener method
    @Override
    public void onVideoSizeChanged(IMediaPlayer mp, int width, int height, int sarNum, int sarDen) {
        // Handle video size changes if needed
        Log.e(TAG,"onVideoSizeChanged,width="+width+",height="+height);
        resolveStretching(this.textureViewWidth,this.textureViewHeight);
    }

    // Method to start playback with a new RTSP URL (reuse the SurfaceView)
    public void startPlaybackWithUrl(String newRtspUrl,String channelId) {
        //releasePlayer(); // Release the current player if any
        setRtspUrl(newRtspUrl,channelId); // Set the new RTSP URL

        //initPlayer(mSurface); // Initialize the player with the new URL
    }

}
