package com.mypoc.pttlibrary.callback;

/**
 * 通用的回调
 */
public interface CommonCallback {
    void onSuccess();
    void onFailure(String error);
}
