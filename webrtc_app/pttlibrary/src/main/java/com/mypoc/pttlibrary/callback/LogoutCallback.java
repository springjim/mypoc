package com.mypoc.pttlibrary.callback;

public interface LogoutCallback {
    void onSuccess();
    void onFailure(String error);
}
