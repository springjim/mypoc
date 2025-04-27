package com.mypoc.pttlibrary.api;

import com.mypoc.pttlibrary.callback.CmpAllUserCallback;
import com.mypoc.pttlibrary.callback.GroupInfoCallback;
import com.mypoc.pttlibrary.callback.GroupsCallback;
import com.mypoc.pttlibrary.callback.InitializeCallback;
import com.mypoc.pttlibrary.callback.LoginCallback;
import com.mypoc.pttlibrary.callback.LogoutCallback;
import com.mypoc.pttlibrary.callback.ReleaseMicCallback;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;
import com.mypoc.pttlibrary.callback.RequestMicCallback;
import com.mypoc.pttlibrary.callback.TempGroupCreateCallback;
import com.mypoc.pttlibrary.callback.TempGroupDeleteCallback;
import com.mypoc.pttlibrary.callback.UserInfoCallback;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTGroupMember;
import com.mypoc.pttlibrary.model.PTTUser;

import java.util.List;
import java.util.concurrent.TimeoutException;

public interface IPTTSDK {

    /**
     * 初始化PTT SDK
     * @param config 配置参数
     * @param callback 初始化回调
     */
    void initialize(PTTConfig config, InitializeCallback callback);

    /**
     * 登录PTT系统, 请求rest服务
     * @param username 用户名
     * @param password 密码
     * @param callback 登录回调
     */
    void login(String username, String password, LoginCallback callback);


    /**
     * 登出socket服务，这里特指向tcp服务端发送登出信令
     */
    void logout(LogoutCallback callback);

    /**
     * 需登录后，获取登录用户的信息，请求rest服务,
     * 同步方式调用
     */
    PTTUser userinfo() throws InterruptedException, TimeoutException;

    /**
     * 需登录后，获取指定固定群组的信息，请求rest服务
     * 同步方式调用
     * @param groupId
     */
    PTTGroup  queryGroupInfo(int groupId) throws InterruptedException, TimeoutException;


    /**
     * 需登录后，获取指定临时群组的信息，请求rest服务
     * 同步方式调用
     * @param groupId
     */
    PTTGroup queryTempGroupInfo(int groupId) throws InterruptedException, TimeoutException;

    /**
     * 需登录后，获取用户参与的固定群组，请求rest服务
     * 同步方式调用
     * @return
     * @throws InterruptedException
     * @throws TimeoutException
     */
    List<PTTGroup> queryFixGroups() throws InterruptedException, TimeoutException ;

    /**
     * 需登录后，获取用户参与的临时群组，请求rest服务
     * 同步方式调用
     * @return
     * @throws InterruptedException
     * @throws TimeoutException
     */
    List<PTTGroup> queryTempGroups() throws InterruptedException, TimeoutException ;


    /**
     * 需登录后，获取指定固定组ID的组成员，一次性获取，没有分页，请求rest服务
     * @return
     * @throws InterruptedException
     * @throws TimeoutException
     */
    List<PTTGroupMember> queryFixGroupMembers(int groupId) throws InterruptedException, TimeoutException ;



    /**
     * 需登录后，获取指定临时组ID的组成员，一次性获取，没有分页，请求rest服务
     * @return
     * @throws InterruptedException
     * @throws TimeoutException
     */
    List<PTTGroupMember> queryTempGroupMembers(int groupId) throws InterruptedException, TimeoutException ;

    /**
     * 需登录后，获取同一个cmpid下的所有人员，请求rest服务，注意：目前没有提供分页查询
     * @param callback
     */
    void queryCmpAllUsers(CmpAllUserCallback callback);


    /**
     * 删除临时组
     * @param groupId 临时组ID
     * @param type 发送状态类型，要写7：表示删除方离开
     * @param priv  1:调度台发送的，终端不用发，终端发0
     */
    void tempGroupDelete(int groupId, int type, int priv, TempGroupDeleteCallback callback);


    /**
     * 创建临时组
     * @param groupName
     * @param ownerId
     * @param userIds
     * @param callback
     */
    void tempGroupCreate(String groupName, int ownerId, String userIds, TempGroupCreateCallback callback);

    /**
     * 加入对讲组
     * @param groupId 组ID
     * @param callback 加入回调
     */
    void joinGroup(int groupId, ReportWorkGroupCallback callback);

    /**
     * 请求抢麦
     * @param callback 抢麦回调
     */
    void requestMicrophone(RequestMicCallback callback);

    /**
     * 释放麦克风
     * @param callback 释放回调
     */
    void releaseMicrophone(ReleaseMicCallback callback);

    /**
     * 获取当前状态
     * @return 当前状态
     */
    PTTState getCurrentState();

    /**
     * 设置事件监听器
     * @param listener 监听器
     */
    void setEventListener(IPTTEventListener listener);

    /**
     * 释放资源
     */
    void release();

}
