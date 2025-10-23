package com.mypoc.pttlibrary.internal;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Binder;
import android.os.Build;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.IBinder;
import android.os.Looper;
import android.util.Log;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.core.app.NotificationCompat;


import com.mypoc.pttlibrary.R;
import com.mypoc.pttlibrary.api.IPTTEventListener;
import com.mypoc.pttlibrary.api.PTTState;
import com.mypoc.pttlibrary.callback.CommonCallback;
import com.mypoc.pttlibrary.callback.LogoutCallback;
import com.mypoc.pttlibrary.callback.ReleaseMicCallback;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;
import com.mypoc.pttlibrary.callback.RequestMicCallback;
import com.mypoc.pttlibrary.enums.TalkStatusEnum;
import com.mypoc.pttlibrary.event.TalkStatusMessageEvent;
import com.mypoc.pttlibrary.internal.audio.AudioRecorder;
import com.mypoc.pttlibrary.internal.network.PTTTcpClient;
import com.mypoc.pttlibrary.internal.tcp.message.ApplyMicMessage;
import com.mypoc.pttlibrary.internal.tcp.message.LogoutMessage;
import com.mypoc.pttlibrary.internal.tcp.message.ReleaseMicMessage;
import com.mypoc.pttlibrary.internal.tcp.message.ReportMessage;
import com.mypoc.pttlibrary.internal.tcp.message.SOSLocationMessage;
import com.mypoc.pttlibrary.internal.tcp.message.SOSSessionMessage;
import com.mypoc.pttlibrary.internal.tcp.message.SingleCallSignalMessage;

import org.greenrobot.eventbus.EventBus;

import java.lang.reflect.Method;
import java.util.Timer;

public class PTTService extends Service {

    private static final String TAG = "PTTService";
    private static final int NOTIFICATION_ID = 1002;
    private static final String CHANNEL_ID = "PTTServiceChannel";

    private final IBinder binder = new LocalBinder();
    private PTTTcpClient tcpClient;

    private Handler heartbeatHandler;
    private Runnable heartbeatRunnable;

    private String serverHost;
    private int serverPort;
    private String userId;
    private String currentGroupId;
    private long   heartbeatIntervalMs;  //心跳间隔，单位：毫秒

    private boolean isHeartbeatStarted = false;

    private IPTTEventListener eventListener;
    private TalkBroadcastReceiver myTalkBroadcastReceiver;
    private AudioRecorder recorder;
    public static Timer micTimer = null;

    /** 0：弹起状态(就是ptt键没有按下) 1：申请状态 2：讲话状态  3: release(这也是弹起状态，即ptt键松开了)
     * */
    public  volatile int nMicStat = 0;

    private  SoundPool sndstarttalk;
    private  int       sndmedia_talk_prepareID;
    private  int       sndmedia_me_offID;
    private  int       sndmedia_onlineID;
    private  int       sndmedia_offlineID;

    /**
     * 是否正在讲话中...
     */
    public boolean isSpeaking = false;
    /**
     * 是否是sos告警模式
     */
    public static boolean isSosAlarmModel = false;

    /**
     * mic超时任务
     */
    private Runnable micOwnerTimeoutTask;
    private long   micownerTimeoutMs;    //麦权拥有的超时时间，单位：毫秒

    //ptt键
    private int pttKeyVal;
    private String pttDownBroadCastVal;
    private String pttUpBroadCastVal;
    private boolean pttUseBroadCastMode;

    //mic owner apply 申请麦权的超时时间
    private int applyMicOwnerTimeoutMs = 5000;
    private Runnable applyMicOwnerTimeoutTask;  //申请麦权超时任务

    private PTTKeyBoradcastReceiver pttKeyBoradcastReceiver;

    private Handler handler;
    AudioManager audioManager;

    //ptt键有否有效
    private boolean pttValid= true;



    public class LocalBinder extends Binder {
        PTTService getService() {
            return PTTService.this;
        }
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return binder;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        startForeground(); //置于前台
        registerServiceReceiver();
        audioManager = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
        recorder = new AudioRecorder(getApplicationContext(),this, audioManager);   //创建录音
        initSoundPool();
        handler= new Handler(Looper.getMainLooper());

        applyMicOwnerTimeoutTask = new Runnable() {
            @Override
            public void run() {

                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ApplyTimeout));
                Log.e(TAG, "申请麦权的超时时间");
            }
        };

    }



    public  void initSoundPool()
    {
        Log.d(TAG, "PTTService InitSoundPool");
        sndstarttalk   = new SoundPool( 2, AudioManager.STREAM_DTMF, 0 ); //MyApplication.streamType AudioManager.STREAM_DTMF
        sndmedia_talk_prepareID = sndstarttalk.load(getApplicationContext(), R.raw.sound_media_talk_prepare, 1 );
        sndmedia_me_offID       = sndstarttalk.load( getApplicationContext(), R.raw.sound_media_me_off, 1 );
        sndmedia_onlineID = sndstarttalk.load( getApplicationContext(), R.raw.hint_online, 1 );
        sndmedia_offlineID       = sndstarttalk.load(getApplicationContext(), R.raw.hint_offline, 1 );

    }
    public  void destroySoundPool()
    {
        Log.d(TAG, "TalkFragment DestroySoundPool");
        sndstarttalk.unload( sndmedia_talk_prepareID );
        sndstarttalk.unload( sndmedia_me_offID );
        sndstarttalk.unload( sndmedia_onlineID );
        sndstarttalk.unload( sndmedia_offlineID );
        sndstarttalk.release();
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        if (intent != null) {
            serverHost = intent.getStringExtra("server_host");
            serverPort = intent.getIntExtra("server_port", 0);
            userId = intent.getStringExtra("user_id");
            heartbeatIntervalMs= intent.getLongExtra("heartbeat_interval_ms",15000);
            micownerTimeoutMs = intent.getLongExtra("micowner_timeout_ms",60000);

            //
            pttKeyVal= intent.getIntExtra("pttkey_val",131);
            pttDownBroadCastVal=intent.getStringExtra("pttdown_broadcast_val");
            pttUpBroadCastVal=intent.getStringExtra("pttup_broadcast_val");
            pttUseBroadCastMode = intent.getBooleanExtra("pttuse_broadcast_mode",true);

            registerPTTKeyReceiver();

            PTTSessionManager.getInstance().setUserId(Integer.valueOf(userId));

            initializeTcpClient();

            // 确保心跳只初始化一次
            if (!isHeartbeatStarted) {
                initHeartbeat();
                isHeartbeatStarted = true;
            }

            micOwnerTimeoutTask = new Runnable() {
                @Override
                public void run() {
                    Log.e(TAG, "麦权超时了");

                    //进行一些状态同步
                    Intent timeoutIntent= new Intent();
                    timeoutIntent.setAction(MyBroadCast.BROADCAST_MIC_TIMEOUT);
                    sendBroadcast(timeoutIntent);

                    //直接在这里回调给sdk，不用tcpevent来周转了
                    if (eventListener!=null){
                        eventListener.onMicTimeout("讲话麦权超时了");
                    }

                }
            };

        }
        return START_STICKY;
    }

    public IPTTEventListener getEventListener() {
        return eventListener;
    }

    //动态注册ptt等键的广播
    private void registerPTTKeyReceiver(){

        Log.i(TAG,"registerPTTKeyReceiver");
        pttKeyBoradcastReceiver= new PTTKeyBoradcastReceiver();
        IntentFilter mItentFilter= new IntentFilter();
        mItentFilter.addAction(pttDownBroadCastVal);
        mItentFilter.addAction(pttUpBroadCastVal);
        registerReceiver(pttKeyBoradcastReceiver,mItentFilter);  //动态注册广播

    }

    private void unregisterPTTKeyReceiver(){

        Log.i(TAG,"unregisterPTTKeyReceiver");
        try{
            if (pttKeyBoradcastReceiver!=null){
                unregisterReceiver(pttKeyBoradcastReceiver);
            }
        } catch (Exception e){
            Log.e(TAG,"报错1");
            Log.e(TAG,Log.getStackTraceString(e));
        }

    }

    private class PTTKeyBoradcastReceiver extends BroadcastReceiver{
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "PTTKeyBoradcastReceiver action = " + action  );

            if (pttUseBroadCastMode && !pttDownBroadCastVal.isEmpty() && action.equalsIgnoreCase(pttDownBroadCastVal)){

                //如果这时,pttvalid无效，不允许抢麦
                if (!pttValid) {
                    handler.post( () -> {
                        Toast.makeText(PTTService.this,"国标对讲期间不允许抢麦",Toast.LENGTH_LONG).show();
                    });
                    return;
                }

                //按下ptt键
                requestMicrophone(new RequestMicCallback() {
                    @Override
                    public void onSuccess() {
                        //发送成功
                        //超时执行的任务
                        setMicStat(1);  //设置mic为申请状态
                        handler.postDelayed(applyMicOwnerTimeoutTask, applyMicOwnerTimeoutMs);
                        //发送等待事件，给主应用UI来同步
                        EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Applying));
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"发送抢麦报错:"+error);
                    }
                });

            } else if (pttUseBroadCastMode && !pttUpBroadCastVal.isEmpty() && action.equalsIgnoreCase(pttUpBroadCastVal)){
                //抬起ptt键

                releaseMicrophone(new ReleaseMicCallback() {
                    @Override
                    public void onSuccess() {
                        //发送释放成功后麦空闲，给主应用UI来同步
                        setMicStat(0);  //设置mic为松开状态
                        EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"释放麦报错:"+error);
                    }
                });


            }

        }
    }

    private void initHeartbeat() {
        // 心跳初始化逻辑
        //Handler handler = new Handler(Looper.getMainLooper());  //这是UI线程上，不能发送网络操作
        HandlerThread handlerThread = new HandlerThread("HeartbeatThread");
        handlerThread.start();
        Handler handler = new Handler(handlerThread.getLooper());
        Runnable heartbeatRunnable = new Runnable() {
            @Override
            public void run() {
                if (tcpClient != null && tcpClient.isConnected()) {
                    Log.i(TAG,"sendHeartbeat");
                    tcpClient.sendHeartbeat();
                }
                handler.postDelayed(this, heartbeatIntervalMs);
            }
        };
        handler.postDelayed(heartbeatRunnable, heartbeatIntervalMs);
    }

    private void registerServiceReceiver() {
        Log.d(TAG, "registerServiceReceiver");
        myTalkBroadcastReceiver = new TalkBroadcastReceiver();
        IntentFilter mIntentFilter = new IntentFilter();
        mIntentFilter.addAction(MyBroadCast.BROADCAST_MIC_FAILED);
        mIntentFilter.addAction(MyBroadCast.BROADCAST_MIC_SUCCESS);
        mIntentFilter.addAction(MyBroadCast.BROADCAST_MIC_TIMEOUT);
        mIntentFilter.addAction(MyBroadCast.BROADCAST_MIC_BREAKED);
        registerReceiver(myTalkBroadcastReceiver, mIntentFilter);

        //ptt,sos按键监听, todo...

    }

    private void unregisterServiceReceiver() {
        Log.d(TAG, "unregisterServiceReceiver");
        try {
            if (myTalkBroadcastReceiver != null) {
                unregisterReceiver(myTalkBroadcastReceiver);
                if (recorder != null) {
                    recorder.stopRecord();
                    recorder.release();
                    recorder = null;
                }

                if (handler != null) {
                     handler.removeCallbacks(micOwnerTimeoutTask);
                }
            }
        } catch (Exception e) {
            Log.e(TAG,"报错2");
            Log.e(TAG, Log.getStackTraceString(e));
        }
    }

    public boolean isPttValid() {
        return pttValid;
    }

    public void setPttValid(boolean pttValid) {
        this.pttValid = pttValid;
    }


    /**
     * 给双向通话用的内部方法，进入双向通话时打开采集语音， 对讲模式还是要抢麦方式
     * 麦克采集与喇叭播放都要打开
     */
    public void openTwoWayRecording(){
        PTTSessionManager.getInstance().setPlayAudio(true); // 同时播放声音
        setMicStat(1); //自动是申请状态了
        //
        if (nMicStat ==1){
            //申请状态下，抢到麦权了
            Log.i(TAG,"PTTService进入采集语音");
            setMicStat(2);  //播放成功抢到麦的提示音，并且将nStat设为2值
            isSpeaking = true;
            //开始采集语音线程
            new Thread(new Runnable() {
                @Override
                public void run() {
                    Log.i(TAG, "new Thread stop --0");
                    if (recorder != null) {
                        Log.i(TAG, "recorder.stopRecord");
                        recorder.stopRecord();
                    }

                    Log.i(TAG, "new Thread run --1");

                    if (recorder != null)
                        recorder.startRecord();
                    if (recorder != null)
                        recorder.readAudio();
                    Log.i(TAG, "new Thread end --2");
                }
            }).start();

        }

    }

    /**
     * 给双向通话用的内部方法，退出双向通话时关闭采集语音， 对讲模式还是要抢麦方式
     * 恢复成：麦克风关闭，喇叭打开
     */
    public void closeTwoWayRecording(){

        PTTSessionManager.getInstance().setPlayAudio(true); // 同时播放声音
        setMicStat(0); //表示停止采集语音了，里面有采集语音状态处理

    }

    private class TalkBroadcastReceiver extends BroadcastReceiver {

        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            Log.i(TAG, "TalkBroadcastReceiver action = " + action
                    + " nStat=" + nMicStat);

            if (MyBroadCast.BROADCAST_MIC_BREAKED.equalsIgnoreCase(action)) {
                //要停止语音采集，切换为播放状态
                Log.i(TAG,"PTTService收到抢麦打断通知");
                PTTSessionManager.getInstance().setPlayAudio(true); //改为采集状态，不播放语音
                setMicStat(0);  //进行停止采集,播放提示音，同步各种状态处理
                //这时还要去除超时任务
                handler.removeCallbacks(micOwnerTimeoutTask);

            }
            else if (MyBroadCast.BROADCAST_MIC_TIMEOUT.equalsIgnoreCase(action)){
                //讲话超时了
                Log.i(TAG,"PTTService收到麦权超时通知");
                PTTSessionManager.getInstance().setPlayAudio(true); //改为采集状态，不播放语音
                setMicStat(0);  //进行停止采集,播放提示音，同步各种状态处理

            }
            else if (MyBroadCast.BROADCAST_MIC_SUCCESS.equalsIgnoreCase(action)){
                //
                Log.i(TAG,"PTTService收到收到抢麦成功");
                handler.removeCallbacks(applyMicOwnerTimeoutTask);
                PTTSessionManager.getInstance().setPlayAudio(false); //改为采集状态，不播放语音
                //判断是不是sos语音包
                boolean isSosAlarm = intent.getBooleanExtra("isSosAlarmModel",false);
                if(isSosAlarm){
                    setMicStat(1);   //因为sos不需要抢麦，先当作已经申请了
                }

                //加入超时任务,先去除之前的，没有的话也能这样操作
                if (handler!=null){
                    handler.removeCallbacks(micOwnerTimeoutTask);
                    handler.postDelayed(micOwnerTimeoutTask,micownerTimeoutMs);
                }

                if (isSpeaking)
                    return;

                if (nMicStat ==1){
                    //申请状态下，抢到麦权了
                    Log.i(TAG,"PTTService进入采集语音");
                    setMicStat(2);  //播放成功抢到麦的提示音，并且将nStat设为2值
                    isSpeaking = true;
                    //开始采集语音线程
                    new Thread(new Runnable() {
                        @Override
                        public void run() {
                            Log.i(TAG, "new Thread stop --0");
                            if (recorder != null) {
                                Log.i(TAG, "recorder.stopRecord");
                                recorder.stopRecord();
                            }

                            Log.i(TAG, "new Thread run --1");

                            if (recorder != null)
                                recorder.startRecord();
                            if (recorder != null)
                                recorder.readAudio();
                            Log.i(TAG, "new Thread end --2");
                        }
                    }).start();

                }


            } else if (MyBroadCast.BROADCAST_MIC_FAILED.equalsIgnoreCase(action)){
                //抢麦失败, 可以不处理
                handler.removeCallbacks(applyMicOwnerTimeoutTask);

            }

        }
    }

    private void startForeground() {

        createNotificationChannel();

        // 创建空Intent（兼容Android 12+）
        PendingIntent emptyIntent = PendingIntent.getActivity(
                this,
                0,
                new Intent(),
                PendingIntent.FLAG_IMMUTABLE | PendingIntent.FLAG_NO_CREATE
        );

        Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .setContentTitle("PTT对讲服务")
                .setContentText("后台运行中")
                .setSmallIcon(R.drawable.icon_poc)
                .setContentIntent(emptyIntent) // 必须设置但无效果
                .setPriority(NotificationCompat.PRIORITY_LOW) // 降低优先级
                .setCategory(NotificationCompat.CATEGORY_SERVICE)
                .setOngoing(true)
                .setAutoCancel(false)
                .setShowWhen(false) // 隐藏时间戳
                .build();

        startForeground(NOTIFICATION_ID, notification);
    }

    private void createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            //大于或等于26要建serviceChannel
            NotificationChannel serviceChannel = new NotificationChannel(
                    CHANNEL_ID,
                    "PTT对讲服务通道",
                    NotificationManager.IMPORTANCE_HIGH
            );
            serviceChannel.setDescription("PTT对讲功能服务通道");
            serviceChannel.setImportance(NotificationManager.IMPORTANCE_HIGH);
            serviceChannel.setLockscreenVisibility(Notification.VISIBILITY_PUBLIC);

            NotificationManager manager = getSystemService(NotificationManager.class);
            manager.createNotificationChannel(serviceChannel);
        }
    }

    private void initializeTcpClient() {

        tcpClient = new PTTTcpClient(serverHost, serverPort,getApplicationContext());
        tcpClient.setTcpEventListener(new PTTTcpClient.TcpEventListener() {
            @Override
            public void onConnected() {
                if (eventListener != null) {
                    eventListener.onConnectionStateChanged(PTTState.TCP_CONNECTED);
                }
            }

            @Override
            public void onDisconnected() {
                if (eventListener != null) {
                    eventListener.onConnectionStateChanged(PTTState.TCP_DISCONNECTED);
                }
            }

            @Override
            public void onUserSpeaking(int groupId, int userId) {
                if (eventListener != null) {
                    eventListener.onUserStartSpeaking(groupId, userId);
                }
            }

            @Override
            public void onUserStoppedSpeaking(int groupId, int userId) {
                if (eventListener != null) {
                    eventListener.onUserStopSpeaking(groupId, userId);
                }
            }

            @Override
            public void onUserSpeakingBreaked(int groupId, int userId) {
                if (eventListener != null) {
                    eventListener.onSpeakingBreaked(groupId, userId);
                }
            }

            @Override
            public void onUserJoined(int groupId, int userId) {
                if (eventListener != null) {
                    eventListener.onUserJoinedGroup(groupId,userId);
                }
            }

            @Override
            public void onUserLeft(int groupId, int userId) {
                if (eventListener != null) {
                    eventListener.onUserLeftGroup(groupId,userId);
                }
            }

            @Override
            public void onUserOffline(int userId) {
                if (eventListener != null) {
                    eventListener.onUserOffline(userId);
                }
            }

            @Override
            public void onGroupInvite(int groupId, String groupName, int userId, int inviteId) {
                if (eventListener != null) {
                    eventListener.onGroupInvite(groupId,groupName,userId,inviteId);
                }
            }

            @Override
            public void onSingleCallSignal(int groupId, int fromUserId, int toUserId, byte signalVal) {
                if (eventListener != null) {
                    eventListener.onSingleCallSignal(groupId,fromUserId,toUserId,signalVal);
                }
            }

            @Override
            public void onGroupInviteTwoWay(int groupId, String groupName, int userId, int inviteId, int inviteMode) {
                if (eventListener != null) {
                    eventListener.onGroupInviteTwoWay(groupId,groupName,userId,inviteId, inviteMode);
                }
            }

            @Override
            public void onGroupDelete(int groupId) {
                if (eventListener != null) {
                    eventListener.onGroupDelete(groupId);
                }
            }

            @Override
            public void onGroupUserChange(int groupId, int groupTypeId, int changeType, String userStr) {
                if (eventListener != null) {
                    eventListener.onGroupUserChange( groupId,  groupTypeId,  changeType,userStr);
                }
            }

            @Override
            public void onAVChatMessage(short videoType, short videoCommand, int fromUserId, int toUserId, String fromUserName, String toUserName, String desc) {
                if (eventListener != null) {
                    eventListener.onAVChatMessage( videoType,videoCommand,fromUserId,toUserId,
                            fromUserName,  toUserName,desc);
                }
            }

            @Override
            public void onGroupSync(int groupId, String groupName, int userId, int inviteId) {
                if (eventListener != null) {
                    eventListener.onGroupSync( groupId,  groupName,  userId, inviteId );
                }
            }

            @Override
            public void onAudioDataReceived(String userId, byte[] audioData) {
                if (eventListener != null) {
                    eventListener.onAudioDataReceived(userId, audioData);
                }
            }

            @Override
            public void onMicrophoneGranted() {
                if (eventListener != null) {
                    eventListener.onApplyMicSuccess();
                }
            }

            @Override
            public void onMicrophoneDenied(String reason) {
                if (eventListener != null) {
                    eventListener.onApplyMicFailed(reason);
                }
            }

            @Override
            public void onKickOff() {
                if (eventListener != null) {
                    eventListener.onKickOff();
                }
            }

            @Override
            public void onRemoteKill() {
                if (eventListener != null) {
                    eventListener.onRemoteKill();
                }
            }

            @Override
            public void onErrorCode(int errorCode) {
                if (eventListener != null) {
                    eventListener.onErrorCode(errorCode);
                }
            }

            @Override
            public void onError(String error) {
                if (eventListener != null) {
                    eventListener.onError(error);
                }
            }
        });

        tcpClient.connect();
    }

    ////////////////////////////////
    //被pttmanager调用的方法
    /**
     * 发送抢麦信令
     * @param callback
     */
    public void logout(LogoutCallback callback) {
        if (tcpClient!=null && tcpClient.isConnected()){
            if (tcpClient.sendMessage(LogoutMessage.buildMessage(PTTSessionManager.getInstance().getGroupId(),
                    PTTSessionManager.getInstance().getUserId() )))
                callback.onSuccess();
            else
                callback.onFailure("发送消息失败");
        } else
            callback.onFailure("tcpclient没有创建或断开了");
    }


    /**
     * 发送抢麦信令
     * @param callback
     */
    public void requestMicrophone(RequestMicCallback callback) {
        if (tcpClient!=null && tcpClient.isConnected()){
            if (tcpClient.sendMessage(ApplyMicMessage.buildMessage()))
                callback.onSuccess();
            else
                callback.onFailure("发送消息失败");
        } else
            callback.onFailure("tcpclient没有创建或断开了");
    }


    public void releaseMicrophone(ReleaseMicCallback callback) {

        if (tcpClient!=null && tcpClient.isConnected()){

            //主动释放麦
            if (handler!=null) {
                Log.i(TAG,"提前释放,取消超时任务");
                handler.removeCallbacks(micOwnerTimeoutTask);
            }

            if (tcpClient.sendMessage(ReleaseMicMessage.buildMessage()))
                callback.onSuccess();
            else
                callback.onFailure("发送消息失败");

        } else
            callback.onFailure("tcpclient没有创建或断开了");

    }

    public void sendSingleCallSignal(int groupId, int fromUserId, int toUserId, byte signalVal, CommonCallback callback){
        if (tcpClient!=null && tcpClient.isConnected()){

            if (tcpClient.sendMessage(SingleCallSignalMessage.buildMessage(groupId,fromUserId,toUserId,signalVal)))
                callback.onSuccess();
            else
                callback.onFailure("发送消息失败");

        } else
            callback.onFailure("tcpclient没有创建或断开了");
    }

    public void sendSOSSession(int groupId, int userId, String sessionIdStr, byte state, CommonCallback callback){
        if (tcpClient!=null && tcpClient.isConnected()){

            if (tcpClient.sendMessage(SOSSessionMessage.buildMessage(groupId,userId,sessionIdStr,state)))
                callback.onSuccess();
            else
                callback.onFailure("发送消息失败");

        } else
            callback.onFailure("tcpclient没有创建或断开了");
    }

    public void sendSOSLocation(int groupId, int userId, String sessionIdStr, byte sosType, String gpsType,
                                double longitude, double latitude, CommonCallback callback) {
        if (tcpClient!=null && tcpClient.isConnected()){

            if (tcpClient.sendMessage(SOSLocationMessage.buildMessage(groupId,userId,sosType,longitude,latitude,sessionIdStr,gpsType)))
                callback.onSuccess();
            else
                callback.onFailure("发送消息失败");

        } else
            callback.onFailure("tcpclient没有创建或断开了");

    }

    public void joinGroup(int groupId, ReportWorkGroupCallback callback) {

        if (tcpClient!=null && tcpClient.isConnected()){
            if (tcpClient.sendMessage(ReportMessage.buildMessage(groupId,PTTSessionManager.getInstance().getUserId()))) {
                //记录最近的人和组, 以便重连时也要上报的
                PTTSessionManager.getInstance().setGroupId(groupId);
                callback.onSuccess();
            }
            else
                callback.onFailure("发送消息失败");
        } else
            callback.onFailure("tcpclient没有创建或断开了");

    }

    //////////////end

    public void setEventListener(IPTTEventListener listener) {
        this.eventListener = listener;
    }

    private void stopForegroundCompat() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.ECLAIR) { // API 5 (Android 2.0)
            stopForeground(true); // 标准方法
        } else {
            // 旧版本使用反射调用 setForeground(false)
            try {
                Method setForeground = getClass().getMethod("setForeground", boolean.class);
                setForeground.invoke(this, false);
            } catch (Exception e) {
                Log.e(TAG, "stopForegroundCompat failed (API <5)", e);
            }
        }
    }

    /**
     * 设置ptt键的状态,很重要
     * @param nType 0：弹起状态(就是ptt键没有按下) 1：申请状态 2：讲话状态  3: release(这也是弹起状态，即ptt键松开了)
     */
    public  void setMicStat(int nType) {
        nMicStat = nType;
        if (nMicStat == 1) {
            playSoundPool( 1 );
        } else if (nMicStat == 2) {
            playSoundPool(0);
        } else if (nMicStat == 0) {
            playSoundPool(1);
        }

        switch (nMicStat) {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                //3就是表示0，写重了
                nMicStat = 0;
                break;
            default:
                break;
        }

        if (nType==0){
            //停止采集
            Log.i(TAG,"PTTService进入停止采集语音");
            if (recorder != null) {
                Log.i(TAG, "recorder.stopRecord");
                recorder.stopRecord();
            }
            isSpeaking = false;
            PTTSessionManager.getInstance().setPlayAudio(true); //改为播放语音状态

        }

    }

    public  void playSoundPool( int soundIndex )
    {
        Log.d( TAG, "TalkFragment PlaySoundPool: " + soundIndex );
        try
        {
            switch( soundIndex )
            {
                case 1: // R.raw.sound_media_talk_prepare
                    sndstarttalk.play( sndmedia_talk_prepareID, (float)0.6, (float)0.6, 0, 0, (float)2.0 );
                    break;

                case 2: //
                    // sndstarttalk.stop( sndstarttalkID );
                    break;

                case 0: // R.raw.sound_media_me_off
                    sndstarttalk.stop( sndmedia_talk_prepareID );
                    sndstarttalk.play( sndmedia_me_offID, (float)0.6, (float)0.6, 0, 0, (float)2.0 );
                    break;
                case 3:
                    sndstarttalk.stop( sndmedia_offlineID );
                    sndstarttalk.play( sndmedia_onlineID, (float)0.9, (float)0.9, 0, 0, (float)1.0 );
                    break;
                case 4:
                    sndstarttalk.play( sndmedia_offlineID, (float)0.9, (float)0.9, 0, 0, (float)1.0 );
                    break;
            }
        }
        catch( Exception e )
        {
            Log.e(TAG,"报错3");
            Log.e(TAG, Log.getStackTraceString(e));
        }
    }

    @Override
    public void onDestroy() {
        unregisterPTTKeyReceiver();

        isHeartbeatStarted = false; // 重置状态
        cleanup();
        stopForegroundCompat();
        unregisterServiceReceiver();
        destroySoundPool();


        super.onDestroy();

    }

    private void cleanup() {

        if (heartbeatHandler != null && heartbeatRunnable != null) {
            heartbeatHandler.removeCallbacks(heartbeatRunnable);
        }

        if (tcpClient != null) {
            tcpClient.disconnect();
        }


    }

}
