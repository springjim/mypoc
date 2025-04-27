package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

/**
 * 收到别人解散组的通知
 */
public class GroupDeleteMessage {

    private short messageId;//2 byte
    private byte length;//1 byte

    private int groupId; //解散的组ID
    private static final short MESSAGE_ID = TCPMessageType.TYPE_DELETE_GROUP;

    public GroupDeleteMessage(short messageId, byte length, int groupId) {
        this.messageId = messageId;
        this.length = length;
        this.groupId = groupId;
    }

    public static GroupDeleteMessage parseBytes(byte[] msgBytes){
        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];
        int  groupId = TextUtil.bytesToInt(msgBytes, 3, 4);
        return new GroupDeleteMessage(messageId,messageLen,groupId);

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
}
