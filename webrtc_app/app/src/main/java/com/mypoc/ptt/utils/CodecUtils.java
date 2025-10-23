package com.mypoc.ptt.utils;

import android.media.MediaCodecInfo;
import android.media.MediaCodecList;
import android.util.Log;

public class CodecUtils {

    /**
     * 检查设备是否支持 H265/HEVC 编码
     */
    public static boolean isH265EncoderSupported() {
        MediaCodecList codecList = new MediaCodecList(MediaCodecList.ALL_CODECS);
        MediaCodecInfo[] codecInfos = codecList.getCodecInfos();

        for (MediaCodecInfo codecInfo : codecInfos) {
            if (codecInfo.isEncoder()) {
                String[] types = codecInfo.getSupportedTypes();
                for (String type : types) {
                    if (type.equalsIgnoreCase("video/hevc")) {
                        Log.d("CodecUtils", "H265 encoder found: " + codecInfo.getName());
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /**
     * 检查设备是否支持 H264 编码（备用方案）
     */
    public static boolean isH264EncoderSupported() {
        MediaCodecList codecList = new MediaCodecList(MediaCodecList.ALL_CODECS);
        MediaCodecInfo[] codecInfos = codecList.getCodecInfos();

        for (MediaCodecInfo codecInfo : codecInfos) {
            if (codecInfo.isEncoder()) {
                String[] types = codecInfo.getSupportedTypes();
                for (String type : types) {
                    if (type.equalsIgnoreCase("video/avc")) {
                        Log.d("CodecUtils", "H264 encoder found: " + codecInfo.getName());
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
