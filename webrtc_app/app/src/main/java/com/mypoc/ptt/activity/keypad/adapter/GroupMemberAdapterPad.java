package com.mypoc.ptt.activity.keypad.adapter;

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
import com.mypoc.ptt.adapter.GroupMemberAdapter;
import com.mypoc.pttlibrary.model.PTTGroupMember;

import java.util.ArrayList;
import java.util.List;


public class GroupMemberAdapterPad extends RecyclerView.Adapter<GroupMemberAdapterPad.ViewHolder>{
    private static final String TAG = "GroupMemberAdapterPad";

    private List<PTTGroupMember> memberList = new ArrayList<>();
    private Context context;

    public GroupMemberAdapterPad(Context context, List<PTTGroupMember> userlist) {
        this.context = context;
        this.memberList= userlist;
    }

    @NonNull
    @Override
    public GroupMemberAdapterPad.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.pad_group_member_item, parent, false);
        GroupMemberAdapterPad.ViewHolder viewHolder = new GroupMemberAdapterPad.ViewHolder(view);
        return viewHolder;
    }

    @Override
    public void onBindViewHolder(@NonNull GroupMemberAdapterPad.ViewHolder holder, int position) {
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
