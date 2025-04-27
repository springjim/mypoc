package com.mypoc.pttlibrary.internal.network;

public class BasicResponse<T> {
    /**
     * code 是为了兼容 对讲系统 web管理端用的，它的值与 error一样
     */
    private int code;
    private int error;
    private String errorMsg;
    private T data;

    // 判断是否成功
    public boolean isSuccess() {
        return error == 0;
    }



    public T getData() {
        return data;
    }

    public void setData(T results) {
        this.data = results;
    }

    public int getCode() {
        return code;
    }

    public void setCode(int code) {
        this.code = code;
    }

    public int getError() {
        return error;
    }

    public void setError(int error) {
        this.error = error;
    }

    public String getErrorMsg() {
        return errorMsg;
    }

    public void setErrorMsg(String message) {
        this.errorMsg = message;
    }
}
