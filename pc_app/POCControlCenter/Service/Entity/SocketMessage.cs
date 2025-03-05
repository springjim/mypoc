using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    public class SocketMessage<T>
    {
        public String messageType { get; set; }
        public T data { get; set; }
    }
}
