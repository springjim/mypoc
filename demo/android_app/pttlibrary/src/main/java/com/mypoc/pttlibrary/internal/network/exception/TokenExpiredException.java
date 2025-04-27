
package com.mypoc.pttlibrary.internal.network.exception;

public class TokenExpiredException extends BaseException {
    public TokenExpiredException(int errorCode, String cause) {
        super(errorCode, cause);
    }

}
