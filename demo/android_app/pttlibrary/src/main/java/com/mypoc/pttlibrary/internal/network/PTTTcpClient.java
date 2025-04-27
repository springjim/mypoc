package com.mypoc.pttlibrary.internal.network;

import android.content.Context;
import android.content.Intent;
import android.util.Log;

import com.mypoc.pttlibrary.internal.MyBroadCast;
import com.mypoc.pttlibrary.internal.PTTSessionManager;
import com.mypoc.pttlibrary.internal.audio.AudioRecorder;
import com.mypoc.pttlibrary.internal.tcp.AudioSender;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TcpReader;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;
import com.mypoc.pttlibrary.internal.tcp.message.CheckServerMessage;
import com.mypoc.pttlibrary.internal.tcp.message.GroupDeleteMessage;
import com.mypoc.pttlibrary.internal.tcp.message.GroupInviteMessage;
import com.mypoc.pttlibrary.internal.tcp.message.GroupSyncMessage;
import com.mypoc.pttlibrary.internal.tcp.message.GroupUserChangeMessage;
import com.mypoc.pttlibrary.internal.tcp.message.LoginMessage;
import com.mypoc.pttlibrary.internal.tcp.message.MediaExFileFrameMessage;
import com.mypoc.pttlibrary.internal.tcp.message.MediaExMessage;
import com.mypoc.pttlibrary.internal.tcp.message.ReportMessage;
import com.mypoc.pttlibrary.internal.tcp.message.SystemReportMessage;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketException;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

/**
 * 具备自动重连tcp服务端
 */
public class PTTTcpClient {

    private static final String TAG = "PTTTcpClient";
    private static final int RECONNECT_DELAY = 5000; // 5秒重连间隔
    private static ReentrantLock   socketLock = new ReentrantLock();
    private final String serverHost;
    private final int serverPort;

    private Socket socket;
    private DataInputStream in;
    private DataOutputStream out;
    public static TcpReader reader = null;  //读取tcp数据，解码线程
    private static AudioSender writer = null; //发送语音包线程

    private Thread receiveThread; //接受线程
    private boolean isRunning;
    private Context context;
    private TcpEventListener eventListener;

    private final AtomicBoolean isManualDisconnect = new AtomicBoolean(false);
    private final ScheduledExecutorService reconnectExecutor =
            Executors.newSingleThreadScheduledExecutor();


    public interface TcpEventListener {
        void onConnected();

        void onDisconnected();

        void onUserSpeaking(int groupId, int userId); //开始说话

        void onUserStoppedSpeaking(int groupId, int userId); //停止说话

        void onUserSpeakingBreaked(int groupId, int userId); //当前讲话被高麦权人打断

        void onUserJoined(int groupId, int userId);

        void onUserLeft(int groupId, int userId);

        void onUserOffline(int userId);  //用户离线

        void onGroupInvite(int groupId, String groupName, int userId, int inviteId);  //收到某某发来的创建组的邀请

        void onGroupDelete(int groupId); //收到某某解散组的通知

        void onAudioDataReceived(String userId, byte[] audioData);

        void onMicrophoneGranted(); //抢麦成功

        void onMicrophoneDenied(String reason);  //抢麦失败

        void onError(String error);

        void onGroupUserChange(int groupId, int groupTypeId, int changeType, String userStr); //强插或强拆通知

        void onGroupSync(int groupId, String groupName, int userId, int inviteId);  //组强制同步通知

        void onKickOff(); //同帐号被踢了

    }

    public PTTTcpClient(String host, int port, Context context) {
        this.serverHost = host;
        this.serverPort = port;
        this.isRunning = false;
        this.context = context;
    }

    // 提供安全的访问方法
    public Socket getSocket() {
        //socketLock.lock();
        try {
            return this.socket;
        } finally {
            //socketLock.unlock();
        }
    }

    public DataInputStream getInputStream() {
        //socketLock.lock();
        try {
            return this.in;
        } finally {
            //socketLock.unlock();
        }
    }

    public DataOutputStream getOutputStream() {
        //socketLock.lock();
        try {
            return this.out;
        } finally {
            //socketLock.unlock();
        }
    }
    // 提供安全的访问方法 end

    public void connect() {
        if (isRunning) return;

        isManualDisconnect.set(false); //非手动断开
        isRunning = true;
        //new Thread(this::connectAndReceive).start();
        startConnectLoop();
    }

    private void startConnectLoop() {
        reconnectExecutor.scheduleWithFixedDelay(() -> {
            if (!isRunning || isManualDisconnect.get()) {
                //如果没在运行或手动断开了，不会重连
                return;
            }

            try {
                if (!isConnected()) {
                    connectAndReceive();
                }
            } catch (Exception e) {
                Log.e(TAG, "Reconnect attempt failed: " + e.getMessage());
                if (eventListener != null) {
                    eventListener.onError("连接断开，正在尝试重连..." + RECONNECT_DELAY + " ms后开始重连...");
                }
            }
        }, 0, RECONNECT_DELAY, TimeUnit.MILLISECONDS);
    }

    private void connectAndReceive() {

        Log.i(TAG, "connectSocket TCP Server  socketlock.lock()");
        socketLock.lock();
        try {

            // 1. 先释放所有旧资源
            //这里如果是重连的socket时，要清除旧的socket的inputstream,很重要的步骤
            releaseAllResources();

            // 2. 创建新Socket
            Socket socketTMP = new Socket();
            socketTMP.setTcpNoDelay(true);
            socketTMP.connect(new InetSocketAddress(serverHost, serverPort), 2000);
            Log.i(TAG, "--->new Socket sucessfully");
            //socket.setKeepAlive(true);

            if (eventListener != null) {
                eventListener.onConnected();
            }

            Log.i(TAG, "--->connectSocket TCP Server connect sucessfully");

            out = new DataOutputStream(socketTMP.getOutputStream());
            in = new DataInputStream(socketTMP.getInputStream());

            socket = socketTMP; //再用新的socket

            writer = null;
            reader = null;

            if (writer == null) {
                writer = new AudioSender(this);
                Log.i(TAG, "--->new AudioSender() sucessfully");
            }
            if (reader == null) {
                reader = new TcpReader(this, context);
                Log.i(TAG, "--->new TcpReader() sucessfully");
            }

            new Thread(writer).start();
            Log.i(TAG, "--->socket writer Thread start");

            new Thread(reader).start();
            Log.i(TAG, "--->socket reader Thread start");

            //重连过后，还要检查有没有工作组，要即时上报的
            if (PTTSessionManager.getInstance().getGroupId() != -1 &&
                    PTTSessionManager.getInstance().getUserId() != -1) {

                Log.i(TAG, "重连后上报工作组");
                sendMessage(ReportMessage.buildMessage(PTTSessionManager.getInstance().getGroupId(),
                        PTTSessionManager.getInstance().getUserId()));

            }


        } catch (Exception e) {

            releaseAllResources();
            if (eventListener != null) {
                eventListener.onError("Connection failed: " + e.getMessage());
            }
            Log.e(TAG, "Connect error", e);

        } finally {
            if (socketLock.isHeldByCurrentThread()) {
                socketLock.unlock();
            }
            Log.i(TAG, "connectSocket TCP Server  socketlock.unlock()");
        }

    }


    public synchronized void releaseAllResources() {

        Log.e(TAG, "releaseAllResources===================== <<<<<<<<<<<<<<<<<<<<<<<<<<<");

        try {
            // 关闭输入流
            if (in != null) {
                try {
                    in.close();
                } catch (IOException e) {
                    Log.e(TAG, "Error closing input stream", e);
                }
                in = null;
            }

            // 关闭输出流
            if (out != null) {
                try {
                    out.close();
                } catch (IOException e) {
                    Log.e(TAG, "Error closing output stream", e);
                }
                out = null;
            }

            // 停止并清理reader/writer
            if (reader != null) {
                reader.stop();
            }
            if (writer != null) {
                writer.stop();
            }

            Thread.sleep(5);

            if (reader != null) {
                reader.onDestory();

            }
            writer = null;
            reader = null;
            Thread.sleep(20);
            // 关闭Socket
            if (socket != null) {
                try {
                    if (!socket.isInputShutdown()) socket.shutdownInput();
                    if (!socket.isOutputShutdown()) socket.shutdownOutput();
                    if (!socket.isClosed()) socket.close();
                } catch (IOException e) {
                    Log.e(TAG, "Error closing socket", e);
                }
                socket = null;
            }
        } catch (InterruptedException e) {
            e.printStackTrace();
        } finally {
            socketLock.unlock();
        }

    }


    private void handleDisconnection() {
        if (!isRunning) return;

        cleanupSocket();
        if (eventListener != null) {
            eventListener.onDisconnected();
        }
        // 自动触发重连（通过正在运行的reconnectExecutor）
    }

    public void cleanupSocket() {

        /*

        try {

            if (reader != null) {
                reader.stop();
                Log.i(TAG, "--->reader.stop");
            }
            if (writer != null) {
                writer.stop();
                Log.i(TAG, "--->writer.stop");
            }

            if (reader != null) {
                reader.onDestory();
                Log.i(TAG, "--->reader.onDestory");
            }

            if (socket != null) {
                if (socket.isConnected() && !socket.isClosed())
                    if (!socket.isInputShutdown()) {
                        socket.shutdownInput();
                        Log.e(TAG, "shutdownInput socket...");
                    }


                if (socket.isConnected() && !socket.isClosed())
                    if (!socket.isOutputShutdown()) {
                        socket.shutdownOutput();
                        Log.e(TAG, "shutdownOutput socket...");
                    }

                if (socket.isConnected() && !socket.isClosed()) {
                    socket.close();
                    Log.e(TAG, "close socket...");
                }
            }

        } catch (IOException e) {
            Log.e(TAG, "Cleanup error: " + Log.getStackTraceString(e));
            Log.i(TAG, "开始重连...");
            //connectAndReceive();
        }
        */
        connectAndReceive();

    }

    /**
     * 处理消息，仅用第3个字节表示payload长度的消息
     *
     * @param messageId 消息ID
     * @param msgBytes  整包消息，包括消息头
     */
    public void processMessageExcludePayloadLen(short messageId, byte[] msgBytes) {
        switch (messageId) {

            case TCPMessageType.TYPE_MEDIA:
                //老版本的没用了
                break;
            case TCPMessageType.TYPE_TOPOC_START:
                //
                break;
            case TCPMessageType.TYPE_TOPOC_END:
                //
                break;
            case TCPMessageType.TYPE_P2G_MESSAGES:
            case TCPMessageType.TYPE_P2P_MESSAGES:

                break;
            case TCPMessageType.SOS_KEY_RELEASE:
                //调度台sos对讲释放了
                break;
            case TCPMessageType.TYPE_KICK_OFF:
                //同帐号的互相踢除，后登入的踢出前一个
                if (eventListener != null)
                    eventListener.onKickOff();

                break;
            case TCPMessageType.TYPE_CAMERA_SWITCH:
                //远程切换前后摄 agora
                break;
            case TCPMessageType.TYPE_CHECK_SERVER:
                //客户端发的请求，一般不会有这种消息
                break;
            case TCPMessageType.TYPE_MIC_SUCCESS:
                //抢麦成功
                Log.i(TAG, "收到抢麦成功回复: " + TextUtil.bytesToIntString(msgBytes));
                //开始采集语音
                if (eventListener != null) {
                    //向sdk发送事件回调
                    eventListener.onMicrophoneGranted();
                }
                //发通知, 在PTTService中处理这个通知
                Intent notifyIntent = new Intent();
                notifyIntent.setAction(MyBroadCast.BROADCAST_MIC_SUCCESS);
                this.context.sendBroadcast(notifyIntent);

                break;
            case TCPMessageType.TYPE_MIC_FAILED:
                //抢麦失败
                Log.i(TAG, "收到抢麦失败回复: " + TextUtil.bytesToIntString(msgBytes));
                if (eventListener != null) {
                    //向sdk发送事件回调
                    eventListener.onMicrophoneDenied("抢麦失败");
                }
                //发通知, 在PTTService中处理这个通知
                Intent notifyFailIntent = new Intent();
                notifyFailIntent.setAction(MyBroadCast.BROADCAST_MIC_FAILED);
                this.context.sendBroadcast(notifyFailIntent);

                break;
            case TCPMessageType.TYPE_SERVICE_RESPONSE:
                // 收到客户端检查服务器心跳检测的响应
                Log.i(TAG, "收到心跳回复: " + TextUtil.bytesToIntString(msgBytes));
                break;

            case TCPMessageType.TYPE_LOGIN:
                //登录
                Log.i(TAG, "收到其它用户登录: " + TextUtil.bytesToIntString(msgBytes));
                LoginMessage msg = LoginMessage.parseBytes(msgBytes);
                Log.i(TAG, msg.toString());
                break;
            case TCPMessageType.TYPE_LOGOUT:
                //注销
                Log.i(TAG, "收到其它用户登出: " + TextUtil.bytesToIntString(msgBytes));
                break;
            case TCPMessageType.TYPE_REJECT_APPLY:
                //收到邀请加入组的回复
                break;
            case TCPMessageType.TYPE_REJECT_INVITE:
                //收到邀请加入组的回复
                break;
            case TCPMessageType.TYPE_KICK_USER:
                //收到踢除用户通知
                break;
            case TCPMessageType.TYPE_DELETE_GROUP:
                //收到解散组通知
                Log.i(TAG, "收到解散组通知");
                GroupDeleteMessage groupDeleteMessage = GroupDeleteMessage.parseBytes(msgBytes);
                if (eventListener != null) {
                    eventListener.onGroupDelete(groupDeleteMessage.getGroupId());
                }

                break;
            case TCPMessageType.TYPE_EXIT_GROUP:
                //收到用户退出组通知
                break;

            case TCPMessageType.TYPE_ACCEPT_APPLY:
                //收到申请加入组的回复
                break;
            case TCPMessageType.TYPE_ACCEPT_INVITE:
                //收到申请加入组的回复
                break;
            case TCPMessageType.TYPE_RECIVED_APPLY:
                //收到用户加入组的申请
                break;
            case TCPMessageType.TYPE_RECIVED_INVITE:
                //收到加入组的邀请
                //别人邀请建组
                Log.i(TAG, "收到建临时组邀请");
                GroupInviteMessage groupInviteMessage = GroupInviteMessage.parseBytes(msgBytes);
                if (eventListener != null) {
                    eventListener.onGroupInvite(groupInviteMessage.getGroupId(),
                            groupInviteMessage.getGroupName(), groupInviteMessage.getUserId(),
                            groupInviteMessage.getInviteId());
                }
                break;
            case TCPMessageType.TYPE_GROUP_SYNC:
                //调度台强制同步入群
                GroupSyncMessage groupSyncMessage = GroupSyncMessage.parseBytes(msgBytes);
                Log.i(TAG, "调度台强制同步入群:" + groupSyncMessage.toString());
                if (eventListener != null) {
                    eventListener.onGroupSync(groupSyncMessage.getGroupId(), groupSyncMessage.getGroupName()
                            , groupSyncMessage.getUserId(), groupSyncMessage.getInviteId());
                }

                break;
            case TCPMessageType.TYPE_SYS_MESSAGE:
                //系统状态消息同步
                //Log.i(TAG, "收到系统状态消息: " + TextUtil.bytesToIntString(msgBytes));
                SystemReportMessage msgSystemReport = SystemReportMessage.parseBytes(msgBytes);
                if (eventListener != null) {

                    Log.i(TAG, "收到系统状态消息: " + msgSystemReport.toString());

                    switch (msgSystemReport.getType()) {

                        case SystemReportMessage.MySysMsgType.SYS_MSSAGE_TALK_START:
                            //开始说话, 这里要判断是否为当前人ID 且是同一个组，如果不是，则是被人打断了
                            if (PTTSessionManager.recording && msgSystemReport.getUserId() != PTTSessionManager.getInstance().getUserId()
                                    && msgSystemReport.getGroupId() == PTTSessionManager.getInstance().getGroupId()
                            ) {

                                //停止采集语音, 发广播给 pttservice
                                Intent stopSpeakIntent = new Intent();
                                stopSpeakIntent.setAction(MyBroadCast.BROADCAST_MIC_BREAKED);
                                this.context.sendBroadcast(stopSpeakIntent);

                                //被打断了,发送回调
                                eventListener.onUserSpeakingBreaked(msgSystemReport.getGroupId(), msgSystemReport.getUserId());

                            } else {

                                eventListener.onUserSpeaking(msgSystemReport.getGroupId(), msgSystemReport.getUserId());

                            }

                            break;
                        case SystemReportMessage.MySysMsgType.SYS_MSSAGE_TALK_STOP:
                            //停止说话
                            eventListener.onUserStoppedSpeaking(msgSystemReport.getGroupId(), msgSystemReport.getUserId());
                            break;
                        case SystemReportMessage.MySysMsgType.SYS_MSSAGE_IN_GROUP:
                        case SystemReportMessage.MySysMsgType.SYS_MSSAGE_ONLINE_PRESON:
                            //用户进入组，与用户上线，会报进入某个组, 都一样处理
                            eventListener.onUserJoined(msgSystemReport.getGroupId(), msgSystemReport.getUserId());
                            break;
                        case SystemReportMessage.MySysMsgType.SYS_MSSAGE_OUT_GROUP:
                            //用户离开组
                            eventListener.onUserLeft(msgSystemReport.getGroupId(), msgSystemReport.getUserId());
                            break;

                        case SystemReportMessage.MySysMsgType.SYS_MSSAGE_OFFLINE_PRESON:
                            //用户下线
                            eventListener.onUserOffline(msgSystemReport.getUserId());
                            break;


                        default:
                            break;
                    }

                }
                break;
            case TCPMessageType.TYPE_PERSON_INVITE:
                //
                break;
            case TCPMessageType.TYPE_PERSON_INVITE_RELEASE:
                break;


            default:
                Log.e(TAG, "未处理的messageId=" + messageId);
                break;
        }
    }

    /**
     * 处理消息，第4，5字节表示payload长度的消息
     *
     * @param messageId
     * @param msgBytes
     */
    public void processMessageIncludePayloadLen(short messageId, byte[] msgBytes) {
        switch (messageId) {

            case TCPMessageType.MEDIA_EX_TOPLATFORM:
                //特殊的包，一般是用来调度台用来监听非当前组的
                //如果在app上收到了，可以不处理
                Log.i(TAG, "收到非当前的语音包,messageId=" + messageId);
                //todo 目前是丢弃该包，以后根据客户需求进行定制
                break;


            case TCPMessageType.TYPE_MEDIA_EX:
                //组内对讲语音包
                MediaExMessage msg = MediaExMessage.parseBytes(msgBytes);
                PTTSessionManager.getInstance().setmMEDIA_EX_FILE_FRAME(0);
                AudioRecorder.decoderData(msg.getMedia());
                break;
            case TCPMessageType.TYPE_MEDIA_EX_FILE_FRAME:
                //广播群语音包
                MediaExFileFrameMessage msgFileFrame = MediaExFileFrameMessage.parseBytes(msgBytes);
                PTTSessionManager.getInstance().setmMEDIA_EX_FILE_FRAME(1);
                AudioRecorder.recvFileAudioFrame(msgFileFrame.getMedia());
                break;
            case TCPMessageType.SOS_MEDIA_EX:
                //sos语音包, 调度台发过来的
                break;

            case TCPMessageType.SOS_LOCATION:
                break;
            case TCPMessageType.TYPE_TOPOC_START_EX:
                break;
            case TCPMessageType.TYPE_TOPOC_END_EX:
                break;
            case TCPMessageType.TYPE_GROUP_USER_CHANGE:
                //组内成员强插或强拆
                GroupUserChangeMessage groupUserChangeMessage = GroupUserChangeMessage.parseBytes(msgBytes);
                Log.i(TAG, "收到groupUserChange消息:" + groupUserChangeMessage.toString());
                if (eventListener != null) {
                    //其它的userList可以不用回调
                    eventListener.onGroupUserChange(groupUserChangeMessage.getGroupId(),
                            groupUserChangeMessage.getGroupTypeId(),
                            groupUserChangeMessage.getChangeType(),
                            groupUserChangeMessage.getUseridlist()
                    );
                }

                break;
            case TCPMessageType.TYPE_MODIFY_GROUP:
                //修改群组
                break;
            case TCPMessageType.TYPE_AV_CHAT_NEW:
                //视频通话agora
                break;
            case TCPMessageType.TYPE_MEET_CHAT:
                //视频会商 agora
                break;
            case TCPMessageType.TYPE_AV_REMOTE_MONI:
                //调取视频监控 agora
                break;
            case TCPMessageType.TYPE_GPS_COMMAND:
                //调度台下达gps定位请求
                break;
            case TCPMessageType.TYPE_SHARE_VIDEOLIVE:
                //有终端分享视频过来, 已废除
                break;
            case TCPMessageType.VIDEO_MESSAGE:
                //废除
                break;
            case TCPMessageType.TYPE_P2G_MESSAGES:
                Log.e(TAG, "不处理TYPE_P2G_MESSAGES, messageId=" + messageId);
                break;
            case TCPMessageType.TYPE_P2P_MESSAGES:
                Log.e(TAG, "不处理TYPE_P2P_MESSAGES, messageId=" + messageId);
                break;
            case TCPMessageType.TYPE_P2G_CHAT_TEXT:
                Log.e(TAG, "不处理TYPE_P2G_CHAT_TEXT, messageId=" + messageId);
                break;
            case TCPMessageType.TYPE_P2P_CHAT_TEXT:
                Log.e(TAG, "不处理TYPE_P2P_CHAT_TEXT, messageId=" + messageId);
                break;
            case TCPMessageType.TYPE_P2G_CHAT_FILE:
                Log.e(TAG, "不处理TYPE_P2G_CHAT_FILE, messageId=" + messageId);
                break;
            case TCPMessageType.TYPE_P2P_CHAT_FILE:
                Log.e(TAG, "不处理TYPE_P2P_CHAT_FILE, messageId=" + messageId);
                break;

            default:
                Log.e(TAG, "未处理的messageId=" + messageId);
                break;
        }
    }

    public void sendHeartbeat() {
        sendMessage(CheckServerMessage.buildMessage());
    }


    /**
     * 用于发送tcp消息
     */
    public boolean sendMessage(byte[] msgBytes) {
        if (!isRunning || out == null)
            return false;
        //要放到子线程中执行，要不然app模块有时会在UI线程中调用，会报错
        new Thread(() -> {
            try {
                out.write(msgBytes);
                out.flush();
                //return true;
            } catch (IOException e) {
                if (eventListener != null) {
                    eventListener.onError("Send command error: " + e.getMessage());
                }
                handleDisconnection();
            }
        }).start();
        return true; // Note: This will return immediately, success/failure is handled via callbacks
    }

    /**
     * 发出socket断开事件，由tcpreader调用
     */
    public void emitSocketDisconnectEvent() {
        if (eventListener != null) {
            eventListener.onDisconnected();
        }
    }

    public void setTcpEventListener(TcpEventListener listener) {
        this.eventListener = listener;
    }

    public boolean isConnected() {
        return socket != null && socket.isConnected() && !socket.isClosed();
    }

    public void disconnect() {
        isManualDisconnect.set(true);
        isRunning = false;

        if (receiveThread != null) {
            receiveThread.interrupt();
        }

        if (in != null) {
            try {
                in.close();
            } catch (IOException e) {
                Log.e(TAG, "Close input stream error: " + e.getMessage());
            }
        }

        if (out != null) {
            try {
                out.close();
            } catch (IOException e) {
                Log.e(TAG, "Close output stream error: " + e.getMessage());
            }
        }

        if (socket != null) {
            try {
                socket.close();
            } catch (IOException e) {
                Log.e(TAG, "Close socket error: " + e.getMessage());
            }
        }

        if (eventListener != null) {
            eventListener.onDisconnected();
        }
    }

}
