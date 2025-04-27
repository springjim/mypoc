using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Tool
{

    /// <summary>
    /// 媒体相关检查，如有无麦克风、音箱或摄像头等
    /// </summary>
    public  class MediaCheckUtils
    {

        /// <summary>
        /// 检查电脑有无安装至少一个麦克风
        /// </summary>
        /// <returns></returns>
        public static Boolean checkValidMic()
        {
            var inDevices = MMDeviceService.GetInputDevices();
            if (inDevices.GetLength(0) >= 1)
                return true;
            else
                return false;

        }

        /// <summary>
        /// 检查电脑有无安装至少一个音箱
        /// </summary>
        /// <returns></returns>
        public static Boolean checkValidSpeaker()
        {
            var outDevices = MMDeviceService.GetOutputDevices();
            if (outDevices.GetLength(0) >= 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 检查电脑有无安装至少一个摄像头
        /// </summary>
        /// <returns></returns>
        public static Boolean checkValidCamera()
        {
            var videoCaptureDevices = MMDeviceService.GetVideoCaptureDevices();
            if (videoCaptureDevices.GetLength(0) >= 1)
                return true;
            else
                return false;      
        }

    }
}
