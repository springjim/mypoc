package com.mypoc.ptt.enums;

/**
 * poc会话状态枚举
 */
public enum PocSessionStatusEnum {
    /**
     * poc会话是空闲的，无人讲话, 默认状态
     */
    Idel,

    /**
     * 我方正在申请麦权中
     */
    Appling,

    /**
     * 我方正在讲话中
     */
    Speaking,

    /**
     * 收听其它人讲话中
     */
    Listening,

    /**
     * 原来我方正在说话中，现在被高优先级话权人打断了
     */
    Breaking

}
