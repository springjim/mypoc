package com.dds.skywebrtc.log;

public class SkyLog {
    private static final String LOG_TAG_PREFIX = "Webrtc_";

    public static String createTag(String className) {
        return LOG_TAG_PREFIX + className;
    }

}
