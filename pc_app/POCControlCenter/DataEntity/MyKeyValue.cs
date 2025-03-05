using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.DataEntity
{
   public class MyKeyValue
    {
        private string Key { get; set; }

        private string Value { get; set; }

        public MyKeyValue(string pid, string pName)

        {
            this.Key = pid;
            this.Value = pName;
        }

        public string pKey

        {
            get { return Key; }

        }
        public string pValue

        {
            get { return Value; }

        }

        public override string ToString()
        {
            return this.Key+"="+this.Value;
        }

        /// <summary>
        /// 根据MyKeyValue中的Value找到特定的ListItem(仅在ComboBox的Item都为MyKeyValue时有效)
        /// </summary>
        /// <param name="cmb">要查找的ComboBox</param>
        /// <param name="strValue">要查找MyKeyValue的Value</param>
        /// <returns>返回传入的ComboBox中符合条件的第一个MyKeyValue，如果没有找到则返回null.</returns>
        public static MyKeyValue FindByValue(ComboBox cmb, string strValue)
        {
            foreach (MyKeyValue li in cmb.Items)
            {
                if (li.pValue == strValue)
                {
                    return li;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据ListItem中的Key找到特定的ListItem(仅在ComboBox的Item都为ListItem时有效)
        /// </summary>
        /// <param name="cmb">要查找的ComboBox</param>
        /// <param name="strValue">要查找ListItem的Key</param>
        /// <returns>返回传入的ComboBox中符合条件的第一个ListItem，如果没有找到则返回null.</returns>
        public static MyKeyValue FindByKey(ComboBox cmb, string strKey)
        {
            foreach (MyKeyValue li in cmb.Items)
            {
                if (li.pKey == strKey)
                {
                    return li;
                }
            }
            return null;
        }

    }
}
