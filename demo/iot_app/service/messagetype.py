class MessageType:
    #以下是定义tcp消息的报文字段
    #消息头，是3个字节，包括messageId(2个字节short类型)，加一个commandLength(1个字节)
    TCP_MESSAGE_HEAD_LEN =3
    
    #客户端上报所在组消息
    MESSAGE_REPORT =1
    #客户端发起的对服务端的心跳检查
    MESSAGE_CHECK_SERVER=4
    #服务端的心跳响应， 是服务发给客户的，decode是客户端处理
    MESSAGE_SERVER_REPORT=5
    #客户端上报的登录
    MESSAGE_LOGIN=6
    #客户端上报的登出
    MESSAGE_LOGOUT=7
    #非中继终端帐号发的抢麦信号
    MESSAGE_APPLY_MIC=10
    
    #抢麦成功, 由服务端发给客户端，由客户端decode
    MESSAGE_APPLY_MIC_SUCCESS =11
    #抢麦失败,由服务端发给客户端，由客户端decode
    MESSAGE_APPLY_MIC_FAILED =12
    #释放麦, 客户端上报
    MESSAGE_RELEASE_MIC =13

    MESSAGE_PTT_POC_START=16
    MESSAGE_PTT_POC_STOP =17
    
    #调度台广播模式下，上报的抢麦
    MESSAGE_APPLY_BROAD_MIC =18
    
    #解散组， 客户端上报
    MESSAGE_DELETE_GROUP =28
    #创建组， 客户端上报
    MESSAGE_CREATE_GROUP =29

    MESSAGE_P2P_CHATMESSAGES=30

    MESSAGE_P2G_CHATMESSAGES=31
    MESSAGE_P2P_FILEMESSAGES=32
    MESSAGE_P2G_FILEMESSAGES=33
    
    #实时GPS定位消息类型
    MESSAGE_GPS_COMMAND=36
    
    #分享直播监控
    MESSAGE_SHARE_VIDEOLIVE=37
    #SOS报文的定位及SOS按下的报文, 2018.1.2
    MESSAGE_SOS_LOCATION=38
    #SOS报文的语音数据包, 2018.1.2
    MESSAGE_SOS_MEDIA_EX=39
    
    #系统消息
    MESSAGE_SERVER_SYSTEM_REPORT=42
    
    #某用户邀请单聊
    MESSAGE_INVITE_GROUP =43
    #某用户释放邀请单聊
    MESSAGE_INVITE_GROUP_RELEASE=44
    #同帐号的互相踢除，后登入的踢出前一个
    MESSAGE_KICK_OFF=45
    #远程监控时发出的前后摄像头切换
    MESSAGE_CAMERA_SWITCH=46

    MESSAGE_OFFLINE_USER_RECORD=60
    MESSAGE_RELAY_USER_MESSAGE=61
    
    #SOS键释放的报文，服务器用来结束录音
    MESSAGE_SOS_KEY_RELEASE=62
    #调度台发送公告消息报文
    MESSAGE_DISPATCHER_SEND_NOTICE=63
    
    #视频统一消息
    MESSAGE_VIDEO_MESSAGE=70
    
    #强制同步到指定组讲话
    MESSAGE_GROUP_SYNC=71
    
    #组内人员的变更报文,用于强插与强拆消息通知
    MESSAGE_GROUP_USER_CHANGE=72
    
    #为了集成声网 agora的音视频通话 (非视频会商模式，用于一对一的通话)
    MESSAGE_AV_CHAT_NEW=75
    #远程监控调取,本质上就是app端直播
    MESSAGE_AV_REMOTE_MONI=76
    
    #增加远程中继传输RTState 状态的消息
    MESSAGE_USER_RTSTATE_UPDATE=80
    
    #专用于调度台的登入
    MESSAGE_LOGIN_PLATFORM=81
    
    #当开始要实现PTT转POC
    MESSAGE_PTT2POC_START_EX=94
    #当结束要实现PTT转POC
    MESSAGE_PTT2POC_END_EX=95
    
    #表示从ptt到poc的语音包
    MESSAGE_MEDIA_EX=99
    
    #服务器转发调度台时，在语音数据包增加组ID，用户ID,iot终端收不到该消息
    MESSAGE_MEDIA_EX_TOPLATFORM=100
    
    #PC用wav的音频帧发的数据包
    MESSAGE_MEDIA_EX_FILE_FRAME =101
    
    #邀请视频会议,专用于PC端发起的视频会商功能
    MESSAGE_MEET_CHAT=102
    
    #视频会议中的屏幕分享通知(开始,结束)
    MESSAGE_MEET_SCREEN_SHARE =103
    
    #扩展一些群组的操作,用于修改群组名称，或将来扩展属性
    MESSAGE_MODIFY_GROUP=110
    
    #2024.12.27 增加SOS会话信令，信令的sessionId用于对 （SOS定位，SOS视频推流频道名称，SOS录像进行关联）
    MESSAGE_SOS_SESSION=111