using ConsoleApp.Model.Request;
using ConsoleApp.Model.Response;
using ConsoleApp.Tool;
using Newtonsoft.Json;
using POCClientNetLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp.Service
{
    /// <summary>
    /// 处理来自chrome 插件的请求
    /// </summary>
    public class ChromeRequestHandler
    {

        private ChromeResponseHandler responseHandler;
        private ContextService contextService;
        private ChatClient client;

        public ChromeRequestHandler(ChatClient client, ChromeResponseHandler responseHandler, ContextService contextService)
        {
            this.responseHandler = responseHandler;
            this.contextService = contextService;
            this.client = client;
        }


        public void handler(RequestBase requestBase, string messageJson)
        {
            //
            string messageType = requestBase.messageType;
            switch (messageType)
            {
                case "TYPE_LOGIN_PLATFORM":
                    //处理登录业务
                    RequestLoginPlatform req = JsonConvert.DeserializeObject<RequestLoginPlatform>(messageJson);

                    //MessageBox.Show("收到TYPE_LOGIN_PLATFORM:" + messageJson + req.messageId);                                                      

                    //创建socket连接                    

                    if (this.contextService == null)
                    {
                        try
                        {
                             contextService = new ContextService(client, responseHandler);                           
                            
                            Log.I("contextService");
                            contextService.InitChatClientCallback(); //这里先加入client的各种回调处理
                            Log.I(" contextService.InitChatClientCallback OK");
                            client.NetInit();  //开始socket网络连接            
                            Log.I("NetInit OK");
                            contextService.InitWaveRecord();  //初始化录音设备
                            contextService.InitWavePlay();      //初始化音箱设备

                            //发送socket后，再返回成功给chrome端
                            client.SendMessage((new Data()).LoginPlatformMessageEncode(-1, Convert.ToInt32(req.userId)));

                            //开启心跳
                            client.StartHeartBeat();

                            ResponseReply replyMessage = ResponseReply.CreateSuccessReply(req.messageId);
                            responseHandler.sendReply(replyMessage);

                            Log.I("已连接socket服务器 OK");
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.StackTrace);

                            Log.E("登录error:" + e.StackTrace);
                        }

                    }
                    else
                    {
                        Log.I("已经有了contextService");
                    }                   



                    break;

                case "TCP_REPORT":
                    //处理上报工作对讲组
                    RequestReportPlatform req2 = JsonConvert.DeserializeObject<RequestReportPlatform>(messageJson);
                    if (!client.IsConnected)
                    {
                        Log.E("收到TCP_REPORT，但socet没有连接");
                        return;
                    }
                    //发送socket
                    client.SendMessage(
                (new Data()).ReportMessageEncode(Convert.ToInt32(req2.groupId), Convert.ToInt32(req2.userId)));
                    ResponseReply replyMessage2 = ResponseReply.CreateSuccessReply(req2.messageId);
                    responseHandler.sendReply(replyMessage2);

                    break;

                case "TCP_APPLY_MIC":
                    //处理抢麦
                    RequestReportPlatform reqMic = JsonConvert.DeserializeObject<RequestReportPlatform>(messageJson);
                    if (!client.IsConnected)
                    {
                        Log.E("收到TYPE_ROB_MIC，但socet没有连接");
                        return;
                    }

                    if (!MediaCheckUtils.checkValidMic())
                    {
                        Log.E("收到TYPE_ROB_MIC，但检查电脑没有麦克风");
                        ResponseReply replyMicFail = ResponseReply.CreateFailureReply(reqMic.messageId, -1, "电脑没有麦克风");
                        responseHandler.sendReply(replyMicFail);
                        return;
                    }
                    //
                    //发送socket
                    client.SendMessage((new Data(MyType.TYPE_ROB_MIC)).ToByte());
                    ResponseReply replyMicSucc = ResponseReply.CreateSuccessReply(reqMic.messageId);
                    responseHandler.sendReply(replyMicSucc);


                    break;

                case "TCP_RELEASE_MIC":
                    //处理释放麦
                    RequestReportPlatform reqReleaseMic = JsonConvert.DeserializeObject<RequestReportPlatform>(messageJson);
                    if (!client.IsConnected)
                    {
                        Log.E("收到TYPE_REALASE_MIC，但socet没有连接");
                        return;
                    }
                    //发送socket
                    client.SendMessage((new Data(MyType.TYPE_REALASE_MIC)).ToByte());
                    ResponseReply replyReleaseMicSucc = ResponseReply.CreateSuccessReply(reqReleaseMic.messageId);
                    responseHandler.sendReply(replyReleaseMicSucc);
                    //
                    contextService.EndChat();
                    client.AEC_State = 0;


                    break;

                case "TYPE_LOGOUT":

                    //处理登出
                    RequestLogout reqLogout = JsonConvert.DeserializeObject<RequestLogout>(messageJson);
                    if (!client.IsConnected)
                    {
                        Log.E("收到TYPE_LOGOUT，但socet没有连接");
                        return;
                    }

                    //先返回chrome
                    ResponseReply replyLogout = ResponseReply.CreateSuccessReply(reqLogout.messageId);
                    responseHandler.sendReply(replyLogout);

                    //再处理socket client和资源释放等
                    Log.I("中断TCP线程开始");
                    contextService.ReleaseWaveRecord();   //停止麦克风采集

                    client.Exit();
                    client.StopHeartBeat();
                    client.SendMessage((new Data(MyType.TYPE_REALASE_MIC)).ToByte());    //再发一次
                    // 
                    client.SendMessage(
                (new Data()).LogoutMessageEncode(Convert.ToInt32(reqLogout.groupId), Convert.ToInt32(reqLogout.userId)));

                    this.contextService.UnInitChatClientCallback();
                    client.CloseConnection();
                    client.ReleaseWavePlayDevices();

                    this.contextService = null;                    

                    //client.Release();   //关闭socket, 并将 socketclient设为null

                    Log.I("中断TCP线程完成");
                    //退出整个应用

                    Environment.Exit(0); 

                    break;

                default:
                    break;
            }

        }

    }
}
