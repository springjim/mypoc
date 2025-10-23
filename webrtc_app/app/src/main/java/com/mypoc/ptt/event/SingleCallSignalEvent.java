package com.mypoc.ptt.event;

public class SingleCallSignalEvent {

    private int groupId;
    private int fromUserId;
    private int toUserId;
    private byte signalVal;

    public SingleCallSignalEvent(int groupId, int fromUserId, int toUserId, byte signalVal) {
        this.groupId = groupId;
        this.fromUserId = fromUserId;
        this.toUserId = toUserId;
        this.signalVal = signalVal;
    }

    public int getGroupId() {
        return groupId;
    }

    public void setGroupId(int groupId) {
        this.groupId = groupId;
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

    public byte getSignalVal() {
        return signalVal;
    }

    public void setSignalVal(byte signalVal) {
        this.signalVal = signalVal;
    }
}
