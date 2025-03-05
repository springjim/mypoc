using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    public class UserAudioRecDto
    {
        public  long id { get; set; }
        /**
         * 
         */
        public int groupId { get; set; }
        /**
         * 
         */
        public int userId { get; set; }
        /**
         * 声音时长,单位:秒
         */
        public double?  duration { get; set; }
        /**
         * 记录日期
         */
        public int recorddate { get; set; }
        /**
         * 文件大小,单位kb
         */
        public Double filesize { get; set; }
        /**
         * 声音文件名称
         */
        public String audioName { get; set; }
        /**
         * 声音文件路径
         */
        public String audioPath { get; set; }
    }
}
