package com.mypoc.ptt.utils;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.telephony.TelephonyManager;

public class NetworkUtil {

    // 检查网络是否可用（4G/WiFi）
    public static boolean isNetworkAvailable(Context context) {
        ConnectivityManager cm = (ConnectivityManager)
                context.getSystemService(Context.CONNECTIVITY_SERVICE);
        if (cm == null) return false;

        NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
        return activeNetwork != null && activeNetwork.isConnected();
    }

    // 检查是否是4G网络
    public static boolean is4G(Context context) {
        ConnectivityManager cm = (ConnectivityManager)
                context.getSystemService(Context.CONNECTIVITY_SERVICE);
        if (cm == null) return false;

        NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
        if (activeNetwork == null || !activeNetwork.isConnected()) {
            return false;
        }

        int type = activeNetwork.getType();
        int subtype = activeNetwork.getSubtype();

        // 移动网络且为LTE(4G)
        return type == ConnectivityManager.TYPE_MOBILE &&
                subtype == TelephonyManager.NETWORK_TYPE_LTE;
    }

    // 检查是否是WiFi网络
    public static boolean isWifi(Context context) {
        ConnectivityManager cm = (ConnectivityManager)
                context.getSystemService(Context.CONNECTIVITY_SERVICE);
        if (cm == null) return false;

        NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
        if (activeNetwork == null || !activeNetwork.isConnected()) {
            return false;
        }

        return activeNetwork.getType() == ConnectivityManager.TYPE_WIFI;
    }

    // 综合检查（4G或WiFi且可用）
    public static boolean is4GOrWifiAvailable(Context context) {
        return isNetworkAvailable(context) &&
                (is4G(context) || isWifi(context));
    }


}
