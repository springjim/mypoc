package com.mypoc.ptt.utils;

import android.content.Context;
import android.content.SharedPreferences;

public class PreferencesUtil {

    public static int MODE = Context.MODE_MULTI_PROCESS+Context.MODE_PRIVATE;

    public static void setStringPreferences(Context context, String preference, String key, String value){
        SharedPreferences sharedPreferences = context.getSharedPreferences(preference, MODE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(key, value);
        editor.commit();
    }

    public static String getStringPreference(Context context, String preference, String key, String defaultValue){
        SharedPreferences sharedPreferences = context.getSharedPreferences(preference, MODE);
        return sharedPreferences.getString(key, defaultValue);
    }

    public static void setIntPreference(Context context, String preference, String key, int value) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(preference, MODE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putInt(key, value);
        editor.commit();
    }


    public static int getIntPreference(Context context, String preference, String key, int defaultValue) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(preference, MODE);
        return sharedPreferences.getInt(key, defaultValue);
    }

    public static void setLongPreference(Context context, String preference, String key, long value) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(preference, MODE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putLong(key, value);
        editor.commit();
    }

    public static long getLongPreference(Context context, String preference, String key, long defaultValue) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(preference, MODE);
        return sharedPreferences.getLong(key, defaultValue);
    }

    public static boolean getDefualtState(Context mContext, String tpreference,String key, boolean flag) {
        SharedPreferences sharedPreferences = mContext.getSharedPreferences(tpreference, MODE);
        return sharedPreferences.getBoolean(key, flag);
    }

    public static void setDefualtState(Context mContext, String tpreference,
                                       String key, boolean flag) {
        SharedPreferences sharedPreferences = mContext.getSharedPreferences(tpreference, MODE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putBoolean(key, flag);
        editor.commit();
    }

}
