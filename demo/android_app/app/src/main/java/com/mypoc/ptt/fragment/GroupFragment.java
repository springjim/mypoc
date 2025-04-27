package com.mypoc.ptt.fragment;

import android.content.Intent;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.LiveData;
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

import com.mypoc.ptt.activity.GroupMemberActivity;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.R;
import com.mypoc.ptt.adapter.GroupAdapter;
import com.mypoc.ptt.event.TtsSpeakEvent;
import com.mypoc.ptt.event.UpdateWorkingGroupEvent;
import com.mypoc.ptt.model.PTTGroupExt;
import com.mypoc.ptt.utils.PttHelper;
import com.mypoc.pttlibrary.api.IPTTSDK;
import com.mypoc.pttlibrary.callback.ReportWorkGroupCallback;
import com.mypoc.pttlibrary.model.PTTGroup;

import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.greenrobot.eventbus.ThreadMode;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeoutException;

import butterknife.BindView;
import butterknife.ButterKnife;
import io.reactivex.Observable;
import io.reactivex.android.schedulers.AndroidSchedulers;
import io.reactivex.disposables.CompositeDisposable;
import io.reactivex.schedulers.Schedulers;

/**
 * 组fragment
 */
public class GroupFragment extends Fragment implements GroupAdapter.OnGroupActionListener {

    private static String TAG = "GroupFragment";

    private IPTTSDK pttSDK;
    @BindView(R.id.group_list)
    RecyclerView rvGroup;

    private GroupAdapter groupAdapter;
    private CompositeDisposable disposables = new CompositeDisposable();
    private Handler mHandler;


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        pttSDK = MyPOCApplication.getInstance().getPttSDK();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        return inflater.inflate(R.layout.fragment_group, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        EventBus.getDefault().register(this);
        ButterKnife.bind(this, view); // 在 onViewCreated 中绑定

        // 绑定View的生命周期
        mHandler = new Handler(Looper.getMainLooper());
        //加载所有组，包括固定组和临时组
        loadGroupsAndInitListView();
    }

    private void loadGroupsAndInitListView() {
        Log.i(TAG,"加载所有组，包括固定组和临时组");
        // 创建合并两个LiveData的Observable
        Observable<List<PTTGroupExt>> combinedObservable= Observable.combineLatest(
                createGroupObservable(MyPOCApplication.getInstance().getFixGroups()),
                createGroupObservable(MyPOCApplication.getInstance().getTempGroups()),

                (fixGroupList,tempGroupList) -> {
                    // 合并逻辑：fixGroups在前，tempGroups在后
                    List<PTTGroupExt> mergedList = new ArrayList<>();
                    if (fixGroupList != null && !fixGroupList.isEmpty()) {
                        for (PTTGroup item:
                                fixGroupList) {
                            mergedList.add(new PTTGroupExt(item.getGroupId(),item.getGroupName(),
                                    item.getOwnerId(), item.getCreateDate(), 1));
                        }
                    }
                    if (tempGroupList != null && !tempGroupList.isEmpty() ) {
                        for (PTTGroup item:
                                tempGroupList) {
                            mergedList.add(new PTTGroupExt(item.getGroupId(),item.getGroupName(),
                                    item.getOwnerId(),item.getCreateDate(), 0));
                        }
                    }

                    Log.i(TAG,"得到所有组，包括固定组和临时组"+mergedList.size() );
                    return mergedList;

                }

        );

        disposables.add(
                combinedObservable
                        .subscribeOn(Schedulers.io())
                        .observeOn(AndroidSchedulers.mainThread())
                        .subscribe(
                                this::initListViewWithData,
                                this::handleError
                        )
        );

    }


    private void initListViewWithData(List<PTTGroupExt> data) {

        if (groupAdapter == null) {
            // 首次初始化Adapter
            groupAdapter = new GroupAdapter(getContext(), data, this);
            if (MyPOCApplication.getInstance().getCurrGroupId()!=null){
                groupAdapter.setCurrentGroupPosition(MyPOCApplication.getInstance().getCurrGroupId());
            }
            rvGroup.setLayoutManager(new LinearLayoutManager(getContext()));
            rvGroup.setAdapter(groupAdapter);
        } else {
            // 后续只更新数据
            if (MyPOCApplication.getInstance().getCurrGroupId()!=null){
                groupAdapter.setCurrentGroupPosition(MyPOCApplication.getInstance().getCurrGroupId());
            }
            groupAdapter.updateData(data);
        }

    }

    private void handleError(Throwable throwable) {

        Log.e(TAG, Log.getStackTraceString(throwable));

        if (throwable instanceof TimeoutException) {
            Toast.makeText(requireContext(), "数据加载超时", Toast.LENGTH_SHORT).show();
        } else {
            Toast.makeText(requireContext(), "错误: " + throwable.getMessage(), Toast.LENGTH_SHORT).show();
        }
    }


    // 创建单个LiveData的Observable
    private Observable<List<PTTGroup>> createGroupObservable(LiveData<List<PTTGroup>> liveData) {
        return Observable.<List<PTTGroup>>create(emitter -> {
                    Observer<List<PTTGroup>> observer = new Observer<List<PTTGroup>>() {
                        @Override
                        public void onChanged(List<PTTGroup> groups) {
                            if (groups != null) {
                                emitter.onNext(groups);
                            }
                        }
                    };

                    // 在主线程注册观察者
                    new Handler(Looper.getMainLooper()).post(() -> {
                        liveData.observe(getViewLifecycleOwner(), observer);
                    });

                    // 设置取消订阅时的清理
                    emitter.setCancellable(() -> new Handler(Looper.getMainLooper()).post(() -> {
                        liveData.removeObserver(observer);
                    }));
                })
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread());

    }

    //进入指定对讲组, 手动进入
    @Override
    public void onEnterGroup(int groupId) {

        pttSDK.joinGroup(groupId, new ReportWorkGroupCallback() {
            @Override
            public void onSuccess() {
                //获得组名称
                String groupName= PttHelper.findGroupName(groupId);
                EventBus.getDefault().post(new UpdateWorkingGroupEvent(groupId,groupName));

                //发送播报tts语音事件
                EventBus.getDefault().post(new TtsSpeakEvent("进入"+groupName));

            }

            @Override
            public void onFailure(String error) {
                new Handler(Looper.getMainLooper()).post(new Runnable() {
                    @Override
                    public void run() {
                        Toast.makeText(requireActivity(),"进入失败,原因:"+error,Toast.LENGTH_LONG).show();
                    }
                });
            }
        });

    }

    @Override
    public void onViewMembers(int groupId, int groupType) {
        Intent intent = new Intent(requireActivity(), GroupMemberActivity.class);
        intent.putExtra(GroupMemberActivity.KEY_GROUP_ID,groupId);
        intent.putExtra(GroupMemberActivity.KEY_GROUP_TYPE,groupType);
        startActivity(intent);
    }


    //这里只更新adapter的当前组同步
    @Subscribe(threadMode =  ThreadMode.MAIN)
    public void onUpdateWorkingGroup(UpdateWorkingGroupEvent event){
        MyPOCApplication.getInstance().setCurrGroupId(event.getGroupId());
        if (groupAdapter!=null) {
            groupAdapter.setCurrentGroupPosition(event.getGroupId());
            groupAdapter.notifyDataSetChanged();
        }
    }


    @Override
    public void onDestroyView() {
        super.onDestroyView();
        if (EventBus.getDefault().isRegistered(this))
            EventBus.getDefault().unregister(this);
        disposables.clear();
    }
}