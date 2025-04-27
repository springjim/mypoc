package com.mypoc.pttlibrary.callback;

import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTUser;

import java.util.List;

public interface GroupsCallback {
    void onSuccess(List<PTTGroup> pttGroups);
    void onFailure(String error);
}
