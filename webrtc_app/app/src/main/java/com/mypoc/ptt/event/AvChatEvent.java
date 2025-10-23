package com.mypoc.ptt.event;

/**
 * av(含直播,监控等信令事件)
 */
public class AvChatEvent {

    private short videoType;
    private short videoCommand;
    private int fromUserId;
    private int toUserId;
    private String fromUserName;
    private String toUserName;
    private String desc;

    public AvChatEvent(short videoType, short videoCommand, int fromUserId, int toUserId, String fromUserName, String toUserName, String desc) {
        this.videoType = videoType;
        this.videoCommand = videoCommand;
        this.fromUserId = fromUserId;
        this.toUserId = toUserId;
        this.fromUserName = fromUserName;
        this.toUserName = toUserName;
        this.desc = desc;
    }

    public short getVideoType() {
        return videoType;
    }

    public void setVideoType(short videoType) {
        this.videoType = videoType;
    }

    public short getVideoCommand() {
        return videoCommand;
    }

    public void setVideoCommand(short videoCommand) {
        this.videoCommand = videoCommand;
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

    public String getFromUserName() {
        return fromUserName;
    }

    public void setFromUserName(String fromUserName) {
        this.fromUserName = fromUserName;
    }

    public String getToUserName() {
        return toUserName;
    }

    public void setToUserName(String toUserName) {
        this.toUserName = toUserName;
    }

    public String getDesc() {
        return desc;
    }

    public void setDesc(String desc) {
        this.desc = desc;
    }

    @Override
    public String toString() {
        return "AvChatEvent{" +
                "videoType=" + videoType +
                ", videoCommand=" + videoCommand +
                ", fromUserId=" + fromUserId +
                ", toUserId=" + toUserId +
                ", fromUserName='" + fromUserName + '\'' +
                ", toUserName='" + toUserName + '\'' +
                ", desc='" + desc + '\'' +
                '}';
    }
}
