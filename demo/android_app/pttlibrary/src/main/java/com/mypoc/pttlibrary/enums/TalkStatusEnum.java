package com.mypoc.pttlibrary.enums;

public enum TalkStatusEnum {
    /**
     * 麦权空闲，默认状态
     */
    Idel,
    /**
     * 我方申请麦权申请麦权中
     */
    Applying,
    /**
     * 我方申请麦权失败,暂未用
     */
    ApplyFail,

    /**
     * 我方申请麦权成功,
     */
    ApplySuccess,

    /**
     * 正在收听别人的讲话
     */
    ListenStart,

    /**
     * 收听别人的讲话停止
     */
    ListenStop,
    /**
     * 申请麦权超时
     */
    ApplyTimeout,
}
