using POCControlCenter.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class GroupTempListResponse : ResponseBase
    {
        public List<GroupTempDto> data { get; set; }
    }
}
