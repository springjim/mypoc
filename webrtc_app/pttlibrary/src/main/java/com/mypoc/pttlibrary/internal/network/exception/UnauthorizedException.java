package com.mypoc.pttlibrary.internal.network.exception;

public class UnauthorizedException extends BaseException{
    public UnauthorizedException(int errorCode, String cause) {
        super(errorCode, cause);
    }
}
