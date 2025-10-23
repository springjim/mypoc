package com.mypoc.ptt.receiver;

import android.annotation.SuppressLint;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Build;
import android.os.Build.VERSION_CODES;
import android.telephony.TelephonyManager;
import android.util.Log;

import com.mypoc.ptt.LoginActivity;
import com.mypoc.ptt.service.DelayLaunchService;
import com.mypoc.ptt.utils.NetworkUtil;

public class BootReceiver extends BroadcastReceiver {

    String TAG = "BootReceiver";

    @Override
    public void onReceive(Context context, Intent intent) {

        Log.i(TAG, "start");
        String action = intent.getAction();
        Log.d(TAG, "Received action: " + action);

        if (action == null) return;

        switch (action) {

            case Intent.ACTION_BOOT_COMPLETED:
                /*if (NetworkUtil.is4GOrWifiAvailable(context)) {
                    startLoginActivity(context);
                }*/
                //startLoginActivity(context);
                break;

            case ConnectivityManager.CONNECTIVITY_ACTION:
                /*if (is4GReady(context)) {
                    startLoginActivity(context);
                }
                break;*/

        }



       /* if (Intent.ACTION_BOOT_COMPLETED.equals(intent.getAction())) {

            Log.i(TAG, "start 启动 LoginActivity");

            Intent intent_login = new Intent(context, LoginActivity.class);
            intent_login.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            context.startActivity(intent_login);


            // 启动前台服务（Android 8.0+ 必须用 startForegroundService）
            Intent serviceIntent = new Intent(context, DelayLaunchService.class);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                Log.i(TAG, "start 启动 DelayLaunchService >= Build.VERSION_CODES.O ");
                context.startForegroundService(serviceIntent);
            } else {
                Log.i(TAG, "start 启动 DelayLaunchService < Build.VERSION_CODES.O  ");
                context.startService(serviceIntent);
            }
        }*/

    }


    private void startLoginActivity(Context context) {
        Intent intent = new Intent(context, LoginActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        context.startActivity(intent);
    }


}
