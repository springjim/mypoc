using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class PageResponseBase<T> : ResponseBase
    {
        public Page page { get; set; }

        public class Page
        {
            public int currPage { get; set; }
            public int totalPage { get; set; }
            public int totalCount { get; set; }
            public int pageSize { get; set; }
            public List<T> list { get; set; }
        }
    }
}
