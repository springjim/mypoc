package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

public class SystemReportMessage {

    private short messageId;//2 byte
    private byte length;//1 byte
    private int groupId;//4 byte
    private int userId;//4 byte
    private int type; //4 byte
    private static final int TOTAL_LENGTH = Config.MSG_HEADER_LEN+12;
    private static final short MESSAGE_ID = TCPMessageType.TYPE_SYS_MESSAGE;

    public SystemReportMessage(short messageId, byte length, int groupId, int userId, int state) {
        this.messageId = messageId;
        this.length = length;
        this.groupId = groupId;
        this.userId = userId;
        this.type = state;
    }

    /**
     * 解析
     * @param msgBytes
     * @return
     */
    public static SystemReportMessage parseBytes(byte[] msgBytes){

        if (msgBytes == null || msgBytes.length < TOTAL_LENGTH) {
            throw new IllegalArgumentException("Invalid message bytes");
        }
        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];
        int groupId = TextUtil.bytesToInt(msgBytes, 3, 4);
        int userId = TextUtil.bytesToInt(msgBytes, 7, 4);
        int type= TextUtil.bytesToInt(msgBytes, 11, 4);

        return new SystemReportMessage(messageId,messageLen,groupId,userId,type);
    }

    public final static class MySysMsgType{
        /**某用户开始说话*/
        public final static int SYS_MSSAGE_TALK_START = 1;
        /**某用户停止说话*/
        public final static int SYS_MSSAGE_TALK_STOP = 2;
        /**某用户进入某群组*/
        public final static int SYS_MSSAGE_IN_GROUP =3;
        /**某用户离开某群组*/
        public final static int SYS_MSSAGE_OUT_GROUP =4;
        /**某用户拒绝邀请*/
        public final static int SYS_MSSAGE_REJECT_INVITE =5;
//    	/**某用户邀请单聊*/
//    	public final static int SYS_MSSAGE_PERSON_INVITE =6;
        /**某用户同意单聊邀请*/
        public final static int SYS_MSSAGE_ENTER_PRESON =6;
        /**某用户拒绝单聊邀请*/
        public final static int SYS_MSSAGE_EXIT_PRESON =7;

        /**某用户上线*/
        public final static int SYS_MSSAGE_ONLINE_PRESON =8;
        /**某用户掉线*/
        public final static int SYS_MSSAGE_OFFLINE_PRESON =9;

        /**转POC某用户开始说话*/
        public final static int SYS_MSSAGE_TALK_START_TOPOC = 10;
        /**转POC某用户停止说话*/
        public final static int SYS_MSSAGE_TALK_STOP_TOPOC = 11;

        /**呼叫中对方正在通话*/
        public final static int SYS_MSSAGE_TALK_INCALL = 12;


        public static final int TYPE_TOPOC_START_MIC = 16;//申请中继台成功
        public static final int TYPE_TOPOC_FAIL_MIC = 17;//申请中继台失败或释放中继台

        public static final int TYPE_TOPOC_RELEASE_SUCCESS_MIC = 18;//申请中继台成功
        public static final int TYPE_TOPOC_RELEASE_FAIL_MIC = 19;//申请中继台失败或释放中继台

    }

    @Override
    public String toString() {
        return "SystemReportMessage{" +
                "messageId=" + messageId +
                ", length=" + length +
                ", groupId=" + groupId +
                ", userId=" + userId +
                ", state=" + type +
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

    public int getType() {
        return type;
    }

    public void setType(int type) {
        this.type = type;
    }

}
