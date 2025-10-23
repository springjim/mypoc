package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

/**
 * 用于发送心跳
 */
public class CheckServerMessage {
    private short messageId;//2 byte
    private byte length;//1 byte
    private static final short MESSAGE_ID = TCPMessageType.TYPE_CHECK_SERVER;

    public static byte[] buildMessage(){
        byte[] packet= new byte[Config.MSG_HEADER_LEN];
        System.arraycopy(TextUtil.getBytes(MESSAGE_ID), 0, packet, 0, 2);
        packet[2]= (byte)0;
        return packet;
    }
}
