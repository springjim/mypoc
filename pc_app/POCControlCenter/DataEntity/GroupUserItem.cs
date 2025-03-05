using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class GroupUserItem
    {
        public int group_id { get; set; }
        public int user_id { get; set; }
        public int voicepriv { get; set; }

        public GroupUserItem(int group_id, int user_id, int voicepriv)
        {
            this.group_id = group_id;
            this.user_id = user_id;
            this.voicepriv = voicepriv;
        }
    }
}
