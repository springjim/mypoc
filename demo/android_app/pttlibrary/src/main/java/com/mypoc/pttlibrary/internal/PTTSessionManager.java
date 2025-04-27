package com.mypoc.pttlibrary.internal;

import java.nio.ByteBuffer;
import java.util.concurrent.ArrayBlockingQueue;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.ConcurrentLinkedQueue;
import java.util.concurrent.LinkedBlockingDeque;
import java.util.concurrent.TimeUnit;

/**
 * 对PTT库的所有会话变量进行管理
 */
public class PTTSessionManager {

    /**
     * 播放类型  0:喇叭出声  1:听筒出声
     */
    private   int mPlayType = 0;

    /**
     * 是否是来自文件的广播帧  1：是的，不用再解码， 0: 是麦克风的语音帧,要解码
     */
    public  int mMEDIA_EX_FILE_FRAME=0;

    /**
     * 是否在播放语音状态
     */
    private  boolean isPlayAudio = true;

    /**
     * true: 表示开始采集语音  false: 表示停止
     */
    public volatile static boolean recording = false;

    /**
     * 当前登录用户
     */
    private int userId=-1;

    /**
     * 当前登录的对讲组
     */
    private int groupId=-1;
    /**
     * 待发送的语音包，且是未编码的包, 不能用 LinkedBlockingDeque 因容量大造成延迟播放
     */
    public static BlockingQueue<ByteBuffer> recordedQueue2 = new LinkedBlockingDeque<ByteBuffer>();  // 是webrtc的aec给出的值

    /**
     * 存放在AEC期间从服务收到的未解码的原始帧容器 todo
     */
    public static ConcurrentLinkedQueue<ByteBuffer> encodedFrmServerForAec= new ConcurrentLinkedQueue<>();

    public static BlockingQueue<short[]> freeQueue2= new ArrayBlockingQueue<>(20); //用于回音处理 // 是webrtc的aec给出的值

    public static short[] Producer_Get(long timeOut) throws InterruptedException {
        return freeQueue2.poll(timeOut, TimeUnit.MILLISECONDS);
    }

    public static void Consumer_Put(short[] e) throws InterruptedException {
        freeQueue2.offer(e);  //用put容易造成堵塞
    }

    /**
     * 从服务器收到的包,等待解码，即amrnb -> pcm
     */
    public static BlockingQueue<ByteBuffer> usedDecodedQueue2 = new ArrayBlockingQueue<ByteBuffer>(20); // 是webrtc的aec给出的值

    public static ByteBuffer  Consumer_Get(long timeOut) throws InterruptedException {
        return usedDecodedQueue2.poll(timeOut, TimeUnit.MILLISECONDS);
    }

    public static void Producer_Put(ByteBuffer e) throws InterruptedException {
        usedDecodedQueue2.put(e);
    }
    public static int UsedSize(){
        return usedDecodedQueue2.size();
    }

    private static PTTSessionManager instance;
    private PTTSessionManager() {
        // 私有构造函数
    }

    public static synchronized PTTSessionManager getInstance() {
        if (instance == null) {
            instance = new PTTSessionManager();
        }
        return instance;
    }

    public int getmPlayType() {
        return mPlayType;
    }

    public void setmPlayType(int mPlayType) {
        this.mPlayType = mPlayType;
    }

    public int getmMEDIA_EX_FILE_FRAME() {
        return mMEDIA_EX_FILE_FRAME;
    }

    public void setmMEDIA_EX_FILE_FRAME(int mMEDIA_EX_FILE_FRAME) {
        this.mMEDIA_EX_FILE_FRAME = mMEDIA_EX_FILE_FRAME;
    }

    public boolean isPlayAudio() {
        return isPlayAudio;
    }

    public void setPlayAudio(boolean playAudio) {
        isPlayAudio = playAudio;
        if (isPlayAudio){
            encodedFrmServerForAec.clear();
        }
    }

    public int getUserId() {
        return userId;
    }

    public void setUserId(int userId) {
        this.userId = userId;
    }

    public int getGroupId() {
        return groupId;
    }

    public void setGroupId(int groupId) {
        this.groupId = groupId;
    }
}
