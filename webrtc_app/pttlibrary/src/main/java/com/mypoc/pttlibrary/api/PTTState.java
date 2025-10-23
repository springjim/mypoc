package com.mypoc.pttlibrary.api;

public enum PTTState {
    RESTFUL_LOGIN_FAIL,  //未登录
    RESTFUL_LOGIN_SUCCESS,      // 已登录
    TCP_DISCONNECTED,   // 未连接
    TCP_CONNECTED,      // TCP已连接
    IN_GROUP,        // 在对讲组中
    ERROR
}
