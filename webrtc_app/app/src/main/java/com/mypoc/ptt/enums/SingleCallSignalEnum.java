package com.mypoc.ptt.enums;

import java.util.HashMap;
import java.util.Map;

/**
 * 单呼信令，各种客户要语义定义一致
 *  0: 呼叫(主叫方到被叫方)
 *  1: 响铃(告知主叫方，被叫方正在响铃中)
 *  2: 被叫方忙线中 (可能正在与另一方单呼中，或者其它业务原因)
 *  3: 被叫方正在国标SIP对讲中，会自动拒绝, SIP国标对讲是指终端接入了28181后与上级平台在sip对讲中
 *  4: 被叫方主动拒绝了
 *  5: 被叫方接听了
 *  6: 主叫方或被叫方主动退出了
 *  99: 被叫方超时未接听...
 */
public enum  SingleCallSignalEnum {

    /**
     * 呼叫(主叫方到被叫方)
     */
    INVITE((byte)0x00),

    /**
     * 响铃(告知主叫方，被叫方正在响铃中)
     */
    RING((byte) 0x01),

    /**
     *被叫方忙线中 (可能正在与另一方单呼中，或者其它业务原因)
     */
    BUSING((byte) 0x02),

    /**
     * 被叫方正在国标SIP对讲中，会自动拒绝, SIP国标对讲是指终端接入了28181后与上级平台在sip对讲中
     */
    GB_TALKING((byte)0x03),

    /**
     * 被叫方主动拒绝了
     */
    REFUSE((byte)0x04),

    /**
     * 被叫方接听了
     */
    ACCEPTED((byte)0x05),

    /**
     * 主叫方或被叫方主动退出了
     */
    LEAVE((byte) 0x06),

    /**
     * 被叫方超时未接听...
     */
    TIMEOUT((byte)0x58),  //被叫方超时未接听

    /**
     * 不存在的一个信令，为了客户调用方便解析写的，由fromByte查找不到返回的
     */
    NO_EXIST((byte)0xFF)   //不存在
    ;

    private final byte value; // 存储关联的byte值
    private static final Map<Byte, SingleCallSignalEnum> byteToEnum = new HashMap<>();

    static {
        for (SingleCallSignalEnum status : SingleCallSignalEnum.values()) {
            byteToEnum.put(status.value, status);
        }
    }

    SingleCallSignalEnum(byte value){
        this.value = value;
    }

    // 获取关联的byte值
    public byte getValue() {
        return value;
    }

    public static SingleCallSignalEnum fromByte(byte value) {
        SingleCallSignalEnum status = byteToEnum.get(value);
        if (status==null)
            return NO_EXIST;
        return byteToEnum.get(value);
    }

}
