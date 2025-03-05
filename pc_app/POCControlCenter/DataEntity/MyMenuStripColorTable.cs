using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    //参考以下的文章
    //http://www.jb51.net/article/70602.htm

    public class MyMenuStripColorTable : System.Windows.Forms.ProfessionalColorTable
    {
        public override System.Drawing.Color MenuItemSelected
        {
            //#16a37b
            get {

                //return Color.FromArgb(22,163,123);
                //return Color.FromArgb(3, 117, 232);
                return Color.FromArgb(0, 202, 202);

            }            

        }

        
        //ToolStripPanelGradientBegin
        public override System.Drawing.Color ToolStripPanelGradientBegin
        {
            //#293846
            get { return Color.FromArgb(255, 135, 15); }

        }

        public override System.Drawing.Color ToolStripPanelGradientEnd
        {
            //#293846
            get { return Color.FromArgb(255, 135, 15); }

        }

        /// <summary>
        /// 主菜单项被点击后，展开的下拉菜单面板的边框
        /// </summary>
        
        //public override Color MenuBorder
        //{
        //    get
        //    {
        //        return Color.FromArgb(37, 37, 37);
        //    }
        //}

        /// <summary>
        /// 鼠标移动到菜单项（主菜单及下拉菜单）时，下拉菜单项的边框
        /// </summary>
        public override Color MenuItemBorder
        {
            get
            {
                return Color.Transparent;
            }
        }

        //下拉菜单面板背景一共分为2个部分，左边为图像区域，右侧为文本区域，需要分别设置
        //ToolStripDropDownBackground设置文本部分的背景色
        public override System.Drawing.Color ToolStripDropDownBackground
        {
            //#293846
            get { return Color.FromArgb(206, 239, 255); }

        }

        //以ImageMarginGradient开头的3个设置的是图像部分的背景色，begin->end是从左到右的顺序
        public override Color ImageMarginGradientBegin
        {
            get
            {
                //#293846
                //return Color.FromArgb(3, 117, 232);
                return Color.FromArgb(0, 64, 64);
            }
        }
        public override Color ImageMarginGradientMiddle
        {
            get
            {
                //#293846
                return Color.FromArgb(0, 117, 117);
            }
        }
        public override Color ImageMarginGradientEnd
        {
            get
            {
                //#293846
                //return Color.FromArgb(41, 56, 70);
                return Color.FromArgb(0, 202, 202);
            }
        }

    }
}
