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

    public static final String lastLoginTimeKey = "lastLoginTime";   //因为即是app又是launcher应用，会造成loginactivity 重复登录


    public static final String singleTalkKey= "singleTalk";  //单呼使能开关，默认N
    public static final String switchGroupKey = "switchGroup";  //切换群组使能开关, 默认N
    public static final String createGroupKey = "createGroup"; //创建临时群组使用开关，默认N

    //userinfo 返回的平台端的设置
    public static final String privSingleCallKey= "privSinglecall";  //poc端单呼权限(现已改成创建临时群组权限)
    public static final String flagAutoLocationKey = "flagAutoLocation";  //是否登录自动开启定位上报
    public static final String privHideLocSwitchKey = "privHideLocSwitch"; //是否隐藏POC上报定位开关
    public static final String locationModeKey = "locationMode";  //定位模式;  0，一般；1，高精,2,用户设置
    public static final String locationIntervalKey = "locationInterval"; //循环定位时间间隔(单位：秒):30,60,180，0则由用户设置


    public static void setData_String(Context mContext, String key, String mstring){
        PreferencesUtil.setStringPreferences(mContext, tPreference, key, mstring);
    }
    public static void setData_Int(Context mContext,String key,int num){
        PreferencesUtil.setIntPreference(mContext, tPreference, key, num);
    }

    public static void setData_Long(Context mContext,String key,long num){
        PreferencesUtil.setLongPreference(mContext, tPreference, key, num);
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

    public static long getData_Long(Context mContext,String key){
        return PreferencesUtil.getLongPreference(mContext, tPreference, key, 0);
    }

    public static boolean getDefualtState(Context mContext,String key,boolean flag){
        return PreferencesUtil.getDefualtState(mContext, tPreference, key,flag);
    }


}
