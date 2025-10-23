package com.mypoc.pttlibrary.callback;

import com.mypoc.pttlibrary.model.PTTGroup;

public interface TempGroupCreateCallback {
    void onSuccess(PTTGroup group);
    void onFailure(String error);
}
