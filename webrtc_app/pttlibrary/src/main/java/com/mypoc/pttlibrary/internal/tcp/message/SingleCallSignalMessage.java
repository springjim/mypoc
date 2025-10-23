package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

public class SingleCallSignalMessage {

    private short messageId;//2 byte
    private byte length;//1 byte
    private int groupId;     //单呼本质上是一个临时组ID
    private int fromUserId;  //发自方
    private int toUserId;    //接受方

    /**
     * 信令值: 在所有客户端要用以下值来解析处理
     *
     * 0: 呼叫(主叫方到被叫方)
     * 1: 响铃(告知主叫方，被叫方正在响铃中)
     * 2: 被叫方忙线中 (可能正在与另一方单呼中，或者其它业务原因)
     * 3: 被叫方正在国标SIP对讲中，会自动拒绝, SIP国标对讲是指终端接入了28181后与上级平台在sip对讲中
     * 4: 被叫方主动拒绝了
     * 5: 被叫方接听了
     * 6: 主叫方或被叫方主动退出了
     * 99: 被叫方超时未接听...
     */
    private byte signalVal;  //信令值

    private static final int TOTAL_LENGTH = Config.MSG_HEADER_LEN+13;
    private static final short MESSAGE_ID = TCPMessageType.TYPE_SINGLE_CALL_SIGNAL;

    public SingleCallSignalMessage( int groupId, int fromUserId, int toUserId, byte signalVal) {
        this.groupId = groupId;
        this.fromUserId = fromUserId;
        this.toUserId = toUserId;
        this.signalVal = signalVal;
    }

    public static SingleCallSignalMessage  parseBytes(byte[] msgBytes){
        if (msgBytes == null || msgBytes.length < TOTAL_LENGTH) {
            throw new IllegalArgumentException("Invalid message bytes");
        }
        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];
        int groupId = TextUtil.bytesToInt(msgBytes, 3, 4);
        int fromUserId = TextUtil.bytesToInt(msgBytes, 7, 4);
        int  toUserId = TextUtil.bytesToInt(msgBytes, 11, 4);
        byte  signalVal = msgBytes[15];

        return new SingleCallSignalMessage(groupId,fromUserId,toUserId,signalVal);
    }

    public static byte[] buildMessage(int groupId, int fromUserId, int toUserId, byte signalVal){
        byte[] packet= new byte[TOTAL_LENGTH];
        System.arraycopy(TextUtil.getBytes(MESSAGE_ID), 0, packet, 0, 2);
        packet[2]= (byte)13;
        System.arraycopy(TextUtil.getBytes(groupId), 0, packet, 3, 4);
        System.arraycopy(TextUtil.getBytes(fromUserId), 0, packet, 7, 4);
        System.arraycopy(TextUtil.getBytes(toUserId), 0, packet, 11, 4);
        packet[15]= signalVal;

        return packet;
    }

    @Override
    public String toString() {
        return "SingleCallSignalMessage{" +
                "messageId=" + messageId +
                ", length=" + length +
                ", groupId=" + groupId +
                ", fromUserId=" + fromUserId +
                ", toUserId=" + toUserId +
                ", signalVal=" + signalVal +
                '}';
    }

    public short getMessageId() {
        return messageId;
    }

    public void setMessageId(short messageId) {
        this.messageId = messageId;
    }

    public byte getLength() {
        return length;
    }

    public void setLength(byte length) {
        this.length = length;
    }

    public int getGroupId() {
        return groupId;
    }

    public void setGroupId(int groupId) {
        this.groupId = groupId;
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

    public byte getSignalVal() {
        return signalVal;
    }

    public void setSignalVal(byte signalVal) {
        this.signalVal = signalVal;
    }
}
