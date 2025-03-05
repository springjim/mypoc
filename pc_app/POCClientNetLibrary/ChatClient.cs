using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using NAudio.Wave;
using System.Threading;
using System.Windows.Forms;
using System.IO;

using System.Collections.Concurrent;
using NAudio.Utils;

using static POCClientNetLibrary.ChatHelper;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace POCClientNetLibrary
{
    public class ChatClient
    {
        #region Fields
        // private static readonly LogSource Log = new LogSource();

        //2017.11.21 加入非当前组是否播放声音报
        private bool IsPlayNonCurrGroup;
        private bool IsExit;
        private TcpClient server;

        //2019.10.02 加入IM MainForm是否创建了
        public bool IsCreateIMForm;
        //       

        // private WaveIn      sourceStream;
        // private WaveInEvent sourceStream;

        private WaveOut receivedPlayStream;
        private BufferedWaveProvider wavePlayProvider;

        private Thread tcpRecieveThread;
        private Thread tcpSendataThread;

        private Thread playMediaThread;
        private Thread heartBeatThread;
        private Thread checkGPSValidThread;

        public  AMRNBCodec codec;
        public  CircularBuffer             recordingbuffer;
        public  BlockingCollection<byte[]> sendQueue;
        public  BlockingCollection<byte[]> recvQueue;

        private bool heartBeatActive;
        private bool checkGPSValidActive;

        #endregion

        #region Events
        public delegate void MessageReceivedEventHandler( object sender, ServerEventArgs e );

        public event MessageReceivedEventHandler MessageReceived;
        public event MessageReceivedEventHandler ChatMessageReceived;
        //2017.9.27 新增对videomessage报文的各事件处理的定义
        public event MessageReceivedEventHandler VideoMessageReceived;
        //2022.11.09  新增对avchatnewmessage报文的各事件处理的定义
        public event MessageReceivedEventHandler AVChatNewMessageReceived;

        public event MessageReceivedEventHandler ServerDisconnected;
        public event MessageReceivedEventHandler ServerConnected;
        #endregion

        #region Properties

        #region Profile Info
        #endregion

        public string  ServerAddress  { get; set; }
        public int     ServerPort     { get; set; }

        public string confServerAddress { get; set; }
        public string confServerPort    { get; set; }

        public bool IsConnected       { get; set; }
        public int  InputAudioDevice  { get; set; }
        public int  OutputAudioDevice { get; set; }

        public bool DoubleClickToCall { get; set; }
        public bool LaunchOnStartup   { get; set; }
        public string Scheme          { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsReceivingData { get; set; }

        /// <summary>
        /// 1, 表示普通对讲方式, 2 表示 SOS对讲方式(不抢麦，直发到服务器)
        /// </summary>
        public int EncodeMedia_Mode { get; set; }

        /// <summary>
        /// 2023.9 实现了AEC功能，在自己发送语音包时，Server端也把自己的包回发，所以在自己对讲期间要丢掉回包
        /// 值1: 表示开始了 (抢麦成功时)， 0: 表示结束了 (释放麦或超时时，网络断开时也要置为0)，则要接收回包
        /// 
        /// </summary>
        public  int AEC_State { get; set; }   //后面考虑 volatile

        public List<int> userids { get; set; }

        /// <summary>
        /// 显示当前SOS会话中的POC正在说话的人
        /// </summary>
        public int SOS_POC_USERID_SPEAKING { get; set; }
        /// <summary>
        /// SOS中用于组成语音包中的groupid
        /// </summary>
        public int SOS_GroupID { get; set; }
        /// <summary>
        /// SOS中用于组成语音包中的userid
        /// </summary>
        public int SOS_UserID { get; set; }

        #endregion


        #region Consructor
        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="port"></param>
        /// <param name="serverAddress"></param>
        public ChatClient(int port, string serverAddress)
        {
            ServerAddress   = serverAddress;
            ServerPort      = port;
            IsExit          = false;
            IsReceivingData = false;
            IsPlayNonCurrGroup = false;
            AEC_State = 0; 
        }

        #endregion

        public void SetPlayNonCurrGroup(bool flag)
        {
            IsPlayNonCurrGroup = flag;
        }

        public void Exit()
        {
            IsExit          = true;
        }

        public void Enter()
        {
            IsExit = false;
        }

        #region Methods
        public bool NetInit()
        {
            try
            {
                server          = new TcpClient(ServerAddress, ServerPort);
                IsConnected     = true;
                EncodeMedia_Mode = 1;
                AEC_State = 0;

                SOS_POC_USERID_SPEAKING = 0;

                if (confServerAddress == null || confServerAddress == "")
                    confServerAddress = ServerAddress;

                if (confServerPort == null || confServerPort == "")
                    confServerPort = ServerPort.ToString();
            }
            catch (SocketException)
            {
                try
                {
                    if (confServerAddress != null && confServerPort!=null)
                    {
                        int port = int.Parse(confServerPort);
                        server   = new TcpClient(confServerAddress, port);
                        IsConnected = true;
                        EncodeMedia_Mode = 1;

                    }
                }
                catch (SocketException)
                {
                    MessageBox.Show(@"Unable to connect to server");
                    IsConnected = false;
                    return false;
                }
            }

            try
            {
                codec = new AMRNBCodec();
                recordingbuffer = new CircularBuffer( 5* 64 * 320 );  //20ms 一帧是320字节, 即缓存 100ms* 64= 6.4秒
                sendQueue = new BlockingCollection<byte[]>();
                recvQueue = new BlockingCollection<byte[]>();

                //网络接受线程
                tcpRecieveThread = new Thread(ReceiveFromServer) { Priority = ThreadPriority.AboveNormal };
                tcpRecieveThread.Start();

                tcpSendataThread = new Thread(SendataToServer) { Priority = ThreadPriority.AboveNormal };
                tcpSendataThread.Start();

                playMediaThread = new Thread(ReceivePlayMedia) { Priority = ThreadPriority.AboveNormal };
                playMediaThread.Start();
            }
            catch (Exception)
            {
                return false;
            }

            //StartRecordingMedia();
            return true;
        }

        public void Release()
        {
            //EndRecordingMedia();

            if (playMediaThread != null )
            {
                playMediaThread.Interrupt();
            }
            if ( tcpRecieveThread != null )
            {
                tcpRecieveThread.Interrupt();
            }
            if (tcpSendataThread != null)
            {
                tcpSendataThread.Interrupt();
            }

            if (recordingbuffer != null)
            {
                recordingbuffer.Reset();
                recordingbuffer = null;
            }
            if (sendQueue != null)
            {
                sendQueue.Dispose();
                sendQueue = null;
            }
            if (recvQueue != null)
            {
                recvQueue.Dispose();
                recvQueue = null;
            }
        }       
        

        public void StartHeartBeat()
        {
            heartBeatThread = new Thread(HeartBeat) { Priority = ThreadPriority.Normal };
            heartBeatThread.Start();
            heartBeatActive = true;
        }
        private void HeartBeat()
        {
            try
            {
                while (IsConnected)
                {
                    if (!heartBeatActive)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    Data dt = new Data(MyType.TYPE_CHECK_SERVER);
                    SendMessage(dt.ToByte());

                    if( heartBeatActive )
                        Thread.Sleep(15 * 1000);
                }
            }
            catch (Exception e)
            {
            }
        }       

        public void StopHeartBeat()
        {
            heartBeatActive = false;
            heartBeatThread.Interrupt();
        }

        private void ReceiveFromServer()
        {
            //var state = new ChatHelper.StateObject
            //{
            //    WorkSocket = server.Client
            //};

            //while ( IsConnected )
            //{
            //    if( IsReceivingData )
            //    {
            //        continue;
            //    }
            //    IsReceivingData = true;

            //    server.Client.BeginReceive(state.Buffer, 0, ChatHelper.StateObject.BUFFER_SIZE, 0, OnReceive, state);
            //}

            Debug.WriteLine("ReceiveFromServer 线程 start");
            var state = new ChatHelper.StateObject
            {
                WorkSocket = server.Client
            };            

            //2017.5.10 以下稍修改
            if (IsConnected)
            {
                //if (IsReceivingData)
                //{
                //    continue;
                // }
                IsReceivingData = true;
                Debug.WriteLine("ReceiveFromServer 线程 进入");
                try
                {
                    server.Client.BeginReceive(state.Buffer, 0, ChatHelper.StateObject.BUFFER_SIZE, 0, OnReceive, state);
                }
                catch (SocketException se)
                {
                    Debug.WriteLine("SocketException:" + se.Message);
                }
            }

            Debug.WriteLine("ReceiveFromServer 线程 end");

        }
        

        private void SendataToServer()
        {
            try
            {
                //2023.9 改成一包一包的发送了
                //ByteBuffer five_buff = ByteBuffer.Allocate(50 * 5);  //5包合一
                ByteBuffer five_buff = ByteBuffer.Allocate(50 * 1);  //1包
                int len = 0;

                while ( IsConnected )
                {
                    byte[] temp = sendQueue.Take();
                    
                    byte[] outbuf = null;
                    codec.Encode(ref outbuf, temp, temp.Length);
                    

                    len = outbuf.Length;
                    //1包1包发送

                    if (EncodeMedia_Mode == 1)
                        SendMessage((new Data()).MediaEncode(outbuf));

                    else if (EncodeMedia_Mode == 2)
                    {
                        //SOS语音包发送
                        Debug.WriteLine(" SOS语音编码后，语音包: len:" + outbuf.Length + ",groupid:" + SOS_GroupID + ",userid:" + SOS_UserID);
                        SendMessage((new Data()).MediaEncode_SOS(outbuf, 1, 3, SOS_GroupID, SOS_UserID));
                    }

                    //new start: 五包合一
                    /*
                    if (five_buff.WriterIndex >= (4 * len))
                    {

                        byte[] packet = new Byte[5 * len];
                        five_buff.WriteBytes(outbuf);
                        five_buff.ReadBytes(packet, 0, 5 * len);
                        five_buff.Clear();
                        Debug.WriteLine(" 编码后，语音包: len:" + packet.Length);

                        //2018.01.12 改为多个模式的语音包
                        if (EncodeMedia_Mode == 1)
                            SendMessage((new Data()).MediaEncode(packet));
                        else if (EncodeMedia_Mode == 2)
                        {
                            //SOS语音包发送
                            Debug.WriteLine(" SOS语音编码后，语音包: len:" + packet.Length+",groupid:"+ SOS_GroupID+",userid:"+SOS_UserID );
                            SendMessage((new Data()).MediaEncode_SOS(packet, 1, 3, SOS_GroupID, SOS_UserID));
                        }

                    }
                    else
                    {
                        //Debug.WriteLine(" 编码后，语音包: len:" + outbuf.Length);
                        five_buff.WriteBytes(outbuf);
                    }
                    */

                    //new end: 五包合一
                    
                }
            }
            catch ( Exception e )
	       {
                Debug.WriteLine("语音采集为数据报错误:"+e.Message);
            }
        }

        private void ReceivePlayMedia( )
        {
            try
            {
                while (IsConnected)
                {
                    byte[] outbuf = recvQueue.Take();
                    Debug.WriteLine("收到播放...........");
                    PlayMediaData(outbuf, outbuf.Length);
                }
            }
            catch (Exception e)
            {
            }
        }

        public void OnReceive(IAsyncResult ar)
        {
            var  state = ar.AsyncState as ChatHelper.StateObject;
            if (state == null)
            {
                return;
            }

            var handler = state.WorkSocket;
            if( !handler.Connected )
                return;

            try
            {
                var bytesRead = handler.EndReceive(ar);
                //if (bytesRead <= 0)
                //{
                //    server.Client.BeginReceive( state.Buffer, 0, ChatHelper.StateObject.BUFFER_SIZE, 0, OnReceive, state );
                //    return;
                //}
                Debug.WriteLine( "OnReceive len of jimmy : " + bytesRead + " pos: " + state.currentPOS + " las:" + state.recieveLEN);

                if( bytesRead > 0 )
                    state.recieveLEN += bytesRead;

                int pos      = state.currentPOS;

                while( !IsExit && IsConnected && state.recieveLEN >= 3  )
                {
                    Data d = new Data( state.Buffer, pos, state.recieveLEN );
                    if( !IsExit && d.paseLen > 0 )
                    {
                        //Debug.WriteLine("OnReceive 运行到这里! ");

                        //2017.11.21 
                        if (d.group_id == 0 &&  EncodeMedia_Mode!=2 )
                        {
                            //group_id=0表示原来报文                            
                            Debug.WriteLine("普通对讲报文 AEC_State:" + AEC_State);

                            foreach (AudioDataPacket audiopacket in d.media_arr)
                            {
                                byte[] outbuf = null;
                                codec.Decode(ref outbuf, audiopacket.getBody(), audiopacket.getBody().Length);
                                if (AEC_State == 0)
                                {
                                    //非AEC期间才能接收
                                    Debug.WriteLine("普通对讲报文，允许播放************************");
                                    recvQueue.Add(outbuf);
                                } else
                                {
                                    //丢掉，不放入recvQueue, todo 或者后期作为AEC处理用
                                }

                            }
                        } else if (d.group_id > 0 && IsPlayNonCurrGroup  &&  EncodeMedia_Mode != 2)
                        {
                            //非当前组的报文
                            if (userids.Contains(d.user_id))
                            {
                                Debug.WriteLine("非当前组的报文 AEC_State:" + AEC_State);
                                foreach (AudioDataPacket audiopacket in d.media_arr)
                                {
                                    byte[] outbuf = null;
                                    codec.Decode(ref outbuf, audiopacket.getBody(), audiopacket.getBody().Length);
                                    if (AEC_State == 0)
                                    {
                                        //非AEC期间才能接收
                                        Debug.WriteLine("普通对讲报文(监听所有人)，允许播放###############!");
                                        recvQueue.Add(outbuf);
                                    }
                                    else
                                    {
                                        //丢掉，不放入recvQueue, todo 或者后期作为AEC处理用
                                    }
                                   
                                }
                            }
                                
                        } else if (d.group_id > 0 &&  EncodeMedia_Mode == 2)
                        {
                            //当前收到SOS报文，优先播放
                            if (userids.Contains(d.user_id) && SOS_POC_USERID_SPEAKING>0  && SOS_POC_USERID_SPEAKING==d.user_id )
                            {
                                Debug.WriteLine("当前收到SOS报文，优先播放!");
                                foreach (AudioDataPacket audiopacket in d.media_arr)
                                {
                                    byte[] outbuf = null;
                                    codec.Decode(ref outbuf, audiopacket.getBody(), audiopacket.getBody().Length);
                                    recvQueue.Add(outbuf);
                                }

                            }                           

                        }
                       
                        pos              += d.paseLen;
                        state.recieveLEN -= d.paseLen;
                    }
                    else
                    {
                        int paseLen = ParseMessage( d.Command, state.Buffer, pos, state.recieveLEN );
                        if (paseLen > 0)
                        {
                            pos              += paseLen;
                            state.recieveLEN -= paseLen;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (pos + state.recieveLEN > ChatHelper.StateObject.BUFFER_SIZE )
                    {
                        Array.Copy(state.Buffer, pos, state.Buffer, 0, state.recieveLEN);
                        state.currentPOS = 0;
                        pos = 0;

                        Array.Clear(state.Buffer, state.recieveLEN, ChatHelper.StateObject.BUFFER_SIZE - state.recieveLEN);
                    }

                    if (state.recieveLEN <= 0)
                    {
                        state.currentPOS = 0;
                        state.recieveLEN = 0;
                        pos = 0;
                    }
                }

                if( !IsExit )
                    server.Client.BeginReceive( state.Buffer, state.currentPOS, ChatHelper.StateObject.BUFFER_SIZE-state.currentPOS, 0, OnReceive, state );
            }
            catch ( SocketException )
            {
                IsConnected = false;

                server.Client.Disconnect( true );

                try
                {
                    Release();

                    if (!IsExit && ServerDisconnected != null)
                    {
                        ServerDisconnected(this, new ServerEventArgs(new ManageDataPacket()));
                        // NetInit();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Parse received message
        /// </summary>
        /// <param name="data"></param>
        public int ParseMessage( int Command, byte[] buffer, int pos, int lastLen )
        {
            int paseLen          = 0;
            ChatMessage  chatMsg = null;
            ManageDataPacket mng = null;
            VideoMessage videoMsg = null;
            GPSCommandMessage gpsMsg = null;
            AVChatNewMessage avChatNewMsg = null;

            switch ( Command )
            {
                case MyType.TYPE_MEDIA_EX:
                case MyType.TYPE_TOPOC_START_EX:
                case MyType.TYPE_TOPOC_END_EX:
                    break;

                case MyType.TYPE_MEDIA:
                case MyType.TYPE_TOPOC_START:
                case MyType.TYPE_TOPOC_END:
                    break;

                //2017.9.27  加入VIDEO_MESSAGE 的解析
                case MyType.VIDEO_MESSAGE:
                    videoMsg = parseVideoMsgData(buffer, pos, lastLen, out paseLen);
                    if (videoMsg != null)
                    {
                        ParseVideoResponse(videoMsg, MyType.VIDEO_MESSAGE);
                    }
                    break;
                case MyType.TYPE_AV_CHAT_NEW:
                    avChatNewMsg = parseAVChatNewMsgData(buffer, pos, lastLen, out paseLen);
                    if (avChatNewMsg != null)
                    {
                        ParseAVChatNewResponse(avChatNewMsg, MyType.TYPE_AV_CHAT_NEW);
                    }
                    break;
                case MyType.TYPE_P2G_MESSAGES:
                case MyType.TYPE_P2P_MESSAGES:
                case MyType.TYPE_P2P_CHAT_TEXT:
                case MyType.TYPE_P2G_CHAT_TEXT:
                case MyType.TYPE_P2P_CHAT_FILE:
                case MyType.TYPE_P2G_CHAT_FILE:
                    chatMsg = parseChatMsgData(buffer, pos, lastLen, out paseLen);
                    if (chatMsg != null)
                    {
                        ParseResponse( chatMsg, int.Parse(chatMsg.getMsgtype()) );
                    }
                    break;

                default:
                    mng = parseManageData(buffer, pos, lastLen, out paseLen);
                    if (mng != null)
                    {
                        ParseResponse( mng, mng.getMessageId() );
                    }
                    break;
            }

            return paseLen;
        }

        private void ParseVideoResponse(VideoMessage videomsg, int response)
        {
            switch (response)
            {
                case MyType.VIDEO_MESSAGE:
                    OnVideoMessageReceived(videomsg);
                    break;
                default  :                    
                    break;
            }
        }

        private void ParseAVChatNewResponse(AVChatNewMessage videomsg, int response)
        {
            switch (response)
            {
                case MyType.TYPE_AV_CHAT_NEW:
                    OnAVChatNewMessageReceived(videomsg);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Parse call response
        /// </summary>
        /// <param name="user">Caller</param>
        /// <param name="response">Response</param>
        /// <param name="address">Caller Address</param>
        /// 
        private void ParseResponse( ChatMessage chat, int response )
        {
            switch (response)
            {
                case MyType.TYPE_P2G_CHAT_TEXT:
                    OnChatMessageReceived(chat);
                    break;
                case MyType.TYPE_P2P_CHAT_TEXT:
                    OnChatMessageReceived(chat);
                    break;

                case MyType.TYPE_P2G_CHAT_FILE:
                    OnChatMessageReceived(chat);
                    break;
                case MyType.TYPE_P2P_CHAT_FILE:
                    OnChatMessageReceived(chat);
                    break;
            }
        }

        /// <summary>
        /// Parse call response
        /// </summary>
        /// <param name="user">Caller</param>
        /// <param name="response">Response</param>
        /// <param name="address">Caller Address</param>
        private void ParseResponse( ManageDataPacket mdp, int response )
        {
            switch ( response )
            {
                case MyType.TYPE_SYS_MESSAGE:
                    OnMessageReceived(mdp);
                    break;

                case MyType.TYPE_MIC_SUCCESS:
                    
                    AEC_State = 1;

                    OnMessageReceived(mdp);
                    break;

                case MyType.TYPE_MIC_FAILED:
                    //EndChat();
                    AEC_State = 0;
                    OnMessageReceived(mdp);
                    break;

                case MyType.TYPE_PERSON_INVITE:
                    OnMessageReceived(mdp);
                    break;

                case MyType.TYPE_PERSON_INVITE_RELEASE:
                    OnMessageReceived(mdp);
                    break;

                //2017.9.25 加入对poc解散组的处理
                case MyType.TYPE_DELETE_GROUP:
                    OnMessageReceived(mdp);
                    break;
                case MyType.TYPE_RECIVED_INVITE:
                    OnMessageReceived(mdp);
                    break;
                //2017.12.04 对实时定位的处理
                case MyType.TYPE_GPS_COMMAND:
                    OnMessageReceived(mdp);
                    break;
                case MyType.TYPE_SOS_LOCATION:
                    //sos按键报警
                    OnMessageReceived(mdp);
                    break;
                case MyType.TYPE_SOS_KEY_RELEASE:
                    //sos按键放开
                    OnMessageReceived(mdp);
                    break;
                case MyType.KICK_OFF:
                    //被同一个帐号在另外地方登入踢出
                    OnMessageReceived(mdp);
                    break;
                default:
                    break;                

            }
        }


        private void StartRecordingMedia()
        {
            try
            {
                ReleaseWaveRecord();
                InitWaveRecord();
            }
            catch (Exception e)
            {
            }
        }
        private void EndRecordingMedia()
        {
            try
            {
                ReleaseWaveRecord();
            }
            catch (Exception e)
            {
            }
        }
        private WaveIn sourceRecordStream = null;
        private void InitWaveRecord()
        {
            if (sourceRecordStream == null)
            {
                sourceRecordStream = new WaveIn();
                sourceRecordStream.BufferMilliseconds = 20;
                sourceRecordStream.DeviceNumber = InputAudioDevice;
                sourceRecordStream.WaveFormat = new WaveFormat(8000, 16, 1);
                sourceRecordStream.DataAvailable += sourceStream_DataAvailable;
            }
        }
        private void ReleaseWaveRecord()
        {
            if (sourceRecordStream != null)
            {
                sourceRecordStream.StopRecording();
                sourceRecordStream.DataAvailable -= sourceStream_DataAvailable;
                sourceRecordStream.Dispose();
                sourceRecordStream = null;
            }
        }

        /*
        public void EndChat()
        {
            if (recordingbuffer != null)
                recordingbuffer.Reset();
            if (sourceRecordStream != null)
                sourceRecordStream.StopRecording();
        }
        */

        /*
        private void StartChat()
        {
            if (recordingbuffer != null)
                recordingbuffer.Reset();
            if (sourceRecordStream != null)
                sourceRecordStream.StartRecording();
        }
        */
        private void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            if ( sourceRecordStream == null ) return;
                 RecordMediaData( e.Buffer, e.BytesRecorded );
        }
        public void RecordMediaData(byte[] buf, int bytesRecorded)
        {
            try
            {
                //如何考虑是否抢到了麦和有效时间30秒内?
                if ( bytesRecorded > 0 )
                     recordingbuffer.Write(buf, 0, bytesRecorded);

                if ( recordingbuffer.Count > 320 )
                {
                    byte[] inbuf = new byte[320];
                    recordingbuffer.Read(inbuf, 0, 320);
                    
                    sendQueue.Add(inbuf);
                }
            }
            catch (Exception)
            {
            }
        }



        public bool InitWavePlayDevices(int deviceNumber)
        {
            if (receivedPlayStream == null)
            {
                wavePlayProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
                receivedPlayStream = new WaveOut();
                receivedPlayStream.DeviceNumber = deviceNumber;
                receivedPlayStream.Init(wavePlayProvider);
            }

            StartPlay();
            return true;
        }
        public void ReleaseWavePlayDevices()
        {
            EndPlay();

            wavePlayProvider.ClearBuffer();
            if (receivedPlayStream != null)
                receivedPlayStream.Stop();
            if (receivedPlayStream != null)
                receivedPlayStream.Dispose();
            receivedPlayStream = null;
        }
        private void EndPlay()
        {
            try
            {
                if (receivedPlayStream != null)
                    receivedPlayStream.Stop();
            }
            catch (Exception)
            {
            }
        }
        private void StartPlay()
        {
            try
            {
                if (receivedPlayStream != null)
                    receivedPlayStream.Play();
            }
            catch (Exception)
            {
            }
        }
        public void PlayMediaData( byte[] Buffer, int bytesRead )
        {
            try
            {
                 if(wavePlayProvider!=null)
                    wavePlayProvider.AddSamples(Buffer, 0, bytesRead);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="recipient"></param>
        public void SendMessage(byte[] data)
        {
            Debug.WriteLine("SendMSG:" + CLog.ByteArrayToStr(data, 0));
            if (data != null && data.Length > 0)
            {
                try
                {
                    server.Client.Send(data);
                }
                catch (ArgumentNullException e1)
                {
                    Debug.WriteLine("SendMSG: ArgumentNullException" + e1.Message);
                }
                catch (SocketException e2)
                {
                    //试图访问套接字时发生错误。有关更多信息，请参见备注部分
                    if (!IsSocketConnected())
                    {
                        //重连接
                        Debug.WriteLine("IsSocketConnected()=false, Socket已经断线: 进行重连");
                        IsConnected = false;
                        IsReceivingData = false;
                        AEC_State = 0;
                        Debug.WriteLine("IsSocketConnected()=false, IsReceivingData = false");
                        Thread.Sleep(200);
                        ServerDisconnected(this, new ServerEventArgs(new ManageDataPacket())); //再重新初始化网络
                    }
                }
                catch (ObjectDisposedException e3)
                {
                    //System.Net.Sockets.Socket 已关闭。
                    //重连接
                    Debug.WriteLine("ObjectDisposedException socket已闭, Socket已经断线: 进行重连");
                    IsConnected = false;
                    IsReceivingData = false;
                    AEC_State = 0;

                    Debug.WriteLine("ObjectDisposedException socket已闭, IsReceivingData = false");
                    Thread.Sleep(200);
                    ServerDisconnected(this, new ServerEventArgs(new ManageDataPacket())); //再重新初始化网络
                }
            }
        }

        /// <summary>
        /// Send message, old 版本
        /// </summary>
        /// <param name="message"></param>
        /// <param name="recipient"></param>
        //public void SendMessage(byte[] data)
        //{
        //    Debug.WriteLine("SendMSG:" + CLog.ByteArrayToStr(data, 0));
        //    if (data != null && data.Length > 0)
        //    {
        //        try
        //        {
        //            server.Client.Send(data);
        //        }
        //        catch (ArgumentNullException e1)
        //        {
        //            Debug.WriteLine("SendMSG: ArgumentNullException" + e1.Message);
        //        }
        //        catch (SocketException e2)
        //        {
        //            //试图访问套接字时发生错误。有关更多信息，请参见备注部分
        //            if (!IsSocketConnected())
        //            {
        //                //重连接
        //                IsConnected = false;
        //                Thread.Sleep(200);
        //                NetInit(); //再重新初始化网络
        //            }
        //        }
        //        catch (ObjectDisposedException e3)
        //        {
        //            //System.Net.Sockets.Socket 已关闭。
        //            //重连接
        //            IsConnected = false;
        //            Thread.Sleep(200);
        //            NetInit(); //再重新初始化网络
        //        }
        //    }
        //}

        private bool IsSocketConnected()
        {
            #region remarks  
            /******************************************************************************************** 
             * 当Socket.Conneted为false时， 如果您需要确定连接的当前状态，请进行非阻塞、零字节的 Send 调用。 
             * 如果该调用成功返回或引发 WAEWOULDBLOCK 错误代码 (10035)，则该套接字仍然处于连接状态；  
             * 否则，该套接字不再处于连接状态。 
             * Depending on http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.connected.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-2 
            ********************************************************************************************/
            #endregion

            #region 过程  
            // This is how you can determine whether a socket is still connected.  
            bool connectState = true;
            bool blockingState = server.Client.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                server.Client.Blocking = false;
                server.Client.Send(tmp, 0, 0);
                //Console.WriteLine("Connected!");  
                connectState = true; //若Send错误会跳去执行catch体，而不会执行其try体里其之后的代码  
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK  
                if (e.NativeErrorCode.Equals(10035))
                {
                    //Console.WriteLine("Still Connected, but the Send would block");  
                    connectState = true;
                }

                else
                {
                    //Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);  
                    connectState = false;
                }
            }
            finally
            {
                server.Client.Blocking = blockingState;
            }

            //Console.WriteLine("Connected: {0}", client.Connected);  
            return connectState;
            #endregion
        }

        /// <summary>
        /// Closes server connection
        /// </summary>
        public void CloseConnection()
        {
            IsConnected = false;
        }
        #endregion

        #region Event Invokers
        protected virtual void OnMessageReceived( ManageDataPacket mdp )
        {
            var handler = MessageReceived;
            if (handler != null)
                handler(this, new ServerEventArgs(mdp) );
        }
        protected virtual void OnChatMessageReceived(ChatMessage chat)
        {
            var handler = ChatMessageReceived;
            if (handler != null)
                handler( this, new ServerEventArgs(chat) );
        }
        //2017.9.27 新增处理视频的报文
        protected virtual void OnVideoMessageReceived(VideoMessage packet)
        {
            var handler = VideoMessageReceived;
            if (handler != null)
                handler(this, new ServerEventArgs(packet));
        }

        //2022.11.09   OnAVChatNewMessageReceived
        protected virtual void OnAVChatNewMessageReceived(AVChatNewMessage packet)
        {
            var handler = AVChatNewMessageReceived;
            if (handler != null)
                handler(this, new ServerEventArgs(packet));
        }


        #endregion

    }

}
