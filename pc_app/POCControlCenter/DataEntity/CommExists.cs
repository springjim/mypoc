using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    /// <summary>
    /// 表示服务器返回的是否存在，其中 error 为0或-1, data为false 或true
    /// </summary>
    public  class CommExists
    {
        public string error { get; set; }
        public string data { get; set; }
    }
}
