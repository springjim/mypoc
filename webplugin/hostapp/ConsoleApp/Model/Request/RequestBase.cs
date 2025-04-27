using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Model.Request
{

    /// <summary>
    /// 从浏览器插件发过来的请求消息基类
    /// </summary>
   public  class RequestBase
    {
        public string messageType { get; set; }
        public string messageId { get; set; }

    }
}
