package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

import java.nio.charset.Charset;

/**
 * 收到别人建组的邀请
 */
public class GroupInviteMessage {
    private short messageId;//2 byte
    private byte length;//1 byte

    private int groupId; //创建的组ID
    private byte groupNameLen;  //组名称的长度，不能超过255个字节
    private String groupName;   //组名称
    private int userId; //被邀请人的ID
    private int inviteId; //发出邀请的人的ID

    private static final short MESSAGE_ID = TCPMessageType.TYPE_RECIVED_INVITE;
    public static final Charset UTF_8 = Charset.forName("UTF-8");

    public GroupInviteMessage(short messageId, byte length, int groupId, byte groupNameLen, String groupName, int userId, int inviteId) {
        this.messageId = messageId;
        this.length = length;
        this.groupId = groupId;
        this.groupNameLen = groupNameLen;
        this.groupName = groupName;
        this.userId = userId;
        this.inviteId = inviteId;
    }

    public static GroupInviteMessage parseBytes(byte[] msgBytes){

        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];

        int  groupId = TextUtil.bytesToInt(msgBytes, 3, 4);
        byte groupNameLen= msgBytes[7];
        byte[] bytes = new byte[groupNameLen];
        System.arraycopy(msgBytes,8,bytes,0,groupNameLen);
        String groupName = new String(bytes, UTF_8);
        int userId = TextUtil.bytesToInt(msgBytes, 8+groupNameLen, 4);
        int inviteId= TextUtil.bytesToInt(msgBytes, 12+groupNameLen, 4);

        return new GroupInviteMessage(messageId,messageLen,groupId,groupNameLen,groupName,userId,inviteId);
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

    public byte getGroupNameLen() {
        return groupNameLen;
    }

    public void setGroupNameLen(byte groupNameLen) {
        this.groupNameLen = groupNameLen;
    }

    public String getGroupName() {
        return groupName;
    }

    public void setGroupName(String groupName) {
        this.groupName = groupName;
    }

    public int getUserId() {
        return userId;
    }

    public void setUserId(int userId) {
        this.userId = userId;
    }

    public int getInviteId() {
        return inviteId;
    }

    public void setInviteId(int inviteId) {
        this.inviteId = inviteId;
    }
}
