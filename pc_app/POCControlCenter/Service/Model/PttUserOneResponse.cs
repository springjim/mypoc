using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    /// <summary>
    /// 查询指定的一个用户
    /// </summary>
    public class PttUserOneResponse : ResponseBase
    {
        public User data { get; set; }
    }
}
