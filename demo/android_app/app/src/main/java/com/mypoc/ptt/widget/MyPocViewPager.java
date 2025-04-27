package com.mypoc.ptt.widget;

import android.content.Context;
import android.util.AttributeSet;
import android.view.MotionEvent;

import androidx.viewpager.widget.ViewPager;

public class MyPocViewPager extends ViewPager {
    private boolean bIsScroll = true;

    public MyPocViewPager(Context context) {
        super(context);
    }

    public MyPocViewPager(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    @Override
    public boolean onInterceptTouchEvent(MotionEvent arg0) {
        if (bIsScroll)
            return super.onInterceptTouchEvent(arg0);
        else
            return false;
    }

    public void setScroll(boolean bScoll) {
        bIsScroll = bScoll;
    }
}
