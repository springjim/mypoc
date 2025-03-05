using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service
{
    /// <summary>
    /// 定义系统各类常量
    /// </summary>
    public class PocConstant
    {

        //一些常量
        public String cmpId { get; set; }  //企业的ID，在视频会议中可当作房间号
                               //以下是用于 TRTC SDK的信息
        public String userSig { get; set; }
        public String sdkappId { get; set; }

        //
        public string userName { get; set; }
        public string password { get; set; }


        private PocConstant()
        {

        }
        private static PocConstant _instance;
        private static readonly object PadLock = new object();

        public static PocConstant GetInstance()
        {
            if (_instance == null)
            {
                lock (PadLock)
                {
                    if (_instance == null)
                    {
                        _instance = new PocConstant();

                    }
                }
            }
            return _instance;
        }

        //


    }
}
