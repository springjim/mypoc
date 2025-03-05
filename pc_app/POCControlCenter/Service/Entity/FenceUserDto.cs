using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    public class FenceUserDto
    {
        public int id { get; set; }
        /**
         *
         */
        public int?  fenceId { get; set; }
        /**
         *
         */
        public int?  userId { get; set; }
        /**
         * 规则类型,是进入警告(income)或出去警告(outcome)
         */
        public String ruleType { get; set; }
        /**
         * 是include(黑名单)或exclude  (白名单)
         */
        public String ruleAssign { get; set; }
        /**
         * 指定规则的生效开始时间
         */
        public String startTime { get; set; }
        /**
         * 指定规则的生效结束时间
         */
        public String endTime { get; set; }


        public String fencePoints { get; set; }
               
        public int cmpId { get; set; }

        public String fenceName { get; set; }
        public String userName { get; set; }
    }
}
