using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Local
{
    /// <summary>
    /// 音视频设备类方法操作
    /// </summary>
    public class MMDeviceService
    {

        public static string[] GetOutputDevices()
        {
            List<string> devsAudioOut = new List<string>();

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
            return devsAudioOut.ToArray();

        }

        public static string[] GetInputDevices()
        {
            List<string> devsAudioIn = new List<string>();
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
            return devsAudioIn.ToArray();
        }

    }
}
