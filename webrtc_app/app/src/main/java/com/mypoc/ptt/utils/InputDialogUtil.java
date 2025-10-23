package com.mypoc.ptt.utils;

import android.content.Context;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;

import com.mypoc.ptt.R;

public class InputDialogUtil {

    /**
     * 输入对话框回调接口
     */
    public interface InputDialogListener {
        /**
         * 确认按钮点击回调
         * @param inputText 用户输入的内容
         */
        void onConfirm(String inputText);

        /**
         * 取消按钮点击回调
         */
        void onCancel();
    }

    /**
     * 显示输入弹窗
     * @param context 上下文
     * @param title 弹窗标题
     * @param message 提示信息
     * @param hint 输入框提示文字
     * @param listener 回调监听器
     */
    public static void showInputDialog(Context context,
                                       String title,
                                       String message,
                                       String hint,
                                       final InputDialogListener listener) {

        // 加载自定义布局
        View view = LayoutInflater.from(context).inflate(R.layout.dialog_custom_input, null);
        EditText editText = view.findViewById(R.id.et_input);
        editText.setHint(hint);

        // 构建弹窗
        AlertDialog dialog = new AlertDialog.Builder(context)
                .setTitle(title)
                .setMessage(message)
                .setView(view)
                .setPositiveButton("确认", null) // 先设置为null，后面自定义点击事件
                .setNegativeButton("取消", null)
                .create();

        // 显示弹窗后再设置按钮点击事件，防止自动关闭
        dialog.setOnShowListener(dialogInterface -> {
            // 确认按钮
            dialog.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(v -> {
                String inputText = editText.getText().toString().trim();
                if (TextUtils.isEmpty(inputText)) {
                    Toast.makeText(context, "输入不能为空", Toast.LENGTH_SHORT).show();
                    return;
                }

                if (listener != null) {
                    listener.onConfirm(inputText);
                }
                dialog.dismiss();
            });

            // 取消按钮
            dialog.getButton(AlertDialog.BUTTON_NEGATIVE).setOnClickListener(v -> {
                if (listener != null) {
                    listener.onCancel();
                }
                dialog.dismiss();
            });
        });

        dialog.show();
    }

}
