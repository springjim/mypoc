package com.mypoc.ptt.event;

/**
 * 更新工作组名称
 */
public class UpdateWorkingGroupEvent {
    private int groupId;
    private String groupName;

    public UpdateWorkingGroupEvent(int groupId, String groupName) {
        this.groupId = groupId;
        this.groupName = groupName;
    }

    public int getGroupId() {
        return groupId;
    }

    public void setGroupId(int groupId) {
        this.groupId = groupId;
    }

    public String getGroupName() {
        return groupName;
    }

    public void setGroupName(String groupName) {
        this.groupName = groupName;
    }
}
