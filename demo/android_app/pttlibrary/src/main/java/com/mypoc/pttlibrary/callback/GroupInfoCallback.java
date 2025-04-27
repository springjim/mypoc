package com.mypoc.pttlibrary.callback;

import com.mypoc.pttlibrary.model.PTTGroup;

import java.util.List;

/**
 * 某个固定群或临时群组信息
 */
public interface GroupInfoCallback {
    void onSuccess(PTTGroup pttGroup);
    void onFailure(String error);
}
