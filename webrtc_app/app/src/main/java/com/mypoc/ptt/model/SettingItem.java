package com.mypoc.ptt.model;

/**
 * 用于系统参数设置
 */
public class SettingItem {
    public enum Type { SWITCH, EDIT_TEXT,NUMBER, LIST}

    private String key;  //preference中存储key
    private String title;  //标题
    private Type type;
    private Object defaultValue;  //值

    public SettingItem(String key, String title, Type type, Object defaultValue) {
        this.key = key;
        this.title = title;
        this.type = type;
        this.defaultValue = defaultValue;
    }

    // Getters
    public String getKey() { return key; }
    public String getTitle() { return title; }
    public Type getType() { return type; }
    public Object getDefaultValue() { return defaultValue; }

}
