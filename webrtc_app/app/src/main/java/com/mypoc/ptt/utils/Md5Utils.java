package com.mypoc.ptt.utils;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;

public class Md5Utils {
    public static String getMd5(String str)throws Exception{
        String result = "";

        MessageDigest md5 = MessageDigest.getInstance("MD5");
        md5.update((str).getBytes(StandardCharsets.UTF_8));
        byte[] b = md5.digest();

        int i;
        StringBuffer buf = new StringBuffer();
        for(int offset=0; offset<b.length; offset++){
            i = b[offset];
            if(i<0){
                i+=256;
            }
            if(i<16){
                buf.append("0");
            }
            buf.append(Integer.toHexString(i));
        }

        result = buf.toString();
        return result;
    }
}
