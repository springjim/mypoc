package com.mypoc.pttlibrary.internal.tcp;

/**
 * tcp消息的messageId枚举
 */
public class TCPMessageType {

    //Media消息
    public static final short TYPE_MEDIA = 0;

    //上报
    public static final short TYPE_REPROT = 1;

    //心跳检查
    public static final short TYPE_CHECK_CLINET = 2;

    public static final short TYPE_CLIENT_RESPONSE = 3;

    public static final short TYPE_CHECK_SERVER = 4;

    public static final short TYPE_SERVICE_RESPONSE = 5;

    public static final short TYPE_LOGIN = 6;

    public static final short TYPE_LOGOUT = 7;


    public static final short REPORT_JPJ = 8;   //废除
    public static final short JPJ_MESSAGE = 0X5AA5; //废除

    //抢麦
    public static final short TYPE_ROB_MIC = 10;//groupId ,userId

    public static final short TYPE_MIC_SUCCESS = 11;//groupId ,userId

    public static final short TYPE_MIC_FAILED = 12;//groupId ,userId

    public static final short TYPE_REALASE_MIC = 13;//groupId ,userId

    /**
     * 中继台抢麦
     */
    public static final short TYPE_ROB_MIC_RELAY = 14;


    public static final short TYPE_TOPOC_START_MIC   = 16;//当开始要实现PTT转POC
    public static final short TYPE_TOPOC_REALASE_MIC = 17;//当结束要实现PTT转POC

    //广播模式下的抢麦
    public static final short TYPE_ROB_BROAD_MIC = 18;//groupId ,userId

    //nofication
    public static final short TYPE_KICK_USER = 20;//userName ,userId

    //nofication
    public static final short TYPE_EXIT_GROUP = 21;//userName,userId

    /**收到某用户创建的会话邀请*/
    public static final short TYPE_RECIVED_INVITE = 22;// invite id,group id

    //nofication
    public static final short TYPE_ACCEPT_INVITE = 23;//

    public static final short TYPE_REJECT_INVITE = 24;//

    //申请处理
    public static final short TYPE_RECIVED_APPLY = 25;//apply id

    //nofication
    public static final short TYPE_ACCEPT_APPLY = 26;//

    public static final short TYPE_REJECT_APPLY = 27;//

    //nofication
    public static final short TYPE_DELETE_GROUP = 28;//groupName

    public static final short TYPE_CREATE_GROUP = 29;  //APP端不收该数据

    //聊天消息类型
    public static final short TYPE_P2P_CHAT_TEXT = 30;
    public static final short TYPE_P2G_CHAT_TEXT  = 31;
    public static final short TYPE_P2P_CHAT_FILE  = 32;
    public static final short TYPE_P2G_CHAT_FILE  = 33;

    public static final short TYPE_TOPOC_START= 34; //当开始要实现PTT转POC 十六进制 22
    public static final short TYPE_TOPOC_END  = 35; //当结束要实现PTT转POC 十六进制 23

    //实时GPS定位消息类型(用于调度台与POC之间GPS命令发送与接收)
    public static final short TYPE_GPS_COMMAND   = 36;
    //分享直播监控
    public static final short TYPE_SHARE_VIDEOLIVE = 37;

    //SOS报文的定位及SOS按下的报文, 服务器用来开始创建录音
    public static final short SOS_LOCATION = 38;
    //SOS报文的语音数据包, 在sos期间,调度台会发这个报文的语音包，app也要处理
    public static final short SOS_MEDIA_EX=39;

    public static final short TYPE_P2P_MESSAGES = 40;
    public static final short TYPE_P2G_MESSAGES = 41;

    /**系统消息*/
    public static final short TYPE_SYS_MESSAGE = 42;

    /**某用户邀请单聊*/
    public static final short TYPE_PERSON_INVITE = 43;

    /**某用户释放邀请单聊*/
    public static final short TYPE_PERSON_INVITE_RELEASE = 44;

    /**同帐号的互相踢除，后登入的踢出前一个*/
    public static final short TYPE_KICK_OFF = 45;

    /**远程切换前后摄*/
    public static final short TYPE_CAMERA_SWITCH = 46;

    /**
     * 遥毙，即管理员对某帐号进行立即下线，且锁定帐号
     */
    public static final short TYPE_REMOTE_KILL=47;

    public static final short OFFLINE_USER_RECORD = 60;
    public static final short RELAY_USER_MESSAGE  = 61;

    //加入视频统一消息，已废除
    public static final short VIDEO_MESSAGE = 70;

    /**强制同步到指定组讲话*/
    public static final short TYPE_GROUP_SYNC = 71;// invite id,group id
    /**强拆或强插*/
    public static final short TYPE_GROUP_USER_CHANGE = 72;

    /**
     * 新版的音视频通话协议，agora
     */
    public static final short TYPE_AV_CHAT_NEW = 75;

    /**
     * 远程监控,agora
     */
    public static final short TYPE_AV_REMOTE_MONI = 76;

    public static final short USER_RTSTATE_UPDATE   = 80;
    //MediaEX消息
    public static final short TYPE_TOPOC_START_EX = 94; //当开始要实现PTT转POC 十六进制 22
    public static final short TYPE_TOPOC_END_EX   = 95; //当结束要实现PTT转POC 十六进制 23
    public static final short TYPE_MEDIA_EX       = 99; //表示从ptt到poc的语音包
    //MediaEX消息

    /**
     * 服务器转发调度台时，在语音数据包增加组ID，用户ID, 这个包不符合设计规范，要特殊处理
     */
    public final static short MEDIA_EX_TOPLATFORM = 100;

    public static final short TYPE_MEDIA_EX_FILE_FRAME       = 101;  //专用于接受PC发来的wav文件的音频帧

    //邀请视频会议,专用于PC端发起的视频会商功能,agora
    public static final short TYPE_MEET_CHAT                 = 102;

    //视频会议中的屏幕分享通知(开始,结束), agora
    public static final short TYPE_MEET_SCREEN_SHARE         = 103;


    //SOS键释放的报文，服务器用来结来录音
    public static final short SOS_KEY_RELEASE=62;

    //扩展一些群组的操作
    public static final short  TYPE_MODIFY_GROUP= 110;   //用于修改群组名称，或将来扩展属性

    //增加SOS会话信令，信令的sessionId用于对 （SOS定位，SOS视频推流频道名称，SOS录像进行关联）
    public static final short SOS_SESSION = 111;

    /**收到某用户创建的会话邀请, 内容与 TYPE_RECIVED_INVITE_TWOWAY一样，但这个是双向对讲，不用抢麦，可以用于一对一，一对多的语音会议模式
     * 不用第4，5字节表示payload长度
     * */
    public static final short TYPE_RECIVED_INVITE_TWOWAY = 112;// invite id,group id

    public static final short TYPE_LOGIN_AUTH = 115;   //webapi 发过来的客户端已认证通过的，非调用度台客户端,一般客户端不会收到，这里做容错

    public static final short TYPE_LOGIN_PLATFORM_AUTH = 116;  //webapi 发过来的客户端已认证通过的，调用度台客户端,一般客户端不会收到，这里做容错

    //专用于单呼的信令设计 2025.7 增加
    public static final short TYPE_SINGLE_CALL_SIGNAL =117;

    //专用于socket服务返回的error 消息， 2025.8 增加
    //要有一套错误码，用于服务端告诉客户端错误信息，不要具体的message
    public static final short TYPE_ERROR_CODE =500;

}
