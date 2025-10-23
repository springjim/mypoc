package com.mypoc.pttlibrary.model;

public class PTTGroup {
    private Integer groupId;
    private String  groupName;
    /**
     * 群主用户id, 一般临时组会有
     */
    private Integer ownerId;
    /**
     * 创建组群的时间, unix时间，单位:秒，固定群组是在web后台创建的，该字段值为null
     */
    private Long createDate;

    public PTTGroup(Integer groupId, String groupName, Integer ownerId, Long createDate) {
        this.groupId = groupId;
        this.groupName = groupName;
        this.ownerId = ownerId;
        this.createDate= createDate;

    }

    public Integer getOwnerId() {
        return ownerId;
    }

    public void setOwnerId(Integer ownerId) {
        this.ownerId = ownerId;
    }

    public Integer getGroupId() {
        return groupId;
    }

    public void setGroupId(Integer groupId) {
        this.groupId = groupId;
    }

    public String getGroupName() {
        return groupName;
    }

    public void setGroupName(String groupName) {
        this.groupName = groupName;
    }

    public Long getCreateDate() {
        return createDate;
    }

    public void setCreateDate(Long createDate) {
        this.createDate = createDate;
    }
}
