using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class VideoRec
    {
        public int seqno { get; set; }  //结果集的序号
        public int id { get; set; }
        public int user_id { get; set; }
        public String user_name { get; set; }  //关联得到

        public string cust_account { get; set; } //关联得到: 客户编号

        public String app { get; set; }
        public String stream { get; set; }
        /// <summary>
        /// 秒单位的时间
        /// </summary>
        public int recorddate { get; set; }
        public String record_date_str { get; set; }  //分析得到

        public String live_lng { get; set; }
        public String live_lat { get; set; }

        /**
         * 视频存放全路径，如  /opt/lampp/htdocs/record/live_600010169_114b21479_22b12542-1481255822-2016-12-09-115702.flv
         */
        public String vedio_path { get; set; }
        public String vedio_name { get; set; }
        public double duration { get; set; }
        public double filesize { get; set; }

        /**
         * 视频封面文件全路径 如 /opt/lampp/htdocs/record/moni_600010172_6b6_8b8-1481253973-2016-12-09-112613.jpg
         */
        public String poster_path { get; set; }

        /**
         * 视频类型:LIVE, TALK,MEET, MONI
         */
        public String video_type { get; set; }
        public String video_type_str { get; set; } //分析获得
        public String map_type { get; set; }

        public int bitrate { get; set; }
        public String vcodec { get; set; }

        public String vformat { get; set; }
        public int resolution_width { get; set; }
        public int resolution_height { get; set; }

        public String acodec { get; set; }
        public int asamplerate { get; set; }
        public double fps { get; set; }
        /**
         * 客户定义的视频分类
         */
        public String cust_category { get; set; }

        /**
         * 存储分区:1表示永久区, 0表示临时区
         */
        public int storeflag { get; set; }

        public String storeflag_str { get; set; }  //分析得到

        /**
         * 位置描述,例如:福田区市花路15号
         */
        public String fixloc_place { get; set; }

    }

    public class NetHttpVideoRecData
    {
        public String error { get; set; }

        public List<VideoRec> data { get; set; }
    }
}
