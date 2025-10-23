package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

import java.nio.ByteBuffer;
import java.nio.charset.Charset;

public class AVChatNewMessage {
    private short messageId;//2 byte
    private byte length;//1 byte
    private short payloadLen;  //2 byte, payload的长度

    /**
     * 视频类型
     * 1、语音通话
     * 2、视频通话
     * 3、视频直播或监控 (包括监控，因为监控的原理本质上也是直播)
     */
    private short     video_type;

    /**
     * 视频命令
     * 1: 请求
     * 2: 应答
     * 3：拒绝
     * 4: 挂断 (含已连接时挂断, 未连接时挂断, 不管是主叫方还是被叫方都是这个挂断 )
     * 5: 对方占线(正在使用中)
     * 6: 切换前后摄像头
     */
    private short     video_command;
    private int       fromUserId;
    private int       toUserId;
    private String    fromUserName;
    private String    toUserName;
    private String    desc;

    public static final Charset UTF_8 = Charset.forName("UTF-8");
    private static final short MESSAGE_ID = TCPMessageType.TYPE_AV_CHAT_NEW;

    public AVChatNewMessage(short messageId, byte length, short payloadLen, short video_type, short video_command,
                            int fromUserId, int toUserId, String fromUserName, String toUserName,String desc) {
        this.messageId = messageId;
        this.length = length;
        this.payloadLen = payloadLen;
        this.video_type = video_type;
        this.video_command = video_command;
        this.fromUserId = fromUserId;
        this.toUserId = toUserId;
        this.fromUserName = fromUserName;
        this.toUserName = toUserName;
        this.desc=desc;
    }

    public static AVChatNewMessage parseBytes(byte[] msgBytes){
        short payloadLen  = (short) (TextUtil.bytesToShort( msgBytes, 3, 2 ) - 2); //减去第4，5两个字节的本身长度
        if( payloadLen <= 0 )
        {
            return null;
        }

        // 将byte[]包装为ByteBuffer，设置为读模式
        ByteBuffer buffer = ByteBuffer.wrap(msgBytes);

        short messageId  = buffer.getShort();
        byte  length = buffer.get();
        payloadLen= buffer.getShort();

        short video_type=buffer.getShort();
        short video_command=buffer.getShort();
        int   fromUserId= buffer.getInt();
        int   toUserId= buffer.getInt();

        short len = 0;
        byte[] bytes = null;
        String fromUserName="";
        String toUserName="";
        String desc="";

        len = buffer.getShort();
        if (len>0) {
            bytes = new byte[len];
            buffer.get(bytes);
            fromUserName = new String(bytes, UTF_8);
        } else fromUserName="";

        len = buffer.getShort();
        if (len>0) {
            bytes = new byte[len];
            buffer.get(bytes);
            toUserName = new String(bytes, UTF_8);
        } else toUserName="";

        len = buffer.getShort();
        if (len>0) {
            bytes = new byte[len];
            buffer.get(bytes);
            desc = new String(bytes, UTF_8);
        } else desc="";

        return new AVChatNewMessage(messageId, length,payloadLen,video_type,video_command,fromUserId,toUserId,
                fromUserName,toUserName,desc);

    }


    public static byte[] buildMessage(short  videoType,short videoCommand,int  fromUserId,
                                      int toUserId,String fromUserName,String toUserName,String desc){
        short itemlen = 0;
        int msglen  = (2 + 2 + 2 + 4 + 4);
        itemlen = (short) fromUserName.getBytes(UTF_8).length;
        msglen += (2+itemlen);

        itemlen = (short) toUserName.getBytes(UTF_8).length;
        msglen += (2+itemlen);

        itemlen = (short) desc.getBytes(UTF_8).length;
        msglen += (2+itemlen);

        // 使用ByteBuffer.allocate()替代IoBuffer.allocate()
        ByteBuffer buffer = ByteBuffer.allocate(Config.MSG_HEADER_LEN+msglen);
        buffer.putShort(MESSAGE_ID);
        buffer.put((byte)0);

        buffer.putShort((short) msglen);
        buffer.putShort(videoType);
        buffer.putShort(videoCommand);
        buffer.putInt(fromUserId);
        buffer.putInt(toUserId);

        itemlen = (short) fromUserName.getBytes(UTF_8).length;
        buffer.putShort(itemlen);
        if (itemlen > 0)
            buffer.put(fromUserName.getBytes(UTF_8));

        itemlen = (short) toUserName.getBytes(UTF_8).length;
        buffer.putShort(itemlen);
        if (itemlen > 0)
            buffer.put(toUserName.getBytes(UTF_8));

        itemlen = (short) desc.getBytes(UTF_8).length;
        buffer.putShort(itemlen);
        if (itemlen > 0)
            buffer.put(desc.getBytes(UTF_8));

        // 使用flip()方法切换为读模式（与IoBuffer相同）
        buffer.flip();
        // 创建正确大小的byte数组
        byte[] result = new byte[buffer.remaining()];
        buffer.get(result);
        return result;
    }

    @Override
    public String toString() {
        return "AVChatNewMessage{" +
                "messageId=" + messageId +
                ", length=" + length +
                ", payloadLen=" + payloadLen +
                ", video_type=" + video_type +
                ", video_command=" + video_command +
                ", fromUserId=" + fromUserId +
                ", toUserId=" + toUserId +
                ", fromUserName='" + fromUserName + '\'' +
                ", toUserName='" + toUserName + '\'' +
                ", desc='" + desc + '\'' +
                '}';
    }

    public short getVideo_type() {
        return video_type;
    }

    public void setVideo_type(short video_type) {
        this.video_type = video_type;
    }

    public short getVideo_command() {
        return video_command;
    }

    public void setVideo_command(short video_command) {
        this.video_command = video_command;
    }

    public int getFromUserId() {
        return fromUserId;
    }

    public void setFromUserId(int fromUserId) {
        this.fromUserId = fromUserId;
    }

    public int getToUserId() {
        return toUserId;
    }

    public void setToUserId(int toUserId) {
        this.toUserId = toUserId;
    }

    public String getFromUserName() {
        return fromUserName;
    }

    public void setFromUserName(String fromUserName) {
        this.fromUserName = fromUserName;
    }

    public String getToUserName() {
        return toUserName;
    }

    public void setToUserName(String toUserName) {
        this.toUserName = toUserName;
    }

    public String getDesc() {
        return desc;
    }

    public void setDesc(String desc) {
        this.desc = desc;
    }
}
