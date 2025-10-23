package com.mypoc.ptt.webrtc.voip;

import android.Manifest;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.media.AsyncPlayer;
import android.media.AudioManager;
import android.net.Uri;
import android.os.Build;
import android.util.Log;
import android.widget.Toast;

import com.dds.base.permission.Permissions;

import com.dds.skywebrtc.SkyEngineKit;

import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.utils.ActivityStackManager;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.ptt.utils.Utils;
import com.mypoc.ptt.webrtc.base.BaseActivity;
import com.tapadoo.alerter.Alerter;

import java.util.ArrayList;
import java.util.Arrays;

/**
 */
public class VoipReceiver extends BroadcastReceiver {
    private static final String TAG = "VoipReceiver";
    private AsyncPlayer ringPlayer;

    @Override
    public void onReceive(Context context, Intent intent) {
        String action = intent.getAction();
        if (Consts.ACTION_VOIP_RECEIVER.equals(action)) {
            String room = intent.getStringExtra("room");
            boolean audioOnly = intent.getBooleanExtra("audioOnly", true);
            String inviteId = intent.getStringExtra("inviteId");
            String inviteUserName = intent.getStringExtra("inviteUserName");
            String userList = intent.getStringExtra("userList");
            String[] list = userList.split(",");
            SkyEngineKit.init(new VoipEvent());

            //todo 处理邀请人名称, 指ptt_user表中的user_name
            if (inviteUserName == null) {
                inviteUserName = PttHelper.findUserName(Integer.valueOf(inviteId));
            }

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                if (Utils.isAppRunningForeground()) {
                    //在前端
                    onForegroundOrBeforeVersionO(room, userList, inviteId, audioOnly, inviteUserName, true);
                } else {
                    //在后台
                    onBackgroundAfterVersionO(room, userList, inviteId, audioOnly, inviteUserName);
                }
            } else {
                onForegroundOrBeforeVersionO(
                        room,
                        userList,
                        inviteId,
                        audioOnly,
                        inviteUserName,
                        Utils.isAppRunningForeground()
                );
            }
        }
    }

    /**
     *
     * @param room
     * @param userList
     * @param inviteId
     * @param audioOnly
     * @param inviteUserName 邀请人姓名
     */
    private void onBackgroundAfterVersionO(
            String room, String userList,
            String inviteId, Boolean audioOnly, String inviteUserName
    ) {

        String[] strArr = userList.split(",");
        ArrayList<String> list = new ArrayList<>(Arrays.asList(strArr));

        SkyEngineKit.init(new VoipEvent());
        BaseActivity activity = (BaseActivity) ActivityStackManager.getInstance().getTopActivity();
        // 权限检测
        String[] per;
        if (audioOnly) {
            per = new String[]{Manifest.permission.RECORD_AUDIO};
        } else {
            per = new String[]{Manifest.permission.RECORD_AUDIO, Manifest.permission.CAMERA};
        }
        boolean hasPermission = Permissions.has(activity, per);
        if (hasPermission) {
            onBackgroundHasPermission(activity, room, list, inviteId, audioOnly, inviteUserName);
        } else {
            //无权限
            CallForegroundNotification notification = new CallForegroundNotification(MyPOCApplication.getInstance());
            notification.sendRequestIncomingPermissionsNotification(
                    activity,
                    room,
                    userList,
                    inviteId,
                    inviteUserName,
                    audioOnly
            );
        }
    }

    private void onBackgroundHasPermission(
            Context context, String room, ArrayList<String> list,
            String inviteId, Boolean audioOnly, String inviteUserName) {
        boolean b = SkyEngineKit.Instance().startInCall(MyPOCApplication.getInstance(), room, inviteId, audioOnly);
        Log.d(TAG, "onBackgroundHasPermission b = " + b);
        if (b) {
            MyPOCApplication.getInstance().setOtherUserId(inviteId);
            if (list.size() == 1) {
                CallForegroundNotification notification = new CallForegroundNotification(MyPOCApplication.getInstance());
                notification.sendIncomingCallNotification(
                        MyPOCApplication.getInstance(),
                        inviteId,
                        false,
                        inviteUserName,
                        audioOnly,
                        true
                );
            }
        }
    }

    private void onForegroundOrBeforeVersionO(
            String room, String userList,
            String inviteId, Boolean audioOnly, String inviteUserName, Boolean isForeGround
    ) {
        String[] strArr = userList.split(",");
        ArrayList<String> list = new ArrayList<>();
        for (String str : strArr)
            list.add(str);
        SkyEngineKit.init(new VoipEvent());
        BaseActivity activity = (BaseActivity) ActivityStackManager.getInstance().getTopActivity();
        // 权限检测
        String[] per;
        if (audioOnly) {
            per = new String[]{Manifest.permission.RECORD_AUDIO};
        } else {
            per = new String[]{Manifest.permission.RECORD_AUDIO, Manifest.permission.CAMERA};
        }
        boolean hasPermission = Permissions.has(activity, per);
        Log.d(TAG, "onForegroundOrBeforeVersionO hasPermission = " + hasPermission + ", isForeGround = " + isForeGround);
        if (hasPermission) {
            onHasPermission(activity, room, list, inviteId, audioOnly, inviteUserName);
        } else {

            ringPlayer = new AsyncPlayer(null);
            shouldStartRing(true); //来电先响铃
            if (isForeGround) {
                Alerter.create(activity).setTitle("来电通知")
                        .setText(
                                "您收到" + inviteUserName + "的来电邀请，请允许"
                                        + (audioOnly ? "录音"
                                        : "录音和相机") + "权限来通话"
                        )
                        .enableSwipeToDismiss()
                        .setBackgroundColorRes(R.color.colorAccent) // or setBackgroundColorInt(Color.CYAN)
                        .setDuration(60 * 1000)
                        .addButton("确定", R.style.AlertButtonBgWhite, v -> {
                            Permissions.request(activity, per, integer -> {
                                shouldStopRing();
                                Log.d(TAG, "Permissions.request integer = " + integer);
                                if (integer == 0) { //权限同意
                                    onHasPermission(activity, room, list, inviteId, audioOnly, inviteUserName);
                                } else {
                                    onPermissionDenied(room, inviteId);
                                }
                                Alerter.hide();
                            });
                        })
                        .addButton("取消", R.style.AlertButtonBgWhite, v -> {
                            shouldStopRing();
                            onPermissionDenied(room, inviteId);
                            Alerter.hide();
                        }).show();
            } else {
                CallForegroundNotification notification = new CallForegroundNotification(MyPOCApplication.getInstance());
                notification.sendRequestIncomingPermissionsNotification(
                        activity,
                        room,
                        userList,
                        inviteId,
                        inviteUserName,
                        audioOnly
                );
            }

        }
    }

    private void onHasPermission(
            Context context, String room, ArrayList<String> list,
            String inviteId, Boolean audioOnly, String inviteUserName
    ) {
        boolean b = SkyEngineKit.Instance().startInCall(MyPOCApplication.getInstance(), room, inviteId, audioOnly);
        Log.d(TAG, "onHasPermission b = " + b);
        if (b) {
            MyPOCApplication.getInstance().setOtherUserId(inviteId);
            Log.d(TAG, "onHasPermission list.size() = " + list.size());
            if (list.size() == 1) {
                //以视频电话拨打，切换到音频或重走这里，结束掉上一个，防止对方挂断后，下边还有一个通话界面
                if (context instanceof CallSingleActivity) {
                    ((CallSingleActivity) context).finish();
                }
                CallSingleActivity.openActivity(context, inviteId, false, inviteUserName, audioOnly, true);
            } else {
                // 群聊
            }
        } else {
            Activity activity = ActivityStackManager.getInstance().getTopActivity();
            activity.finish(); //销毁掉刚才拉起的
        }
    }

    // 权限拒绝
    private void onPermissionDenied(String room, String inviteId) {
        SkyEngineKit.Instance().sendRefuseOnPermissionDenied(room, inviteId);//通知对方结束
        Toast.makeText(MyPOCApplication.getInstance(), "权限被拒绝，无法通话", Toast.LENGTH_SHORT).show();
    }

    private void shouldStartRing(boolean isComing) {
        if (isComing) {
            Uri uri = Uri.parse("android.resource://" + MyPOCApplication.getInstance().getPackageName() + "/" + R.raw.income_ring);
            ringPlayer.play(MyPOCApplication.getInstance(), uri, true, AudioManager.STREAM_RING);
        } else {
            Uri uri = Uri.parse("android.resource://" + MyPOCApplication.getInstance().getPackageName() + "/" + R.raw.wr_ringback);
            ringPlayer.play(MyPOCApplication.getInstance(), uri, true, AudioManager.STREAM_RING);
        }
    }

    private void shouldStopRing() {
        Log.d(TAG, "shouldStopRing begin");
        ringPlayer.stop();
    }
}
