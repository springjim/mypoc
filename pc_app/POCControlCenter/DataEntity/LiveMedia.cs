using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class LiveMedia
    {
        public int user_id { get; set; }
        public String streamurl { get; set; }
        public int pubdate { get; set; }

        public String username { get; set; }
        public String video_type { get; set; }
        public String lng { get; set; }
        public String lat { get; set; }

        public String place { get; set; }
        public String app { get; set; }
        public String stream { get; set; }

    }

    public class NetHttpLiveMediaData
    {
        public String error { get; set; }

        public List<LiveMedia> data { get; set; }

    }
}
