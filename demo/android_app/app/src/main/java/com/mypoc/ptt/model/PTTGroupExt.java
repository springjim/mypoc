package com.mypoc.ptt.model;

import android.os.Parcel;
import android.os.Parcelable;

import com.mypoc.pttlibrary.model.PTTGroup;

/**
 * 对PTTGroup进行扩展，增加一些管理字段
 */
public class PTTGroupExt extends PTTGroup  {
    /**
     * 1: 表示固定组  0：表示临时组
     */
    private int groupType=1;

    public PTTGroupExt(Integer groupId, String groupName, Integer ownerId, Long createDate, int groupType) {
        super(groupId, groupName, ownerId,createDate);
        this.groupType= groupType;
    }

    public int getGroupType() {
        return groupType;
    }

    public void setGroupType(int groupType) {
        this.groupType = groupType;
    }



}
