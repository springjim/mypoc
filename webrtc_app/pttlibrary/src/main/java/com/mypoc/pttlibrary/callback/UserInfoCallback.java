package com.mypoc.pttlibrary.callback;

import com.mypoc.pttlibrary.model.PTTUser;

public interface UserInfoCallback {
    void onSuccess(PTTUser pttUser);
    void onFailure(String error);
}
