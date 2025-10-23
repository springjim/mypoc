package com.mypoc.pttlibrary.internal.converter;

import android.util.Log;

import com.google.gson.TypeAdapter;
import com.mypoc.pttlibrary.internal.network.BasicResponse;
import com.mypoc.pttlibrary.internal.network.ErrorCode;
import com.mypoc.pttlibrary.internal.network.exception.NoDataExceptionException;
import com.mypoc.pttlibrary.internal.network.exception.NoPermissionException;
import com.mypoc.pttlibrary.internal.network.exception.RefreshTokenExpiredException;
import com.mypoc.pttlibrary.internal.network.exception.ServerInternalException;
import com.mypoc.pttlibrary.internal.network.exception.ServerResponseException;
import com.mypoc.pttlibrary.internal.network.exception.TokenExpiredException;

import java.io.IOException;

import okhttp3.ResponseBody;
import retrofit2.Converter;

final class GsonResponseBodyConverter<T> implements Converter<ResponseBody, Object>  {

    private final TypeAdapter<T> adapter;

    GsonResponseBodyConverter(TypeAdapter<T> adapter) {
        this.adapter = adapter;
    }

    @Override
    public  Object convert(ResponseBody value) throws IOException {

        try {

            BasicResponse response = (BasicResponse) adapter.fromJson(value.charStream());  //正式情况下用
            //
            if (response.getError() == ErrorCode.SUCCESS) {
                if (response.getData() == null) {
                    //有些请求返回没有data, 是正常的
                    Log.e("GsonResponseConverter", ((TypeAdapter<BasicResponse>)adapter).toJson(response));
                    response.setData("");   //这里要对data字段设置, 要不然调用方报错

                }
                return response.getData();
            } else if (response.getError() == ErrorCode.NO_PERMISSION) {

                throw new NoPermissionException(response.getError(), response.getErrorMsg());

            } else if (response.getError() == ErrorCode.TOKEN_EXPIRED) {
                throw new TokenExpiredException(response.getError(), response.getErrorMsg());
            } else if (response.getError() == ErrorCode.REFRESH_TOKEN_EXPIRED) {
                throw new RefreshTokenExpiredException(response.getError(), response.getErrorMsg());
            } else if (response.getError() == ErrorCode.SERVER_ERROR) {
                //Log.e("GsonBodyConverter",response.getErrorMsg());
                throw new ServerInternalException(response.getError(), response.getErrorMsg());
            }else if (response.getError() != ErrorCode.SUCCESS) {
                // 特定 API 的错误，在相应的 DefaultObserver 的 onError 的方法中进行处理
                Log.e("GsonResponseConverter", ((TypeAdapter<BasicResponse>)adapter).toJson(response));

                throw new ServerResponseException(response.getError(), response.getErrorMsg());
            }
        } finally {
            value.close();
        }
        return null;
    }

}
