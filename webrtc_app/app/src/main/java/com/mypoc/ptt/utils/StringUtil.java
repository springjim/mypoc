package com.mypoc.ptt.utils;

import java.util.List;

public class StringUtil {

    public static String listToString(List<String> mList) {
        final String SEPARATOR = ",";
        StringBuilder sb = new StringBuilder();
        String convertedListStr;
        if (null != mList && mList.size() > 0) {
            for (String item : mList) {
                sb.append(item);
                sb.append(SEPARATOR);
            }
            convertedListStr = sb.toString();
            convertedListStr = convertedListStr.substring(0, convertedListStr.length() - SEPARATOR.length());
            return convertedListStr;
        } else return "";
    }
}
