using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace POCControlCenter.Service.Model
{
   public  class AllFenceInfoResponse : ResponseBase
    {
        public List<FenceInfo> data { get; set; }
    }
}
