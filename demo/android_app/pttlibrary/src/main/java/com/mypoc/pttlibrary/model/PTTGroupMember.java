package com.mypoc.pttlibrary.model;

/**
 * 专用于组成员
 */
public class PTTGroupMember {
    private Integer userId;
    private String  userName;
    private int logon;  //1：在线 0: 离线
    private Integer aclass;  //麦权
    private String  listen;  //n: 不在组 y: 在组

    public PTTGroupMember(Integer userId, String userName, int logon, Integer aclass, String listen) {
        this.userId = userId;
        this.userName = userName;
        this.logon = logon;
        this.aclass = aclass;
        this.listen = listen;
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

    public int getLogon() {
        return logon;
    }

    public void setLogon(int logon) {
        this.logon = logon;
    }

    public Integer getAclass() {
        return aclass;
    }

    public void setAclass(Integer aclass) {
        this.aclass = aclass;
    }

    public String getListen() {
        return listen;
    }

    public void setListen(String listen) {
        this.listen = listen;
    }
}
