package com.mypoc.ptt.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.mypoc.ptt.R;
import com.mypoc.ptt.model.PTTGroupExt;
import com.mypoc.pttlibrary.model.PTTGroup;

import java.util.List;

public class GroupAdapter extends RecyclerView.Adapter<GroupAdapter.GroupViewHolder> {

    public interface OnGroupActionListener {
        void onEnterGroup(int groupId);

        /**
         *
         * @param groupId 组ID
         * @param groupType 组类型
         */
        void onViewMembers(int groupId, int groupType);
    }

    private List<PTTGroupExt> groupList;
    private int currentGroupId  = -1; // 当前对讲组id，-1表示没有
    private OnGroupActionListener listener;
    private Context context;

    public GroupAdapter(Context context, List<PTTGroupExt> groupList, OnGroupActionListener listener) {
        this.context=context;
        this.groupList = groupList;
        this.listener = listener;
    }

    public void updateData(List<PTTGroupExt> data) {
        if (this.groupList!=null){
            this.groupList.clear();
            this.groupList.addAll(data);
        } else {
            this.groupList= data;
        }
        notifyDataSetChanged();
    }

    public void setCurrentGroupPosition(int groupId) {
        this.currentGroupId = groupId;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public GroupViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.group_item, parent, false);
        return new GroupViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull GroupViewHolder holder, int position) {

        PTTGroupExt group = groupList.get(position);

        // 设置群组图标和名称
        if (group.getGroupType()==1)
            holder.ivGroupIcon.setImageResource(R.mipmap.head_grpfix);
        else
            holder.ivGroupIcon.setImageResource(R.mipmap.head_grptmp);

        holder.tvGroupName.setText(group.getGroupName());

        // 设置当前对讲组状态
        if (group.getGroupId().equals(currentGroupId)) {
            holder.btnEnterGroup.setVisibility(View.INVISIBLE);
            holder.ivCurrentGroup.setVisibility(View.VISIBLE);
        } else {
            holder.btnEnterGroup.setVisibility(View.VISIBLE);
            holder.ivCurrentGroup.setVisibility(View.GONE);
        }

        // 设置按钮点击事件
        holder.btnEnterGroup.setOnClickListener(v -> {
            if (listener != null) {
                listener.onEnterGroup(group.getGroupId());
            }
        });

        holder.btnViewMembers.setOnClickListener(v -> {
            if (listener != null) {
                listener.onViewMembers(group.getGroupId(),group.getGroupType());
            }
        });
    }

    @Override
    public int getItemCount() {
        return groupList.size();
    }

    static class GroupViewHolder extends RecyclerView.ViewHolder {
        ImageView ivGroupIcon;
        TextView tvGroupName;
        ImageView ivCurrentGroup;
        Button btnEnterGroup;
        Button btnViewMembers;

        public GroupViewHolder(@NonNull View itemView) {
            super(itemView);
            ivGroupIcon = itemView.findViewById(R.id.iv_group_icon);
            tvGroupName = itemView.findViewById(R.id.tv_group_name);
            ivCurrentGroup = itemView.findViewById(R.id.iv_current_group);
            btnEnterGroup = itemView.findViewById(R.id.btn_enter_group);
            btnViewMembers = itemView.findViewById(R.id.btn_view_members);
        }
    }

}
