package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;

import java.nio.ByteBuffer;
import java.nio.charset.Charset;

/**
 * SOS会话消息，它表示一个包含: sos推流、sos定位的会活
 */
public class SOSSessionMessage {
    private short messageId;//2 byte
    private byte length;//1 byte
    private short msglen;
    /**
     * 当前群组Id
     */
    private int groupId;

    /**
     * 请求的终端用户ID
     */
    private int userId;
    /**
     * SOS会话ID，32位GUID去除了分隔符 - 后的小写字符串，如 d22a4fa72e4042a79fe62a3a0e4be2a8
     */
    /**
     * sessionId的字符串形式，SOS会话ID，32位GUID去除了分隔符 - 后的小写字符串，如 d22a4fa72e4042a79fe62a3a0e4be2a8
     */
    private String sessionIdStr="";
    /**
     * SOS报警时间,秒数
     */
    private int sos_datetime;

    /**
     * 1 : 开始， 2：结束
     */
    private byte state;

    public static final Charset UTF_8 = Charset.forName("UTF-8");
    private static final short MESSAGE_ID = TCPMessageType.SOS_SESSION;

    public static byte[] buildMessage(int groupId,int userId,String sessionIdStr,byte state){
        short msglen=(short) (2 + 1 + 4 + 32 + 8);
        ByteBuffer buffer = ByteBuffer.allocate(Config.MSG_HEADER_LEN+msglen);
        buffer.putShort(MESSAGE_ID);
        buffer.put((byte)0);

        buffer.putShort((short) msglen);
        buffer.putInt(groupId);
        buffer.putInt(userId);
        buffer.put(sessionIdStr.getBytes(UTF_8));
        buffer.putInt((int)(System.currentTimeMillis()/1000));
        buffer.put(state);

        // 使用flip()方法切换为读模式（与IoBuffer相同）
        buffer.flip();
        // 创建正确大小的byte数组
        byte[] result = new byte[buffer.remaining()];
        buffer.get(result);
        return result;

    }


}
