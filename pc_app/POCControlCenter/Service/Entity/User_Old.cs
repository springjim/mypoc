using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    /// <summary>
    /// 兼容以前的旧服务
    /// </summary>
    public class User_Old
    {
        
        public int user_id { get; set; }
        public String password { get; set; }
        public String phone { get; set; }
        public String user_name { get; set; }
        public String address { get; set; }
        public int grade { get; set; }
        public byte is_chat { get; set; }
        public byte gender { get; set; }
        public String register_Date { get; set; }
        public int voicepriv { get; set; }

        /**
         * 上报GPS定位保存的时间, 单位天
         */
        public int saveLocationTime { get; set; }
        /**
         * 中继
         */
        public String flag_relay { get; set; }
        /**
         * 录音
         */
        public String flag_record { get; set; }
        /**
         * 调度
         */
        public String flag_dispatch { get; set; }
        /**
         * 是否登录自动开启定位
         */
        public String flag_autoLocation { get; set; }
        /**
         * 是否隐藏poc上报定位开关
         */
        public String priv_hideLocSwitch { get; set; }
        /**
         * 定位模式;  0，一般；1，高精, 2由用户设置
         */
        public int locationMode { get; set; }
        /**
         * 循环定位时间间隔(单位：秒)，  0由用户设置
         */
        public int locationInterval { get; set; }

        //2017.8.31 
        public int cmpid { get; set; }  //调度台需要返回这个字段	

        public int group_number { get; set; }
        public int person_number { get; set; }

        public int group_id { get; set; } = -1;
        public int logon { get; set; }       //表示是否登入 , 1 表示在线, 0表示离线

        //2018.10.27 加入NIM网易云信的设置
        public String nim_appkey { get; set; }
        public String nim_account { get; set; }
        public String nim_pswd { get; set; }
        public String nim_valid { get; set; }
        public int life_state { get; set; }
        public int myclass { get; set; }

    }
}
