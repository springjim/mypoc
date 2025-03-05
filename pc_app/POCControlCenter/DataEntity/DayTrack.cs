using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public  class DayTrack
    {        
        public int user_id { get; set; }
        public String user_name { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string task_name { get; set; }

        public string upstart_time { get; set; }
        public string upstart_locdesc { get; set; }

        public string upend_time { get; set; }
        public string upend_locdesc { get; set; }

        public string dnstart_time { get; set; }
        public string dnstart_locdesc { get; set; }

        public string dnend_time { get; set; }
        public string dnend_locdesc { get; set; }

        public String loactionname { get; set; }

        //2017.5.19  分析日期
        public string analyzeday { get; set; }
      
    }

    public class NetHttpDayTrackData
    {
        public String error { get; set; }
        public List<DayTrack> data { get; set; }
    }


}
