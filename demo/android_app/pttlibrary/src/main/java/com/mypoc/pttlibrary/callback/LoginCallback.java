package com.mypoc.pttlibrary.callback;

public interface LoginCallback {
    void onSuccess(String token, long expiresIn, String refreshToken);
    void onFailure(String error);
}
