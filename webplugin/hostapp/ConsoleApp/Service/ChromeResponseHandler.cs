using ConsoleApp.Model.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Service
{
    public class ChromeResponseHandler
    {
        public void sendReply(ResponseReply  replyMessage)
        {
            // 自定义序列化设置
            var settings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii // 转义非 ASCII 字符
            };

            string responseJson = JsonConvert.SerializeObject(replyMessage, settings);

            //// 发送响应长度（4 字节）
            byte[] responseLengthBytes = BitConverter.GetBytes(responseJson.Length);
            Console.OpenStandardOutput().Write(responseLengthBytes, 0, 4);

            //// 发送响应内容
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
            Console.OpenStandardOutput().Write(responseBytes, 0, responseBytes.Length);

        }

        /// <summary>
        /// 通用消息
        /// </summary>
        public void sendMessage(ResponseBase message)
        {
            // 自定义序列化设置
            var settings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii // 转义非 ASCII 字符
            };

            string responseJson = JsonConvert.SerializeObject(message, settings);

            //// 发送响应长度（4 字节）
            byte[] responseLengthBytes = BitConverter.GetBytes(responseJson.Length);
            Console.OpenStandardOutput().Write(responseLengthBytes, 0, 4);

            //// 发送响应内容
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
            Console.OpenStandardOutput().Write(responseBytes, 0, responseBytes.Length);
        }



    }
}
