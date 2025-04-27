package com.mypoc.pttlibrary.internal.network.interceptor;

import com.mypoc.pttlibrary.internal.network.PTTRestClient;

import java.io.IOException;

import okhttp3.Interceptor;
import okhttp3.Request;
import okhttp3.Response;

public class RefreshTokenInterceptor implements Interceptor {
    private final PTTRestClient restClient;

    public RefreshTokenInterceptor(PTTRestClient restClient) {
        this.restClient = restClient;
    }

    @Override
    public Response intercept(Chain chain) throws IOException {
        Request request = chain.request();
        Response response = chain.proceed(request);

        // 如果Token过期，尝试刷新
        if (response.code() == 401 && restClient.isTokenExpired()) {
            try {
                // 同步刷新Token
                String newToken = restClient.refreshToken().blockingFirst();

                // 使用新Token重试请求
                Request newRequest = request.newBuilder()
                        .header("token", newToken)
                        .build();

                return chain.proceed(newRequest);
            } catch (Exception e) {
                // 刷新Token失败
                throw new IOException("Failed to refresh token: " + e.getMessage());
            }
        }

        return response;
    }

}
