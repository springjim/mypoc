package com.mypoc.pttlibrary.api;

/**
 * ptt sdk 的所有事件
 */
public interface IPTTEventListener {
    /**
     * 抢麦成功事件
     */
    void onApplyMicSuccess();

    /**
     * 抢麦失败事件
     * @param reason 失败原因
     */
    void onApplyMicFailed(String reason);


    /**
     * 讲话人麦权超时了的通知
     * @param reason
     */
    void onMicTimeout(String reason);


    /**
     * 当前麦克风被其它APP(高优先级的)占用
     */
    void onMicOccupied();

    /**
     * 组内有人开始讲话
     * @param groupId 组ID
     * @param userId 用户名
     */
    void onUserStartSpeaking(int groupId,  int userId );

    /**
     * 组内有人停止讲话
     * @param groupId 组ID
     * @param userId 用户名
     */
    void onUserStopSpeaking(int groupId,  int userId );

    /**
     * 有人加入当前对讲组
     * @param groupId 组ID
     * @param userId 用户名
     */
    void onUserJoinedGroup(int groupId,  int userId);

    /**
     * 有人离开当前对讲组
     * @param groupId 组ID
     * @param userId 用户名
     */
    void onUserLeftGroup(int groupId,  int userId);


    /**
     * 某用户下线了
     * @param userId
     */
    void onUserOffline(int userId);


    /**
     * 收到某某的建组邀请, 对讲模式
     * @param groupId
     * @param groupName
     * @param userId  被邀请人的ID
     * @param inviteUserId  发出邀请人的ID
     */
    void onGroupInvite(int groupId,String groupName,int userId,int inviteUserId);


    /**
     * 收到对方发来的单呼信令
     * @param groupId
     * @param fromUserId
     * @param toUserId
     * @param signalVal
     */
    void onSingleCallSignal(int groupId, int fromUserId, int toUserId, byte signalVal);


    /**
     * 收到某某的建组邀请, 双向语音模式
     * @param groupId
     * @param groupName
     * @param userId   被邀请人的ID
     * @param inviteUserId  发出邀请人的ID
     * @param inviteMode  邀请模式， 1： 单呼， 2: 群呼
     */
    void onGroupInviteTwoWay(int groupId,String groupName,int userId,int inviteUserId, int inviteMode);

    /**
     * 收到解散组(一般是临时组或广播组)的通知
     * @param groupId
     */
    void onGroupDelete(int groupId);


    /**
     * 收到强插或强拆通知
     * @param groupId    通知的组ID
     * @param groupTypeId   0: 表示固定组, 1:表示临时组
     * @param changeType   0: 表示强拆, 1: 表示强插
     * @param userStr 被强拉或强踢的用户id拼接的字符串，以半角逗号隔开
     */
    void onGroupUserChange(int groupId, int groupTypeId, int changeType, String userStr);

    /**
     *
     * @param videoType  1、语音通话  2、视频通话  3、视频直播 (包括监控，因为监控的原理本质上也是直播)
     * @param videoCommand  * 视频命令
     * 	 * 1: 请求
     * 	 * 2: 应答
     * 	 * 3：拒绝
     * 	 * 4: 挂断 (含已连接时挂断, 未连接时挂断, 不管是主叫方还是被叫方都是这个挂断 )
     * 	 * 5: 对方占线(正在使用中)
     * 	 * 6: 切换前后摄像头
     * @param fromUserId
     * @param toUserId
     * @param fromUserName
     * @param toUserName
     * @param desc
     */
    void onAVChatMessage(short videoType, short videoCommand, int   fromUserId,int   toUserId,String  fromUserName,String  toUserName,String  desc);

    /**
     * 收到组强制同步通知
     * @param groupId
     * @param groupName
     * @param userId    被同步的用户ID，可以用于过滤，是否是本端的用户ID
     * @param inviteId  发出强制同步的用户ID，一般是调度台用户
     */
    void onGroupSync(int groupId, String groupName, int userId, int inviteId);


    /**
     * 收到当前讲话被麦权高的人打断的通知
     * @param userId
     */
    void onSpeakingBreaked(int groupId,  int userId);
    /**
     * 收到语音数据
     * @param userId 用户ID
     * @param audioData 语音数据
     */
    void onAudioDataReceived(String userId, byte[] audioData);


    /**
     * 连接状态变化， 主要是tcp 连接上线或下线通知, app应用可以进行上下线tts播报
     * @param newState 新状态
     */
    void onConnectionStateChanged(PTTState newState);


    /**
     * 被同帐号登录的踢出事件
     */
    void onKickOff();

    void onRemoteKill();

    /**
     * 专用的错误码消息信令
     * @param errorCode
     */
    void onErrorCode(int errorCode);

    /**
     * 错误事件
     * @param error 错误信息
     */
    void onError(String error);

}
