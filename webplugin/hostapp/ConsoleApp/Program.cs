using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using ConsoleApp.Tool;
using POCClientNetLibrary;
using ConsoleApp.Model.Request;
using ConsoleApp.Service;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp
{
    class Program
    {

        public static ChatClient client;
        public static ChromeRequestHandler requestHandler;
        public static ChromeResponseHandler responseHandler;
        public static ContextService contextService;


        static void Main(string[] args)
        {

            try
            {

                // 设置输入和输出的编码为 UTF-8
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;

                //初始化日志
                Log.Open();   //日志文件   在当前工作目录下log子目录中   
                Process processes = Process.GetCurrentProcess();
                Log.I(String.Format("Progress <{0}, {1}>", processes.ProcessName, processes.Id));

                //读取配置文件：含socket服务器的IP和Port
                string serverIp = CommUtils.readServerIp();
                string serverPort = CommUtils.readServerPort();
                if (string.IsNullOrEmpty(serverIp) || string.IsNullOrEmpty(serverPort))
                {
                    Log.E("程序异常，未发现正确的serverIp或serverPort");
                    Log.Close();
                    return;  //退出
                }
                else
                {
                    Log.I(String.Format("serverIp: {0}, serverPort: {1}", serverIp, serverPort));
                }

                //初始化socket client

                client = new ChatClient(Int32.Parse(serverPort), serverIp);
                responseHandler = new ChromeResponseHandler();
                contextService = null;   //这个要延迟到，loginplatform下达上线时才创建
                requestHandler = new ChromeRequestHandler(client, responseHandler, contextService);                            
                          
                //浏览器端：按照业务来说，连接服务器后，还要按顺序调用
                //1.  TYPE_LOGIN_PLATFORM ：随后调用心跳  （这个要先调用）
                //2.  各类业务......
                //3.  TYPE_LOGOUT: 帐号下线

                //4.  将来是不是要从 chrome可以设置选 音频输入、音频输出的设备名称, 这样可以设备到 contextService中

                while (true)
                {
                    try
                    {
                        // 读取消息长度（4 字节）                    
                        byte[] lengthBytes = new byte[4];
                        Console.OpenStandardInput().Read(lengthBytes, 0, 4);
                        int messageLength = BitConverter.ToInt32(lengthBytes, 0);

                        if (messageLength == 0) continue;
                        
                        // 读取消息内容
                        byte[] messageBytes = new byte[messageLength];
                        Console.OpenStandardInput().Read(messageBytes, 0, messageLength);
                        string messageJson = Encoding.UTF8.GetString(messageBytes);

                        Log.I("收到:"+ messageJson);
                     
                        //处理收到的原始字符串

                        // string receivedJson = "{\"type\":\"SEND_TO_NATIVE_APP\",\"message\":\"{\\\"messageType\\\":\\\"TYPE_LOGOUT\\\",\\\"messageId\\\":\\\"abc1232afdsfsdfd\\\",\\\"messageDesc\\\":\\\"下达调度台登出信令\\\",\\\"groupId\\\":\\\"someGroupId\\\",\\\"userId\\\":\\\"someUserId\\\"}\"}";

                       
                        //进入处理
                        try
                        {
                            messageJson = messageJson.Replace("\\\"", "\"");
                            messageJson = messageJson.Trim('"');   //去除两头的双引号
                          
                            JObject outerObject = JObject.Parse(messageJson);
                            string newjson = JsonConvert.SerializeObject(outerObject);                          

                            RequestBase requestBase = JsonConvert.DeserializeObject<RequestBase>(newjson);
                            requestHandler.handler(requestBase, newjson);

                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show(ee.Message);
                        }                       



                    }
                    catch (Exception ex)
                    {
                        
                        Log.E("Error:" + ex.StackTrace);

                        Log.Close();
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("error:" + e.StackTrace);
                Log.E("Error:" + e.StackTrace);
                Log.Close();
            }

        }
    }
}
