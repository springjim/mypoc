package com.mypoc.ptt.activity.keypad.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.mypoc.ptt.R;
import com.mypoc.ptt.model.PTTGroupExt;

import java.util.List;

/**
 * 专用于按键类界面的组适配器
 */
public class GroupAdapterPad extends ArrayAdapter<PTTGroupExt> {

    private Context context;
    private List<PTTGroupExt> items;
    private int currentGroupId  = -1; // 当前对讲组id，-1表示没有


    public GroupAdapterPad(Context context, List<PTTGroupExt> items) {
        super(context, R.layout.pad_group_item, items);
        this.context = context;
        this.items = items;
    }

    public void setCurrentGroupPosition(int groupId) {
        this.currentGroupId = groupId;
        notifyDataSetChanged();
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        ViewHolder holder;

        if (convertView == null) {
            // 第一次加载，初始化 ViewHolder
            convertView = LayoutInflater.from(context).inflate(R.layout.pad_group_item, parent, false);
            holder = new ViewHolder();
            holder.tvGroupName = convertView.findViewById(R.id.tv_group_name);
            holder.ivGroupIcon = convertView.findViewById(R.id.iv_group_icon);
            holder.ivCurrentGroup = convertView.findViewById(R.id.iv_current_group);
            convertView.setTag(holder); // 存储 ViewHolder
        } else {
            // 复用 convertView
            holder = (ViewHolder) convertView.getTag();
        }

        // 绑定数据
        PTTGroupExt item = items.get(position);
        holder.tvGroupName.setText(item.getGroupName());
        if (item.getGroupType()==1)
            holder.ivGroupIcon.setImageResource(R.mipmap.head_grpfix);
        else
            holder.ivGroupIcon.setImageResource(R.mipmap.head_grptmp);

        //再判断当前组
        // 设置当前对讲组状态
        if (item.getGroupId().equals(currentGroupId)) {
            holder.ivCurrentGroup.setVisibility(View.VISIBLE);
        } else {
            holder.ivCurrentGroup.setVisibility(View.GONE);
        }

        return convertView;
    }

    // ViewHolder 模式，减少 findViewById 调用
    private static class ViewHolder {
        ImageView ivGroupIcon;
        ImageView ivCurrentGroup;
        TextView  tvGroupName;
    }

}
