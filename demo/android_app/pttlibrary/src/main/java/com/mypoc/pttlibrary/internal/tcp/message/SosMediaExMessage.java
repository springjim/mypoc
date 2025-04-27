package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

/**
 * sos语音包
 */
public class SosMediaExMessage {
    private short messageId;//2 byte
    private byte length;//1 byte
    private byte[] media;   //经amrnb编码后的一帧数据包
    /**
     * 转发模式，0 表示转发给调度台,如果一个企业号内有多个调度台登入，即都转发， 1 表示转发到当前群组内所有人,除了自己
     */
    private byte transfer_mode;
    /**
     * 表示发起端: 0 表示 是 poc 端发起来的 sos 语音包， 1 表示的是调度端发起的 sos 语音包
     */
    private byte endpoint_type;
    /**
     * sos 键按下时所在的当前群组
     */
    private int groupId;//4 byte
    /**
     * 用户帐号 ID,
     * 1) 调度台发往 poc 的, userid 一定要写成目标用户的 userid
     * 2) 是 poc 端发起来的 sos 语音包时,userid 为发起方的 userid
     */
    private int userId;//4 byte

    private static final short MESSAGE_ID = TCPMessageType.SOS_MEDIA_EX;

    public static byte[] buildMessage(int groupId,int userId,byte[] media){
        byte[] packet= new byte[Config.MSG_HEADER_LEN+ media.length+12 ];
        System.arraycopy(TextUtil.getBytes(MESSAGE_ID), 0, packet, 0, 2);
        packet[2]= (byte)0;
        System.arraycopy(TextUtil.getBytes((short)(media.length+12)), 0, packet, 3, 2); //写payload长度
        int mediaLen= media.length;
        System.arraycopy(media,0,packet,5,media.length);
        int index = 5 + mediaLen;
        packet[index]= (byte)0;
        index+=1;
        packet[index]= (byte)0;
        index+=1;
        System.arraycopy(TextUtil.getBytes(groupId),0,packet,index,4);
        index+=4;
        System.arraycopy(TextUtil.getBytes(userId),0,packet,index,4);
        return packet;
    }


}
