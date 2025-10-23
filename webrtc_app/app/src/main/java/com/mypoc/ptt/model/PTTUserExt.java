package com.mypoc.ptt.model;

import com.mypoc.pttlibrary.model.PTTUser;

public class PTTUserExt extends PTTUser {
    private boolean selected;

    public boolean isSelected() {
        return selected;
    }

    public void setSelected(boolean selected) {
        this.selected = selected;
    }
}
