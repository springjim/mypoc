using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Model.Request
{
    /// <summary>
    /// 上报工作组信令
    /// </summary>
    public class RequestReportPlatform: RequestBase
    {
        public string groupId { get; set; }
        public string userId { get; set; }
    }
}
