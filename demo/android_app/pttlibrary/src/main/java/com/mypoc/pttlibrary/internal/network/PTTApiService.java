package com.mypoc.pttlibrary.internal.network;

import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTGroupMember;
import com.mypoc.pttlibrary.model.PTTUser;
import com.mypoc.pttlibrary.model.request.CreateTempGroupRequest;
import com.mypoc.pttlibrary.model.request.LoginRequest;
import com.mypoc.pttlibrary.model.response.LoginResponse;

import java.util.List;
import java.util.Optional;

import io.reactivex.Maybe;
import io.reactivex.Observable;
import retrofit2.Response;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.Query;

public interface PTTApiService {

    /**
     * 登录
     * @param request
     * @return
     */
    @POST("app/login")
    Observable<LoginResponse> login(@Body LoginRequest request);

    /**
     * 刷新token
     * @param refreshToken
     * @return
     */
    @GET("app/refresh-token")
    Observable<LoginResponse> refreshToken(@Query("refreshToken") String refreshToken);

    /**
     * 获取登录用户的信息
     * @return
     */
    @GET("app/user/info")
    Observable<PTTUser> userInfo();

    /**
     * 查询固定组
     * @return
     */
    @GET("app/group/query-belonggroup-by-userid")
    Observable<List<PTTGroup>> queryFixGroups();

    /**
     * 查询临时组
     * @return
     */
    @GET("app/group/query-belongtempgroup-by-userid")
    Observable<List<PTTGroup>> queryTempGroups();

    /**
     * 查询固定组成员-简化版
     * @param groupId
     * @return
     */
    @GET("app/query-group-member-by-groupId")
    Observable<List<PTTGroupMember>>  queryFixGroupMembers(@Query("groupId") int groupId);


    /**
     * 查询临时组成员-简化版
     * @param groupId
     * @return
     */
    @GET("app/get-tmpgroup-member-short")
    Observable<List<PTTGroupMember>>  queryTempGroupMembers(@Query("groupId") int groupId);

    /**
     * 查询固定群组信息
     * @param groupId
     * @return
     */
    @GET("app/group/query")
    Observable<PTTGroup> queryGroupInfo(@Query("groupId") int groupId);

    /**
     * 查询临时群组信息
     * @param groupId
     * @return
     */
    @GET("app/group/tmp-query")
    Observable<PTTGroup> queryTempGroupInfo(@Query("groupId") int groupId);

    /**
     * 查询同一个cmpid下的所有通讯录用户，如果太多了，要做分页查询
     * @return
     */
    @GET("app/get-group-customerVo")
    Observable<List<PTTUser>>  queryAllUsersByCmpid();

    //删除临时组
    @GET("app/group/tmp-delete")
    Observable<Object> tempGroupDelete(@Query("groupId") Integer groupId, @Query("type") Integer type, @Query("priv") Integer priv);

    //创建临时组
    @POST("app/group/tmp-create")
    Observable<PTTGroup> tempGroupCreate(@Body CreateTempGroupRequest req);

}
