package com.mypoc.ptt.widget;

import android.content.Context;
import android.media.MediaPlayer;
import android.os.Build;
import android.os.Handler;
import android.os.Looper;
import android.os.PowerManager;
import android.view.Window;
import android.view.WindowManager;

import androidx.appcompat.app.AlertDialog;

import com.mypoc.ptt.R;

/**
 * 含声音提示的弹窗
 */
public class SoundDialog {

    private static final String TAG = "SoundDialog";

    private MediaPlayer mediaPlayer;
    private AlertDialog dialog;
    private final Context context;
    private final String title;
    private final String message;
    private final DialogListener listener;
    private PowerManager.WakeLock wakeLock;

    public interface DialogListener {
        void onConfirm();
        void onCancel();
    }

    public SoundDialog(Context context, String title, String message, DialogListener listener) {
        this.context = context;
        this.title = title;
        this.message = message;
        this.listener = listener;
    }

    public void show() {

        // 1. 获取唤醒锁，保持屏幕亮起
        acquireWakeLock();

        // 2. 播放提示音
        playAlertSound();

        // 在主线程中创建对话框
        new Handler(Looper.getMainLooper()).post(() -> {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.setTitle(title)
                    .setMessage(message)
                    .setPositiveButton("确认", (dialog, which) -> {
                        releaseMediaPlayer();
                        if (listener != null) {
                            listener.onConfirm();
                        }
                    })
                    .setNegativeButton("取消", (dialog, which) -> {
                        releaseMediaPlayer();
                        if (listener != null) {
                            listener.onCancel();
                        }
                    })
                    .setCancelable(false); // 阻止点击外部取消

            dialog = builder.create();

            // 4. 设置窗口属性，使其能在锁屏和后台显示
            setupWindowAttributes();

            dialog.show();
        });
    }

    private void acquireWakeLock() {
        try {
            PowerManager powerManager = (PowerManager) context.getSystemService(Context.POWER_SERVICE);
            wakeLock = powerManager.newWakeLock(
                    PowerManager.FULL_WAKE_LOCK |
                            PowerManager.ACQUIRE_CAUSES_WAKEUP |
                            PowerManager.ON_AFTER_RELEASE,
                    "MyApp:SoundDialogWakeLock");
            wakeLock.acquire(10 * 60 * 1000L /*10 minutes*/);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void playAlertSound() {
        try {
            // 创建MediaPlayer并设置循环播放
            mediaPlayer = MediaPlayer.create(context, R.raw.income_ring); // 替换为你的音效资源
            mediaPlayer.setLooping(true);
            mediaPlayer.start();

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void setupWindowAttributes() {
        if (dialog == null || dialog.getWindow() == null) return;

        Window window = dialog.getWindow();

        // 设置窗口类型（不同Android版本处理不同）
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            window.setType(WindowManager.LayoutParams.TYPE_APPLICATION_OVERLAY);
        } else {
            window.setType(WindowManager.LayoutParams.TYPE_SYSTEM_ALERT);
        }

        // 添加必要的窗口标志
        window.addFlags(
                WindowManager.LayoutParams.FLAG_SHOW_WHEN_LOCKED |
                        WindowManager.LayoutParams.FLAG_TURN_SCREEN_ON |
                        WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON |
                        WindowManager.LayoutParams.FLAG_DISMISS_KEYGUARD);
    }

    private void releaseMediaPlayer() {
        if (mediaPlayer != null) {
            try {
                mediaPlayer.stop();
                mediaPlayer.release();
            } catch (Exception e) {
                e.printStackTrace();
            }
            mediaPlayer = null;
        }

        // 释放WakeLock
        if (wakeLock != null && wakeLock.isHeld()) {
            try {
                wakeLock.release();
            } catch (Exception e) {
                e.printStackTrace();
            }
            wakeLock = null;
        }

    }

    public void dismiss() {
        if (dialog != null && dialog.isShowing()) {
            dialog.dismiss();
            releaseMediaPlayer();
        }
    }

    public boolean isShowing() {
        if (dialog!=null)
            return dialog.isShowing();
        return false;

    }

}
