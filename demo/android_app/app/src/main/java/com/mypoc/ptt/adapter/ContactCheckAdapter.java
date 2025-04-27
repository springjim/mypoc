package com.mypoc.ptt.adapter;

import android.content.Context;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.ptt.model.PTTUserExt;
import com.mypoc.pttlibrary.model.PTTUser;

import java.util.List;

//用于创建临时组
public class ContactCheckAdapter extends RecyclerView.Adapter<ContactCheckAdapter.ViewHolder> {

    private static final String TAG = "ContactCheckAdapter";
    private final List<PTTUserExt> userlist;
    private Context context;
    private OnItemClickListener onItemClickListener;
    public interface OnItemClickListener {
        void onItemClick(PTTUserExt  item);
    }
    public ContactCheckAdapter(Context context,List<PTTUserExt> userlist,OnItemClickListener listener ) {
        this.context = context;
        this.userlist = userlist;
        this.onItemClickListener = listener;
    }

    @NonNull
    @Override
    public ContactCheckAdapter.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.contact_item_select,parent,false);
        ContactCheckAdapter.ViewHolder viewHolder = new ContactCheckAdapter.ViewHolder(view);
        return viewHolder;
    }

    @Override
    public void onBindViewHolder(@NonNull ContactCheckAdapter.ViewHolder holder, int position) {
        PTTUserExt userEntry = userlist.get(position);
        if(userEntry!=null){

            holder.useridTV.setText(userEntry.getUserId()+"");
            holder.usernameTV.setText(userEntry.getUserName());
        }

        Integer logon= userEntry.getLogon();
        if (logon!=null && logon.equals(1))
            holder.avatarIV.setImageResource(R.mipmap.user_icon_online);
        else
            holder.avatarIV.setImageResource(R.mipmap.user_icon_offline);

        if (userEntry.getUserId().equals(MyPOCApplication.getInstance().getUserId())){
            //自身不显示出来
            holder.checkTV.setVisibility(View.INVISIBLE);
        } else {

            holder.checkTV.setVisibility(View.VISIBLE);
            if (userEntry.isSelected())
                holder.checkTV.setBackgroundResource(R.mipmap.check_true);
            else
                holder.checkTV.setBackgroundResource(R.mipmap.check_false);
        }

    }

    @Override
    public int getItemCount() {
        return userlist.size();
    }



    class ViewHolder extends RecyclerView.ViewHolder {
        ImageView avatarIV;
        TextView useridTV;
        TextView  usernameTV;
        TextView  checkTV;
        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            //设置整个item的点击事件
            itemView.setOnClickListener(v -> {

                int position = getAdapterPosition();
                if (position != RecyclerView.NO_POSITION) {
                    PTTUserExt item = userlist.get(position);
                    item.setSelected(!item.isSelected()); // 切换选中状态
                    notifyItemChanged(position); // 刷新当前项

                    // 回调给 Activity
                    if (onItemClickListener != null) {
                        onItemClickListener.onItemClick(item);
                    }
                }

            });

            usernameTV=itemView.findViewById(R.id.username);
            useridTV = itemView.findViewById(R.id.userid);
            avatarIV = itemView.findViewById(R.id.userAvatarIV);
            checkTV = itemView.findViewById(R.id.memberCheck);
        }
    }
}
