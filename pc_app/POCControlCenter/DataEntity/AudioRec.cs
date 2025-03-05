using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class AudioRec
    {
        public  int seqno { get; set; }  //结果集的序号

        public  int id { get; set; }

        public int user_id { get; set; }
        public String user_name { get; set; }   //关联得到
        public string cust_account { get; set; } //关联得到: 客户编号

        public int group_id { get; set; }
        public String group_name { get; set; }  //关联得到	

        public int recorddate { get; set; }
        public String record_date_str { get; set; }  //分析得到

        /**
         * 音频存放全路径，如  /hdd/htdocs/voice//2017-08-24/80001196/600010171/80001196_600010171_1503560929786.amr
         */
        public String audio_path { get; set; }
        /**
         * 音频文件名称,无扩展名称,如80001196_600010171_1503560929786
         */
        public String audio_name { get; set; }

        /**
         * 时长，单位秒
         */
        public double duration { get; set; }
        /**
         * 文件大小,单位KB
         */
        public double filesize { get; set; }

    }

    public class NetHttpAudioRecData
    {
        public String error { get; set; }

        public List<AudioRec> data { get; set; }
    }


}
