package com.mypoc.ptt.event;

/**
 * 专用于通知退到上一个工作组的事件
 */
public class EnterPrevGroupEvent {
    /**
     * 指定方式进入上一个群组
     */
    private Integer prevGroupId;
    public EnterPrevGroupEvent() {
        this.prevGroupId=null;
    }

    public EnterPrevGroupEvent(Integer prevGroupId) {
        this.prevGroupId = prevGroupId;
    }

    public Integer getPrevGroupId() {
        return prevGroupId;
    }

    public void setPrevGroupId(Integer prevGroupId) {
        this.prevGroupId = prevGroupId;
    }
}
