package com.mypoc.ptt.event;

/**
 * webrtc的会议邀请
 */
public class MeetInviteEvent {

    private  String room;
    private  String fromUserId;

    public MeetInviteEvent(String room, String fromUserId) {
        this.room = room;
        this.fromUserId = fromUserId;
    }

    public String getRoom() {
        return room;
    }

    public void setRoom(String room) {
        this.room = room;
    }

    public String getFromUserId() {
        return fromUserId;
    }

    public void setFromUserId(String fromUserId) {
        this.fromUserId = fromUserId;
    }
}
