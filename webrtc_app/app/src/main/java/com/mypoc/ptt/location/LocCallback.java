package com.mypoc.ptt.location;

public interface  LocCallback {
    void onLocationSuccess(String provider, double latitude, double longitude);
    void onLocationFailed(int errorCode, String errorMsg);
    void onLocationTimeout();

    // 提供默认实现，让调用方选择性重写
    default void deal(String provider, double latitude, double longitude) {
        onLocationSuccess(provider, latitude, longitude);
    }

    default void timeout() {
        onLocationTimeout();
    }
}
