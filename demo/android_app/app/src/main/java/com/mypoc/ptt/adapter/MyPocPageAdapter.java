package com.mypoc.ptt.adapter;

import android.view.ViewGroup;

import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentPagerAdapter;
import androidx.viewpager.widget.PagerAdapter;

import java.util.List;

public class MyPocPageAdapter extends FragmentPagerAdapter {

    private List<Fragment> list;
    public MyPocPageAdapter(FragmentManager fm, List<Fragment> list) {
        super(fm);
        this.list = list;
    }

    public void Setlist(List<Fragment> mylist){
        this.list = mylist;
    }

    @Override
    public Fragment getItem(int arg0) {
        return list.get(arg0);
    }

    @Override
    public int getCount() {
        return list.size();
    }

    public void CleanAll(){
        list.clear();
    }

    public Object instantiateItem(ViewGroup container, int position ) {
        Fragment f = (Fragment) super.instantiateItem(container, position);
        //String title = list.get(position);
        //f.setTitle(title);
        return f;
    }

    public int getItemPosition(Object object) {
        return PagerAdapter.POSITION_NONE;
    }
}
