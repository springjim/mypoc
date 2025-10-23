package com.mypoc.pttlibrary.internal.tcp;

import android.content.Context;
import android.util.Log;

import com.mypoc.pttlibrary.internal.PTTSessionManager;
import com.mypoc.pttlibrary.internal.audio.AudioPlayer;
import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.network.PTTTcpClient;

import java.io.DataInputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.SocketException;

public class TcpReader implements Runnable {

    private static final String TAG = "TcpReader";

    private final PTTTcpClient tcpClient;
    private Context mContext;

    /**
     * AudioPlayer 是喇叭出声
     */
    private AudioPlayer player = null;
    /**
     * AudioPlayer2 是听筒出声
     */
    //private AudioPlayer2 player2 = null;

    // 读完一个消息后剩余的byte array
    private byte[] leftBuffer = new byte[0];

    volatile boolean isRunning = false;
    private boolean ischeckRecRunning = false;

    public TcpReader(PTTTcpClient client, Context context) {
        this.tcpClient = client;
        this.mContext = context;
    }


    public void stop() {
        isRunning = false;
        ischeckRecRunning = false;
        try {
            Socket socket = tcpClient.getSocket();
            if (socket != null && socket.isConnected()) {
                if (socket.isConnected() && !socket.isClosed())
                    if (!socket.isInputShutdown())
                        socket.shutdownInput();

                Log.i(TAG, "Stop TcpReader thread--- socket.shutdownInput");
            }
        } catch (Exception ex) {
            Log.e(TAG, "stop TcpReader thread:\n" + Log.getStackTraceString(ex));
        }
    }

    public void onDestory() {
        try {
            Log.d(TAG, "onDestory player =========<<<<<<<<<<<<<<<<<<<<<<<<");

            if (player != null) {
                player.stopPlay();
                player.release();
                player = null;

                Log.d(TAG, "close player...");
            }
        } catch (Exception e) {
            Log.e(TAG, "onDestory player.stopPlay:\n" + Log.getStackTraceString(e));
        }

        //下面屏蔽听筒处理
        /*
        try
        {
            Log.d(TAG, "onDestory player2 ==========<<<<<<<<<<<<<<<<<<<<<<<<" );

            if (player2 != null) {
                player2.stopPlay();
                player2.release();
                player2 = null;

                Log.d(TAG, "close player2...");
            }
        }
        catch( Exception e )
        {
            Log.e(TAG, "onDestory player2.stopPlay:\n" + Log.getStackTraceString(e) );
        }
        */
    }

    /**
     * 得到真实的全部流字节（如果上次有剩余就组装在一起）
     *
     * @param netLen
     * @param netStream
     * @return
     */
    private byte[] getRealStream(int netLen, byte[] netStream) {
        byte[] buffer;

        if (leftBuffer.length > 0) {
            buffer = new byte[netLen + leftBuffer.length];

            System.arraycopy(leftBuffer, 0, buffer, 0, leftBuffer.length);
            System.arraycopy(netStream, 0, buffer, leftBuffer.length, netLen);
        } else {
            buffer = new byte[netLen];

            System.arraycopy(netStream, 0, buffer, 0, netLen);
        }
        return buffer;
    }

    /**
     * 判断是否是第4，5个字节表示payload长度的消息
     *
     * @param messageId
     * @return
     */
    private boolean isUsePayloadLenMessage(short messageId) {
        return
                messageId == TCPMessageType.TYPE_MEDIA_EX ||
                        messageId == TCPMessageType.TYPE_MEDIA_EX_FILE_FRAME ||
                        messageId == TCPMessageType.SOS_MEDIA_EX ||

                        messageId == TCPMessageType.TYPE_TOPOC_START_EX ||
                        messageId == TCPMessageType.TYPE_TOPOC_END_EX ||

                        messageId == TCPMessageType.TYPE_P2P_CHAT_TEXT ||
                        messageId == TCPMessageType.TYPE_P2G_CHAT_TEXT ||
                        messageId == TCPMessageType.TYPE_P2P_CHAT_FILE ||
                        messageId == TCPMessageType.TYPE_P2G_CHAT_FILE ||

                        messageId == TCPMessageType.SOS_LOCATION ||
                        messageId == TCPMessageType.OFFLINE_USER_RECORD ||
                        messageId == TCPMessageType.TYPE_TOPOC_START_MIC ||
                        messageId == TCPMessageType.TYPE_TOPOC_REALASE_MIC ||
                        messageId == TCPMessageType.TYPE_CREATE_GROUP ||

                        messageId == TCPMessageType.TYPE_GPS_COMMAND ||
                        messageId == TCPMessageType.TYPE_SHARE_VIDEOLIVE ||
                        messageId == TCPMessageType.RELAY_USER_MESSAGE ||
                        messageId == TCPMessageType.VIDEO_MESSAGE ||

                        messageId == TCPMessageType.TYPE_GROUP_USER_CHANGE ||
                        messageId == TCPMessageType.TYPE_AV_CHAT_NEW ||
                        messageId == TCPMessageType.TYPE_AV_REMOTE_MONI ||

                        messageId == TCPMessageType.USER_RTSTATE_UPDATE ||

                        messageId == TCPMessageType.TYPE_MEET_CHAT ||
                        messageId == TCPMessageType.TYPE_MEET_SCREEN_SHARE ||

                        messageId == TCPMessageType.SOS_SESSION ||
                        messageId == TCPMessageType.TYPE_MODIFY_GROUP ||

                        messageId == TCPMessageType.MEDIA_EX_TOPLATFORM     //这是一个不规范的包，要特殊处理
                ;

    }


    @Override
    public void run() {
        isRunning = true;
        Log.i(TAG, ">>>>>>>>>>>>==================  start TcpReader thread---isRunning:" + isRunning);
        if (PTTSessionManager.getInstance().getmPlayType() == 1) {
            //听筒
        } else {
            //喇叭出声
            if (player == null) {
                // 开启播放线程
                player = new AudioPlayer(mContext);
                player.startPlay();
                player.writeAudio();

                Log.i(TAG, "new AudioPlayer...");
            }

        }
        //
        PTTSessionManager.usedDecodedQueue2.clear();
        byte[] netStream = new byte[Config.RCV_BUF_LEN];
        while (isRunning) {
            int netLen = 0;
            try {
                DataInputStream in = tcpClient.getInputStream();
                try {
                    if (in != null) {
                        Log.d(TAG, "start TcpReader thread TcpWrapper.in.available() :" + in.available());
                        netLen = in.read(netStream);  //取一包
                        Log.d(TAG, "TcpReader thread read netLen :" + netLen);
                        if (netLen == -1) {
                            Socket socket = tcpClient.getSocket();
                            if (socket != null) {
                                if (socket.isConnected())
                                    Log.d(TAG, "TcpWrapper.socket.isConnected()");
                                if (!socket.isClosed())
                                    Log.d(TAG, "TcpWrapper.socket.isClosed() NO");
                                if (socket.isBound())
                                    Log.d(TAG, "TcpWrapper.socket.isBound()");
                            }

                            if (isRunning)
                                Thread.sleep(10);
                        }
                    } else {
                        Log.e(TAG, "################### TcpReader TcpWrapper.in is null ##############################");
                        tcpClient.emitSocketDisconnectEvent(); //触发断开事件回调
                        tcpClient.cleanupSocket();  //清除方便重连
                        break;
                    }
                } catch (SocketException e) {
                    Log.e(TAG, "TcpReader thread -- SocketException:");
                    Log.e(TAG, Log.getStackTraceString(e));
                    Log.e(TAG, "TcpReader thread -- SocketException reconnectSocket:");
                    //在 PTTTcpClient类中用定时器去探测发起重连接
                    tcpClient.emitSocketDisconnectEvent(); //触发断开事件回调
                    tcpClient.cleanupSocket();  //清除方便重连
                    break;
                } catch (IOException e) {
                    Log.e(TAG, "TcpReader thread 1.-- IOException:");
                    Log.e(TAG, Log.getStackTraceString(e));
                    tcpClient.emitSocketDisconnectEvent(); //触发断开事件回调
                    tcpClient.cleanupSocket();  //清除方便重连
                }

                // leftBuffer为这次的真实字节流，包括上次包剩下的
                if (netLen < 0) {
                    Log.e(TAG, "-------the network stream length is -1");
                    continue;
                }

                //每次将剩余的与刚读取的进行合并
                leftBuffer = getRealStream(netLen, netStream);

                if (leftBuffer.length < Config.MSG_HEADER_LEN) {
                    Log.e(TAG, "00. incomplete msg!!!!!!!!!!!!!!!!!!");
                    continue;
                }

                //第3个字节，有些消息是用第3个字节表示payload长度，具体请参考mypoc socket协议文档
                int msgLen = (leftBuffer[2] < 0 ? ((int) leftBuffer[2] + 256) : ((int) leftBuffer[2]));

                if (leftBuffer.length < Config.MSG_HEADER_LEN + msgLen) {
                    Log.e(TAG, "01. incomplete msg!!!!!!!!!!!!!!!!!!");
                    continue;
                }

                int leftLen = leftBuffer.length;
                Log.d(TAG, "leftLen" + leftLen);

                //剩余字节数组长度至少要大于或等于 Config.MSG_HEADER_LEN
                while (leftLen >= Config.MSG_HEADER_LEN) {

                    // 取得MessageId
                    short messageId = TextUtil.bytesToShort(leftBuffer, 0, 2);
                    Log.d(TAG, "recv msgtype:" + messageId + " leftBuffer.length:" + leftBuffer.length
                            + "内容: " + TextUtil.bytesToIntString(leftBuffer));

                    Log.e(TAG, "messageid - " + messageId);

                    //下面全是 length=0 即第三个字节为0, 然后第4，5为short类型的msglen
                    if (isUsePayloadLenMessage(messageId)) {

                        if (leftLen < 5) {
                            Log.e(TAG, "是拆包，incomplete msg!!!!!!!!!!!!!!!!!!");
                            break; //跳到下一次取包
                        }

                        msgLen = TextUtil.bytesToShort(leftBuffer, 3, 2);  //PAYLOAD 长度，含自身2个字节

                        ////////这里要注意，messageId=100的是个不规范包，后续修改它
                        if (messageId== TCPMessageType.MEDIA_EX_TOPLATFORM ){
                            msgLen= msgLen+8;   //要另外加一个8
                        }

                        Log.i(TAG, "messageid - " + messageId + " / msgLen - " + msgLen);

                        if (leftLen < Config.MSG_HEADER_LEN + msgLen) {
                            Log.e(TAG, "11. incomplete msg!!!!!!!!!!!!!!!!!!");
                            break; //跳到下一次取包
                        }

                        byte[] msgBytes = new byte[Config.MSG_HEADER_LEN + msgLen];
                        System.arraycopy(leftBuffer, 0, msgBytes, 0, Config.MSG_HEADER_LEN + msgLen);

                        //余下未处理的字节数组
                        leftLen = leftLen - Config.MSG_HEADER_LEN - msgLen;
                        byte[] leftBytes = new byte[leftLen];
                        System.arraycopy(leftBuffer, Config.MSG_HEADER_LEN + msgLen, leftBytes, 0, leftLen);

                        // 重新刷新剩下的流
                        leftBuffer = leftBytes;
                        Log.i(TAG, "....msgLen:" + msgLen + " rest leftLen:" + leftLen);

                        //回调ptttcpclient的处理
                        tcpClient.processMessageIncludePayloadLen(messageId, msgBytes);

                    } else {
                        //这些消息是第三个字节表示长度，如果没有payload 则第三个字节为0， 如果有payload 但长度小于256，也是用第三个字节表示的
                        msgLen = (leftBuffer[2] < 0 ? ((int) leftBuffer[2] + 256) : ((int) leftBuffer[2]));
                        Log.d(TAG, "....msgLen......0:" + msgLen + " leftLen:" + leftLen);

                        if (leftLen < Config.MSG_HEADER_LEN + msgLen) {
                            Log.e(TAG, "20. incomplete msg!!!!!!!!!!!!!!!!!!");
                            break;  //跳到下一次取包
                        }

                        // 得到Message消息，包括头部
                        byte[] msgBytes = new byte[Config.MSG_HEADER_LEN + msgLen];

                        try {
                            System.arraycopy(leftBuffer, 0, msgBytes, 0, Config.MSG_HEADER_LEN + msgLen);
                        } catch (Exception e) {
                            Log.e(TAG, "------ else -- 1 -- Exception :" + e);
                            break;
                        }

                        // 获取剩下的长度
                        leftLen = leftLen - Config.MSG_HEADER_LEN - msgLen;
                        Log.d(TAG, "....msgLen......1:" + msgLen + " leftLen:" + leftLen);
                        byte[] leftBytes = new byte[leftLen];

                        try {
                            System.arraycopy(leftBuffer, Config.MSG_HEADER_LEN + msgLen, leftBytes, 0, leftLen);
                        } catch (Exception e) {
                            Log.e(TAG, "------ else -- 2 -- Exception :" + e);
                            break;
                        }

                        // 重新刷新剩下的流
                        leftBuffer = leftBytes;
                        Log.d(TAG, "....msgLen......2:" + msgLen + " leftLen:" + leftLen);

                        //消息处理
                        tcpClient.processMessageExcludePayloadLen(messageId, msgBytes);
                    }

                }
            } catch (Exception e) {
                if (isRunning) {
                    Log.e(TAG, "---------finish-Exception--- :" + Log.getStackTraceString(e));
                    tcpClient.emitSocketDisconnectEvent(); //触发断开事件回调
                    tcpClient.cleanupSocket();
                    break;
                }
            }
        }

        Log.e(TAG, "------ out off while ---------");
    }
}
