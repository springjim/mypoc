package com.mypoc.ptt.player.listener;

import com.mypoc.ptt.player.views.VideoTextureView;

/**
 * 指示某view view获得焦点事件
 */
public class VideoFocusEvent {
    private VideoTextureView view;

    public VideoFocusEvent(VideoTextureView view) {
        this.view = view;
    }

    public VideoTextureView getView() {
        return view;
    }

    public void setView(VideoTextureView view) {
        this.view = view;
    }
}
