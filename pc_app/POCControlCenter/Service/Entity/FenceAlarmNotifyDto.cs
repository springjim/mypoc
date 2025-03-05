using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    public class FenceAlarmNotifyDto
    {       
         
       public int id { get; set; }
        /**
         *
         */
        public int fenceId { get; set; }
        /**
         *
         */
        public int userId { get; set; }
        /**
         * 警告类型,有 outcome, income 之分
         */
        public String alarmType { get; set; }
        /**
         * 警各的开始时间
         */
        public int startActionTime { get; set; }
        /**
         * 警各的结束时间
         */
        public int endActionTime { get; set; }
        /**
         * 监控的开始时纬度
         */
        public Double startLatitude { get; set; }
        /**
         * 监控的开始时经度
         */
        public Double startLongitude { get; set; }
        /**
         * 监控的结束时纬度
         */
        public Double endLatitude { get; set; }
        /**
         * 监控的结束时纬度
         */
        public Double endLongitude { get; set; }
        /**
         * 处理状态, Y 已经处理，由调度台设置
         */
        public String processStatus { get; set; }

        /**
         * 触发通知警告的时间
         */
        public int notifyTime { get; set; }

        public String fenceName { get; set; }
        public String fencePoints { get; set; }
        public String userName { get; set; }
        /**
         * 警告名称 outcome: 出去警告, income 进入警告
         */
        public String alarmTypeName { get; set; }

        /**
         * 警各的开始时间
         */
        public String startActionTimeStr { get; set; }
        /**
         * 警各的结束时间
         */
        public String endActionTimeStr { get; set; }
        /**
         * 触发通知警告的时间
         */
        public String notifyTimeStr { get; set; }

        /**
         * 停留时间,单位:分
         */
        public Double stayTimeMin { get; set; }

    }
}
