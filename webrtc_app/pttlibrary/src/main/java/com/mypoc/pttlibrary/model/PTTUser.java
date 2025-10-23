package com.mypoc.pttlibrary.model;

import android.content.Intent;

public class PTTUser {

    private Integer userId;
    /**
     * 2025.5以后，当作登录帐号了，客户使用时，可以修改为和userId不一样的值
     */
    private String phone;
    private String userName;
    private int cmpid;
    private String flagRecord;  //是否录音
    private int myclass;  //麦权
    private int currGroupId;  //当前在线在组的组ID
    private Integer defaultGroupId; //用户默认登录的组ID，用于初始登录或从临时组退出时进入
    private Integer logon;  //在线或离线
    private String  defaultAppconfigPwd;  //用户在app上修改配置的密码,默认为666666

    //以下是因为业务方面要展示的用户应用设置
    /**
     * poc端单呼权限(现已改成创建临时群组权限)，实际上，单呼也是一种建临时组方式
     * Y 开通， 空或N表示未开通
     */
    private String privSinglecall;
    /**
     * 是否登录自动开启定位上报,Y 开通， 空或N表示未开通
     */
    private String flagAutoLocation;
    /**
     * 定位模式;  0，一般；1，高精,2,用户设置
     */
    private Integer locationMode;

    /**
     * 循环定位时间间隔(单位：秒):30,60,180，0则由用户设置
     */
    private Integer locationInterval;

    /**
     * 是否隐藏POC上报定位开关, Y : 终端无法改  N或空, 在终端可以修改
     */
    private String privHideLocSwitch;

    public String getPrivHideLocSwitch() {
        return privHideLocSwitch;
    }

    public void setPrivHideLocSwitch(String privHideLocSwitch) {
        this.privHideLocSwitch = privHideLocSwitch;
    }

    public String getPrivSinglecall() {
        return privSinglecall;
    }

    public void setPrivSinglecall(String privSinglecall) {
        this.privSinglecall = privSinglecall;
    }

    public String getFlagAutoLocation() {
        return flagAutoLocation;
    }

    public void setFlagAutoLocation(String flagAutoLocation) {
        this.flagAutoLocation = flagAutoLocation;
    }

    public Integer getLocationMode() {
        return locationMode;
    }

    public void setLocationMode(Integer locationMode) {
        this.locationMode = locationMode;
    }

    public Integer getLocationInterval() {
        return locationInterval;
    }

    public void setLocationInterval(Integer locationInterval) {
        this.locationInterval = locationInterval;
    }

    public String getDefaultAppconfigPwd() {
        return defaultAppconfigPwd;
    }

    public void setDefaultAppconfigPwd(String defaultAppconfigPwd) {
        this.defaultAppconfigPwd = defaultAppconfigPwd;
    }

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

    public String getPhone() {
        return phone;
    }

    public void setPhone(String phone) {
        this.phone = phone;
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
