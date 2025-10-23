package com.mypoc.ptt.utils;

import com.mypoc.ptt.application.MyPOCApplication;
import com.mypoc.pttlibrary.model.PTTGroup;
import com.mypoc.pttlibrary.model.PTTUser;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Iterator;
import java.util.List;
import java.util.TimeZone;
import java.util.concurrent.TimeoutException;

public class PttHelper {

    public static final String NO_FOUND_GROUP="未知组名";

    public static String findGroupName(int groupId){

        List<PTTGroup> groups= MyPOCApplication.getInstance().getFixGroups().getValue();
        if (groups!=null && !groups.isEmpty()){
            for (PTTGroup item:groups  ) {
                if (item.getGroupId().equals(groupId)){
                    return item.getGroupName();
                }
            }
        }

        groups= MyPOCApplication.getInstance().getTempGroups().getValue();
        if (groups!=null && !groups.isEmpty()){
            for (PTTGroup item:groups  ) {
                if (item.getGroupId().equals(groupId)){
                    return item.getGroupName();
                }
            }
        }

        return NO_FOUND_GROUP;
    }

    public static String findUserName(int userId){

        String nofound="未找到用户名称";

        List<PTTUser> users= MyPOCApplication.getInstance().getAllUsers().getValue();
        if (users==null || users.size()==0)
            return nofound;

        Iterator<PTTUser> iterator = users.iterator();
        while (iterator.hasNext()) {
            PTTUser obj = iterator.next();
            if (obj != null &&  obj.getUserId().equals(userId)) {
                return obj.getUserName();
            }
        }
        return nofound;

    }

    /**
     * 从通讯录中查找用户
     * @param userId
     * @return
     */
    public static PTTUser findPttUser(int userId){
        PTTUser user=null;
        List<PTTUser> users= MyPOCApplication.getInstance().getAllUsers().getValue();
        if (users==null || users.size()==0)
            return null;
        Iterator<PTTUser> iterator = users.iterator();
        while (iterator.hasNext()) {
            PTTUser obj = iterator.next();
            if (obj != null &&  obj.getUserId().equals(userId)) {
                return obj;
            }
        }
        return user;
    }

    /**
     *
     * @param userId
     * @param loginStatus 1: 上线 0：下线
     */
    public static void updateAllUserStatus(int userId, int loginStatus){
        List<PTTUser> users=  MyPOCApplication.getInstance().getAllUsers().getValue();
        if (users==null || users.isEmpty())
            return;
        for (PTTUser item: users
             ) {
            if (item.getUserId().equals(userId)){
                item.setLogon(loginStatus);
                break;
            }
        }

        MyPOCApplication.getInstance().setAllUsers(users);

    }

    /**
     *
     * @param defaultGroupId  没有找到时，指定缺省值，如设为 -1
     * @return
     * @throws InterruptedException
     * @throws TimeoutException
     */
    public static Integer getFirstGroupIdFromApplication(int defaultGroupId) {
        //
        List<PTTGroup> pttGroups = MyPOCApplication.getInstance().getFixGroups().getValue();
        if (pttGroups != null && pttGroups.size() > 0) {
            return pttGroups.get(0).getGroupId();
        }

        pttGroups = MyPOCApplication.getInstance().getTempGroups().getValue();
        if (pttGroups != null && pttGroups.size() > 0) {
            return pttGroups.get(0).getGroupId();
        }

        return defaultGroupId;
    }


    public static String convertUnixTime(long unixTimestamp) {
        // 1. 将秒级时间戳转为毫秒（Date 构造函数需要毫秒）
        Date date = new Date(unixTimestamp * 1000L);

        // 2. 定义日期格式
        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

        // 3. 设置时区（可选，默认使用系统时区）
        sdf.setTimeZone(TimeZone.getTimeZone("Asia/Shanghai"));

        // 4. 格式化输出
        return sdf.format(date);
    }

}
