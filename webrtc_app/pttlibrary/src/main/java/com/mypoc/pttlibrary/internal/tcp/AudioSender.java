package com.mypoc.pttlibrary.internal.tcp;

import android.util.Log;

import com.mypoc.pttlibrary.internal.PTTService;
import com.mypoc.pttlibrary.internal.PTTSessionManager;
import com.mypoc.pttlibrary.internal.audio.AudioRecorder;
import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.network.PTTTcpClient;
import com.mypoc.pttlibrary.internal.tcp.message.MediaExMessage;
import com.mypoc.pttlibrary.internal.tcp.message.SosMediaExMessage;

import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.util.Arrays;

public class AudioSender implements Runnable {

    private volatile boolean active = true;
    private final String TAG = "AudioSender";
    private final PTTTcpClient tcpClient;

    public AudioSender(PTTTcpClient tcpClient) {
        this.tcpClient = tcpClient;
    }

    public void stop() {
        active = false;
        Log.d(TAG, "Stop AudioSender thread......isRunning:" + active);
        try {
            Thread.sleep(1);
        } catch (Exception ex) {
            Log.e(TAG, "Stop AudioSender thread\n" + Log.getStackTraceString(ex));
        }
        ByteBuffer b1 = ByteBuffer.allocate(AudioRecorder.PACKSIZE);
        ByteBuffer b2 = ByteBuffer.allocate(AudioRecorder.PACKSIZE);
        for (int i = 0; i < AudioRecorder.PACKSIZE; ++i) {
            b1.put((byte) 0xFF);
            b2.put((byte) 0xFF);
        }

        try {
            PTTSessionManager.recordedQueue2.put(b1);
            PTTSessionManager.recordedQueue2.put(b2);
        } catch (InterruptedException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }

        Log.d(TAG, "Stop AudioSender thread......MyApplication.decodedQueue2.put");
        try {
            Socket socket = tcpClient.getSocket();
            if (socket != null && socket.isConnected()) {
                if (socket.isConnected() && !socket.isClosed())
                    if (!socket.isOutputShutdown())
                        socket.shutdownOutput();
            }
        } catch (Exception ex) {
            Log.e(TAG, "stop AudioSender thread:\n" + Log.getStackTraceString(ex));
        }

    }

    @Override
    public void run() {

        Log.d(TAG, "run2");
        active = true;
        try {

            PTTSessionManager.recordedQueue2.clear();

            ByteBuffer sb = null;
            int len = 0;
            byte[] packet = null;

            //由于Q5是13个字节，而中瑞科是32个字节, 这里用MaxBytesPerFrame=50, 考虑不同厂商，这里要设一个最大值
            int MaxBytesPerFrame = 100;

            ByteBuffer buffer = ByteBuffer.allocate(MaxBytesPerFrame);

            AudioRecorder.startEcoder();
            while (active) {
                sb = PTTSessionManager.recordedQueue2.take();
                if (!active)
                    break;
                byte[] data = AudioRecorder.getEncoderData(sb.array());  //得到已编码的语音包
                if (data == null) {
                    continue;
                }

                len = data.length;
                Log.e(TAG, "***************** data.length :" + len);

                // 一包一包的发送，同时接收到也是一包一包的接收
                if (PTTService.isSosAlarmModel) {
                    //发送sos告警特殊语音包
                    buffer.put(data, 0, data.length);
                    buffer.flip();
                    if (len > 0) {

                        byte[] media = new byte[len];
                        System.arraycopy(buffer.array(), 0, media, 0, len);
                        buffer.clear();
                        //2024.12.28 重构，不发送SOS语音，改为推送视频流含语音
                        //如果需要，以下不要注释代码
                        byte[] encodeMsg = SosMediaExMessage.buildMessage(PTTSessionManager.getInstance().getGroupId(),
                                PTTSessionManager.getInstance().getUserId(), media);
                        if (tcpClient != null) {
                            tcpClient.sendMessage(encodeMsg);
                        }

                    }
                } else {

                    //非sos语音包
                    buffer.put(data, 0, data.length);
                    buffer.flip();

                    if (len > 0) {
                        //
                        byte[] media = new byte[len];
                        System.arraycopy(buffer.array(), 0, media, 0, len);
                        buffer.clear();
                        Log.i("PTTService", Arrays.toString(packet));  //发送语音包
                        byte[] encodeMsg = MediaExMessage.buildMessage(PTTSessionManager.getInstance().getGroupId(),
                                PTTSessionManager.getInstance().getUserId(), media);
                        if (tcpClient != null) {
                            tcpClient.sendMessage(encodeMsg);
                        }

                    }
                }

            }

            Log.d(TAG, "run2 end");
        } catch (Exception e2) {
            Log.e(TAG, "--------AudioSender Exception 2:\n" + Log.getStackTraceString(e2));
            tcpClient.cleanupSocket();

        } finally {
            Log.e(TAG, "--------AudioSender finally  AudioRecorder.stopEcoder");
            AudioRecorder.stopEcoder();
        }

    }
}
