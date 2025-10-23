package com.mypoc.ptt.fragment;

import android.content.Intent;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import com.mypoc.ptt.activity.CreateGroupActivity;
import com.mypoc.ptt.activity.PttSingleCallActivity;
import com.mypoc.ptt.activity.RtcTalkActivity;
import com.mypoc.ptt.activity.TempTalkActivity;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.R;
import com.mypoc.ptt.adapter.ContactAdapter;
import com.mypoc.ptt.event.TtsSpeakEvent;
import com.mypoc.ptt.pref.LoginPrefereces;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;
import com.mypoc.pttlibrary.callback.TempGroupCreateCallback;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTUser;

import org.greenrobot.eventbus.EventBus;

import java.util.List;
import java.util.concurrent.TimeoutException;

import butterknife.BindView;
import butterknife.ButterKnife;


import io.reactivex.Observable;
import io.reactivex.android.schedulers.AndroidSchedulers;
import io.reactivex.disposables.CompositeDisposable;
import io.reactivex.schedulers.Schedulers;

/**
 * 通讯录fragment
 */
public class ContactFragment extends Fragment {

    private static String TAG = "ContactFragment";

    @BindView(R.id.contact_list)
    RecyclerView rvContact;

    private IPTTSDK pttSDK;
    private ContactAdapter contactAdapter;
    private Handler handler;
    private CompositeDisposable disposables = new CompositeDisposable();

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        pttSDK = MyPOCApplication.getInstance().getPttSDK();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_contact, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        ButterKnife.bind(this, view); // 在 onViewCreated 中绑定
        handler = new Handler(Looper.getMainLooper());
        //加载通讯录
        loadContactsAndInitListView();
    }


    private void loadContactsAndInitListView() {

        List<PTTUser> currVal = MyPOCApplication.getInstance().getAllUsers().getValue();
        if (currVal != null && !currVal.isEmpty()) {
            initListViewWithData(currVal);
            return;
        }

        Observable<List<PTTUser>> dataObservable = Observable.<List<PTTUser>>create(
                        emitter -> {
                            //没有有效数据则设置观察者
                            Observer<List<PTTUser>> observer = new Observer<List<PTTUser>>() {
                                @Override
                                public void onChanged(List<PTTUser> pttUsers) {
                                    if (pttUsers != null && !pttUsers.isEmpty()) {
                                        Log.i(TAG, "收到用户列表更新");
                                        emitter.onNext(pttUsers);
                                        //只观察一次, 不要注释下面
                                        //emitter.onComplete();
                                        //removeObserverSafely(this);
                                    }
                                }
                            };

                            // 在主线程注册观察者
                            new Handler(Looper.getMainLooper()).post(() -> {
                                MyPOCApplication.getInstance().getAllUsers().observe(getViewLifecycleOwner(), observer);
                            });

                            // 设置取消订阅时的清理
                            emitter.setCancellable(() -> removeObserverSafely(observer));

                        })
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread());

        //
        disposables.add(
                dataObservable
                        //.timeout(5, TimeUnit.SECONDS)
                        .subscribeOn(Schedulers.io())
                        .observeOn(AndroidSchedulers.mainThread())
                        .subscribe(
                                this::initListViewWithData,
                                this::handleError
                        )
        );

    }

    private void initListViewWithData(List<PTTUser> data) {

        if (contactAdapter==null){
            contactAdapter = new ContactAdapter(getContext(), data);
            contactAdapter.setOnButtonClickListener(new ContactAdapter.OnButtonClickListener() {

                @Override
                public void onButtonRtcTalkClick(PTTUser user) {
                    //进入一对一的音视频聊天
                    Intent rtcTalkIntent= new Intent(requireActivity(), RtcTalkActivity.class);
                    rtcTalkIntent.putExtra(RtcTalkActivity.KEY_USER_ID, user.getUserId()+"");
                    rtcTalkIntent.putExtra(RtcTalkActivity.KEY_USER_NAME, user.getUserName());
                    startActivity(rtcTalkIntent);
                }

                @Override
                public void onButtonSingleTalkClick(PTTUser user) {
                    //
                    if (user==null) return;

                    //检查单呼权限，默认有权限，为了测试
                    boolean singleTalkPriv =
                            LoginPrefereces.getDefualtState(requireActivity(),LoginPrefereces.singleTalkKey,true);

                    if (!singleTalkPriv){
                        handler.post(() -> {
                            Toast.makeText(requireActivity(),"单呼权限没有打开，请进入系统参数修改设置",Toast.LENGTH_LONG).show();
                        });

                        return;
                    }

                    Log.i(TAG,"BUTTON, userId="+user.getUserId());
                    String singleGroupName="单呼:"+ MyPOCApplication.getInstance().getUserName()+"-"+
                            PttHelper.findUserName(user.getUserId());
                    String currUserIdJoin= MyPOCApplication.getInstance().getUserId()+","+user.getUserId().toString();

                    //调用创建临时组, 来模拟单呼
                    pttSDK.tempGroupCreate(singleGroupName, MyPOCApplication.getInstance().getUserId(), currUserIdJoin,
                            new TempGroupCreateCallback() {
                                @Override
                                public void onSuccess(PTTGroup group) {
                                    //加入组
                                    PTTGroup pttGroupExt= new PTTGroup(group.getGroupId(),group.getGroupName(),
                                            MyPOCApplication.getInstance().getUserId(),System.currentTimeMillis()/1000);
                                    MyPOCApplication.getInstance().addToTempGroups(pttGroupExt);  //让groupfragment 会感知更新
                                    int oldGroupId=  MyPOCApplication.getInstance().getCurrGroupId();

                                    pttSDK.joinGroup(group.getGroupId(), new ReportWorkGroupCallback() {
                                        @Override
                                        public void onSuccess() {

                                            MyPOCApplication.getInstance().setCurrGroupId(group.getGroupId());

                                            EventBus.getDefault().post(new TtsSpeakEvent("进入单呼"));
                                            String peerUserName= PttHelper.findUserName(user.getUserId());

                                            //创建成功,跳转到一个专用的对讲页面（与单呼一样）
                                            Intent intent = new Intent(requireActivity(), PttSingleCallActivity.class);
                                            intent.putExtra(PttSingleCallActivity.KEY_PREV_GROUP_ID,oldGroupId);
                                            intent.putExtra(PttSingleCallActivity.KEY_GROUP_ID,group.getGroupId());
                                            intent.putExtra(PttSingleCallActivity.KEY_PEER_NAME, peerUserName);
                                            intent.putExtra(PttSingleCallActivity.KEY_PEER_USERID,user.getUserId());
                                            intent.putExtra(PttSingleCallActivity.KEY_IS_CALLER,true);
                                            startActivity(intent);

                                        }

                                        @Override
                                        public void onFailure(String error) {
                                            Log.e(TAG,error);
                                        }
                                    });

                                }

                                @Override
                                public void onFailure(String error) {
                                    handler.post(() -> {
                                        Toast.makeText(requireActivity(),"创建单呼失败:"+error,Toast.LENGTH_LONG).show();
                                    });
                                }
                            });

                }
            });
        }
        rvContact.setLayoutManager(new LinearLayoutManager(getContext()));
        rvContact.setAdapter(contactAdapter);
        contactAdapter.notifyDataSetChanged();

    }

    private void handleError(Throwable throwable) {

        Log.e(TAG, Log.getStackTraceString(throwable));

        if (throwable instanceof TimeoutException) {
            Toast.makeText(requireContext(), "数据加载超时", Toast.LENGTH_SHORT).show();
        } else {
            Toast.makeText(requireContext(), "错误: " + throwable.getMessage(), Toast.LENGTH_SHORT).show();
        }
    }

    private void removeObserverSafely(Observer<List<PTTUser>> observer) {
        new Handler(Looper.getMainLooper()).post(() -> {
            MyPOCApplication.getInstance().getAllUsers().removeObserver(observer);
        });
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        // 取消所有订阅，防止内存泄漏
        disposables.clear();
    }
}