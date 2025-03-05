using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    class BaiduGeoLat
    {
        public int status { get; set; }
        public BaiduGeoLat_result result { get; set; }

    }

    class BaiduGeoLat_result
    {
        public string sematic_description { get; set; }
        public string business { get; set; }
        public string cityCode { get; set; }
        public string formatted_address { get; set; }
    }
}
