package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

public class LoginMessage {

    private short messageId;//2 byte
    private byte length;//1 byte
    private int groupId;//4 byte
    private int userId;//4 byte
    private static final int TOTAL_LENGTH = Config.MSG_HEADER_LEN+8;
    private static final short MESSAGE_ID = TCPMessageType.TYPE_LOGIN;

    public LoginMessage(short messageId, byte length, int groupId, int userId) {
        this.messageId = messageId;
        this.length = length;
        this.groupId = groupId;
        this.userId = userId;
    }

    /**
     * 解析
     * @param msgBytes
     * @return
     */
    public static LoginMessage parseBytes(byte[] msgBytes){
        if (msgBytes == null || msgBytes.length < TOTAL_LENGTH) {
            throw new IllegalArgumentException("Invalid message bytes");
        }
        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];
        int groupId = TextUtil.bytesToInt(msgBytes, 3, 4);
        int userId = TextUtil.bytesToInt(msgBytes, 7, 4);

        return new LoginMessage(messageId,messageLen,groupId,userId);
    }

    /**
     * 构建消息
     * @param groupId
     * @param userId
     * @return
     */
    public static byte[] buildMessage(int groupId,int userId){
        byte[] packet= new byte[Config.MSG_HEADER_LEN+8];
        System.arraycopy(TextUtil.getBytes(MESSAGE_ID), 0, packet, 0, 2);
        packet[2]= (byte)8;
        System.arraycopy(TextUtil.getBytes(groupId), 0, packet, 3, 4);
        System.arraycopy(TextUtil.getBytes(userId), 0, packet, 7, 4);
        return packet;
    }

    @Override
    public String toString() {
        return "LoginMessage{" +
                "messageId=" + messageId +
                ", length=" + length +
                ", groupId=" + groupId +
                ", userId=" + userId +
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

    public int getUserId() {
        return userId;
    }

    public void setUserId(int userId) {
        this.userId = userId;
    }
}
