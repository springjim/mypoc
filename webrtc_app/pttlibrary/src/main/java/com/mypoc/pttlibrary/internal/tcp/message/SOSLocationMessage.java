package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;

import java.nio.ByteBuffer;
import java.nio.charset.Charset;

/**
 * 在SOS会话期间发送的定位消息
 */
public class SOSLocationMessage {
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
     * SOS类型, 报警级别,1表示SOS, 2 表示其它, 3表示是调度台发给POC
     */
    private byte sos_type;
    /**
     * SOS报警时间,秒数
     */
    private int sos_datetime;

    /**
     * 经度
     */
    private double longitude;

    /**
     * 纬度
     */
    private double latitude;

    /**
     * 定位类型: 如 baidu,gps,Google
     */
    private String gps_type;

    /**
     * SOS会话的ID，32位的guid字符串
     */
    private String session_id;

    public static final Charset UTF_8 = Charset.forName("UTF-8");
    private static final short MESSAGE_ID = TCPMessageType.SOS_LOCATION;

    public static byte[] buildMessage(int groupId,int userId,byte sosType,double longitude,double latitude,String sessionIdStr,String gpsType){

        short itemlen = (short) gpsType.getBytes(UTF_8).length;
        ByteBuffer buffer = ByteBuffer.allocate(Config.MSG_HEADER_LEN+2 + 13 + 8 + 8 + 2 +32+ itemlen);
        buffer.putShort(MESSAGE_ID);
        buffer.put((byte)0);

        buffer.putShort((short) (2 + 13 + 8 + 8 + 2 +32+  itemlen));
        buffer.putInt(groupId);
        buffer.putInt(userId);
        buffer.put(sosType);
        buffer.putInt((int)(System.currentTimeMillis()/1000));
        buffer.putDouble(longitude);
        buffer.putDouble(latitude);
        buffer.put(sessionIdStr.getBytes(UTF_8));

        buffer.putShort(itemlen);
        if (itemlen>0)
            buffer.put(gpsType.getBytes(UTF_8));

        // 使用flip()方法切换为读模式（与IoBuffer相同）
        buffer.flip();
        // 创建正确大小的byte数组
        byte[] result = new byte[buffer.remaining()];
        buffer.get(result);
        return result;
    }

}
