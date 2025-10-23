package com.mypoc.ptt.webrtc.socket;

import android.content.ComponentName;
import android.content.Intent;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import com.dds.skywebrtc.CallSession;
import com.dds.skywebrtc.EnumType;
import com.dds.skywebrtc.SkyEngineKit;
import com.dds.skywebrtc.log.SkyLog;
import com.mypoc.ptt.activity.MainActivity;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.event.MeetInviteEvent;
import com.mypoc.ptt.event.UpdateWorkingGroupEvent;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.ptt.webrtc.room.CallMultiActivity;
import com.mypoc.ptt.webrtc.voip.Consts;
import com.mypoc.ptt.webrtc.voip.VoipReceiver;
import com.mypoc.ptt.widget.SoundDialog;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;

import org.greenrobot.eventbus.EventBus;

import java.lang.ref.WeakReference;
import java.net.URI;
import java.net.URISyntaxException;
import java.security.SecureRandom;
import java.util.List;

import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSocketFactory;
import javax.net.ssl.TrustManager;

/**
 * webrtc信令服务管理类
 */
public class SocketManager implements IEvent {

    private final static String TAG = SkyLog.createTag("SocketManager");

    private MyWebSocket webSocket;
    /**
     * 用户状态,1:已连接信令服务，0：未连接或退出了
     */
    private int userState;

    /**
     * 信令中自己的id标识
     */
    private String myId;

    private final Handler handler = new Handler(Looper.getMainLooper());

    // 添加心跳和重连相关变量
    private static final long HEARTBEAT_INTERVAL = 15*1000; // 15秒心跳，心跳越短，重连接会越快
    private static final long RECONNECT_INTERVAL = 10000; // 10秒重连间隔

    private Runnable heartbeatRunnable;
    private Runnable reconnectRunnable;
    private boolean shouldReconnect = true;
    private boolean isConnecting = false;
    private String currentUrl;
    private String currentUserId;
    private int currentDevice;

    private SocketManager() {
        initHeartbeatRunnable();
        initReconnectRunnable();
    }

    private static class Holder {
        private static final SocketManager socketManager = new SocketManager();
    }

    public static SocketManager getInstance() {
        return Holder.socketManager;
    }

    /**
     * 初始化心跳任务
     */
    private void initHeartbeatRunnable() {
        heartbeatRunnable = new Runnable() {
            @Override
            public void run() {
                sendHeartbeat();
                // 继续下一次心跳
                if (isConnected()) {
                    handler.postDelayed(this, HEARTBEAT_INTERVAL);
                }
            }
        };
    }

    /**
     * 初始化重连任务
     */
    private void initReconnectRunnable() {
        reconnectRunnable = new Runnable() {
            @Override
            public void run() {
                if (shouldReconnect && !isConnected() && !isConnecting) {
                    attemptReconnect();
                }
            }
        };
    }

    /**
     * 发送心跳
     */
    private void sendHeartbeat() {
        if (isConnected()) {
            try {
                // 发送简单的心跳字符串
                webSocket.send("heart");
                Log.d(TAG, "Heartbeat sent");
            } catch (Exception e) {
                Log.e(TAG, "Send heartbeat error: " + e.getMessage());
                // 发送心跳失败也触发重连
                scheduleReconnect();
            }
        }
    }

    /**
     * 开始心跳
     */
    private void startHeartbeat() {
        stopHeartbeat(); // 先停止现有的
        if (isConnected()) {
            handler.postDelayed(heartbeatRunnable, HEARTBEAT_INTERVAL);
            Log.d(TAG, "Heartbeat started");
        }
    }

    /**
     * 停止心跳
     */
    private void stopHeartbeat() {
        handler.removeCallbacks(heartbeatRunnable);
        Log.d(TAG, "Heartbeat stopped");
    }

    /**
     * 尝试重连
     */
    private void attemptReconnect() {
        if (currentUrl == null || currentUserId == null) {
            Log.d(TAG, "No connection info available for reconnect");
            return;
        }

        if (isConnecting) {
            Log.d(TAG, "Already connecting, skip");
            return;
        }

        Log.i(TAG, "Attempting to reconnect...");
        isConnecting = true;

        // 执行重连
        internalConnect(currentUrl, currentUserId, currentDevice);
    }

    /**
     * 调度重连
     */
    private void scheduleReconnect() {
        if (shouldReconnect) {
            Log.d(TAG, "Schedule reconnect after " + RECONNECT_INTERVAL + "ms");
            handler.removeCallbacks(reconnectRunnable);
            handler.postDelayed(reconnectRunnable, RECONNECT_INTERVAL);
        }
    }

    /**
     * 停止重连
     */
    private void stopReconnect() {
        handler.removeCallbacks(reconnectRunnable);
        isConnecting = false;
        Log.d(TAG, "Reconnect stopped");
    }

    /**
     * 检查是否连接
     */
    public boolean isConnected() {
        return webSocket != null && webSocket.isOpen();
    }

    /**
     * websocket 连接，后期要加鉴权验证，比如生成一个 aes加密验证
     * @param url
     * @param userId
     * @param device 0：表示移动端， 1：表示PC端
     */
    public void connect(String url, String userId, int device) {
        // 保存连接信息用于重连
        this.currentUrl = url;
        this.currentUserId = userId;
        this.currentDevice = device;
        this.shouldReconnect = true;

        stopReconnect(); // 停止现有的重连任务
        if (!isConnected()) {
            internalConnect(url, userId, device);
        } else {
            Log.i(TAG, "WebSocket is already connected");
        }

        /*
        if (webSocket == null || !webSocket.isOpen()) {
            URI uri;
            try {
                String urls = url + "/" + userId + "/" + device;
                Log.i(TAG,"urls="+urls);
                uri = new URI(urls);
            } catch (URISyntaxException e) {
                e.printStackTrace();
                return;
            }
            webSocket = new MyWebSocket(uri, this);
            // 设置wss
            if (url.startsWith("wss")) {
                try {
                    SSLContext sslContext = SSLContext.getInstance("TLS");
                    if (sslContext != null) {
                        sslContext.init(null, new TrustManager[]{new MyWebSocket.TrustManagerTest()}, new SecureRandom());
                    }

                    SSLSocketFactory factory = null;
                    if (sslContext != null) {
                        factory = sslContext.getSocketFactory();
                    }

                    if (factory != null) {
                        webSocket.setSocket(factory.createSocket());
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
            // 开始connect
            webSocket.connect();
        }
        */

    }

    /**
     * 内部连接方法
     */
    private void internalConnect(String url, String userId, int device) {
        if (webSocket != null) {
            webSocket.setConnectFlag(false);
            webSocket.close();
            webSocket = null;
        }

        URI uri;
        try {
            String urls = url + "/" + userId + "/" + device;
            Log.i(TAG,"urls="+urls);
            uri = new URI(urls);
        } catch (URISyntaxException e) {
            e.printStackTrace();
            // 连接失败，调度重连
            scheduleReconnect();
            return;
        }

        webSocket = new MyWebSocket(uri, this);
        // 设置wss
        if (url.startsWith("wss")) {
            try {
                SSLContext sslContext = SSLContext.getInstance("TLS");
                if (sslContext != null) {
                    sslContext.init(null, new TrustManager[]{new MyWebSocket.TrustManagerTest()}, new SecureRandom());
                }

                SSLSocketFactory factory = null;
                if (sslContext != null) {
                    factory = sslContext.getSocketFactory();
                }

                if (factory != null) {
                    webSocket.setSocket(factory.createSocket());
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        // 开始connect
        webSocket.connect();
    }

    public void unConnect() {
        // 设置标志位，停止自动重连
        shouldReconnect = false;
        // 停止心跳和重连任务
        stopHeartbeat();
        stopReconnect();
        if (webSocket != null) {
            webSocket.setConnectFlag(false);
            webSocket.close();
            webSocket = null;
        }

    }

    @Override
    public void onOpen() {
        Log.i(TAG, "socket is open!");
        isConnecting = false;
        // 连接成功后开始心跳
        startHeartbeat();
        // 停止重连任务
        stopReconnect();
    }

    @Override
    public void loginSuccess(String userId, String avatar) {
        Log.i(TAG, "loginSuccess:" + userId);
        myId = userId;
        userState = 1;
        isConnecting = false;
        // 登录成功后确保心跳开始
        startHeartbeat();

        if (iUserState != null && iUserState.get() != null) {
            iUserState.get().userLogin();
        } else {
            //login页面无回调
            Log.i(TAG,"login页面无回调");
        }
    }

    @Override
    public void reConnect() {
        Log.i(TAG, "Manual reConnect called");
        // 直接尝试重连
        attemptReconnect();

       /* handler.post(() -> {
            webSocket.reconnect();
        });*/
    }

    // ======================================================================================
    public void createRoom(String room, int roomSize,String roomOtherUsers) {
        if (webSocket != null) {
            webSocket.createRoom(room, roomSize, myId,roomOtherUsers);
        }

    }

    public void sendInvite(String room, List<String> users, boolean audioOnly) {
        if (webSocket != null) {
            webSocket.sendInvite(room, myId, users, audioOnly);
        }
    }

    public void sendLeave(String room, String userId) {
        if (webSocket != null) {
            webSocket.sendLeave(myId, room, userId);
        }
    }

    public void sendRingBack(String targetId, String room) {
        if (webSocket != null) {
            webSocket.sendRing(myId, targetId, room);
        }
    }

    public void sendRefuse(String room, String inviteId, int refuseType) {
        if (webSocket != null) {
            webSocket.sendRefuse(room, inviteId, myId, refuseType);
        }
    }

    public void sendCancel(String mRoomId, List<String> userIds) {
        if (webSocket != null) {
            webSocket.sendCancel(mRoomId, myId, userIds);
        }
    }

    public void sendJoin(String room) {
        if (webSocket != null) {
            webSocket.sendJoin(room, myId);
        }
    }

    public void sendMeetingInvite(String userList) {

    }

    public void sendOffer(String userId, String sdp) {
        if (webSocket != null) {
            webSocket.sendOffer(myId, userId, sdp);
        }
    }

    public void sendAnswer(String userId, String sdp) {
        if (webSocket != null) {
            webSocket.sendAnswer(myId, userId, sdp);
        }
    }

    public void sendIceCandidate(String userId, String id, int label, String candidate) {
        if (webSocket != null) {
            webSocket.sendIceCandidate(myId, userId, id, label, candidate);
        }
    }

    public void sendTransAudio(String userId) {
        if (webSocket != null) {
            webSocket.sendTransAudio(myId, userId);
        }
    }

    public void sendDisconnect(String room, String userId) {
        if (webSocket != null) {
            webSocket.sendDisconnect(room, myId, userId);
        }
    }


    // ========================================================================================
    @Override
    public void onInvite(String room, boolean audioOnly, String inviteId, String userList) {
        Intent intent = new Intent();
        intent.putExtra("room", room);
        intent.putExtra("audioOnly", audioOnly);
        intent.putExtra("inviteId", inviteId);
        intent.putExtra("userList", userList);
        intent.setAction(Consts.ACTION_VOIP_RECEIVER);
        intent.setComponent(new ComponentName(MyPOCApplication.getInstance().getPackageName(), VoipReceiver.class.getName()));
        // 发送广播
        MyPOCApplication.getInstance().sendBroadcast(intent);

    }

    @Override
    public void onInviteMeet(String room, String fromUserId) {

        EventBus.getDefault().post(new MeetInviteEvent(room,fromUserId));

    }

    @Override
    public void onCancel(String inviteId) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onCancel(inviteId);
            }
        });

    }

    @Override
    public void onRing(String fromId) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onRingBack(fromId);
            }
        });


    }

    @Override  // 加入房间
    public void onPeers(String myId, String connections, int roomSize) {
        handler.post(() -> {
            //自己进入了房间，然后开始发送offer
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onJoinHome(myId, connections, roomSize);
            }
        });

    }

    @Override
    public void onNewPeer(String userId) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.newPeer(userId);
            }
        });

    }

    @Override
    public void onReject(String userId, int type) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onRefuse(userId, type);
            }
        });

    }

    @Override
    public void onOffer(String userId, String sdp) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onReceiveOffer(userId, sdp);
            }
        });


    }

    @Override
    public void onAnswer(String userId, String sdp) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onReceiverAnswer(userId, sdp);
            }
        });

    }

    @Override
    public void onIceCandidate(String userId, String id, int label, String candidate) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onRemoteIceCandidate(userId, id, label, candidate);
            }
        });

    }

    @Override
    public void onLeave(String userId) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onLeave(userId);
            }
        });
    }

    @Override
    public void logout(String str) {
        Log.i(TAG, "logout:" + str);
        userState = 0;

        isConnecting = false;
        // 登出时开始重连
        scheduleReconnect();

        if (iUserState != null && iUserState.get() != null) {
            iUserState.get().userLogout();
        }
    }

    @Override
    public void onTransAudio(String userId) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onTransAudio(userId);
            }
        });
    }

    @Override
    public void onDisConnect(String userId) {
        handler.post(() -> {
            CallSession currentSession = SkyEngineKit.Instance().getCurrentSession();
            if (currentSession != null) {
                currentSession.onDisConnect(userId, EnumType.CallEndReason.RemoteSignalError);
            }
        });
    }


    //===========================================================================================


    public int getUserState() {
        return userState;
    }

    private WeakReference<IUserState> iUserState;

    public void addUserStateCallback(IUserState userState) {
        iUserState = new WeakReference<>(userState);
    }

    /**
     * 清理资源
     */
    public void destroy() {
        shouldReconnect = false;
        stopHeartbeat();
        stopReconnect();
        unConnect();
    }

}
