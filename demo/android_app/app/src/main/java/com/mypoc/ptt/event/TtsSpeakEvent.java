package com.mypoc.ptt.event;

/**
 * tts播放文本事件
 */
public class TtsSpeakEvent {
    private String speakText;

    public TtsSpeakEvent(String speakText) {
        this.speakText = speakText;
    }

    public String getSpeakText() {
        return speakText;
    }

    public void setSpeakText(String speakText) {
        this.speakText = speakText;
    }
}
