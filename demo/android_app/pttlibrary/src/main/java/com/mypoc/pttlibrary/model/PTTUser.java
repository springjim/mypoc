package com.mypoc.pttlibrary.model;

import android.content.Intent;

public class PTTUser {

    private Integer userId;
    private String userName;
    private int cmpid;
    private String flagRecord;  //是否录音
    private int myclass;  //麦权
    private int currGroupId;  //当前在线在组的组ID
    private Integer defaultGroupId; //用户默认登录的组ID，用于初始登录或从临时组退出时进入
    private Integer logon;  //在线或离线

    public Integer getLogon() {
        return logon;
    }

    public void setLogon(Integer logon) {
        this.logon = logon;
    }

    public Integer getUserId() {
        return userId;
    }

    public void setUserId(Integer userId) {
        this.userId = userId;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public int getCmpid() {
        return cmpid;
    }

    public void setCmpid(int cmpid) {
        this.cmpid = cmpid;
    }

    public String getFlagRecord() {
        return flagRecord;
    }

    public void setFlagRecord(String flagRecord) {
        this.flagRecord = flagRecord;
    }

    public int getMyclass() {
        return myclass;
    }

    public void setMyclass(int myclass) {
        this.myclass = myclass;
    }

    public int getCurrGroupId() {
        return currGroupId;
    }

    public void setCurrGroupId(int currGroupId) {
        this.currGroupId = currGroupId;
    }

    public Integer getDefaultGroupId() {
        return defaultGroupId;
    }

    public void setDefaultGroupId(Integer defaultGroupId) {
        this.defaultGroupId = defaultGroupId;
    }

    @Override
    public String toString() {
        return "PTTUser{" +
                "userId=" + userId +
                ", userName='" + userName + '\'' +
                ", cmpid=" + cmpid +
                ", flagRecord='" + flagRecord + '\'' +
                ", myclass=" + myclass +
                ", currGroupId=" + currGroupId +
                ", defaultGroupId=" + defaultGroupId +
                ", logon=" + logon +
                '}';
    }
}
