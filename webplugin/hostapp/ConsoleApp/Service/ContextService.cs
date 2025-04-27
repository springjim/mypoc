using ConsoleApp.Model.Response;
using ConsoleApp.Tool;
using NAudio.Wave;
using POCClientNetLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Service
{
    /// <summary>
    /// 上下文后台服务
    /// </summary>
    public class ContextService
    {

        private  ChatClient client;   //底层Socket操作封装的类
        private  ChromeResponseHandler chromeResponseHandler;

        private WaveInEvent sourceRecordStream = null;
        private bool isRecording;  //记录是否在录音
   
        private readonly object lockRecordObj = new object();   //用于录音的锁

        public string Full_RecordDeviceName = "";    //音频输入设备名称
        public int Full_RecordDeviceIndex = 0;    //音频输入设备序号

        public string Full_PlaybackDeviceName = "";  //音箱
        public int Full_PlaybackDeviceIndex = 0;  //音箱设备序号

        /// <summary>
        /// 要在chrome发上报工作对讲组时，要对它赋值
        /// </summary>
        private int Full_GroupId = -1;  //当前上报的工作组，如果没有就是 -1

        /// <summary>
        /// 要在chrome发上报登录上线时，要对它赋值
        /// </summary>
        private int Full_UserId = -1;    //当前上报的用户ID，如果没有就是 -1

        public ContextService(ChatClient client, ChromeResponseHandler  chromeRespHandler )
        {

            this.client = client;
            chromeResponseHandler = chromeRespHandler;

        }

        public void InitChatClientCallback()
        {
            client.MessageReceived += ServerOnMessageEventNotfiy;   //接受的tcp消息回调处理
            client.ChatMessageReceived += ServerOnChatEventNotfiy;  //IM 聊天功能关闭   

            //处理AVChatNewMessage消息，用了声网的音视频通话
            client.AVChatNewMessageReceived += ServerOnAVChatNewMessageReceived;    

            //监听到的对讲组的人，和语音包的处理
            client.MonitorAudioMessageReceived += ServerOnMonitorAudioMessageReceived;

            client.ServerDisconnected += ServerOnClientDisconnected;   //与socket服务断开
            client.ServerConnected += ServerOnClientConnected;   //与socket服务连接了
            Log.I("事件回调设置OK");
        }

        public void UnInitChatClientCallback()
        {
            client.MessageReceived -= ServerOnMessageEventNotfiy;   //接受的tcp消息回调处理
            client.ChatMessageReceived -= ServerOnChatEventNotfiy;  //IM 聊天功能关闭   

            //处理AVChatNewMessage消息，用了声网的音视频通话
            client.AVChatNewMessageReceived -= ServerOnAVChatNewMessageReceived;

            //监听到的对讲组的人，和语音包的处理
            client.MonitorAudioMessageReceived -= ServerOnMonitorAudioMessageReceived;

            client.ServerDisconnected -= ServerOnClientDisconnected;   //与socket服务断开
            client.ServerConnected -= ServerOnClientConnected;   //与socket服务连接了
            Log.I("事件回调解除OK");
        }

        private void ServerOnClientConnected(object sender, ServerEventArgs e)
        {
            Log.I("ServerOnClientConnected : Socket连接成功！");
        }

        private void ServerOnClientDisconnected(object sender, ServerEventArgs e)
        {
            //触发重连接
            doReconnectServerAsync(sender, e);

        }

        /// <summary>
        /// 监听到的对讲组的人，和语音包的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerOnMonitorAudioMessageReceived(object sender, ServerEventArgs e)
        {
            MonitorAudioMessage message = e.monitorAudio;

            //由于监听，还要获取从chrome发过的哪些是监听组，还有动态同步等，后续todo ...  下面的 CurrentListenGroups  为空的
            if (RunparaUtils.CurrentListenGroups != null && RunparaUtils.CurrentListenGroups.Contains(message.groupId + ""))
            {
                //
                Debug.WriteLine("监听grouid=" + message.groupId);

                if (client != null)
                {
                    if (client.recvQueue.TryAdd(message.outbuf))
                    {
                        Debug.WriteLine("加入recvQueue成功");   //加入了后面才能播放
                    }
                }                             
                
                // 再发给 chrome 消息，告诉它是监听的组和人，在讲话


            }



        }

        /// <summary>
        /// 处理AVChatNewMessage消息，用了声网的音视频通话,  浏览器插件中暂时不支持
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerOnAVChatNewMessageReceived(object sender, ServerEventArgs e)
        {
            return;
        }

        private void ServerOnChatEventNotfiy(object sender, ServerEventArgs e)
        {
            //IM聊天消息关闭了
            return;
        }

        private void ServerOnMessageEventNotfiy(object sender, ServerEventArgs e)
        {

            ManageDataPacket mng = e.manage;
            Debug.WriteLine(mng.toString());
            //Log.I("收到socket消息, "+ mng.toString());   //收到语音不能老打印

            string name;
            switch (mng.getMessageId())
            {

                case MyType.TYPE_SYS_MESSAGE:
                    //系统状态消息
                    doSysMessageHandler(mng);
                    break;

                case MyType.TYPE_MIC_SUCCESS:
                    //收到抢麦成功消息
                    ResponseMicSuccess responseMicSuccess = new ResponseMicSuccess();
                    chromeResponseHandler.sendMessage(responseMicSuccess);
                    //启动采集
                    StartChat(1, 0, 0);

                    break;
                case MyType.TYPE_MIC_FAILED:
                    //收到抢麦失败消息
                    ResponseMicFailed responseMicFailed = new ResponseMicFailed();
                    chromeResponseHandler.sendMessage(responseMicFailed);

                    break;

                    ///to do ....其它信令

                default:
                    break;

            }


        }

        public void EndChat()
        {
            client.recordingbuffer.Reset();
            if (sourceRecordStream != null)
            {
                MicStopRecording();
            }

        }


        /// <summary>
        /// 1 表示普通对讲 (需要抢麦) 2 表示SOS对讲(不需抢麦，直发服务器，且语音报文又不一样)
        /// </summary>
        /// <param name="EncodeMedia_Mode"> 1 表示普通对讲 (需要抢麦) 2 表示SOS对讲(不需抢麦，直发服务器，且语音报文又不一样)</param>
        public void StartChat(int EncodeMedia_Mode, int SOS_GROUPID, int SOS_USERID)
        {

            Debug.WriteLine("StartChat, EncodeMedia_Mode=" + EncodeMedia_Mode + ",SOS_GROUPID=" + SOS_GROUPID
                + ",SOS_USERID=" + SOS_USERID);

            client.EncodeMedia_Mode = EncodeMedia_Mode;
            client.SOS_GroupID = SOS_GROUPID;
            client.SOS_UserID = SOS_USERID;

            client.recordingbuffer.Reset();
            if (sourceRecordStream != null)
            {
                //Debug.WriteLine("开始采集");
                //Log.I("开始采集");     //不要显示
                MicStartRecording();
            }         
            else
            {
                InitWaveRecord();
                MicStartRecording();    
            }          

        }


        private void doSysMessageHandler(ManageDataPacket mng)
        {

            int type = mng.getType();
            ResponseSysMessage response = new ResponseSysMessage(mng.getGroupId()+"", mng.getUserId()+"",type);
            chromeResponseHandler.sendMessage(response);

        }

        private async void doReconnectServerAsync(object sender, ServerEventArgs e)
        {
            try
            {

                //断网了，改变UI  
                Debug.WriteLine("============================================================");
                Debug.WriteLine("ServerOnClientDisconnected  Start");
               
                ReleaseWaveRecord();
                client.Exit();

                client.CloseConnection();
                client.ReleaseWavePlayDevices();
                client.Release();

                Debug.WriteLine("重连接Socket Server  Restart");

                InitWaveRecord();
                client.Enter();

                //client.InitWavePlayDevices();
                InitWavePlay();

                if (client.NetInit())   //重连
                {

                    //连上了，改变UI                    
                    if (Full_GroupId!=-1  && Full_UserId!=-1 )
                        client.SendMessage((new Data()).LogoutMessageEncode(Full_GroupId, Full_UserId));   //登出，再上报，因为socket服务端的 上下文也变了

                    if (Full_GroupId != -1 && Full_UserId != -1)
                        client.SendMessage(
                        (new Data()).ReportMessageEncode(Full_GroupId, Full_UserId) );

                    client.RestartHeartbeat();  //重连成功后，原来的心跳线程可能退出了，这里要再调用

                }
                else
                {
                    //断网了，改变UI                            

                    await Task.Delay(2000);
                    //下面再次触发重连
                    if (Full_GroupId != -1 && Full_UserId != -1)

                        client.SendMessage((new Data()).LogoutMessageEncode(Full_GroupId, Full_UserId));

                }

                Debug.WriteLine("ServerOnClientDisconnected  end");
                Debug.WriteLine("============================================================");

            }
            catch (Exception)
            {
                Debug.WriteLine("ServerOnClientDisconnected  Exception");
                Debug.WriteLine("============================================================");
            }

        }

        public  void ReleaseWaveRecord()
        {
            if (sourceRecordStream != null)
            {                
                MicStopRecording();               
                sourceRecordStream.Dispose();
                sourceRecordStream = null;
            }

        }

        private void MicStartRecording()
        {
            lock (lockRecordObj)
            {
                if (!isRecording)
                {
                    isRecording = true;
                    if (sourceRecordStream != null)
                    {
                        sourceRecordStream.DataAvailable += sourceStream_DataAvailable;
                        sourceRecordStream.StartRecording();
                    }

                    //Console.WriteLine("Recording started.");
                }
                else
                {
                    //Console.WriteLine("Already recording.");
                }
            }
        }

        private void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (sourceRecordStream == null) return;
            client.RecordMediaData(e.Buffer, e.BytesRecorded);

        }

        private void MicStopRecording()
        {
            lock (lockRecordObj)
            {
                if (isRecording)
                {
                    if (sourceRecordStream != null)
                    {

                        sourceRecordStream.DataAvailable -= sourceStream_DataAvailable;
                        sourceRecordStream.StopRecording();
                    }

                    isRecording = false;
                    //Console.WriteLine("Recording stopped.");
                }
                else
                {
                    //Console.WriteLine("Not recording.");
                }
            }
        }

        public void InitWavePlay()
        {
            try
            {
                Debug.WriteLine("Full_PlaybackDeviceName=" + this.Full_PlaybackDeviceName);
                Log.I("Full_PlaybackDeviceName=" + this.Full_RecordDeviceName);

                int deviceNumber = 0;
                if (this.Full_PlaybackDeviceName != null && !this.Full_PlaybackDeviceName.Trim().Equals(""))
                {
                    String[] devArr = MMDeviceService.GetOutputDevices();                   

                    if (devArr != null && devArr.Length > 0)
                    {
                        for (int index = 0; index < devArr.Length; index++)
                        {
                            if (devArr[index].Equals(this.Full_PlaybackDeviceName))
                            {
                                deviceNumber = index;
                                break;
                            }
                        }
                    }
                }

                this.Full_PlaybackDeviceIndex = deviceNumber;

                client.InitWavePlayDevices(deviceNumber);

            }
            catch (Exception e)
            {
   
                Log.E("InitWavePlay error: " + e.StackTrace);
            }
        }


        /// <summary>
        /// 初努化麦克风采集
        /// </summary>
        public void InitWaveRecord()
        {
            try
            {

                Debug.WriteLine("Full_RecordDeviceName=" + this.Full_RecordDeviceName);
                Log.I("Full_RecordDeviceName=" + this.Full_RecordDeviceName);

                int deviceNumber = 0;
                if (!string.IsNullOrEmpty(this.Full_RecordDeviceName))
                {
                    String[] devArr = MMDeviceService.GetInputDevices();                 

                    if (devArr != null && devArr.Length > 0)
                    {
                        for (int index = 0; index < devArr.Length; index++)
                        {
                            if (devArr[index].Equals(this.Full_RecordDeviceName))
                            {
                                deviceNumber = index;
                                break;
                            }
                        }
                    }
                }

                this.Full_RecordDeviceIndex = deviceNumber; //记住序号

                if (sourceRecordStream == null)
                {
                    sourceRecordStream = new WaveInEvent();
                    sourceRecordStream.DeviceNumber = deviceNumber;

                    sourceRecordStream.BufferMilliseconds = 20;  //是20ms一帧的buffer
                    sourceRecordStream.NumberOfBuffers = 1;  //一定要设置为1， 要不然stoprecording时会有底层缓存，再startrecording会报错：WaveStillPlaying calling waveUnprepareHeader

                    sourceRecordStream.WaveFormat = new WaveFormat(8000, 16, 1);              
                    sourceRecordStream.RecordingStopped += sourceStream_RecordingStopped;

                }
            }
            catch (Exception e)
            {
                Log.E("InitWaveRecord error: " + e.StackTrace);
            }
        }

        private void sourceStream_RecordingStopped(object sender, StoppedEventArgs e)
        {
            Debug.WriteLine("停止采集语音了！！！，原因：" + (e.Exception != null ? e.Exception.Message : "空"));
        }



    }
}
