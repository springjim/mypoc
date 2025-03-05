using POCControlCenter.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class User
    {
        public int userId { get; set; }

        /// <summary>
        /// 当作poc帐号用的，有历史原因呀，不能改
        /// </summary>
        public String phone { get; set; }

        /// <summary>
        /// 当作电话号码用，有历史原因呀，不能改
        /// </summary>
        public String account { get; set; }

        public String userName { get; set; }

        //用户头像,存放在minIO的地址, 后面要改...todo 
        public String headImgUrl { get; set; }


        public String address { get; set; }

        //当作警员编号用,当用于政法时, 通用行业当作客户编号 
        public String signature { get; set; }

        public String email { get; set; }

        public int? grade { get; set; }  //要用可空类型, 满足restful的接口传参要求
        public byte? isChat { get; set; }

        //0表示男, 1表示女
        public byte? gender { get; set; }
        public Int32? registerDate { get; set; }  //暂时没用到      

        //public int? groupId { get; set; }

        //是否登录
        public int? logon { get; set; }

        //1表示已正常或复活, 0 表示已遥晕, -1 表示已遥毙
        public int? lifeState { get; set; }

        //表示是否录音
        public string flagRecord { get; set; }

        //数据库字段为class, 当作话权值,0表示普通, 1表示调度台, 2表示领导
        public int? myclass  { get;set;}

        //2018.10.27 增加网易云信的帐号设置
        public string nimAppkey { get; set; }  //废除
        public string nimAccount { get; set; } //废除

        public string nimPswd { get; set; } //废除

        //帐号是否有效, Y 表示有， N或空表示无
        public string nimValid { get; set; } //废除

        public int? cmpid { get; set; }

        //最近一次gps定位时间，表中无该字段
        public long lastGpsTimeMs { get; set; } = 0;

        
        //是否登录自动开启定位上报
        public String flagAutoLocation { get; set; }

        //是否隐藏poc上报定位开关
        public String privHideLocSwitch { get; set; }

        //定位模式;  0，一般；1，高精,2,用户设置
        public int? locationMode { get; set; }

        //循环定位时间间隔(单位：秒):30,60,180，0则由用户设置
        public int? locationInterval { get; set; }

    }


    public class NetHttpUserData
    {
        public String error { get; set; }

        public User_Old data { get; set; }
    }

    public class NetHttpGroupUserData
    {
        public String error { get; set; }

        public List<User> data { get; set; }
    }
    


}
