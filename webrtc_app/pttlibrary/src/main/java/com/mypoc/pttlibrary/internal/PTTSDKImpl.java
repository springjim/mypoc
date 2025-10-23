package com.mypoc.pttlibrary.internal;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Build;
import android.os.IBinder;
import android.util.Log;

import com.mypoc.pttlibrary.api.IPTTEventListener;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.api.PTTConfig;
import com.mypoc.pttlibrary.api.PTTState;
import com.mypoc.pttlibrary.callback.CmpAllUserCallback;
import com.mypoc.pttlibrary.callback.CommonCallback;
import com.mypoc.pttlibrary.callback.GroupInfoCallback;
import com.mypoc.pttlibrary.callback.GroupMembersCallback;
import com.mypoc.pttlibrary.callback.GroupsCallback;
import com.mypoc.pttlibrary.callback.InitializeCallback;
import com.mypoc.pttlibrary.callback.LoginCallback;
import com.mypoc.pttlibrary.callback.LogoutCallback;
import com.mypoc.pttlibrary.callback.ReleaseMicCallback;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;
import com.mypoc.pttlibrary.callback.RequestMicCallback;
import com.mypoc.pttlibrary.callback.TempGroupCreateCallback;
import com.mypoc.pttlibrary.callback.TempGroupDeleteCallback;
import com.mypoc.pttlibrary.callback.UpdateUserLocationCallback;
import com.mypoc.pttlibrary.callback.UserInfoCallback;
import com.mypoc.pttlibrary.internal.network.PTTRestClient;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTGroupMember;
import com.mypoc.pttlibrary.model.PTTUser;

import java.util.List;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

import io.reactivex.disposables.CompositeDisposable;

public class PTTSDKImpl implements IPTTSDK {

    private static final String TAG = "PTTManager";
    private static PTTSDKImpl instance;

    private Context context;
    private PTTServiceConnection serviceConnection;
    private PTTState currentState = PTTState.RESTFUL_LOGIN_FAIL;

    /**
     * 都是监听tcp的接受数据转成的相应事件监听
     */
    private IPTTEventListener eventListener;
    private PTTConfig config;

    private PTTRestClient restClient;
    private String authToken;
    private String currentUserId;  //这里就等于 login时的username

    private final CompositeDisposable disposables = new CompositeDisposable();

    private PTTSDKImpl(Context context) {
        this.context = context.getApplicationContext();
        this.serviceConnection = new PTTServiceConnection();
    }

    public static synchronized PTTSDKImpl getInstance(Context context) {
        if (instance == null) {
            instance = new PTTSDKImpl(context);
        }
        return instance;
    }


    @Override
    public void initialize(PTTConfig config, InitializeCallback callback) {
        this.config = config;
        updateState(PTTState.RESTFUL_LOGIN_FAIL);
        this.restClient = new PTTRestClient(config.getRestServerUrl(), context);
        if (callback != null)
            callback.onSuccess();
    }

    @Override
    public void loginImei(String imei, LoginCallback callback) {

        disposables.add(restClient.loginImei(imei, new LoginCallback() {
            @Override
            public void onSuccess(String token, long expiresIn, String refreshToken, Integer userId) {
                authToken = token;
                //更新pttmanager的状态
                updateState(PTTState.RESTFUL_LOGIN_SUCCESS);
                //开始调用用户信息接口
                currentUserId = userId+"";
                //登录成功后才启动tcp客户端
                // 登录成功后才启动服务, 这里username 代表userId
                startPTTService(config.getTcpServerHost(),
                        config.getTcpServerPort(),
                        config.getHeartbeatIntervalSec() *  1000,
                        config.getMicOwnerTimeoutSec()*1000,
                        userId+"",
                        config.getPttKeyVal(),
                        config.getPttDownBroadCastVal(),
                        config.getPttUpBroadCastVal(),
                        config.isPttUseBroadCastMode()
                );

                if (callback != null)
                    callback.onSuccess(token, expiresIn, refreshToken,userId);
            }

            @Override
            public void onFailure(String error) {
                updateState(PTTState.RESTFUL_LOGIN_FAIL);
                callback.onFailure(error);
            }
        }));

    }

    @Override
    public void login(String username, String password, LoginCallback callback) {
        disposables.add(restClient.login(username, password, new LoginCallback() {
            @Override
            public void onSuccess(String token, long expiresIn, String refreshToken, Integer userId) {
                authToken = token;
                //更新pttmanager的状态
                updateState(PTTState.RESTFUL_LOGIN_SUCCESS);
                //开始调用用户信息接口
                currentUserId = userId+"";
                //登录成功后才启动tcp客户端
                // 登录成功后才启动服务, 这里username 代表userId
                startPTTService(config.getTcpServerHost(),
                        config.getTcpServerPort(),
                        config.getHeartbeatIntervalSec() *  1000,
                        config.getMicOwnerTimeoutSec()*1000,
                        userId+"",
                        config.getPttKeyVal(),
                        config.getPttDownBroadCastVal(),
                        config.getPttUpBroadCastVal(),
                        config.isPttUseBroadCastMode()
                        );

                if (callback != null)
                    callback.onSuccess(token, expiresIn, refreshToken,userId);

            }

            @Override
            public void onFailure(String error) {
                updateState(PTTState.RESTFUL_LOGIN_FAIL);
                callback.onFailure(error);
            }
        }));
    }

    @Override
    public void logout(LogoutCallback callback) {
        if (serviceConnection.getService() != null) {
            serviceConnection.getService().setMicStat(1);
            serviceConnection.getService().logout(callback);
        } else {
            callback.onFailure("Service not ready or not in group");
        }
    }

    @Override
    public PTTUser userinfo() throws InterruptedException, TimeoutException {
        final CountDownLatch latch = new CountDownLatch(1);
        final PTTUser[] resultInfo = new PTTUser[1];
        final Exception[] errorInfo = new Exception[1];
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            errorInfo[0] = new RuntimeException("未登录rest服务");
            throw new RuntimeException(errorInfo[0]);
        }

        restClient.userInfo(new UserInfoCallback() {

            @Override
            public void onSuccess(PTTUser pttUser) {
                resultInfo[0] = pttUser;
                latch.countDown();
            }

            @Override
            public void onFailure(String error) {
                errorInfo[0] = new RuntimeException(error);
                latch.countDown();
            }
        });
        //
        // 等待最多 10 秒，防止无限阻塞
        if (!latch.await(10, TimeUnit.SECONDS)) {
            throw new TimeoutException("Timeout while fetching user info");
        }

        if (errorInfo[0] != null) {
            throw new RuntimeException(errorInfo[0]);
        }

        return resultInfo[0];

    }


    //异步调用，废除掉
    /*
    public void userinfoAsync(UserInfoCallback callback) {

        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            if (callback != null) {
                callback.onFailure("未登录rest服务");
            }
            return;
        }

        disposables.add(restClient.userInfo(new UserInfoCallback() {

            @Override
            public void onSuccess(PTTUser pttUser) {
                if (callback != null) {
                    callback.onSuccess(pttUser);
                }
            }

            @Override
            public void onFailure(String error) {
                if (callback != null) {
                    callback.onFailure("获取登录用户信息失败," + error);
                }
            }
        }));

    }
    */

    @Override
    public PTTGroup queryGroupInfo(int groupId) throws InterruptedException, TimeoutException {
        final CountDownLatch latch = new CountDownLatch(1);
        final PTTGroup[] resultInfo = new PTTGroup[1];
        final Exception[] errorInfo = new Exception[1];
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            errorInfo[0] = new RuntimeException("未登录rest服务");
            throw new RuntimeException(errorInfo[0]);
        }
        restClient.groupInfo(true, groupId, new GroupInfoCallback() {

            @Override
            public void onSuccess(PTTGroup pttGroup) {
                resultInfo[0] = pttGroup;
                latch.countDown();
            }

            @Override
            public void onFailure(String error) {
                errorInfo[0] = new RuntimeException(error);
                latch.countDown();
            }
        });

        // 等待最多 10 秒，防止无限阻塞
        if (!latch.await(10, TimeUnit.SECONDS)) {
            throw new TimeoutException("Timeout while fetching user info");
        }

        if (errorInfo[0] != null) {
            throw new RuntimeException(errorInfo[0]);
        }

        return resultInfo[0];

    }


//异步调用，废除掉
/*
    public void queryGroupInfoAsync(int groupId, GroupInfoCallback callback) {
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL))
        {
            if (callback!=null){
                callback.onFailure("未登录rest服务");
            }
            return;
        }

        disposables.add(restClient.groupInfo(true,groupId,  new GroupInfoCallback() {

            @Override
            public void onSuccess(PTTGroup pttGroup) {
                if (callback!=null){
                    callback.onSuccess(pttGroup);
                }
            }

            @Override
            public void onFailure(String error) {
                if (callback!=null){
                    callback.onFailure("获取失败,"+error);
                }
            }
        }));

    }
    */

    @Override
    public PTTGroup queryTempGroupInfo(int groupId) throws InterruptedException, TimeoutException {

        final CountDownLatch latch = new CountDownLatch(1);
        final PTTGroup[] resultInfo = new PTTGroup[1];
        final Exception[] errorInfo = new Exception[1];
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            errorInfo[0] = new RuntimeException("未登录rest服务");
            throw new RuntimeException(errorInfo[0]);
        }
        restClient.groupInfo(false, groupId, new GroupInfoCallback() {

            @Override
            public void onSuccess(PTTGroup pttGroup) {
                resultInfo[0] = pttGroup;
                latch.countDown();
            }

            @Override
            public void onFailure(String error) {
                errorInfo[0] = new RuntimeException(error);
                latch.countDown();
            }
        });

        // 等待最多 10 秒，防止无限阻塞
        if (!latch.await(10, TimeUnit.SECONDS)) {
            throw new TimeoutException("Timeout while fetching user info");
        }

        if (errorInfo[0] != null) {
            throw new RuntimeException(errorInfo[0]);
        }

        return resultInfo[0];
    }

    //异步调用，废除掉
    /*
    public void queryTempGroupInfoAsync(int groupId, GroupInfoCallback callback) {
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL))
        {
            if (callback!=null){
                callback.onFailure("未登录rest服务");
            }
            return;
        }

        disposables.add(restClient.groupInfo(false,groupId,  new GroupInfoCallback() {

            @Override
            public void onSuccess(PTTGroup pttGroup) {
                if (callback!=null){
                    callback.onSuccess(pttGroup);
                }
            }

            @Override
            public void onFailure(String error) {
                if (callback!=null){
                    callback.onFailure("获取失败,"+error);
                }
            }
        }));
    }
    */

    @Override
    public void queryCmpAllUsers(CmpAllUserCallback callback) {

        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            if (callback != null) {
                callback.onFailure("未登录rest服务");
            }
            return;
        }

        disposables.add(restClient.getCmpAllUsers(new CmpAllUserCallback() {

            @Override
            public void onSuccess(List<PTTUser> pttCmpUsers) {
                if (callback != null) {
                    callback.onSuccess(pttCmpUsers);
                }
            }

            @Override
            public void onFailure(String error) {
                if (callback != null) {
                    callback.onFailure("获取失败," + error);
                }
            }
        }));
    }

    @Override
    public void tempGroupDelete(int groupId, int type, int priv, TempGroupDeleteCallback callback) {

        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            if (callback != null) {
                callback.onFailure("未登录rest服务");
            }
            return;
        }

        disposables.add(restClient.tempGroupDelete(groupId,type,priv, new TempGroupDeleteCallback() {

            @Override
            public void onSuccess() {
                if (callback != null) {
                    callback.onSuccess();
                }
            }

            @Override
            public void onFailure(String error) {
                if (callback != null) {
                    callback.onFailure("获取失败," + error);
                }
            }
        }));

    }

    @Override
    public void tempGroupCreate(String groupName, int ownerId, String userIds, TempGroupCreateCallback callback) {
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            if (callback != null) {
                callback.onFailure("未登录rest服务");
            }
            return;
        }

        disposables.add(restClient.tempGroupCreate(groupName,ownerId,userIds, new TempGroupCreateCallback() {

            @Override
            public void onSuccess(PTTGroup group) {
                if (callback != null) {
                    callback.onSuccess(group);
                }
            }

            @Override
            public void onFailure(String error) {
                if (callback != null) {
                    callback.onFailure("获取失败," + error);
                }
            }
        }));
    }

    @Override
    public void updateUserLocation(Integer groupId, Double latitude, Double longitude, Integer batterylevel, Integer savelocday,
                                   UpdateUserLocationCallback callback) {

        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            if (callback != null) {
                callback.onFailure("未登录rest服务");
            }
            return;
        }

        disposables.add(restClient.updateUserLocation(groupId,latitude, longitude,batterylevel,savelocday, new UpdateUserLocationCallback() {

            @Override
            public void onSuccess() {
                if (callback != null) {
                    callback.onSuccess();
                }
            }

            @Override
            public void onFailure(String error) {
                if (callback != null) {
                    callback.onFailure("获取失败," + error);
                }
            }
        }));

    }

    @Override
    public List<PTTGroup> queryFixGroups() throws InterruptedException, TimeoutException {
        final CountDownLatch latch = new CountDownLatch(1);
        final List<PTTGroup>[] resultInfo = (List<PTTGroup>[]) new List<?>[1];
        final Exception[] errorInfo = new Exception[1];
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            errorInfo[0] = new RuntimeException("未登录rest服务");
            throw new RuntimeException(errorInfo[0]);
        }
        restClient.getFixGroups( new GroupsCallback() {

            @Override
            public void onSuccess(List<PTTGroup>  pttGroups) {
                resultInfo[0] = pttGroups;
                latch.countDown();
            }

            @Override
            public void onFailure(String error) {
                errorInfo[0] = new RuntimeException(error);
                latch.countDown();
            }
        });

        // 等待最多 10 秒，防止无限阻塞
        if (!latch.await(10, TimeUnit.SECONDS)) {
            throw new TimeoutException("Timeout while fetching user info");
        }

        if (errorInfo[0] != null) {
            throw new RuntimeException(errorInfo[0]);
        }

        return resultInfo[0];
    }

    @Override
    public List<PTTGroup> queryTempGroups() throws InterruptedException, TimeoutException {
        final CountDownLatch latch = new CountDownLatch(1);
        final List<PTTGroup>[] resultInfo = (List<PTTGroup>[]) new List<?>[1];
        final Exception[] errorInfo = new Exception[1];
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            errorInfo[0] = new RuntimeException("未登录rest服务");
            throw new RuntimeException(errorInfo[0]);
        }
        restClient.getTempGroups( new GroupsCallback() {

            @Override
            public void onSuccess(List<PTTGroup>  pttGroups) {
                resultInfo[0] = pttGroups;
                latch.countDown();
            }

            @Override
            public void onFailure(String error) {
                errorInfo[0] = new RuntimeException(error);
                latch.countDown();
            }
        });

        // 等待最多 10 秒，防止无限阻塞
        if (!latch.await(10, TimeUnit.SECONDS)) {
            throw new TimeoutException("Timeout while fetching user info");
        }

        if (errorInfo[0] != null) {
            throw new RuntimeException(errorInfo[0]);
        }

        return resultInfo[0];
    }

    @Override
    public List<PTTGroupMember> queryFixGroupMembers(int groupId) throws InterruptedException, TimeoutException {
        final CountDownLatch latch = new CountDownLatch(1);
        final List<PTTGroupMember>[] resultInfo = (List<PTTGroupMember>[]) new List<?>[1];
        final Exception[] errorInfo = new Exception[1];
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            errorInfo[0] = new RuntimeException("未登录rest服务");
            throw new RuntimeException(errorInfo[0]);
        }
        restClient.getFixGroupMembers(groupId, new GroupMembersCallback() {

            @Override
            public void onSuccess(List<PTTGroupMember>  pttGroups) {
                resultInfo[0] = pttGroups;
                latch.countDown();
            }

            @Override
            public void onFailure(String error) {
                errorInfo[0] = new RuntimeException(error);
                latch.countDown();
            }
        });

        // 等待最多 10 秒，防止无限阻塞
        if (!latch.await(10, TimeUnit.SECONDS)) {
            throw new TimeoutException("Timeout while fetching");
        }

        if (errorInfo[0] != null) {
            throw new RuntimeException(errorInfo[0]);
        }

        return resultInfo[0];
    }

    @Override
    public List<PTTGroupMember> queryTempGroupMembers(int groupId) throws InterruptedException, TimeoutException {
        final CountDownLatch latch = new CountDownLatch(1);
        final List<PTTGroupMember>[] resultInfo = (List<PTTGroupMember>[]) new List<?>[1];
        final Exception[] errorInfo = new Exception[1];
        if (getCurrentState().equals(PTTState.RESTFUL_LOGIN_FAIL)) {
            errorInfo[0] = new RuntimeException("未登录rest服务");
            throw new RuntimeException(errorInfo[0]);
        }
        restClient.getTempGroupMembers(groupId, new GroupMembersCallback() {

            @Override
            public void onSuccess(List<PTTGroupMember>  pttGroups) {
                resultInfo[0] = pttGroups;
                latch.countDown();
            }

            @Override
            public void onFailure(String error) {
                errorInfo[0] = new RuntimeException(error);
                latch.countDown();
            }
        });

        // 等待最多 10 秒，防止无限阻塞
        if (!latch.await(10, TimeUnit.SECONDS)) {
            throw new TimeoutException("Timeout while fetching");
        }

        if (errorInfo[0] != null) {
            throw new RuntimeException(errorInfo[0]);
        }

        return resultInfo[0];
    }


    /////////////////////////////

    private void startPTTService(String host, int port, long heatbeatMs, long micOwnerTimeoutSec,
                                 String userId,
                                 int pttKeyVal, String pttDownBroadCastVal,
                                 String pttUpBroadCastVal,
                                 Boolean pttUseBroadCastMode) {

        Intent serviceIntent = new Intent(context, PTTService.class);
        serviceIntent.putExtra("server_host", host);
        serviceIntent.putExtra("server_port", port);
        serviceIntent.putExtra("user_id", userId);
        serviceIntent.putExtra("heartbeat_interval_ms", heatbeatMs);
        serviceIntent.putExtra("micowner_timeout_ms",micOwnerTimeoutSec);
        serviceIntent.putExtra("pttkey_val",pttKeyVal);
        serviceIntent.putExtra("pttdown_broadcast_val",pttDownBroadCastVal);
        serviceIntent.putExtra("pttup_broadcast_val",pttUpBroadCastVal);
        serviceIntent.putExtra("pttuse_broadcast_mode",pttUseBroadCastMode);

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            context.startForegroundService(serviceIntent);
        } else {
            context.startService(serviceIntent);
        }

        // 绑定服务
        boolean bindResult = context.bindService(serviceIntent, serviceConnection, Context.BIND_AUTO_CREATE);
        if (!bindResult) {
            Log.e(TAG, "Bind service failed");
            updateState(PTTState.ERROR);
        }
    }


    private void updateState(PTTState newPTTState) {
        this.currentState = newPTTState;
        if (eventListener != null) {
            eventListener.onConnectionStateChanged(newPTTState);
        }
    }

    private class PTTServiceConnection implements ServiceConnection {
        private PTTService pttService;

        @Override
        public void onServiceConnected(ComponentName name, IBinder service) {
            PTTService.LocalBinder binder = (PTTService.LocalBinder) service;
            pttService = binder.getService();

            // 设置事件监听
            if (eventListener != null) {
                pttService.setEventListener(eventListener);
            }

            //updateState(PTTState.TCP_CONNECTED);
        }

        @Override
        public void onServiceDisconnected(ComponentName name) {
            pttService = null;
            //updateState(PTTState.TCP_DISCONNECTED);
        }

        public PTTService getService() {
            return pttService;
        }
    }

    ///////////////////////////////////////////////////////////////////////
    ////////外部传入的方法调用
    ///////////////////////////////////////////////////////////////////////
    @Override
    public void joinGroup(int groupId, ReportWorkGroupCallback callback) {
        if (serviceConnection.getService() != null) {
            serviceConnection.getService().joinGroup(groupId, callback);
        } else {
            callback.onFailure("Service not ready");
        }
    }

    @Override
    public void requestMicrophone(RequestMicCallback callback) {

        if (serviceConnection.getService() != null) {

            boolean pttValid= serviceConnection.getService().isPttValid();
            if (!pttValid){
                Log.i(TAG,"PTT键不可用，可能是在国标对讲期间");
                return;
            }
            serviceConnection.getService().setMicStat(1);
            serviceConnection.getService().requestMicrophone(callback);

        } else {
            callback.onFailure("Service not ready or not in group");
        }
    }

    @Override
    public void releaseMicrophone(ReleaseMicCallback callback) {
        if (serviceConnection.getService() != null) {
            serviceConnection.getService().setMicStat(0);
            serviceConnection.getService().releaseMicrophone(callback);

        } else {
            callback.onFailure("Service not ready or not in group");
        }
    }

    @Override
    public void sendSingleCallSignal(int groupId, int fromUserId, int toUserId, byte signalVal, CommonCallback callback) {
        if (serviceConnection.getService()!=null) {

            serviceConnection.getService().sendSingleCallSignal(groupId,fromUserId,toUserId,signalVal,callback);

        } else {
            callback.onFailure("Service not ready");
        }
    }

    @Override
    public void sendSOSSession(int groupId, int userId, String sessionIdStr, byte state,CommonCallback callback) {
        if (serviceConnection.getService()!=null) {

            serviceConnection.getService().sendSOSSession(groupId,userId,sessionIdStr,state,callback);

        } else {
            callback.onFailure("Service not ready");
        }
    }

    @Override
    public void sendSOSLocation(int groupId, int userId, String sessionIdStr, byte sosType, String gpsType, double longitude, double latitude, CommonCallback callback) {
        if (serviceConnection.getService()!=null) {

            serviceConnection.getService().sendSOSLocation(groupId,userId,sessionIdStr,sosType,gpsType,longitude,latitude,callback);

        } else {
            callback.onFailure("Service not ready");
        }
    }

    @Override
    public void openTwoWay() {
        if (serviceConnection.getService() != null) {
            serviceConnection.getService().openTwoWayRecording();

        } else {
            Log.e(TAG,"Service not ready");
        }
    }

    @Override
    public void closeTwoWay() {
        if (serviceConnection.getService() != null) {
            serviceConnection.getService().closeTwoWayRecording();
        } else {
            Log.e(TAG,"Service not ready");
        }
    }

    @Override
    public void setPttValid(boolean pttValidStatus) {
        if (serviceConnection.getService() != null) {
            serviceConnection.getService().setPttValid(pttValidStatus);
            Log.i(TAG,"set ok");
        } else {

            Log.e(TAG,"Service not ready");
        }
    }

    @Override
    public PTTState getCurrentState() {
        return currentState;
    }

    @Override
    public void setEventListener(IPTTEventListener listener) {
        this.eventListener = listener;
        if (serviceConnection.getService() != null) {
            serviceConnection.getService().setEventListener(listener);
        }
    }

    @Override
    public void release() {
        Log.e(TAG,"清理SDK资源");
        try {
            disposables.clear(); // 清理所有订阅
            context.unbindService(serviceConnection);
            Intent pttServiceIntent= new Intent(context, PTTService.class);
            context.stopService(pttServiceIntent);
            Log.e(TAG,"清理SDK资源,停止服务");
            instance = null;
        } catch (Exception e){

        }

    }
}
