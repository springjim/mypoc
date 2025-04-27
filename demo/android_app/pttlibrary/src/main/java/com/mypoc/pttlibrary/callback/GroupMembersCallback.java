package com.mypoc.pttlibrary.callback;

import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTGroupMember;

import java.util.List;

public interface GroupMembersCallback {
    void onSuccess(List<PTTGroupMember> pttGroupMembers);
    void onFailure(String error);
}
