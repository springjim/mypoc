package com.mypoc.ptt.adapter;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.mypoc.ptt.R;
import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.pttlibrary.model.PTTUser;

import java.util.List;

/**
 * 通讯录适配器
 */
public class ContactAdapter extends RecyclerView.Adapter<ContactAdapter.ViewHolder> {

    private static final String TAG = "ContactAdapter";
    private final List<PTTUser> userlist;
    private Context context;

    private OnButtonClickListener listener; // 定义接口

    // 定义点击回调接口
    public interface OnButtonClickListener {
        void onButtonClick(PTTUser  user); // position 表示点击的 item 位置
    }

    // 设置监听器
    public void setOnButtonClickListener(OnButtonClickListener listener) {
        this.listener = listener;
    }


    public ContactAdapter(Context context,List<PTTUser> userlist) {
        this.context = context;
        this.userlist = userlist;
    }

    @NonNull
    @Override
    public ContactAdapter.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {

        View view = LayoutInflater.from(context).inflate(R.layout.contact_item,parent,false);
        ContactAdapter.ViewHolder viewHolder = new ContactAdapter.ViewHolder(view);
        return viewHolder;
    }

    @Override
    public void onBindViewHolder(@NonNull ContactAdapter.ViewHolder holder, int position) {
        PTTUser userEntry = userlist.get(position);
        if(userEntry!=null){

            Log.i(TAG,userEntry.toString());

            holder.useridTV.setText(userEntry.getUserId()+"");
            holder.usernameTV.setText(userEntry.getUserName());
        }

        Integer logon= userEntry.getLogon();
        if (logon!=null && logon.equals(1))
            holder.avatarIV.setImageResource(R.mipmap.user_icon_online);
        else
            holder.avatarIV.setImageResource(R.mipmap.user_icon_offline);

        if (userEntry.getUserId().equals(MyPOCApplication.getInstance().getUserId())){
            holder.singleTalkBTN.setVisibility(View.INVISIBLE);
        } else
        {
            holder.singleTalkBTN.setVisibility(View.VISIBLE);
        }

        holder.singleTalkBTN.setOnClickListener(v-> {
            if (listener != null) {
                listener.onButtonClick(userEntry); // 回调点击事件
            }
        });

    }

    @Override
    public int getItemCount() {
        return userlist.size();
    }

    class ViewHolder extends RecyclerView.ViewHolder {
        ImageView avatarIV;
        TextView useridTV;
        TextView  usernameTV;
        Button  singleTalkBTN;
        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            usernameTV=itemView.findViewById(R.id.username);
            useridTV = itemView.findViewById(R.id.userid);
            avatarIV = itemView.findViewById(R.id.userAvatarIV);
            singleTalkBTN = itemView.findViewById(R.id.btn_single_talk);
        }
    }

}
