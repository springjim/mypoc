using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class ResponseBase
    {
        public int code { get; set; }
        public int error { get; set; }
        public string errorMsg { get; set; }
    }
}
