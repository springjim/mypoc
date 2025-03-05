using POCControlCenter.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class RegionChinaResponse : ResponseBase
    {
        public List<RegionChinaDto> data { get; set; }
    }
}
