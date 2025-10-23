package com.mypoc.ptt.activity.backgroud;

import androidx.annotation.NonNull;
import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.Observer;

import android.app.ActivityManager;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.view.SurfaceHolder;
import android.widget.Button;

import com.mypoc.ptt.R;
import com.pedro.rtplibrary.view.OpenGlView;

@RequiresApi(api = Build.VERSION_CODES.LOLLIPOP)
public class BackgroundActivity extends AppCompatActivity implements SurfaceHolder.Callback  {

    private RtpService service;
    private String rtspUrl = "rtsp://192.168.101.140:554/345/100668_moni?callId=3663778888&sign=36673788888"; // 替换为你的RTSP服务器地址

    private OpenGlView surfaceView;
    private Button bStartStop;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_background);

        surfaceView= findViewById(R.id.surfaceView);
        bStartStop= findViewById(R.id.b_start_stop);

        RtpService.getObserver().observe(this, new Observer<RtpService>() {
            @Override
            public void onChanged(RtpService rtpService) {
                service = rtpService;
                startPreview();
            }
        });

        bStartStop.setOnClickListener(v -> {
            if (service == null || !service.isStreaming()) {
                if (service != null && service.prepare()) {
                    service.startStream(rtspUrl);
                    bStartStop.setText(R.string.stop_button);
                }
            } else {
                service.stopStream();
                bStartStop.setText(R.string.start_button);
            }
        });

        surfaceView.getHolder().addCallback(this);
    }

    @Override
    public void surfaceChanged(@NonNull SurfaceHolder holder, int format, int width, int height) {
        startPreview();
    }

    @Override
    public void surfaceDestroyed(@NonNull SurfaceHolder holder) {
        if (service != null) {
            service.setView(this);
            if (service.isOnPreview()) {
                service.stopPreview();
            }
        }
    }

    @Override
    public void surfaceCreated(@NonNull SurfaceHolder holder) {
        // Do nothing
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (!isMyServiceRunning(RtpService.class)) {
            Intent intent = new Intent(getApplicationContext(), RtpService.class);
            startService(intent);
        }
        if (service != null && service.isStreaming()) {
            bStartStop.setText(R.string.stop_button);
        } else {
            bStartStop.setText(R.string.start_button);
        }
    }

    @Override
    protected void onPause() {
        super.onPause();
        if (!isChangingConfigurations()) { // stop if no rotation activity
            if (service != null && service.isOnPreview()) {
                service.stopPreview();
            }
            if (service == null || !service.isStreaming()) {
                service = null;
                stopService(new Intent(getApplicationContext(), RtpService.class));
            }
        }
    }

    @SuppressWarnings("deprecation")
    private boolean isMyServiceRunning(Class<?> serviceClass) {
        ActivityManager manager = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
        for (ActivityManager.RunningServiceInfo service : manager.getRunningServices(Integer.MAX_VALUE)) {
            if (serviceClass.getName().equals(service.service.getClassName())) {
                return true;
            }
        }
        return false;
    }


    private void startPreview() {

        if (surfaceView.getHolder().getSurface().isValid()) {
            if (service != null) {
                service.setView(surfaceView);
            }
        }
        // check if onPreview and if surface is valid
        if (service != null && !service.isOnPreview() && surfaceView.getHolder().getSurface().isValid()) {
            service.startPreview();
        }
    }


}