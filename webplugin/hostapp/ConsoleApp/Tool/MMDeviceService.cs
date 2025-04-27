using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using AForge.Video.DirectShow;

namespace ConsoleApp.Tool
{
    /// <summary>
    ///  音视频设备类方法操作
    /// </summary>
    public class MMDeviceService
    {

        /// <summary>
        /// 摄像头设备
        /// </summary>
        /// <returns></returns>
        public static String[] GetVideoCaptureDevices()
        {
            
            List<string> devsVideoIn = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);//得到机器所有接入的摄像设备
            if (videoDevices.Count != 0)//读取到摄像设备
            {
                foreach (FilterInfo device in videoDevices)
                {
                    devsVideoIn.Add(device.Name);//把摄像设备添加到摄像列表中
                }
            }
            return devsVideoIn.ToArray();

        }

        /// <summary>
        /// 音频输出设备
        /// </summary>
        /// <returns></returns>
        public static string[] GetOutputDevices()
        {
            List<string> devsAudioOut = new List<string>();
            try
            {

                MMDeviceEnumerator enumberator = new MMDeviceEnumerator();
                MMDeviceCollection mmcollect = enumberator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

                for (int waveOutDevice = 0; waveOutDevice < WaveOut.DeviceCount; waveOutDevice++)
                {
                    WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveOutDevice);
                    foreach (MMDevice device in mmcollect)
                    {
                        if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                        {
                            devsAudioOut.Add(device.FriendlyName);
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // 处理异常（例如，没有音频设备、访问被拒绝）
                //Console.WriteLine($"获取音频输出设备时出错: {ex.StackTrace}");
                Log.E("获取音频输出设备时出错"+ex.StackTrace); 
            }

            return devsAudioOut.ToArray();

        }

        /// <summary>
        /// 音频输入设备
        /// </summary>
        /// <returns></returns>
        public static string[] GetInputDevices()
        {
            List<string> devsAudioIn = new List<string>();

            try
            {

                MMDeviceEnumerator enumberator = new MMDeviceEnumerator();
                MMDeviceCollection deviceCollection = enumberator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All);
                for (int waveInDevice = 0; waveInDevice < WaveIn.DeviceCount; waveInDevice++)
                {
                    WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                    foreach (MMDevice device in deviceCollection)
                    {
                        if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                        {
                            devsAudioIn.Add(device.FriendlyName);
                            break;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                // 处理异常（例如，没有设备或访问被拒绝）
               // Console.WriteLine($"获取音频输入设备时出错: {ex.StackTrace}");
                Log.E("获取音频输入设备时出错" + ex.StackTrace);

            }


            return devsAudioIn.ToArray();
        }

    }
}
