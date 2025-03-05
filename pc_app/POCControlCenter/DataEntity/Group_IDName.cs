using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    class Group_IDName
    {
        public int group_id { get; set; }        
        public String group_name { get; set; }

        public Group_IDName(int group_id, String group_name)
        {
            this.group_id = group_id;
            this.group_name = group_name;
        }

        public override string ToString()
        {
            return this.group_name;
        }

    }

    

}
