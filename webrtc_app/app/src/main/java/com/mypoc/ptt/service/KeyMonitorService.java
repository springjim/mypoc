package com.mypoc.ptt.service;

import android.accessibilityservice.AccessibilityService;
import android.content.Intent;
import android.util.Log;
import android.view.KeyEvent;
import android.view.accessibility.AccessibilityEvent;

public class KeyMonitorService extends AccessibilityService {

    private static final String TAG = "KeyMonitorService";

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        return super.onStartCommand(intent, flags, startId);
    }

    @Override
    public void onServiceConnected() {
        Log.d(TAG, "无障碍服务已连接");
    }

    @Override
    public boolean onKeyEvent(KeyEvent event) {
        int keyCode = event.getKeyCode();
        int action = event.getAction();
        String actionStr = action == KeyEvent.ACTION_DOWN ? "按下" : "松开";

        Log.d(TAG, "按键事件: " + KeyEvent.keyCodeToString(keyCode) + " " + actionStr);

        // 示例：监听音量键
        if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
            if (action == KeyEvent.ACTION_DOWN) {
                Log.d(TAG, "音量+键被按下");
            } else {
                Log.d(TAG, "音量+键被释放");
            }
            return true; // 消费事件，阻止默认行为
        }

        return super.onKeyEvent(event);
    }

    @Override
    public void onAccessibilityEvent(AccessibilityEvent event) {
        // 处理无障碍事件（非必须）
    }

    @Override
    public void onInterrupt() {
        Log.d(TAG, "服务被中断");
    }

}
