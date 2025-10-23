package com.mypoc.ptt.utils;

import android.app.ActivityManager;
import android.app.Application;


import com.mypoc.ptt.application.MyPOCApplication;

import java.util.List;

public class Utils {

    public static boolean isAppRunningForeground() {

        ActivityManager activityManager =
                (ActivityManager) MyPOCApplication.getInstance().getSystemService(Application.ACTIVITY_SERVICE);
        List<ActivityManager.RunningAppProcessInfo> runningAppProcesses = activityManager.getRunningAppProcesses();
        for (ActivityManager.RunningAppProcessInfo appProcessInfo : runningAppProcesses) {
            if (appProcessInfo.importance == ActivityManager.RunningAppProcessInfo.IMPORTANCE_FOREGROUND) {
                if (appProcessInfo.processName.equals(MyPOCApplication.getInstance().getApplicationInfo().processName))
                    return true;
            }
        }
        return false;
    }
}
