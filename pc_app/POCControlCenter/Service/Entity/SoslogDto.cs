using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    public class SoslogDto
    {
        public long id { get; set; }
        /**
         * 纬度
         */
        public Double latitude { get; set; }
        /**
         * 经度
         */
        public Double longitude { get; set; }
        /**
         *
         */
        public int userId { get; set; }
        /**
         *
         */
        public int recdate { get; set; }
        /**
         * 报警级别,1表示SOS, 2 表示其它
         */
        public int alarmLevel { get; set; }
        /**
         * 定位类型: 如 baidu,gps,Google
         */
        public String gpsType { get; set; }
        /**
         * SOS都有录音文件，这里写下文件名称
         */
        public String voiceFilename { get; set; }

        /**
         * 关联到 ptt_user_audiorec的 audio_path字段, hdd下相对路径
         */
        public String audioPath { get; set; }

        /**
         * 关联到 ptt_user_audiorec的 duration字段，时长 (单位: 秒)
         */
        public Double duration { get; set; }

        /**
         * GPS坐标反向查询地址描述
         */
        public String gpsAddr { get; set; }



        public String recdateStr { get; set; }



        public String userName { get; set; }

        

        public int cmpid { get; set; }
    }
}
