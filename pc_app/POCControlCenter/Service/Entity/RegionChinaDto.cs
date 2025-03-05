using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    public class RegionChinaDto
    {
        public int id { get; set; }
        public int code { get; set; }
        public int parentCode { get; set; }
        public string myname { get; set; }
        public int mylevel { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

    }
}
