package com.mypoc.ptt.receiver;

import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Build.VERSION_CODES;
import android.util.Log;

import com.mypoc.ptt.LoginActivity;
import com.mypoc.ptt.service.DelayLaunchService;

public class BootReceiver extends BroadcastReceiver {

    String TAG = "BootReceiver";

    @Override
    public void onReceive(Context context, Intent intent) {

        Log.i(TAG, "start");

        if (Intent.ACTION_BOOT_COMPLETED.equals(intent.getAction())) {
            Log.i(TAG, "start 启动 LoginActivity");
            // 启动前台服务（Android 8.0+ 必须用 startForegroundService）
            Intent serviceIntent = new Intent(context, DelayLaunchService.class);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                Log.i(TAG, "start 启动 DelayLaunchService >= Build.VERSION_CODES.O ");
                context.startForegroundService(serviceIntent);
            } else {
                Log.i(TAG, "start 启动 DelayLaunchService < Build.VERSION_CODES.O  ");
                context.startService(serviceIntent);
            }
        }
    }
}
