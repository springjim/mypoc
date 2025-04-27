package com.mypoc.pttlibrary.internal.network;

import android.util.Log;
import android.widget.Toast;

import com.google.gson.JsonParseException;
import com.mypoc.pttlibrary.internal.network.exception.NoDataExceptionException;
import com.mypoc.pttlibrary.internal.network.exception.NoPermissionException;
import com.mypoc.pttlibrary.internal.network.exception.ServerInternalException;
import com.mypoc.pttlibrary.internal.network.exception.ServerResponseException;
import com.mypoc.pttlibrary.internal.network.exception.UnauthorizedException;

import org.json.JSONException;

import java.io.InterruptedIOException;
import java.net.ConnectException;
import java.net.UnknownHostException;
import java.text.ParseException;

import io.reactivex.Observer;
import io.reactivex.disposables.Disposable;
import retrofit2.adapter.rxjava2.HttpException;

public abstract class ResponseObserver<T> implements Observer<T>  {

    private static final String TAG = "ResponseObserver";

    @Override
    public void onSubscribe(Disposable d) {

    }

    @Override
    public void onNext(T response) {
        onSuccess(response);
        onFinish();
    }

    @Override
    public void onError(Throwable e) {
        Log.e(TAG,e.getMessage());
        if (e instanceof HttpException) {     //   HTTP错误
            onException(ExceptionReason.BAD_NETWORK);
        } else if (e instanceof ConnectException
                || e instanceof UnknownHostException) {   //   连接错误
            onException(ExceptionReason.CONNECT_ERROR);
        } else if (e instanceof InterruptedIOException) {   //  连接超时
            onException(ExceptionReason.CONNECT_TIMEOUT);
        } else if (e instanceof JsonParseException
                || e instanceof JSONException
                || e instanceof ParseException) {   //  解析错误
            onException(ExceptionReason.PARSE_ERROR);
        } else if (e instanceof ServerResponseException) {
            onFail(e.getMessage());
        } else if (e instanceof ServerInternalException) {
            //服务端返回的内部错误 500
            onFail(e.getMessage());
        }
        else if (e instanceof NoPermissionException) {
            //调用指定业务没有相应的权限
            onFail(e.getMessage());

        }
        else if (e instanceof UnauthorizedException) {
            //接口未传入token
            onFail(e.getMessage());

        }  else if (e instanceof NoDataExceptionException) {
            //没有业务数据,也回调onSuccess方法, 只不过传入的Response为null, 这个要在调用方去判断null情况
            onSuccess(null);
        } else {

            onException(ExceptionReason.UNKNOWN_ERROR);
        }

        onFinish();

    }

    /**
     * 请求成功
     *
     * @param response 服务器返回的数据
     */
    abstract public void onSuccess(T response);


    public void onFail(String message) {
        Log.e(TAG,"error="+message);
    }

    public void onFinish() {
    }

    @Override
    public void onComplete() {

    }

    /**
     * 请求异常
     *
     * @param reason
     */
    public void onException(ExceptionReason reason) {
        switch (reason) {
            case CONNECT_ERROR:
                Log.e(TAG,"网络连接失败,请检查网络");
                break;

            case CONNECT_TIMEOUT:
                Log.e(TAG,"连接超时，请稍后再试");
                break;

            case BAD_NETWORK:
                Log.e(TAG,"服务器异常");
                break;

            case PARSE_ERROR:
                Log.e(TAG,"解析服务器响应数据失败");
                break;

            case UNKNOWN_ERROR:
            default:
                Log.e(TAG,"未知错误");
                break;
        }
    }

    /**
     * 请求网络失败原因
     */
    public enum ExceptionReason {
        /**
         * 解析数据失败
         */
        PARSE_ERROR,
        /**
         * 网络问题
         */
        BAD_NETWORK,
        /**
         * 连接错误
         */
        CONNECT_ERROR,
        /**
         * 连接超时
         */
        CONNECT_TIMEOUT,
        /**
         * 未知错误
         */
        UNKNOWN_ERROR,
    }

}
