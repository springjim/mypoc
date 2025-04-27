package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

import java.nio.charset.Charset;

public class GroupSyncMessage {
    private short messageId;//2 byte
    private byte length;//1 byte

    private byte groupNameLength;  //组名称长度
    private String groupName;      //组名称，一般不要求超过200个字节

    private int groupId;//4 byte
    private int userId;//4 byte  被邀请人的用户ID
    private int inviteId;  //邀请人的用户ID

    private static final short MESSAGE_ID = TCPMessageType.TYPE_GROUP_SYNC;
    private static final Charset UTF_8 = Charset.forName("UTF-8");

    public static GroupSyncMessage parseBytes(byte[] msgBytes){

        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];
        int  groupId = TextUtil.bytesToInt(msgBytes, 3, 4);
        byte groupNameLength= msgBytes[7];

        byte[] bytes     = new byte[ groupNameLength ];

        System.arraycopy( msgBytes, 8, bytes, 0, groupNameLength );
        String groupName  = new String(bytes, UTF_8);

        int  userId = TextUtil.bytesToInt(msgBytes, 8+groupNameLength, 4);
        int  inviteId = TextUtil.bytesToInt(msgBytes, 12+groupNameLength, 4);
        return new GroupSyncMessage(messageId,messageLen,groupNameLength,groupName,groupId,userId,inviteId);

    }

    public GroupSyncMessage(short messageId, byte length, byte groupNameLength, String groupName, int groupId, int userId, int inviteId) {
        this.messageId = messageId;
        this.length = length;
        this.groupNameLength = groupNameLength;
        this.groupName = groupName;
        this.groupId = groupId;
        this.userId = userId;
        this.inviteId = inviteId;
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

    public byte getGroupNameLength() {
        return groupNameLength;
    }

    public void setGroupNameLength(byte groupNameLength) {
        this.groupNameLength = groupNameLength;
    }

    public String getGroupName() {
        return groupName;
    }

    public void setGroupName(String groupName) {
        this.groupName = groupName;
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

    public int getInviteId() {
        return inviteId;
    }

    public void setInviteId(int inviteId) {
        this.inviteId = inviteId;
    }

    @Override
    public String toString() {
        return "GroupSyncMessage{" +
                "messageId=" + messageId +
                ", length=" + length +
                ", groupNameLength=" + groupNameLength +
                ", groupName='" + groupName + '\'' +
                ", groupId=" + groupId +
                ", userId=" + userId +
                ", inviteId=" + inviteId +
                '}';
    }
}
