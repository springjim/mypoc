
package com.mypoc.pttlibrary.internal.network.exception;


/**
 * Created by zhpan on 2018/3/27.
 */
public class RemoteLoginExpiredException extends BaseException {

    public RemoteLoginExpiredException(int errorCode, String cause) {
        super(errorCode, cause);
    }

}