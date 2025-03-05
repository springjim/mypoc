using agora.rtc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Agora
{
    public abstract class IEngine
    {
        public delegate void ReceiveStreamMessage(string message);
        internal abstract int Init(string appId, string channelId);
        internal abstract int UnInit();
        internal abstract int JoinChannel(string channelName, string rtcToken, uint uid);
        internal abstract int LeaveChannel();
        //AidoEffect
        public virtual int PlayEffect(int soundId, string filePath, int loopCount, int startPos, double pitch = 1.0,
            double pan = 0.0, int gain = 100, bool publish = false)
        { return -1; }
        public virtual int StopEffect(int soundId) { return 0; }
        public virtual int PauseEffect(int soundId) { return 0; }
        public virtual int ResumeEffect(int soundId) { return 0; }

        //DeviceManager
        public virtual agora.rtc.DeviceInfo[] GetVideoDevices() { return null; }
        public virtual agora.rtc.DeviceInfo[] GetRecordingDevices() { return null; }
        public virtual agora.rtc.DeviceInfo[] GetPlaybackDevices() { return null; }
        public virtual void SetVideoDevice(string id) { }
        public virtual void SetRecordingDevice(string id) { }
        public virtual void SetPlaybackDevice(string id) { }

        //RtmpStreaming
        public virtual int AddPublishStreamUrl(string url) { return -1; }
        public virtual int RemovePublishStreamUrl(string url) { return -1; }

        //EnableEncryption
        public virtual int EnableEncryption(ENCRYPTION_MODE mode) { return -1; }
        //SetVideoEncoderConfiguration
        public virtual int setVideoEncoderConfiguration(agora.rtc.VideoDimensions dimension, agora.rtc.FRAME_RATE fps) { return -1; }

        //voice changer
        public virtual int SetVoiceBeautifierPreset(int index) { return -1; }
        public virtual int SetVoiceBeautifierParameters(int index, int param1, int param2) { return -1; }
        public virtual int SetAudioEffectPreset(int index) { return -1; }
        public virtual int SetAudioEffectParameters(int index, int param1, int param2) { return -1; }

        //SendStreamMessage
        public virtual int SendStreamMessage(string str) { return -1; }

        //SendStreamMessage
        public virtual int StartMediaRelay(string channelName) { return -1; }
        public virtual int StopMediaRelay() { return -1; }
        // not necessary
        internal abstract string GetSDKVersion();
        internal abstract IAgoraRtcEngine GetEngine();

    }
}
