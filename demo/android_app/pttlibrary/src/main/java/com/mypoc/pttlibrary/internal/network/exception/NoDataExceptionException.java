package com.mypoc.pttlibrary.internal.network.exception;

import android.util.Log;

/**
 * 服务器返回的异常
 */
public class NoDataExceptionException extends BaseException {
    public NoDataExceptionException() {
        super(-1,"服务器没有返回对应的Data数据，是正常情况的");
        Log.e("NoDataException","服务器没有返回对应的Data数据");
    }
}
