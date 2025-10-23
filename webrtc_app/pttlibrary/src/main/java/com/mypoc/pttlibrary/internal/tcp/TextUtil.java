package com.mypoc.pttlibrary.internal.tcp;

/**
 * 工具类
 */
public class TextUtil {
    /**
     * byte[] 转short, 按大端字节顺
     * @param b
     * @return
     */
    public static short bytesToShort(byte[] b, int startPos, int length)
    {
        byte[] temp = new byte[length];
        System.arraycopy(b, startPos, temp, 0, length);
        return (short) (temp[1] & 0xff | (temp[0] & 0xff) << 8);
    }

    /**
     * 将 short转为2个字节数组，按大端字节顺序
     * @param s
     * @return
     */
    public final static byte[] getBytes(short s)
    {
        byte[] buf = new byte[2];
        for (int i = buf.length - 1; i >= 0; i--)
        {
            buf[i] = (byte) (s & 0x00ff);
            s >>= 8;
        }
        return buf;
    }

    /**
     * 将 int 转为4个字节数组，按大端字节顺序
     * @param s
     * @return
     */
    public final static byte[] getBytes(int s)
    {
        byte[] buf = new byte[4];
        for (int i = buf.length - 1; i >= 0; i--)
        {
            buf[i] = (byte) (s & 0x000000ff);
            s >>= 8;
        }
        return buf;
    }

    /**
     * 将字节数组转成0~255 之间的无符号整数字符串，用 , 隔开，用于日志查看
     * @param src
     * @return
     */
    public static String bytesToIntString(byte[] src){
        StringBuilder stringBuilder = new StringBuilder("");
        if (src == null || src.length <= 0) {
            return null;
        }
        for (int i = 0; i < src.length; i++) {
            int v = src[i] & 0xFF;  //转换为无符号整数, 0~255
            stringBuilder.append(v+",");
        }
        return stringBuilder.toString();
    }

    /**
     * 大端字节顺序，将4个字节转成int
     * @param bytes
     * @param startPos
     * @param length
     * @return
     */
    public static int bytesToInt(byte[] bytes, int startPos, int length)
    {
        byte[] temp = new byte[length];
        System.arraycopy(bytes, startPos, temp, 0, length);
        int fromByte = 0;
        for (int i = 0; i < 4; i++)
        {
            int n = (temp[i] < 0 ? (int) temp[i] + 256 : (int) temp[i]) << (8 * (3 - i));
            fromByte += n;

        }
        return fromByte;
    }

}
