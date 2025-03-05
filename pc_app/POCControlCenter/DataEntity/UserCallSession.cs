using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class UserCallSession
    {
        public int calltype;     // 0:主叫   1：被叫
        public int callid;       // 主叫ID
        public int calledid;     // 被叫ID 
        public int callgroudid;  // call group id
        public int curgroudid;   // call pre current groud id;
        public int state;        //

        public UserCallSession()
        {
            calltype    = -1; //
            state       = -1; //
            callid      = -1; // 主叫ID
            calledid    = -1; // 被叫ID 
            callgroudid = -1; // call group id
            curgroudid  = -1; // call pre current groud id;
        }
    }

    public class NetHttpCallData
    {
        public String error { get; set; }
        public int    data  { get; set; }
    }
}
