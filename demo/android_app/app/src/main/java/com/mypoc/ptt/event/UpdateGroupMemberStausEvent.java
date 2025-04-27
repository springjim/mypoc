package com.mypoc.ptt.event;

/**
 * 这个事件用于通知组内成员的状态，在组成员页面要监听这个事件
 * 状态有：进组，离开组，强拉进组，强拆踢出组
 */
public class UpdateGroupMemberStausEvent {

    private int groupId;
    private int userId;
    private int status;
    /**
     * 当是强拉或强踢时，可能会有多个人的userId拼接的，以半角逗号隔开, 如  "100123,100124"
     */
    private String changeUserStr;

    /**
     * 进组
     */
    public static final int Status_EnterGroup=1;
    /**
     * 离开组
     */
    public static final int Status_ExitGroup=2;
    /**
     * 强拉进组
     */
    public static final int Status_PullInGroup=3;
    /**
     * 强踢出组
     */
    public static final int Status_KickOutGroup=4;

    /**
     * 用户下线了，登出app了
     */
    public static final int Status_Offline=5;


    public UpdateGroupMemberStausEvent(int groupId, int userId, int status,String changeUserStr) {
        this.groupId = groupId;
        this.userId = userId;
        this.status = status;
        this.changeUserStr= changeUserStr;
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

    public int getStatus() {
        return status;
    }

    public void setStatus(int status) {
        this.status = status;
    }

    public String getChangeUserStr() {
        return changeUserStr;
    }

    public void setChangeUserStr(String changeUserStr) {
        this.changeUserStr = changeUserStr;
    }
}
