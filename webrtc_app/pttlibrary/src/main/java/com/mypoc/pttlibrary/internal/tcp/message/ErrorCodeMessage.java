package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.audio.Config;
import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

public class ErrorCodeMessage {

    private short messageId;//2 byte
    private byte length;//1 byte
    private int  errorCode;     //错误码

    private static final int TOTAL_LENGTH = Config.MSG_HEADER_LEN+4;
    private static final short MESSAGE_ID = TCPMessageType.TYPE_ERROR_CODE;

    public ErrorCodeMessage(int errorCode) {
        this.errorCode = errorCode;
    }

    public static ErrorCodeMessage  parseBytes(byte[] msgBytes){
        if (msgBytes == null || msgBytes.length < TOTAL_LENGTH) {
            throw new IllegalArgumentException("Invalid message bytes");
        }
        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];
        int errorCode = TextUtil.bytesToInt(msgBytes, 3, 4);

        return new ErrorCodeMessage(errorCode);
    }

    @Override
    public String toString() {
        return "ErrorCodeMessage{" +
                "messageId=" + messageId +
                ", length=" + length +
                ", errorCode=" + errorCode +
                '}';
    }

    public int getErrorCode() {
        return errorCode;
    }

    public void setErrorCode(int errorCode) {
        this.errorCode = errorCode;
    }
}
