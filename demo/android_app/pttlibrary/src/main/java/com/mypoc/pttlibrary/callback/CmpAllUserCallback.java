package com.mypoc.pttlibrary.callback;

import com.mypoc.pttlibrary.model.PTTUser;
import java.util.List;

/**
 * 同一个cmpid下的所有用户
 */
public interface CmpAllUserCallback {
    void onSuccess(List<PTTUser> pttCmpUsers);
    void onFailure(String error);
}
