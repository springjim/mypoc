package com.mypoc.ptt.service;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Intent;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.os.Looper;
import android.util.Log;

import androidx.core.app.NotificationCompat;

import com.mypoc.ptt.LoginActivity;
import com.mypoc.ptt.R;

public class DelayLaunchService extends Service {
    private static final int NOTIFICATION_ID = 1;
    private static final String CHANNEL_ID = "delay_launch_channel";
    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        // 创建通知渠道（Android 8.0+ 要求）
        createNotificationChannel();

        // 创建前台服务通知（必须显示）
        Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .setContentTitle("正在准备启动应用")
                .setContentText("请稍候...")
                .setSmallIcon(com.mypoc.pttlibrary.R.drawable.icon_poc)
                .setPriority(NotificationCompat.PRIORITY_LOW)
                .build();

        // 启动为前台服务
        startForeground(NOTIFICATION_ID, notification);

        // 延迟执行（例如 5 秒后启动 Activity）
        new Handler(Looper.getMainLooper()).postDelayed(() -> {
            launchLoginActivity();
            stopSelf(); // 任务完成后停止服务
        }, 5000);

        return START_STICKY;
    }

    private void launchLoginActivity() {
        Intent loginIntent = new Intent(this, LoginActivity.class);
        loginIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);

        // Android 12+ 必须使用 PendingIntent
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            PendingIntent pendingIntent = PendingIntent.getActivity(
                    this, 0, loginIntent, PendingIntent.FLAG_IMMUTABLE
            );
            try {
                pendingIntent.send();
            } catch (PendingIntent.CanceledException e) {
                Log.e("DelayLaunchService", "Failed to launch activity", e);
            }
        } else {
            startActivity(loginIntent);
        }
    }

    private void createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel(
                    CHANNEL_ID,
                    "Delay Launch",
                    NotificationManager.IMPORTANCE_LOW
            );
            getSystemService(NotificationManager.class).createNotificationChannel(channel);
        }
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }
}
