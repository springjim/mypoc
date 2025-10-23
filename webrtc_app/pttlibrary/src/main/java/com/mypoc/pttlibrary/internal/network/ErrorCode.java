package com.mypoc.pttlibrary.internal.network;

import android.util.Log;

/**
 * 这个要求与 服务端定义要一致
 */
public class ErrorCode {
    /**
     * request success
     */
    public static final int SUCCESS = 0;

    /**
     * 当接口应该有业务数据的,但为空时会触发这个
     */
    public static final int REQUEST_FAILED = -1;

    /**
     * access token过期错误码
     */
    public static final int TOKEN_EXPIRED = -2001;
    /**
     * refresh token过期错误码
     */
    public static final int REFRESH_TOKEN_EXPIRED = -2002;


    /**
     * 需要token凭证,请在header或query中传入token
     */
    public static final int UNAUTHORIZED = 401;

    /**
     * 用户没有相应权限, 如没有视频直播权限
     */
    public static final int NO_PERMISSION = 402;

    /**
     * 服务端内部错误
     */
    public static final int SERVER_ERROR = 500;

    /**
     * 登录状态失效
     */
    public static final int INVALID_LOGIN_STATUS = -1001;
    public static final int VERIFY_CODE_ERROR = 110011;
    public static final int VERIFY_CODE_EXPIRED = 110010;
    public static final int ACCOUNT_NOT_REGISTER = 110009;
    public static final int PASSWORD_ERROR = 110012;

    /**
     * Wrong old password
     */
    public static final int OLD_PASSWORD_ERROR = 110015;

    public static final int USER_REGISTERED = 110006;

    public static final int PARAMS_ERROR = 19999;
    /**
     * 异地登录
     */
    public static final int REMOTE_LOGIN = 91011;

    public static String getErrorMessage(int errorCode) {
        return getErrorMessage(errorCode, "");
    }

    /**
     * get error message with error code
     *
     * @param errorCode error code
     * @return error message
     */
    public static String getErrorMessage(int errorCode, String errorMsg) {
        String message;
        switch (errorCode) {
            case REQUEST_FAILED:
                message = "errorCode:" + errorCode + ",Error Message:" + errorMsg;
                break;
            case VERIFY_CODE_ERROR:
                message = "输入验证码有误";
                break;
            case INVALID_LOGIN_STATUS:
                message =  "登录状态已失效, 请重新登录";
                break;
            case VERIFY_CODE_EXPIRED:
                message = "验证码已过期";
                break;
            case ACCOUNT_NOT_REGISTER:
                message = "该帐户未注册";
                break;
            case PASSWORD_ERROR:
                message = "用户名或密码错误";
                break;
            case USER_REGISTERED:
                message = "该帐户已存在";
                break;
            case OLD_PASSWORD_ERROR:
                message ="密码错误";
                break;
            case PARAMS_ERROR:
                message = "参数错误";
                break;
            case REMOTE_LOGIN:
                message =  "您的账号已在其它设备上登录，如非本人操作，请及时修改密码";
                break;
            case SERVER_ERROR:
                message = "Server端发现异常,请联系管理员";
                Log.i("ErrorCode",errorMsg);
                break;
            case UNAUTHORIZED:
                message = "需要token凭证,请在header或query中传入token";
                break;
            case NO_PERMISSION:
                message = "没有相应的权限, 请联系相应管理员";
                break;
            default:
                message =  "错误码:"+ errorCode+",错误描述:"+ errorMsg;
                break;
        }
        return message;
    }

}
