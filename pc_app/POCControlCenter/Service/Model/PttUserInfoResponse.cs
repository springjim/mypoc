using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class PttUserInfoResponse : ResponseBase
    {

        public Data data;

        public class Data
        {

            public int userId { get; set; }
            /**
             * 用户头像,存放在minIO的地址
             */
            public String headImgUrl { get; set; }
            /**
             * 当作poc帐号用的，有历史原因呀，不能改
             */
            public String phone { get; set; }
            /**
             * 当作电话号码用，有历史原因呀，不能改
             */
            public String account { get; set; }
            /**
             * 当作话权值,0表示普通, 1表示调度台, 2表示领导
             */
            public int aclass { get; set; }
            /**
             * 云信的appkey
             */
            public String nimAppkey { get; set; }
            /**
             * 云信的帐号
             */
            public String nimAccount { get; set; }
            /**
             * 云信的密码
             */
            public String nimPswd { get; set; }
            /**
             * 帐号是否有效, Y 表示有， N或空表示无
             */
            public String nimValid { get; set; }
        }

    }
}
