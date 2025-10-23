package com.mypoc.ptt.activity.keypad;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.widget.TextView;

import com.mypoc.ptt.R;
import com.mypoc.ptt.webrtc.base.BaseActivity;

public class ImeiActivity extends BaseActivity {
    private TextView imeiTv;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_imei);

        imeiTv=findViewById(R.id.imeiTv);
        Intent intent = getIntent();
        if (intent != null && intent.hasExtra("imei")) {
            String text = intent.getStringExtra("imei");
            imeiTv.setText(text);
        }

    }
}