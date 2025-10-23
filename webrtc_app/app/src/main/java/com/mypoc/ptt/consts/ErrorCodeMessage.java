package com.mypoc.ptt.consts;

/**
 * 服务端发来的错误码信令
 */
public class ErrorCodeMessage {
    //抢麦中，无工作组 gau为null
    public final static int WORKGROUP_NOFOUND=10001;

    //被遥晕，不能讲话，只能听
    public final static int PROHIBIT_TALK=10002;

    //同话权的别人正在讲话中
    public final static int OTHER_TALKING=10003;

    //抢麦中，gid或uid为-1
    public final static int WORKGROUP_INVALID=10004;
}
