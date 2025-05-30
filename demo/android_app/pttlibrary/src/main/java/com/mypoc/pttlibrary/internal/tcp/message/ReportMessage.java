package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

/**
 * 上报工作组
 */
public class ReportMessage {

    private short messageId;//2 byte
    private byte length;//1 byte
    private int groupId;//4 byte
    private int userId;//4 byte
    private static final int TOTAL_LENGTH = Config.MSG_HEADER_LEN+8;
    private static final short MESSAGE_ID = TCPMessageType.TYPE_REPROT;

    public static byte[] buildMessage(int groupId,int userId){
        byte[] packet= new byte[TOTAL_LENGTH];
        System.arraycopy(TextUtil.getBytes(MESSAGE_ID), 0, packet, 0, 2);
        packet[2]= (byte)8;
        System.arraycopy(TextUtil.getBytes(groupId), 0, packet, 3, 4);
        System.arraycopy(TextUtil.getBytes(userId), 0, packet, 7, 4);
        return packet;
    }

}
