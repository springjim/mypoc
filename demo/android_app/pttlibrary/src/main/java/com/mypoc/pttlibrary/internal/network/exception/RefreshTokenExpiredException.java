
package com.mypoc.pttlibrary.internal.network.exception;

public class RefreshTokenExpiredException extends BaseException {

    public RefreshTokenExpiredException(int errorCode, String cause) {
        super(errorCode, cause);
    }
}
