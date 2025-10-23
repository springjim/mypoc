package com.mypoc.pttlibrary.internal.network.exception;

public class NoPermissionException extends BaseException {
    public NoPermissionException(int errorCode, String cause) {
        super(errorCode, cause);
    }
}
