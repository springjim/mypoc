package com.mypoc.ptt.pref;

import android.content.Context;

import com.mypoc.ptt.utils.PreferencesUtil;

public class LoginPrefereces {
    /**本地缓存文件名*/
    private static final String tPreference = "login";

    public static final String accountKey = "account";
    public static final String pswKey = "psw";
    public static final String serverAddrKey ="serverAddr";  //服务器地址: xxx.xxx.xxx.xxx
    public static final String isLoggedInKey = "isLoggedIn"; //是否登录过

    public static void setData_String(Context mContext, String key, String mstring){
        PreferencesUtil.setStringPreferences(mContext, tPreference, key, mstring);
    }
    public static void setData_Int(Context mContext,String key,int num){
        PreferencesUtil.setIntPreference(mContext, tPreference, key, num);
    }
    public static void setState(Context mContext,String key,boolean flag){
        PreferencesUtil.setDefualtState(mContext, tPreference, key, flag);
    }

    //

    public static String getData_String(Context mContext,String key){
        return PreferencesUtil.getStringPreference(mContext, tPreference, key, null);
    }

    public static int getData_Int(Context mContext,String key){
        return PreferencesUtil.getIntPreference(mContext, tPreference, key, 0);
    }

    public static boolean getDefualtState(Context mContext,String key,boolean flag){
        return PreferencesUtil.getDefualtState(mContext, tPreference, key,flag);
    }


}
