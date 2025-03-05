using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class FenceInfo
    {
        public int fenceId { get; set; }
        public String fenceName { get; set; }
        public int userId { get; set; }
        public int groupId { get; set; }
        public String fencePoints { get; set; }
        public String startTime { get; set; }
        public String endTime { get; set; }
        public short alertType { get; set; }
        public String loactionname { get; set; }

        public int cmpid { get; set; }
    }

    public class NetHttpFenceInfoData
    {
        public String error { get; set; }

        public List<FenceInfo> data { get; set; }
    }

}
