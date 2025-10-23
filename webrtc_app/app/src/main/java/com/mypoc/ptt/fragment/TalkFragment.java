package com.mypoc.ptt.fragment;

import android.content.Intent;
import android.os.Build;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.R;
import com.mypoc.ptt.enums.PocSessionStatusEnum;
import com.mypoc.ptt.service.FloatingTalkService;
import com.mypoc.pttlibrary.enums.TalkStatusEnum;
import com.mypoc.pttlibrary.event.TalkStatusMessageEvent;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.ReleaseMicCallback;
import com.mypoc.pttlibrary.callback.RequestMicCallback;

import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.greenrobot.eventbus.ThreadMode;

import java.util.concurrent.TimeUnit;

import butterknife.BindView;
import butterknife.ButterKnife;
import io.reactivex.Observable;
import io.reactivex.android.schedulers.AndroidSchedulers;
import io.reactivex.disposables.CompositeDisposable;
import io.reactivex.disposables.Disposable;
import io.reactivex.schedulers.Schedulers;

/**
 * 对讲fragment
 */
public class TalkFragment extends Fragment {

    private static String TAG = "TalkFragment";

    @BindView(R.id.user_account)
    TextView tvUserAccount;

    @BindView(R.id.talk_float_switch)
    Switch switchFloatWindow;

    @BindView(R.id.talk_img)
    ImageView ivTalkImg;

    private boolean isLongPressed = false;
    private final long longPressTimeout = 300L; // 长按时间阈值(毫秒)

    private IPTTSDK pttSDK;
    private CompositeDisposable compositeDisposable = new CompositeDisposable();
    //mic owner apply 申请麦权的超时时间
    private int applyMicOwnerTimeoutMs = 5000;
    private Runnable applyMicOwnerTimeoutTask;
    Handler mHandler;

    private Runnable longPressRunnable = new Runnable() {
        @Override
        public void run() {
            isLongPressed = true;
            doTouchHandler();
        }
    };

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        pttSDK = MyPOCApplication.getInstance().getPttSDK();
    }

    //避免直接操作 Activity 或其他 Fragment, 用于初始化UI组件
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_talk, container, false);
    }

    //UI 创建完成后（替代 onActivityCreated 的现代方式）
    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        ButterKnife.bind(this, view); // 在 onViewCreated 中绑定

        mHandler= new Handler(Looper.getMainLooper());
        // 使用 RxJava 发起异步请求
        fetchUserInfo();
        initTasks();     //初始化一些超时任务
        initListeners(); // 之后初始化事件监听
    }

    private void initTasks() {
        //下面任务，定义麦权申请超时任务，给后面麦权申请时用
        applyMicOwnerTimeoutTask = new Runnable() {
            @Override
            public void run() {

                EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.ApplyTimeout));

                //MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);

                Log.e(TAG, "申请麦权的超时时间");
            }
        };

    }

    private void initListeners() {

        switchFloatWindow.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                if (b){

                    // 发送显示命令
                    Intent intent = new Intent(FloatingTalkService.BROADCAST_ACTION_TOGGLE_FLOATING_WINDOW);
                    intent.putExtra("show", true);
                    requireActivity().sendBroadcast(intent);

                } else {
                    // 发送隐藏命令
                    Intent intent = new Intent(FloatingTalkService.BROADCAST_ACTION_TOGGLE_FLOATING_WINDOW);
                    intent.putExtra("show", false);
                    requireActivity().sendBroadcast(intent);
                }
            }
        });



        //取消点击做法，改用 onTouch事件
        /*ivTalkImg.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

            }
        });*/

        ivTalkImg.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                        // 按下时开始计时
                        isLongPressed = false;
                        mHandler.postDelayed(longPressRunnable, longPressTimeout);
                        return true;
                    case MotionEvent.ACTION_UP:
                        // 抬起时移除回调并触发结束事件
                        mHandler.removeCallbacks(longPressRunnable);
                        if (isLongPressed) {
                            doTouchHandler();
                            isLongPressed = false;
                        }
                        return true;
                    case MotionEvent.ACTION_CANCEL:
                        // 取消时移除回调
                        mHandler.removeCallbacks(longPressRunnable);
                        isLongPressed = false;
                        return true;
                    default:
                        return false;
                }
            }
        });

    }

    /**
     * 专用于当ivTalkImg,长按和长按松开后的抢麦、抢麦失败、放麦的各种业务处理
     */
    private void doTouchHandler(){
        PocSessionStatusEnum currStatus= MyPOCApplication.getInstance().getPocSession();

        switch (currStatus){

            case Idel:

                if (!MyPOCApplication.getInstance().isPttKeyValid()){
                    mHandler.post( () -> {
                        Toast.makeText(requireActivity(),"国标对讲期间不允许抢麦",Toast.LENGTH_LONG).show();
                    });
                    return;
                };

                //可以抢麦
                pttSDK.requestMicrophone(new RequestMicCallback() {
                    @Override
                    public void onSuccess() {
                        //超时执行的任务
                        mHandler.postDelayed(applyMicOwnerTimeoutTask, applyMicOwnerTimeoutMs);
                        //发送等待事件
                        EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Applying));
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"发送抢麦报错:"+error);
                    }
                });

                break;
            case Appling:
                //已经在抢麦中了，等待超时，或等服务返回结果
                Log.i(TAG,"已经在抢麦中了，等待超时，或等服务返回结果");
                break;
            case Speaking:
                //我方说话中, 可以释放麦
                pttSDK.releaseMicrophone(new ReleaseMicCallback() {
                    @Override
                    public void onSuccess() {
                        //发送等待事件
                        EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Idel));
                        MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"发送释放麦报错:"+error);
                        MyPOCApplication.getInstance().setPocSession(PocSessionStatusEnum.Idel);
                    }
                });


                break;
            case Listening:
                if (!MyPOCApplication.getInstance().isPttKeyValid()){
                    mHandler.post( () -> {
                        Toast.makeText(requireActivity(),"国标对讲期间不允许抢麦",Toast.LENGTH_LONG).show();
                    });
                    return;
                };
                //别人在讲, 麦权高的人可以打断
                pttSDK.requestMicrophone(new RequestMicCallback() {
                    @Override
                    public void onSuccess() {
                        //超时执行的任务
                        mHandler.postDelayed(applyMicOwnerTimeoutTask, applyMicOwnerTimeoutMs);
                        //发送等待事件
                        EventBus.getDefault().post(new TalkStatusMessageEvent(TalkStatusEnum.Applying));
                    }

                    @Override
                    public void onFailure(String error) {
                        Log.e(TAG,"发送抢麦报错:"+error);
                    }
                });

                break;
            case Breaking:
                //正在被打断中
                break;

            default:
                break;
        }
    }

    private void fetchUserInfo() {

        //延迟几秒让登录初始化成功
        Disposable disposable = Observable.timer(2, TimeUnit.SECONDS)
                .flatMap(ignore -> Observable.fromCallable(() -> pttSDK.userinfo()))
                .subscribeOn(Schedulers.io()) // 指定异步线程
                .observeOn(AndroidSchedulers.mainThread()) // 切换回主线程
                .subscribe(
                        pttUser -> {
                            // 请求成功
                            if (pttUser == null) {
                                Log.e(TAG, "获取userinfo失败");
                                return;
                            }
                            // 更新 UI
                            tvUserAccount.setText(
                                    MyPOCApplication.getInstance().getPhone() + "[" +
                                            MyPOCApplication.getInstance().getUserName() + "]"
                            );
                        },
                        throwable -> {
                            // 请求失败
                            Log.e(TAG, "Error fetching data", throwable);
                            if (getActivity() != null) {
                                Toast.makeText(
                                        getActivity(),
                                        "Error: " + throwable.getMessage(),
                                        Toast.LENGTH_SHORT
                                ).show();
                            }
                        }
                );
        compositeDisposable.add(disposable); // 统一管理 Disposable
    }

    @Override
    public void onStart() {
        super.onStart();
        EventBus.getDefault().register(this);
    }

    //这里只对ivtalk图标进行同步
    @Subscribe(threadMode = ThreadMode.MAIN)
    public void onTalkStatusMessage(TalkStatusMessageEvent messageEvent) {
        //
        Log.i(TAG, "messageEvent=" + messageEvent);
        TalkStatusEnum talkStatus= messageEvent.getStatus();
        switch (talkStatus){

            case ListenStart:
                ivTalkImg.setImageResource(R.mipmap.btn_talk_listening);
                break;

            case Applying:
                if (MyPOCApplication.getInstance().getPocSession().equals(PocSessionStatusEnum.Listening)){
                    //当前正在监听中
                } else {
                    ivTalkImg.setImageResource(R.mipmap.btn_talk_press);
                }

                break;

            case ListenStop:
            case Idel:
                ivTalkImg.setImageResource(R.mipmap.btn_talk_idle);
                break;


            case ApplyFail:
                PocSessionStatusEnum currPocSessionStatus= MyPOCApplication.getInstance().getPocSession();
                Log.e(TAG,"当前currPocSessionStatus="+currPocSessionStatus);
                if (MyPOCApplication.getInstance().getPocSession()!=PocSessionStatusEnum.Listening){
                    //不是收听下的抢麦失败，要显示
                    ivTalkImg.setImageResource(R.mipmap.btn_talk_idle);
                }
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);
                break;

            case ApplyTimeout:
                //申请超时
                Log.e(TAG,"申请麦权超时...");
                ivTalkImg.setImageResource(R.mipmap.btn_talk_idle);
                break;

            case ApplySuccess:
                mHandler.removeCallbacks(applyMicOwnerTimeoutTask);  //当申请成功后, 要取消掉
                ivTalkImg.setImageResource(R.mipmap.btn_talk_speak);
                break;

            default:
                break;
        }

    }

    @Override
    public void onStop() {
        super.onStop();
        //
        EventBus.getDefault().unregister(this); // 反注册 EventBus
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        // 取消所有订阅，防止内存泄漏
        compositeDisposable.clear();
        mHandler.removeCallbacksAndMessages(null); // 清理所有消息
    }
}