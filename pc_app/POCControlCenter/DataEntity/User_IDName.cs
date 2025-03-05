using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    class User_IDName
    {
        public int user_id { get; set; }        
        public String user_name { get; set; }

        public User_IDName(int user_id, String user_name) {
            this.user_id = user_id;
            this.user_name = user_name;
        }

        public override string ToString()
        {
            return this.user_name;
        }

    }
}
