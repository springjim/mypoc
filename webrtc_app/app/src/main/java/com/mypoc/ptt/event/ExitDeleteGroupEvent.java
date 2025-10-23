package com.mypoc.ptt.event;

/**
 * 通知退出被删除的临时组，一般给正在打开的临时组窗口
 */
public class ExitDeleteGroupEvent {
    private int groupId;

    public ExitDeleteGroupEvent(int groupId) {
        this.groupId = groupId;
    }

    public int getGroupId() {
        return groupId;
    }

    public void setGroupId(int groupId) {
        this.groupId = groupId;
    }
}
