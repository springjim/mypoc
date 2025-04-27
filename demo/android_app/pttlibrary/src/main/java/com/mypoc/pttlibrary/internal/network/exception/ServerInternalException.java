package com.mypoc.pttlibrary.internal.network.exception;

/**
 * Server端内部异常,特指为error=500
 */
public class ServerInternalException extends BaseException {

    public ServerInternalException(int errorCode, String cause) {
        super(errorCode, cause);
    }
}
