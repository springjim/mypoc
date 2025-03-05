using POCControlCenter.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class GroupListResponse : ResponseBase
    {
        public List<GroupDto> data { get; set; }
    }
}
