using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Model.Request
{
    public class RequestLogout : RequestBase
    {
        public string groupId { get; set; }
        public string userId { get; set; }
    }
}
