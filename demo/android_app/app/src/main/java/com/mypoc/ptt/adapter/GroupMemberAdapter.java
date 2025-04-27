package com.mypoc.ptt.adapter;

import android.content.Context;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.mypoc.ptt.R;
import com.mypoc.pttlibrary.model.PTTGroupMember;

import java.util.ArrayList;
import java.util.List;

/**
 * 组成员适配器
 */
public class GroupMemberAdapter extends RecyclerView.Adapter<GroupMemberAdapter.ViewHolder> {

    private static final String TAG = "GroupMemberAdapter";

    private List<PTTGroupMember> memberList = new ArrayList<>();
    private Context context;

    public GroupMemberAdapter(Context context, List<PTTGroupMember> userlist) {
        this.context = context;
        //this.memberList= new ArrayList<>(userlist); //这里用复制的方式，以免修改了传入的userlist
        this.memberList= userlist;
    }

    @NonNull
    @Override
    public GroupMemberAdapter.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {

        View view = LayoutInflater.from(context).inflate(R.layout.group_member_item, parent, false);
        GroupMemberAdapter.ViewHolder viewHolder = new GroupMemberAdapter.ViewHolder(view);
        return viewHolder;
    }

    @Override
    public void onBindViewHolder(@NonNull GroupMemberAdapter.ViewHolder holder, int position) {
        PTTGroupMember userEntry = memberList.get(position);
        if (userEntry != null) {

            Log.i(TAG, userEntry.toString());

            holder.useridTV.setText(userEntry.getUserId() + "");
            holder.usernameTV.setText(userEntry.getUserName());
        }

        Integer logon = userEntry.getLogon();
        String listen = userEntry.getListen();

        if (logon != null && logon.equals(1) && !listen.isEmpty() && listen.equalsIgnoreCase("y")) {

            holder.avatarIV.setImageResource(R.mipmap.user_icon_ingroup);
            holder.userStatusTV.setText("在线在组");
            holder.userStatusTV.setTextColor(ContextCompat.getColor(context, R.color.mypoc_useronline_color));
        } else if (logon != null && logon.equals(1) && (listen.isEmpty() || listen.equalsIgnoreCase("n"))) {

            holder.avatarIV.setImageResource(R.mipmap.user_icon_online);
            holder.userStatusTV.setText("在线");
            holder.userStatusTV.setTextColor(ContextCompat.getColor(context, R.color.mypoc_useronline_color));
        } else {
            holder.avatarIV.setImageResource(R.mipmap.user_icon_offline);
            holder.userStatusTV.setText("离线");
            holder.userStatusTV.setTextColor(ContextCompat.getColor(context, R.color.mypoc_bottomtab_txtcolor));
        }

    }

    @Override
    public int getItemCount() {
        return memberList.size();
    }

    public void updateData(List<PTTGroupMember> groupMembers) {

        //this.memberList= new ArrayList<>(groupMembers);  //这里用复制的方式，以免修改了传入的userlist
        this.memberList= groupMembers;
        notifyDataSetChanged();

    }

    class ViewHolder extends RecyclerView.ViewHolder {
        ImageView avatarIV;
        TextView useridTV;
        TextView usernameTV;
        TextView userStatusTV;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            usernameTV = itemView.findViewById(R.id.username);
            useridTV = itemView.findViewById(R.id.userid);
            avatarIV = itemView.findViewById(R.id.userAvatarIV);
            userStatusTV = itemView.findViewById(R.id.userStatus);
        }
    }
}
