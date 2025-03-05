using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class PageQueryTotal
    {
        public int count { get; set; }
        public int page_num { get; set; }
    }

    public class NetHttpPageQueryTotalData
    {
        public String error { get; set; }
        public PageQueryTotal  data { get; set; }
    }
}
