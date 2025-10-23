package com.mypoc.ptt.location;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.location.LocationManager;
import android.util.Log;

import com.baidu.location.BDAbstractLocationListener;
import com.baidu.location.BDLocation;
import com.baidu.location.LocationClient;
import com.baidu.location.LocationClientOption;

import java.util.Timer;
import java.util.TimerTask;

/**
 * 百度定位工具类，
 * 因为太多地方需要定位了，所以封装一个工具类，减少代码量
 *
 */
public class BaiduLocUtils {

    private Context context;
    private LocationClient mLocClient;
    private MyLocationListenner myLocationListenner;

    private Timer overtimeTimer;
    private int overtime = 0; //定位超时时间(s)
    private int scanSpan = 5; //定位简隔

    //过滤点次数，因为gps定位首次打开需要搜星，一般前几个点都是不太准确。
    private int filterPoint = 2;
    private int currNum = 0;
    private LocCallback locCallback;
    private TimeoutReceiver timeoutReceiver;

    /**
     * 是否一直定位，如果设置成false，定到定位时，定位就会关闭。
     */
    private boolean isContinue = false;

    private enum LocState { IDLE, LOCATING, STOPPED }
    private LocState currentState = LocState.IDLE;
    private final Object lock = new Object();

    public BaiduLocUtils(Context context, int spanInterval, LocCallback locCallback){
        this.context = context.getApplicationContext();
        this.locCallback = locCallback;
        this.scanSpan= spanInterval;
        LocationClient.setAgreePrivacy(true);  //每次tmd都要调用一下这个
    }

    /**
     * 默认定位模式是LocationMode.Hight_Accuracy
     */
    public void startBaiduLoc(){
        //startBaiduLoc(LocationMode.Hight_Accuracy);  //这个是高精度的，但耗电和时间
        startBaiduLoc(LocationClientOption.LocationMode.Battery_Saving); // 用这个定位会快点
    }



    public void startBaiduLoc(LocationClientOption.LocationMode locationMode){
        synchronized (lock) {
            if (currentState == LocState.LOCATING) {
                Log.w("BaiduLocUtils", "定位正在进行中，无需重复启动");
                return;
            }

            try {

                currentState = LocState.LOCATING;
                //定位请求，开启定位
                mLocClient = new LocationClient(context);
                myLocationListenner = new MyLocationListenner(locCallback);
                mLocClient.registerLocationListener(myLocationListenner);
                LocationClientOption option = new LocationClientOption();

                //option.setFirstLocType(LocationClientOption.FirstLocType.ACCURACY_IN_FIRST_LOC);
                option.setLocationMode(locationMode);
                option.setCoorType("bd09ll"); // 设置坐标类型
                //可选，设置发起定位请求的间隔，int类型，单位ms
                //如果设置为0，则代表单次定位，即仅定位一次，默认为0
                //如果设置非0，需设置1000ms以上才有效
                option.setScanSpan(scanSpan*1000);
                //可选，设置是否使用卫星定位，默认false
                //使用高精度和仅用设备两种定位模式的，参数必须设置为true
                option.setOpenGnss(true);
                //可选，定位SDK内部是一个service，并放到了独立进程。
                //设置是否在stop的时候杀死这个进程，默认（建议）不杀死，即setIgnoreKillProcess(true)
                option.setIgnoreKillProcess(true);

                option.setWifiCacheTimeOut(5*60*1000);
                //可选，V7.2版本新增能力
                //如果设置了该接口，首次启动定位时，会先判断当前Wi-Fi是否超出有效期，若超出有效期，会先重新扫描Wi-Fi，然后定位

                mLocClient.setLocOption(option);
                currNum = 0;
                mLocClient.start();
                if(overtime > 0 && !isContinue){
                    registerTimeoutReceiver();
                    //如果要开启定位超时，必须满足超时时间不为0，而且不是持续定位的（即只定位一次）
                    startTimer();
                }
            } catch (Exception e){
                currentState = LocState.IDLE;
                Log.e("BaiduLocUtils", "启动定位失败: " + e.getMessage());
                e.printStackTrace();
            }

        }

    }

    private void stopLocating() {
        synchronized (lock) {
            stopTimer();
            stopBaiduLoc();
            unregisterTimeoutReceiver();
            currentState = LocState.STOPPED;
        }
    }

    public void stopBaiduLoc(){
        synchronized (lock) {

            if (currentState!=LocState.LOCATING){
                return;
            }
            try {
                if (mLocClient != null) {
                    if (myLocationListenner != null) {
                        mLocClient.unRegisterLocationListener(myLocationListenner);
                        myLocationListenner = null;
                    }
                    mLocClient.stop();
                    mLocClient = null;
                }
                stopTimer();
                unregisterTimeoutReceiver();
                currentState=LocState.STOPPED;
                Log.d("BaiduLocUtils", "定位已停止");

            }catch (Exception e) {
                Log.e("BaiduLocUtils", "停止定位异常: " + e.getMessage());
            }


        }

    }

    public boolean isLocating() {
        return currentState==LocState.LOCATING;
    }


    public class MyLocationListenner extends BDAbstractLocationListener {

        LocCallback locCallback;
        public MyLocationListenner(LocCallback locCallback){
            this.locCallback = locCallback;
        }

        @Override
        public void onReceiveLocation(BDLocation location) {

            if (currentState != LocState.LOCATING) {
                return;
            }

            int errorCode = location.getLocType();//百度地图定位错误返回码 对照表在最后面
            Log.d("BaiduLocUtils","定位返回码:"+errorCode);

            // 成功的定位类型
            if (isSuccessLocation(errorCode)) {
                handleSuccessLocation(location);
            } else {
                handleFailedLocation(errorCode);
            }

        }

    }

    private boolean isSuccessLocation(int errorCode) {
        return errorCode == BDLocation.TypeGpsLocation ||
                errorCode == BDLocation.TypeNetWorkLocation ||
                errorCode == BDLocation.TypeOffLineLocation;
    }

    private void handleSuccessLocation(BDLocation location) {

        double latitude = location.getLatitude();
        double longitude = location.getLongitude();

        Log.d("BaiduLocUtils", "定位成功: " + longitude + "|" + latitude);

        if (locCallback != null) {
            locCallback.deal("baidu", latitude, longitude);
        }

        if (!isContinue) {
            //中用一次
            stopLocating();
        }

    }

    private void handleFailedLocation(int errorCode) {
        Log.e("BaiduLocUtils", "定位失败, 错误码: " + errorCode);
        if (locCallback != null) {
            locCallback.onLocationFailed(errorCode, getErrorMsg(errorCode));
        }
    }

    private String getErrorMsg(int errorCode) {
        switch (errorCode) {
            case 62: return "无法获取有效定位依据";
            case 63: return "网络异常";
            case 505: return "AK不存在或者非法";
            default: return "定位失败，错误码: " + errorCode;
        }
    }

    private class TimeoutReceiver extends BroadcastReceiver {

        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if(action == null){
                return;
            }
            if(action.equals("com.mypoc.loc.timeout")){
                if(locCallback != null){
                    locCallback.timeout();
                    unregisterTimeoutReceiver();
                }
            }
        }
    }

    private void registerTimeoutReceiver(){
        if(timeoutReceiver == null){
            timeoutReceiver = new TimeoutReceiver();
            IntentFilter filter =  new IntentFilter();
            filter.addAction("com.mypoc.loc.timeout");
            context.registerReceiver(timeoutReceiver,filter);
        }
    }

    private void unregisterTimeoutReceiver(){
        if(timeoutReceiver != null){
            context.unregisterReceiver(timeoutReceiver);
            timeoutReceiver = null;
        }
    }

    /**
     * 启动超时定时器
     */
    private void startTimer(){


        /*if(overtimeTimer == null){
            overtimeTimer = new Timer();
            TimerTask task = new TimerTask() {
                @Override
                public void run() {
                    if(context != null){
                        context.sendBroadcast(new Intent("com.mypoc.loc.timeout"));
                    }
                    stopTimer();
                    stopBaiduLoc();
                }
            };
            overtimeTimer.schedule(task,overtime*1000);
        }*/

        synchronized (lock) {
            if (overtimeTimer == null && overtime > 0) {
                overtimeTimer = new Timer();
                TimerTask task = new TimerTask() {
                    @Override
                    public void run() {
                        synchronized (lock) {
                            if (currentState == LocState.LOCATING) {
                                Log.w("BaiduLocUtils", "定位超时");
                                if (locCallback != null) {
                                    locCallback.timeout();
                                }
                                stopLocating();
                            }
                        }
                    }
                };
                overtimeTimer.schedule(task, overtime * 1000L);
            }
        }

    }

    /**
     * 停止
     */
    private void stopTimer(){
        if(overtimeTimer != null){
            overtimeTimer.cancel();
            overtimeTimer = null;
        }
    }

    public boolean isContinue() {
        return isContinue;
    }

    /**
     * 是否持续定位
     * @param aContinue
     */
    public void setContinue(boolean aContinue) {
        isContinue = aContinue;
    }

    public int getOvertime() {
        return overtime;
    }

    /**
     * 定位超时时间,单位:秒
     * @param overtime
     */
    public void setOvertime(int overtime) {
        this.overtime = overtime;
    }

    public int getScanSpan() {
        return scanSpan;
    }

    /**
     * 定位间隔
     * @param scanSpan
     */
    public void setScanSpan(int scanSpan) {
        this.scanSpan = scanSpan;
    }

    /**
     * 判断GPS是否开启
     */
    public boolean isOpenGps(Context context){
        LocationManager locationManager = (LocationManager) context.getSystemService(Context.LOCATION_SERVICE);
        // 通过GPS卫星定位，定位级别可以精确到街（通过24颗卫星定位，在室外和空旷的地方定位准确、速度快）
        boolean gps = locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER);
        return  gps;
    }

    // 添加资源释放方法
    public void release() {
        stopBaiduLoc();
        stopTimer();
        unregisterTimeoutReceiver();
        context = null;
        locCallback = null;
    }

    //61	GPS定位结果，GPS定位成功
//62	无法获取有效定位依据，定位失败，请检查运营商网络或者WiFi网络是否正常开启，尝试重新请求定位
//63	网络异常，没有成功向服务器发起请求，请确认当前测试手机网络是否通畅，尝试重新请求定位
//66	离线定位结果。通过requestOfflineLocaiton调用时对应的返回结果
//68	网络连接失败时，查找本地离线定位时对应的返回结果
//161	网络定位结果，网络定位成功
//162	请求串密文解析失败，一般是由于客户端SO文件加载失败造成，请严格参照开发指南或demo开发，放入对应SO文件
//167	服务端定位失败，请您检查是否禁用获取位置信息权限，尝试重新请求定位
//505	AK不存在或者非法，请按照说明文档重新申请AK
}
