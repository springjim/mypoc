package com.mypoc.pttlibrary.callback;

import com.mypoc.pttlibrary.model.PTTGroup;

import java.util.List;

public interface RequestMicCallback {
    void onSuccess();
    void onFailure(String error);
}
