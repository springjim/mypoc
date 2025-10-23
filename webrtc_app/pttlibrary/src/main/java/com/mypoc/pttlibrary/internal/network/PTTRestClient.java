package com.mypoc.pttlibrary.internal.network;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.mypoc.pttlibrary.callback.CmpAllUserCallback;
import com.mypoc.pttlibrary.callback.GroupInfoCallback;
import com.mypoc.pttlibrary.callback.GroupMembersCallback;
import com.mypoc.pttlibrary.callback.GroupsCallback;
import com.mypoc.pttlibrary.callback.LoginCallback;
import com.mypoc.pttlibrary.callback.TempGroupCreateCallback;
import com.mypoc.pttlibrary.callback.TempGroupDeleteCallback;
import com.mypoc.pttlibrary.callback.UpdateUserLocationCallback;
import com.mypoc.pttlibrary.callback.UserInfoCallback;
import com.mypoc.pttlibrary.internal.converter.BasicResponseConverterFactory;
import com.mypoc.pttlibrary.internal.network.exception.AuthException;
import com.mypoc.pttlibrary.internal.network.interceptor.AuthInterceptor;
import com.mypoc.pttlibrary.internal.network.interceptor.RefreshTokenInterceptor;
import com.mypoc.pttlibrary.model.request.CreateTempGroupRequest;
import com.mypoc.pttlibrary.model.request.LoginRequest;
import com.mypoc.pttlibrary.model.response.LoginResponse;

import java.util.concurrent.TimeUnit;

import io.reactivex.Observable;
import io.reactivex.android.schedulers.AndroidSchedulers;
import io.reactivex.disposables.CompositeDisposable;
import io.reactivex.disposables.Disposable;
import io.reactivex.schedulers.Schedulers;
import io.reactivex.subjects.BehaviorSubject;
import okhttp3.OkHttpClient;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.adapter.rxjava2.RxJava2CallAdapterFactory;
import retrofit2.converter.gson.GsonConverterFactory;

public class PTTRestClient {

    private static final String TAG = "PTTRestClient";
    private static final int MAX_RETRY = 3;

    private final PTTApiService apiService;
    private final SharedPreferences preferences;
    public  final BehaviorSubject<String> tokenSubject;
    private final CompositeDisposable disposables = new CompositeDisposable();

    public PTTRestClient(String baseUrl, Context context) {

        Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd HH:mm:ss").serializeNulls().create();
        HttpLoggingInterceptor loggingInterceptor = getLogInterceptor(HttpLoggingInterceptor.Level.BODY);
        OkHttpClient okHttpClient = new OkHttpClient.Builder()
                .addInterceptor(loggingInterceptor)  //打印日志
                .addInterceptor(new AuthInterceptor(this))  //加token
                .addInterceptor(new RefreshTokenInterceptor(this))  //token过期刷新
                .connectTimeout(10, TimeUnit.SECONDS)
                .readTimeout(30, TimeUnit.SECONDS)
                .writeTimeout(30, TimeUnit.SECONDS)
                .build();

        Retrofit retrofit = new Retrofit.Builder()
                .baseUrl(baseUrl)
                .client(okHttpClient)
                .addConverterFactory(BasicResponseConverterFactory.create(gson))  //自定义gson序列化
                .addCallAdapterFactory(RxJava2CallAdapterFactory.create())
                .build();


        this.apiService = retrofit.create(PTTApiService.class);
        this.preferences = context.getSharedPreferences("PTT_Prefs", Context.MODE_PRIVATE);
        this.tokenSubject = BehaviorSubject.create(); //用于存放token

        // 从SharedPreferences加载token，可以考虑不用
       /* String savedToken = preferences.getString("auth_token", null);
        if (savedToken != null) {
            tokenSubject.onNext(savedToken);
        } else {

        }*/
        tokenSubject.onNext(""); // 初始化为空字符串
    }

    private HttpLoggingInterceptor getLogInterceptor(HttpLoggingInterceptor.Level body) {
        HttpLoggingInterceptor loggingInterceptor = new HttpLoggingInterceptor(new HttpLoggingInterceptor.Logger() {
            @Override
            public void log(String message) {
                Log.i("PTT OKHttp-----", message);
            }
        });
        loggingInterceptor.setLevel(body);
        return loggingInterceptor;
    }

    /**
     * 重要，对于业务接口，调用前要检查
     * @return
     */
    private Observable<String> getCurrentToken() {
        return tokenSubject.firstElement()
                .flatMapObservable(token -> {
                    if (token == null || token.isEmpty()) {
                        return Observable.error(new AuthException("未登录，请先调用登录接口"));
                    }
                    return Observable.just(token);
                })
                .subscribeOn(Schedulers.io());
    }

    /**
     * 刷新Token
     */
    public Observable<String> refreshToken() {
        String refreshToken = preferences.getString("refresh_token", null);
        if (refreshToken == null) {
            return Observable.error(new Throwable("No refresh token available"));
        }

        return apiService.refreshToken(refreshToken)
                .flatMap(response -> {
                    if (response.getAccessToken()!=null) {

                        saveToken(response.getAccessToken(), response.getExpiresIn(),response.getRefreshToken());
                        return Observable.just(response.getAccessToken());
                    } else {
                        return Observable.error(new Throwable("Refresh token failed: "));
                    }
                });
    }

    /**
     * 无屏或iot终端用imei号登录，注意userId 靠该接口返回得知
     * @param imei
     * @param callback
     * @return
     */
    public Disposable loginImei(String imei, LoginCallback callback){
        return apiService.loginimei(imei)
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {
                    if (response.getAccessToken()!=null) {
                        saveToken(response.getAccessToken(), response.getExpiresIn(),response.getRefreshToken());
                        if (callback!=null)
                            callback.onSuccess(response.getAccessToken(), response.getExpiresIn(),response.getRefreshToken(),
                                    response.getUserId()
                            );
                    } else {
                        if (callback!=null)
                            callback.onFailure("登录失败,没有返回token");
                    }
                }, throwable -> {
                    if (callback!=null)
                        callback.onFailure("登录失败,"+throwable.getMessage());
                });
    }

    /**
     * 登录
     */
    public Disposable login(String username, String password, LoginCallback callback) {

        LoginRequest request = new LoginRequest(username, password);

        return apiService.login(request)
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {
                    if (response.getAccessToken()!=null) {
                        saveToken(response.getAccessToken(), response.getExpiresIn(),response.getRefreshToken());
                        if (callback!=null)
                            callback.onSuccess(response.getAccessToken(), response.getExpiresIn(),response.getRefreshToken(),response.getUserId());
                    } else {
                        if (callback!=null)
                            callback.onFailure("登录失败,没有返回token");
                    }
                }, throwable -> {
                    if (callback!=null)
                        callback.onFailure("登录失败,"+throwable.getMessage());
                });


    }


    /**
     * 获取固定组
     * @param callback
     * @return
     */
    public Disposable getFixGroups(GroupsCallback callback){
        return getCurrentToken()
                .flatMap(token -> apiService.queryFixGroups())
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {
                    if (callback!=null)
                        callback.onSuccess(response);
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + throwable.getMessage();
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });

    }

    /**
     * 获取临时组
     * @param callback
     * @return
     */
    public Disposable getTempGroups(GroupsCallback callback){
        return getCurrentToken()
                .flatMap(token -> apiService.queryTempGroups())
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {
                    if (callback!=null)
                        callback.onSuccess(response);
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + throwable.getMessage();
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });

    }

    /**
     * 获取固定组成员列表
     * @param groupId
     * @param callback
     * @return
     */
    public Disposable getFixGroupMembers(int groupId, GroupMembersCallback callback){
        return getCurrentToken()
                .flatMap(token -> apiService.queryFixGroupMembers(groupId))
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {
                    if (callback!=null)
                        callback.onSuccess(response);
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + throwable.getMessage();
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });
    }


    /**
     * 获取临时组成员列表
     * @param groupId
     * @param callback
     * @return
     */
    public Disposable getTempGroupMembers(int groupId, GroupMembersCallback callback){
        return getCurrentToken()
                .flatMap(token -> apiService.queryTempGroupMembers(groupId))
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {
                    if (callback!=null)
                        callback.onSuccess(response);
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + throwable.getMessage();
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });
    }

    /**
     * 该方法要先登录后，再调用
     * @param callback
     * @return
     */
    public Disposable userInfo(UserInfoCallback callback) {
        return  getCurrentToken()
                .flatMap(token -> apiService.userInfo())
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {
                    if (response.getUserId()!=null) {
                        if (callback!=null)
                            callback.onSuccess(response );
                    } else {
                        if (callback!=null)
                            callback.onFailure("failed: ");
                    }
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + Log.getStackTraceString(throwable);
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });
    }


    /**
     *
     * @param fixGroup  true 固定组  false 临时组
     * @param groupId
     * @param callback
     * @return
     */
    public Disposable groupInfo(boolean fixGroup, int groupId, GroupInfoCallback callback) {

        if (fixGroup){
            return  getCurrentToken()
                    .flatMap(token -> apiService.queryGroupInfo(groupId))
                    .subscribeOn(Schedulers.io())
                    .observeOn(AndroidSchedulers.mainThread())
                    .subscribe(response -> {
                        if (response!=null) {
                            if (callback!=null)
                                callback.onSuccess(response);
                        } else {
                            if (callback!=null)
                                callback.onFailure("为空failed");
                        }
                    }, throwable -> {
                        // 统一处理所有错误（包括Token错误）
                        String errorMsg = (throwable instanceof AuthException)
                                ? "认证错误: " + throwable.getMessage()
                                : "错误: " + throwable.getMessage();
                        if (callback!=null)
                            callback.onFailure("error: " + errorMsg);
                    });
        } else {

            return  getCurrentToken()
                    .flatMap(token -> apiService.queryTempGroupInfo(groupId))
                    .subscribeOn(Schedulers.io())
                    .observeOn(AndroidSchedulers.mainThread())
                    .subscribe(response -> {
                        if (response!=null) {
                            if (callback!=null)
                                callback.onSuccess(response );
                        } else {
                            if (callback!=null)
                                callback.onFailure("为空failed");
                        }
                    }, throwable -> {
                        // 统一处理所有错误（包括Token错误）
                        String errorMsg = (throwable instanceof AuthException)
                                ? "认证错误: " + throwable.getMessage()
                                : "错误: " + throwable.getMessage();
                        if (callback!=null)
                            callback.onFailure("error: " + errorMsg);
                    });

        }


    }


    /**
     * 查询cmpid的所有人,相当于通讯录
     * @param callback
     * @return
     */
    public Disposable getCmpAllUsers(CmpAllUserCallback callback) {
        return getCurrentToken()
                .flatMap(token -> apiService.queryAllUsersByCmpid())
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {
                    if (callback!=null)
                        callback.onSuccess(response);
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + throwable.getMessage();
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });
    }

    public Disposable tempGroupDelete(int groupId, int type, int priv, TempGroupDeleteCallback callback ) {
        return getCurrentToken()
                .flatMap(token -> apiService.tempGroupDelete(groupId,type,priv))
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {

                    if (callback!=null)
                        callback.onSuccess();
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + throwable.getMessage();
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });
    }

    //创建临时群组
    public Disposable tempGroupCreate(String groupName, int ownerId, String  userIds, TempGroupCreateCallback callback ) {

        CreateTempGroupRequest request= new CreateTempGroupRequest();
        request.setGroupName(groupName);
        request.setOwnerId(ownerId);
        request.setUserIds(userIds);

        return getCurrentToken()
                .flatMap(token -> apiService.tempGroupCreate(request))
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {

                    if (callback!=null)
                        callback.onSuccess(response);
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + throwable.getMessage();
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });
    }

    //上报GPS定位
    public Disposable updateUserLocation(Integer groupId,Double latitude,Double longitude,Integer batterylevel,Integer savelocday,
                                         UpdateUserLocationCallback callback){
        return getCurrentToken()
                .flatMap(token -> apiService.updateUserLocation(groupId,latitude,longitude, batterylevel,savelocday))
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .subscribe(response -> {

                    if (callback!=null)
                        callback.onSuccess();
                }, throwable -> {
                    // 统一处理所有错误（包括Token错误）
                    String errorMsg = (throwable instanceof AuthException)
                            ? "认证错误: " + throwable.getMessage()
                            : "错误: " + throwable.getMessage();
                    if (callback!=null)
                        callback.onFailure("error: " + errorMsg);
                });
    }


    private void saveToken(String token, long expiresIn, String refreshToken){
        preferences.edit()
                .putString("auth_token", token)
                .putLong("token_expires_at", System.currentTimeMillis() + expiresIn * 1000)
                .putString("refresh_token",refreshToken)
                .apply();
        tokenSubject.onNext(token);
    }

    /**
     * Token是否过期
     */
    public boolean isTokenExpired() {
        long expiresAt = preferences.getLong("token_expires_at", 0);
        return System.currentTimeMillis() >= expiresAt;
    }

    /**
     * 清理资源
     */
    public void dispose() {
        disposables.clear();
    }

}
