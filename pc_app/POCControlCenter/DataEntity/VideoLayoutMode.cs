using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public  enum VideoLayoutMode
    {

        /// <summary>
        /// 特殊的L型排版，模仿捷思锐的L型，3列3行，其中第一路跨2列2行
        /// </summary>
        LAYOUT_L_6,
        /// <summary>
        /// 2列2行
        /// </summary>
        LAYOUT_2_2,

        /// <summary>
        /// 3列2行
        /// </summary>
        LAYOUT_3_2,

        /// <summary>
        /// 2列3行
        /// </summary>
        LAYOUT_2_3,

        /// <summary>
        /// 4列2行
        /// </summary>
        LAYOUT_4_2,

        /// <summary>
        /// 3列3行
        /// </summary>
        LAYOUT_3_3,

        /// <summary>
        /// 4列3行
        /// </summary>
        LAYOUT_4_3,

        /// <summary>
        /// 4列4行
        /// </summary>
        LAYOUT_4_4,

        /// <summary>
        /// 5列4行
        /// </summary>
        LAYOUT_5_4,
        
    }

    public enum VideoLayoutScaleMode
    {
        /// <summary>
        /// 正常模式
        /// </summary>
        VideoLayoutScaleMode_Normal,

        /// <summary>
        /// 选中一个的最大模式
        /// </summary>
        VideoLayoutScaleMode_Max
    }
}
