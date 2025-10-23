package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

import java.nio.charset.Charset;

public class GroupUserChangeMessage {
    private short messageId;//2 byte
    private byte length;//1 byte
    private short payloadLen;  //2 byte, payload的长度

    private int groupId;  //变动的组ID
    /**
     * 0: 表示固定组, 1:表示临时组
     */
    private int groupTypeId;
    /**
     * 0: 表示强拆, 1: 表示强插
     */
    private int changeType;
    /**
     * 目标用户userid的内容(多个userid以, (半角逗号)隔开)
     */
    private String useridlist;

    private static final short MESSAGE_ID = TCPMessageType.TYPE_GROUP_USER_CHANGE;
    public static final Charset UTF_8 = Charset.forName("UTF-8");

    public GroupUserChangeMessage(short messageId, byte length, short payloadLen, int groupId, int groupTypeId, int changeType, String useridlist) {
        this.messageId = messageId;
        this.length = length;
        this.payloadLen = payloadLen;
        this.groupId = groupId;
        this.groupTypeId = groupTypeId;
        this.changeType = changeType;
        this.useridlist = useridlist;
    }

    public static GroupUserChangeMessage parseBytes(byte[] msgBytes){
        short payloadLen  = (short) (TextUtil.bytesToShort( msgBytes, 3, 2 ) - 2); //减去第4，5两个字节的本身长度
        if( payloadLen <= 0 )
        {
            return null;
        }
        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];
        int  groupId = TextUtil.bytesToInt(msgBytes, 5, 4);
        int  groupTypeId= TextUtil.bytesToInt(msgBytes, 9, 4);
        int  changeType = TextUtil.bytesToInt(msgBytes, 13, 4);

        int restLen= (short)(payloadLen-12);
        byte[] bytes     = new byte[ restLen ];
        System.arraycopy( msgBytes, 17, bytes, 0, restLen );
        String useridlist  = new String(bytes, UTF_8);
        return new GroupUserChangeMessage(messageId,messageLen,payloadLen,groupId,groupTypeId,changeType,useridlist);

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

    public short getPayloadLen() {
        return payloadLen;
    }

    public void setPayloadLen(short payloadLen) {
        this.payloadLen = payloadLen;
    }

    public int getGroupId() {
        return groupId;
    }

    public void setGroupId(int groupId) {
        this.groupId = groupId;
    }

    public int getGroupTypeId() {
        return groupTypeId;
    }

    public void setGroupTypeId(int groupTypeId) {
        this.groupTypeId = groupTypeId;
    }

    public int getChangeType() {
        return changeType;
    }

    public void setChangeType(int changeType) {
        this.changeType = changeType;
    }

    public String getUseridlist() {
        return useridlist;
    }

    public void setUseridlist(String useridlist) {
        this.useridlist = useridlist;
    }

    @Override
    public String toString() {
        return "GroupUserChangeMessage{" +
                "messageId=" + messageId +
                ", length=" + length +
                ", payloadLen=" + payloadLen +
                ", groupId=" + groupId +
                ", groupTypeId=" + groupTypeId +
                ", changeType=" + changeType +
                ", useridlist='" + useridlist + '\'' +
                '}';
    }
}
