package com.mypoc.ptt.activity;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.text.TextUtils;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.GridLayout;
import android.widget.TextView;

import com.mypoc.ptt.R;
import com.mypoc.ptt.player.views.PlayState;
import com.mypoc.ptt.player.views.VideoTextureView;
import com.mypoc.ptt.utils.PttHelper;

import java.util.ArrayList;
import java.util.List;

import butterknife.BindView;
import butterknife.ButterKnife;

/**
 * 采用 ijkplayer 播放分享来的rtsp流
 */
public class RtspPlayActivity extends AppCompatActivity {

    private String TAG = "RtspPlayActivity";

    @BindView(R.id.sv_video1)
    VideoTextureView svVideo1;
    @BindView(R.id.tv_video_1)
    TextView tvVideo1;
    @BindView(R.id.fl1)
    FrameLayout fl1;

    @BindView(R.id.btn_back)
    Button btnBack;

    @BindView(R.id.tv_title)
    TextView tvTitle;

    private Handler handler;
    //surfaceview与textview集中管理
    private List<VideoTextureView> videoViews;
    private List<FrameLayout>      videoFrames; //video的外部容器，定位用的

    public static final String KEY_RTSP_URL = "rtsp_url";
    public static final String KEY_USER_ID = "userId";
    private String rtsp_url;
    private int userId=0;
    private String userName="";
    private int         mCurrFrameCount=1;    //当前容纳的video的个数
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_rtsp_play);

        // 设置全屏模式
        Window window = getWindow();
        window.setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

        ButterKnife.bind(this);
        initVideoFramelayout();

        addListener();

        Intent intent = getIntent();
        rtsp_url = intent.getStringExtra(KEY_RTSP_URL);
        userId = intent.getIntExtra(KEY_USER_ID, -1);
        userName= PttHelper.findUserName(userId);

        tvTitle.setText("播放来自:"+userName);

    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent); // 更新Intent
        rtsp_url = intent.getStringExtra(KEY_RTSP_URL);
        userId = intent.getIntExtra(KEY_USER_ID, -1);
        userName= PttHelper.findUserName(userId);

        tvTitle.setText("播放来自:"+userName);
        if (!TextUtils.isEmpty(rtsp_url)){
            doPlayRtsp(rtsp_url,userId+"");
        }
    }

    @Override
    protected void onDestroy() {

        if (svVideo1 != null) {
            svVideo1.releaseResources();
        }

        super.onDestroy();
    }

    @Override
    protected void onStart() {
        super.onStart();
        doPlayRtsp(rtsp_url,userId+"");
    }

    private void addListener() {
        btnBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                finish();
            }
        });
    }


    private void initVideoFramelayout() {
        videoViews= new ArrayList<>();
        svVideo1.setStatusTextView(tvVideo1,"通道1", (FrameLayout) svVideo1.getParent().getParent(), (GridLayout.LayoutParams) ((FrameLayout) svVideo1.getParent().getParent()).getLayoutParams());
        videoViews.add(svVideo1);

        videoFrames= new ArrayList<>();
        videoFrames.add(fl1);
    }

    @Override
    public void onBackPressed() {
        this.finish();
    }

    /**
     * 实际执行的
     * @param rtspUrl
     * @param channelId
     */
    private void doPlayRtsp(String rtspUrl, String channelId){
        //遍历videoviews
        List<VideoTextureView> myviews= this.videoViews;
        //因为要在多线下运行, 要考虑当前的排版
        synchronized (myviews){

            boolean handled=false;
            for (int fIndex=0; fIndex<mCurrFrameCount; fIndex++ ) {

                if (myviews.get(fIndex).getPlayState()== PlayState.NOT_PLAYING){
                    myviews.get(fIndex).startPlaybackWithUrl(rtspUrl,channelId);
                    handled=true;
                    break;
                }

            }
            //
            if (!handled){
                //查找失败的
                for (int fIndex=0; fIndex<mCurrFrameCount; fIndex++ ) {

                    if (myviews.get(fIndex).getPlayState()== PlayState.PLAYBACK_FAILED){
                        myviews.get(fIndex).startPlaybackWithUrl(rtspUrl,channelId);
                        handled=true;
                        break;
                    }

                }

            }
            //
            if (!handled){
                //查找正在播放的，直接替换掉
                for (int fIndex=0; fIndex<mCurrFrameCount; fIndex++) {

                    if (myviews.get(fIndex).getPlayState()== PlayState.PLAYING){
                        myviews.get(fIndex).startPlaybackWithUrl(rtspUrl,channelId);
                        handled=true;
                        break;
                    }

                }

            }

        }
    }


}