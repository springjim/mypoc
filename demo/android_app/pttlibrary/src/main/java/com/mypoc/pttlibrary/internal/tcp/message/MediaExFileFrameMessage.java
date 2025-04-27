package com.mypoc.pttlibrary.internal.tcp.message;

import com.mypoc.pttlibrary.internal.tcp.TCPMessageType;
import com.mypoc.pttlibrary.internal.tcp.TextUtil;

/**
 * 文件广播的语音帧
 */
public class MediaExFileFrameMessage {

    private short messageId;//2 byte
    private byte length;//1 byte
    private short payloadLen;  //2 byte, payload的长度
    private byte[] media;   //经amrnb编码后的一帧数据包
    private static final short MESSAGE_ID = TCPMessageType.TYPE_MEDIA_EX_FILE_FRAME;

    public MediaExFileFrameMessage(short messageId, byte length, short payloadLen, byte[] media) {
        this.messageId = messageId;
        this.length = length;
        this.payloadLen = payloadLen;
        this.media = media;
    }

    public static MediaExFileFrameMessage parseBytes(byte[] msgBytes){
        short payloadLen  = (short) (TextUtil.bytesToShort( msgBytes, 3, 2 ) - 2);
        if( payloadLen <= 0 )
        {
            return null;
        }
        short messageId  = TextUtil.bytesToShort( msgBytes, 0, 2 );
        byte  messageLen = msgBytes[2];

        byte[] media= new byte[payloadLen];
        System.arraycopy( msgBytes, 5, media, 0, payloadLen );
        return  new MediaExFileFrameMessage(messageId,messageLen,payloadLen,media);
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

    public byte[] getMedia() {
        return media;
    }

    public void setMedia(byte[] media) {
        this.media = media;
    }
}
