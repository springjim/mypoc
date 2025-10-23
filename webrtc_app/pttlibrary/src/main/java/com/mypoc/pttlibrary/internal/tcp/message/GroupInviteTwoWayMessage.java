package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

import java.nio.charset.Charset;

/**
 * 收到别人建组的邀请, 进行双向语音通话，不用抢麦
 */
public class GroupInviteTwoWayMessage {
    private short messageId;//2 byte
    private byte length;//1 byte

    private int groupId; //创建的组ID
    private byte groupNameLen;  //组名称的长度，不能超过255个字节
    private String groupName;   //组名称
    /**
     * 被邀请人的ID
     */
    private int userId;
    /**
     * 发出邀请的人的ID
     */
    private int inviteId;

    /**
     * 模式， 1： 单呼双向语音， 2：群呼（多于2个人）的双向语音，即语音会议， 其它再扩展...
      */
    private int  mode;

    private static final short MESSAGE_ID = TCPMessageType.TYPE_RECIVED_INVITE_TWOWAY;
    public static final Charset UTF_8 = Charset.forName("UTF-8");

    public GroupInviteTwoWayMessage(short messageId, byte length, int groupId, byte groupNameLen, String groupName, int userId, int inviteId,
                                    int mode) {
        this.messageId = messageId;
        this.length = length;
        this.groupId = groupId;
        this.groupNameLen = groupNameLen;
        this.groupName = groupName;
        this.userId = userId;
        this.inviteId = inviteId;
        this.mode = mode;
    }

    public static GroupInviteTwoWayMessage parseBytes(byte[] msgBytes){

        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];

        int  groupId = TextUtil.bytesToInt(msgBytes, 3, 4);
        byte groupNameLen= msgBytes[7];
        byte[] bytes = new byte[groupNameLen];
        System.arraycopy(msgBytes,8,bytes,0,groupNameLen);
        String groupName = new String(bytes, UTF_8);
        int userId = TextUtil.bytesToInt(msgBytes, 8+groupNameLen, 4);
        int inviteId= TextUtil.bytesToInt(msgBytes, 12+groupNameLen, 4);
        int mode = TextUtil.bytesToInt(msgBytes, 16+groupNameLen, 4);

        return new GroupInviteTwoWayMessage(messageId,messageLen,groupId,groupNameLen,groupName,userId,inviteId,mode);
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

    public int getMode() {
        return mode;
    }

    public void setMode(int mode) {
        this.mode = mode;
    }
}
