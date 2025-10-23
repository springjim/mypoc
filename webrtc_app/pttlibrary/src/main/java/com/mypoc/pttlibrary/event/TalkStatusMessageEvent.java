package com.mypoc.pttlibrary.event;


import com.mypoc.pttlibrary.enums.TalkStatusEnum;

public class TalkStatusMessageEvent {
    private TalkStatusEnum status;
    private Integer userId;
    private Integer groupId;

    public TalkStatusMessageEvent(TalkStatusEnum status) {
        this.status = status;
    }

    public TalkStatusMessageEvent(TalkStatusEnum status,Integer groupId,Integer userId) {
        this.status = status;
        this.groupId = groupId;
        this.userId = userId;
    }

    public TalkStatusEnum getStatus() {
        return status;
    }

    public void setStatus(TalkStatusEnum status) {
        this.status = status;
    }

    public Integer getUserId() {
        return userId;
    }

    public void setUserId(Integer userId) {
        this.userId = userId;
    }

    public Integer getGroupId() {
        return groupId;
    }

    public void setGroupId(Integer groupId) {
        this.groupId = groupId;
    }

    @Override
    public String toString() {
        return "TalkStatusMessageEvent{" +
                "status=" + status +
                '}';
    }
}
