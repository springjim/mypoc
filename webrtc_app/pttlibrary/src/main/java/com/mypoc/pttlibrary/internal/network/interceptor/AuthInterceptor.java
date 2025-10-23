package com.mypoc.pttlibrary.internal.network.interceptor;

import com.mypoc.pttlibrary.internal.network.PTTRestClient;

import java.io.IOException;

import okhttp3.Interceptor;
import okhttp3.Request;
import okhttp3.Response;

public class AuthInterceptor implements Interceptor {

    private final PTTRestClient restClient;
    private static final String[] NO_AUTH_PATHS = {
            "app/login",
            "app/refresh-token"
    };

    public AuthInterceptor(PTTRestClient restClient) {
        this.restClient = restClient;
    }

    @Override
    public Response intercept(Chain chain) throws IOException {

        Request original = chain.request();

        // 检查是否为不需要Token的路径
        if (isNoAuthPath(original.url().encodedPath())) {
            return chain.proceed(addCommonHeaders(original));
        }

        // 获取Token
        String token = restClient.tokenSubject.getValue();
        if (token == null) {
            // 可以在这里选择抛出异常或继续无Token请求
            //throw new IOException("No authentication token available");
        }

        return chain.proceed(
                addCommonHeaders(original)
                        .newBuilder()
                        .header("token", token != null ? token : "") // 确保不为null
                        .build()
        );

    }

    /**
     * 添加通用请求头
     */
    private Request addCommonHeaders(Request request) {
        return request.newBuilder()
                //.header("app_key", "appId")
                .header("Content-Type", "application/json")
                .header("Connection", "close")
                .header("Accept-Encoding", "identity")
                .build();
    }

    /**
     * 判断是否为不需要认证的路径
     */
    private boolean isNoAuthPath(String requestPath) {
        for (String path : NO_AUTH_PATHS) {
            if (requestPath.endsWith(path)) {
                return true;
            }
        }
        return false;
    }

}
