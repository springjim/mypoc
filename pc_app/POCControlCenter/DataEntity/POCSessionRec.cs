using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class POCSessionRec
    {
        public  int seqno { get; set; }  //结果集的序号

        public int file_id { get; set; }
        public String file_key { get; set; }
        public String file_name { get; set; }

        public String file_type { get; set; }

        public String file_type_str { get; set; } //分析得到

        public String file_MD5 { get; set; }
        public String file_msgcontent { get; set; }

        public int file_size { get; set; }
        public int file_upsize { get; set; }
        public int file_upload { get; set; }

        public long file_uptime { get; set; }
        public String file_uptime_str { get; set; }  //分析得到, 2017/08/30 hh:mm:ss 格式

        public int group_id { get; set; }
        public int user_id { get; set; }
        public String user_name { get; set; }   //关联得到
        public String group_name { get; set; }  //关联得到
        public string cust_account { get; set; } //关联得到: 客户编号
    }
    public class NetHttpPOCSessionRecData
    {
        public String error { get; set; }
        public List<POCSessionRec> data { get; set; }
    }
}
