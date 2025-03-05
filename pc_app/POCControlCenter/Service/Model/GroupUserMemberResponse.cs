using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
   public class GroupUserMemberResponse : ResponseBase
    {
        public List<User> data { get; set; }
    }
}
