package com.mypoc.pttlibrary.internal;

/**
 * 广播类型
 */
public class MyBroadCast {

    //以下是 pttlibrary 中用到的广播通知
    public final static String BROADCAST_MIC_SUCCESS = "hr.android.ptt.broadcast.mic.success";
    public final static String BROADCAST_MIC_FAILED = "hr.android.ptt.broadcast.mic.failed";
    public final static String BROADCAST_KICK_OFF = "hr.android.ptt.broadcast.kickoff";

    public final static String BROADCAST_MIC_BREAKED = "hr.android.ptt.broadcast.mic.breaked"; //麦被打断
    public final static String BROADCAST_MIC_TIMEOUT = "hr.android.ptt.broadcast.mic.timeout"; //麦被超时


    //申请抢麦广播
    //public final static String BROADCAST_ROB_MIC_SPEAK = "hr.android.ptt.broadcast.speak";
    //释放麦停止说话广播
    //public final static String BROADCAST_ROB_MIC_STOPSPEAK = "hr.android.ptt.broadcast.stopspeak";

}
