package com.mypoc.ptt.service;

import android.animation.ValueAnimator;
import android.app.ActivityManager;
import android.app.KeyguardManager;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.PixelFormat;
import android.graphics.Rect;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.os.PowerManager;
import android.text.TextUtils;
import android.util.Log;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.VelocityTracker;
import android.view.View;
import android.view.WindowManager;
import android.widget.Scroller;
import android.widget.TextView;

import androidx.annotation.Nullable;
import androidx.core.app.NotificationCompat;

import com.mypoc.ptt.R;
import com.mypoc.ptt.activity.MainActivity;

import java.util.List;

/**
 * 对讲悬浮窗
 */
public class FloatingTalkService  extends Service {

    private String TAG = "FloatingTalkService";

    private WindowManager windowManager;
    private View floatingView;
    private WindowManager.LayoutParams params;


    private int initialX;
    private int initialY;
    private float initialTouchX;
    private float initialTouchY;
    private long lastClickTime = 0;
    private static final int CLICK_DELAY = 200; // 点击间隔阈值(毫秒)

    private boolean isShowing = false;  //控制悬浮窗显示或关闭
    private boolean isEnabled = false;  //放后台显示的开关
    private boolean isAppInForeground = false;

    // UI组件
    private TextView groupNameView;
    private TextView micStatusView;

    public final static String BROADCAST_UPDATE_FLOATING_WINDOW = "UPDATE_FLOATING_WINDOW";
    public final static String BROADCAST_ACTION_TOGGLE_FLOATING_WINDOW = "ACTION_TOGGLE_FLOATING_WINDOW";
    Handler handler ;
    private static final int NOTIFICATION_ID = 123;
    private static final String CHANNEL_ID = "FloatingTalkServiceChannel";
    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        handler = new Handler();
        // 初始化WindowManager
        windowManager = (WindowManager) getSystemService(WINDOW_SERVICE);

        // 创建悬浮窗布局
        floatingView = LayoutInflater.from(this).inflate(R.layout.floating_talk_window, null);
        groupNameView = floatingView.findViewById(R.id.talk_group);
        micStatusView = floatingView.findViewById(R.id.talk_mic);

        // 设置点击事件 - 返回主页面
        floatingView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Log.i(TAG,"悬浮窗点击");
                 returnToApp();
            }
        });

        // 设置布局参数
        params = new WindowManager.LayoutParams(
                WindowManager.LayoutParams.WRAP_CONTENT,
                WindowManager.LayoutParams.WRAP_CONTENT,
                Build.VERSION.SDK_INT >= Build.VERSION_CODES.O ?
                        WindowManager.LayoutParams.TYPE_APPLICATION_OVERLAY :
                        WindowManager.LayoutParams.TYPE_PHONE,
                WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE,
                PixelFormat.TRANSLUCENT);
        // 初始位置 - 右上角
        params.gravity = Gravity.TOP | Gravity.END;
        params.x = 0;
        params.y = 50;

        // 注册广播接收器
        IntentFilter filter = new IntentFilter( );
        filter.addAction(BROADCAST_UPDATE_FLOATING_WINDOW);
        filter.addAction(BROADCAST_ACTION_TOGGLE_FLOATING_WINDOW);
        registerReceiver(updateReceiver, filter);

        //默认显示，后续从 pref配置中读取
        isEnabled=true;

        // 设置拖动逻辑
        /*
        floatingView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                        // 记录初始位置
                        initialX = params.x;
                        initialY = params.y;
                        initialTouchX = event.getRawX();
                        initialTouchY = event.getRawY();
                        lastClickTime = System.currentTimeMillis();
                        return true;

                    case MotionEvent.ACTION_MOVE:
                        // 计算偏移量并更新位置
                        params.x = initialX + (int) (event.getRawX() - initialTouchX);
                        params.y = initialY + (int) (event.getRawY() - initialTouchY);
                        windowManager.updateViewLayout(floatingView, params);
                        return true;

                    case MotionEvent.ACTION_UP:
                        // 判断是否为点击事件（移动距离小且时间短）
                        if (isClick(event)) {
                            floatingView.performClick(); // 触发点击事件
                        }
                        return true;
                }
                return false;
            }
        });

        */

        //下面这个性能更好
        floatingView.setOnTouchListener(new View.OnTouchListener() {
            private Rect screenRect;
            private int viewWidth, viewHeight;
            private VelocityTracker velocityTracker;
            private Scroller scroller;

            @Override
            public boolean onTouch(View v, MotionEvent event) {
                // 初始化屏幕尺寸和视图尺寸
                if (screenRect == null) {
                    screenRect = new Rect();
                    windowManager.getDefaultDisplay().getRectSize(screenRect);
                    viewWidth = floatingView.getWidth();
                    viewHeight = floatingView.getHeight();
                }

                if (velocityTracker == null) {
                    velocityTracker = VelocityTracker.obtain();
                }
                velocityTracker.addMovement(event);

                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                        initialX = params.x;
                        initialY = params.y;
                        initialTouchX = event.getRawX();
                        initialTouchY = event.getRawY();
                        lastClickTime = System.currentTimeMillis();

                        // 停止当前动画
                        if (scroller != null && !scroller.isFinished()) {
                            scroller.abortAnimation();
                        }

                        // 按下效果
                        floatingView.animate().alpha(0.8f).setDuration(100).start();
                        return true;

                    case MotionEvent.ACTION_MOVE:
                        int newX = initialX - (int) (event.getRawX() - initialTouchX);
                        int newY = initialY + (int) (event.getRawY() - initialTouchY);

                        // 边界检查
                        newX = Math.max(screenRect.left, Math.min(newX, screenRect.right - viewWidth));
                        newY = Math.max(screenRect.top, Math.min(newY, screenRect.bottom - viewHeight));

                        // 更新位置
                        if (params.x != newX || params.y != newY) {
                            params.x = newX;
                            params.y = newY;
                            windowManager.updateViewLayout(floatingView, params);
                        }
                        return true;

                    case MotionEvent.ACTION_UP:
                    case MotionEvent.ACTION_CANCEL:
                        // 恢复透明度
                        floatingView.animate().alpha(1.0f).setDuration(100).start();

                        // 计算速度
                        velocityTracker.computeCurrentVelocity(1000);
                        float velocityX = velocityTracker.getXVelocity();
                        float velocityY = velocityTracker.getYVelocity();

                        // 判断点击
                        if (Math.abs(velocityX) < 500 && Math.abs(velocityY) < 500 &&
                                System.currentTimeMillis() - lastClickTime < CLICK_DELAY &&
                                Math.abs(event.getRawX() - initialTouchX) < 10 &&
                                Math.abs(event.getRawY() - initialTouchY) < 10) {
                            floatingView.performClick();
                        } else {
                            // 惯性滑动
                            if (scroller == null) {
                                scroller = new Scroller(getApplicationContext());
                            }
                            scroller.fling(params.x, params.y,
                                    (int) velocityX, (int) velocityY,
                                    screenRect.left, screenRect.right - viewWidth,
                                    screenRect.top, screenRect.bottom - viewHeight);

                            ValueAnimator animator = ValueAnimator.ofFloat(0, 1);
                            animator.setDuration(scroller.getDuration());
                            animator.addUpdateListener(animation -> {
                                if (scroller.computeScrollOffset()) {
                                    params.x = scroller.getCurrX();
                                    params.y = scroller.getCurrY();
                                    windowManager.updateViewLayout(floatingView, params);
                                }
                            });
                            animator.start();
                        }

                        velocityTracker.recycle();
                        velocityTracker = null;
                        return true;
                }
                return false;
            }
        });

        // 启动应用状态检测
        checkAppStatePeriodically();

    }

    // 判断是否为点击事件（而非拖动）
    private boolean isClick(MotionEvent event) {
        long currentTime = System.currentTimeMillis();
        float dx = Math.abs(event.getRawX() - initialTouchX);
        float dy = Math.abs(event.getRawY() - initialTouchY);

        return (currentTime - lastClickTime < CLICK_DELAY) &&
                (dx < 10) && (dy < 10); // 10像素内的移动视为点击
    }

    private void checkAppStatePeriodically() {

        handler.postDelayed(new Runnable() {
            @Override
            public void run() {
                boolean newState = isAppInForeground();
                if (newState != isAppInForeground) {
                    isAppInForeground = newState;
                    updateWindowVisibility();
                }
                handler.postDelayed(this, 1000); // 每秒检查一次
            }
        }, 1000);
    }

    private boolean isAppInForeground() {
        ActivityManager am = (ActivityManager) getSystemService(ACTIVITY_SERVICE);
        List<ActivityManager.RunningAppProcessInfo> processes = am.getRunningAppProcesses();
        if (processes == null) return false;

        for (ActivityManager.RunningAppProcessInfo process : processes) {
            if (process.processName.equals(getPackageName())) {
                return process.importance == ActivityManager.RunningAppProcessInfo.IMPORTANCE_FOREGROUND;
            }
        }
        return false;
    }

    private void updateWindowVisibility() {
        if (isEnabled && !isAppInForeground) {
            showOverlay();
        } else {
            hideOverlay();
        }
    }

    private void showOverlay() {
        if (!isShowing) {
            windowManager.addView(floatingView, params);
            isShowing = true;
        }
    }

    private void hideOverlay() {
        if (isShowing) {
            windowManager.removeView(floatingView);
            isShowing = false;
        }
    }

    private void returnToApp() {
        Intent intent = new Intent(this, MainActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP |
                Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);


        // 如果设备锁屏，唤醒设备
        KeyguardManager km = (KeyguardManager) getSystemService(KEYGUARD_SERVICE);
        if (km.isKeyguardLocked()) {
            PowerManager pm = (PowerManager) getSystemService(POWER_SERVICE);
            PowerManager.WakeLock wakeLock = pm.newWakeLock(
                    PowerManager.FULL_WAKE_LOCK |
                            PowerManager.ACQUIRE_CAUSES_WAKEUP |
                            PowerManager.ON_AFTER_RELEASE,
                    "TalkOverlay:WakeLock");
            wakeLock.acquire(1000);
        }

        startActivity(intent);

    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        // 创建通知渠道 (Android 8.0+需要)
        createNotificationChannel();

        // 创建通知
        Notification notification = buildNotification();

        // 启动为前台服务
        startForeground(NOTIFICATION_ID, notification);

        return START_STICKY;
    }

    private void createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel(
                    CHANNEL_ID,
                    "对讲悬浮窗服务",
                    NotificationManager.IMPORTANCE_LOW);
            channel.setDescription("保持对讲悬浮窗运行");

            NotificationManager manager = getSystemService(NotificationManager.class);
            manager.createNotificationChannel(channel);
        }
    }

    private Notification buildNotification() {
        return new NotificationCompat.Builder(this, CHANNEL_ID)
                .setContentTitle("对讲服务运行中")
                .setContentText("悬浮窗服务正在后台运行")
                .setSmallIcon(com.mypoc.pttlibrary.R.drawable.icon_poc) // 替换为你的通知图标
                .setPriority(NotificationCompat.PRIORITY_LOW)
                .build();
    }

    //设置显示状态
    public void setVisibility(boolean enableFlag) {
        if (windowManager == null || floatingView == null) {
            return;
        }
        isEnabled= enableFlag;
    }

    private BroadcastReceiver updateReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            if (BROADCAST_UPDATE_FLOATING_WINDOW.equals(intent.getAction())){
                String group = intent.getStringExtra("group");
                String mic= intent.getStringExtra("mic");
                updateContent(group, mic);
            } else if (BROADCAST_ACTION_TOGGLE_FLOATING_WINDOW.equals(intent.getAction())){
                boolean show = intent.getBooleanExtra("show", false);
                setVisibility(show);
            }

        }
    };

    // 更新显示内容的方法
    public void updateContent(String groupName, String micDescribe) {
        if (floatingView != null) {
            if (!TextUtils.isEmpty(groupName))
                ((TextView) floatingView.findViewById(R.id.talk_group)).setText(groupName);
            if (!TextUtils.isEmpty(micDescribe))
                ((TextView) floatingView.findViewById(R.id.talk_mic)).setText(micDescribe);
        }
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (floatingView != null) {
            if (floatingView.isAttachedToWindow()){
                if (windowManager!=null)
                    windowManager.removeView(floatingView);
            }

            floatingView=null;

        }

        unregisterReceiver(updateReceiver);
        stopForeground(true); // 移除通知
    }
}
