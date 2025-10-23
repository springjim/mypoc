package com.mypoc.ptt.service;

import android.annotation.SuppressLint;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;
import android.os.IBinder;
import android.os.PowerManager;
import android.util.Log;

import com.baidu.location.BDAbstractLocationListener;
import com.baidu.location.BDLocation;
import com.baidu.location.LocationClient;
import com.baidu.location.LocationClientOption;
import com.baidu.mapapi.SDKInitializer;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.pref.LoginPrefereces;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.UpdateUserLocationCallback;

public class BaiduLocationService extends Service {
    private static final String TAG = "BaiduLocationService";
    private LocationClient mLocationClient;
    private PowerManager.WakeLock mWakeLock;
    private IPTTSDK pttSDK;
    // 定位间隔时间(毫秒)
    private  int LOCATION_INTERVAL = 30 * 1000; // 30秒

    public   int BatteryC,BatteryV,BatteryT;
    public   String BatteryStatus;
    @SuppressLint("InvalidWakeLockTag")
    @Override
    public void onCreate() {
        super.onCreate();
        Log.d(TAG, "Location service created");

        // 获取唤醒锁，防止CPU休眠
        PowerManager pm = (PowerManager) getSystemService(POWER_SERVICE);
        mWakeLock = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, TAG);
        mWakeLock.acquire();

        //注册电量广播
        IntentFilter filter = new IntentFilter();
        filter.addAction(Intent.ACTION_BATTERY_CHANGED);
        registerReceiver(mBatInfoReceiver, filter);


        //获取定位间隔
       int locationInterval = LoginPrefereces.getData_Int(BaiduLocationService.this,LoginPrefereces.locationIntervalKey);
       if (locationInterval==0)
           locationInterval=60;

       this.LOCATION_INTERVAL= locationInterval*1000;
       //
       Log.d(TAG,"Location interval ="+this.LOCATION_INTERVAL);
       pttSDK = MyPOCApplication.getInstance().getPttSDK();
       initLocation();

    }

    /* 创建广播接收器 */
    private BroadcastReceiver mBatInfoReceiver = new BroadcastReceiver()   //anjb add
    {
        public void onReceive(Context context, Intent intent)
        {
            String action = intent.getAction();
            /*
             * 如果捕捉到的action是ACTION_BATTERY_CHANGED， 就运行onBatteryInfoReceiver()
             */
            if (Intent.ACTION_BATTERY_CHANGED.equals(action))
            {
                BatteryC = intent.getIntExtra("level", 100);    //目前电量
                BatteryV = intent.getIntExtra("voltage", 4350);  //电池电压
                BatteryT = intent.getIntExtra("temperature", 340);  //电池温度
                switch (intent.getIntExtra("status", BatteryManager.BATTERY_STATUS_UNKNOWN))
                {
                    case BatteryManager.BATTERY_STATUS_CHARGING:
                        BatteryStatus = "充电状态";
                        break;
                    case BatteryManager.BATTERY_STATUS_DISCHARGING:
                        BatteryStatus = "放电状态";
                        break;
                    case BatteryManager.BATTERY_STATUS_UNKNOWN:
                        BatteryStatus = "未知道状态";
                        break;
                }

            }
        }
    };

    private void initLocation() {

        try {

            // 再次确认隐私合规（某些版本需要）
            LocationClient.setAgreePrivacy(true);
            mLocationClient = new LocationClient(getApplicationContext());

            // 设置定位参数
            LocationClientOption option = new LocationClientOption();
            option.setLocationMode(LocationClientOption.LocationMode.Hight_Accuracy); // 高精度模式
            option.setCoorType("bd09ll"); // 返回的定位结果是百度经纬度
            option.setScanSpan(LOCATION_INTERVAL); // 设置定时定位
            option.setIsNeedAddress(false); // 不需要地址信息
            option.setOpenGps(true); // 打开GPS
            option.setLocationNotify(true); // 设置当GPS有效时按照1秒间隔输出GPS结果
            option.setIgnoreKillProcess(false); // 可选，默认false，定位SDK内部是一个SERVICE

            mLocationClient.setLocOption(option);

            // 注册定位监听
            mLocationClient.registerLocationListener(new BDAbstractLocationListener() {
                @Override
                public void onReceiveLocation(BDLocation location) {
                    if (location == null) {
                        return;
                    }

                    // 定位结果回调
                    if (location.getLocType() == BDLocation.TypeGpsLocation
                            || location.getLocType() == BDLocation.TypeNetWorkLocation) {
                        double latitude = location.getLatitude();
                        double longitude = location.getLongitude();

                        Log.d(TAG, "Location received: " + latitude + ", " + longitude);

                        // 上报位置信息
                        reportLocationToServer(latitude, longitude);
                    }
                }
            });

            // 启动定位
            mLocationClient.start();

        } catch (Exception e) {
            Log.e(TAG, "Initialize location client failed", e);
        }

    }

    private void reportLocationToServer(double latitude,double longitude){
        pttSDK.updateUserLocation(MyPOCApplication.getInstance().getCurrGroupId(),
                latitude, longitude, BatteryC, 30, new UpdateUserLocationCallback() {
                    @Override
                    public void onSuccess() {
                        Log.i(TAG,"上报定位成功");
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"上报定位失败,"+error);
                    }
                }
        );
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        Log.d(TAG, "Location service started");
        return START_STICKY; // 系统会尝试重启服务
    }

    @Override
    public void onDestroy() {
        Log.d(TAG, "Location service destroyed");
        if (mLocationClient != null && mLocationClient.isStarted()) {
            mLocationClient.stop();
        }
        if (mWakeLock != null && mWakeLock.isHeld()) {
            mWakeLock.release();
        }

        unregisterReceiver(mBatInfoReceiver);
        super.onDestroy();
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }
}