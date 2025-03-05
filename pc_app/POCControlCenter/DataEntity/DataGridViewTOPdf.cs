using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace POCControlCenter.DataEntity
{
    public static class DataGridViewTOPdf
    {
        /// 
        /// 转换GridView为PDF文档    /// 
        /// GridView
        /// 目标PDF文件名字
        /// 字体所在路径
        /// 字体大小
        /// 返回调用是否成功
        public static void ExportTOPdf(DataGridView datagridview)
        {

            ///设置导出字体
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string path =Environment.CurrentDirectory;
            string FontPath = path + "\\simsun.ttc";
            int FontSize = 12;
            if (File.Exists(FontPath))
            {
                FontPath += ",1";
            }
            else
            {
                MessageBox.Show(WinFormsStringResource.PDFExportFail);
                return;
            }


            Boolean cc = false;
            string strFileName;
            SaveFileDialog savFile = new SaveFileDialog();
            savFile.Filter = "PDF file|*.pdf";
            savFile.ShowDialog();
            if (savFile.FileName != "")
            {
                strFileName = savFile.FileName;
            }
            else
            {
                //MessageBox.Show("终止导出", "终止导出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //初始化一个目标文档类        
            //Document document = new Document();
            //竖排模式,大小为A4，四周边距均为25
            //Document document = new Document(PageSize.A4, 25, 25, 25, 25);
            //横排模式,大小为A4，四周边距均为25
            //Document document = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25);
            //2017.8.28 改为A4竖向
            Document document = new Document(PageSize.A4, 25, 25, 110, 25);

            //调用PDF的写入方法流
            //注意FileMode-Create表示如果目标文件不存在，则创建，如果已存在，则覆盖。
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(strFileName, FileMode.Create));

            //创建PDF文档中的字体
            BaseFont baseFont = BaseFont.CreateFont(
                FontPath,
                BaseFont.IDENTITY_H,
                BaseFont.NOT_EMBEDDED);

            //根据字体路径和字体大小属性创建字体
            Font font = new Font(baseFont, FontSize);

            // 添加页脚
            // HeaderFooter footer = new HeaderFooter(new Phrase("-- ", font), new Phrase(" --", font));
            //footer.Border = Rectangle.NO_BORDER;        // 不显示两条横线
            //footer.Alignment = Rectangle.UNDEFINED;  // 让页码居中
            //document.Footer = footer;

            //打开目标文档对象
            document.Open();

            //2017.8.26 插入客户的logo
            iTextSharp.text.Image
                hgLogo = iTextSharp.text.Image.GetInstance(Environment.CurrentDirectory+ "\\pdffile_logo.png");
            
            hgLogo.ScalePercent(5f);  //图片比例
            hgLogo.SetAbsolutePosition(25f, PageSize.A4.Height-25f- (hgLogo.Height*0.05f)); //iamge 位置 
            document.Add(hgLogo);

            //加入标题
            Font bold_underlined = new Font(Font.FontFamily.TIMES_ROMAN, 16, Font.BOLD | Font.UNDERLINE);

            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.SpacingAfter = 20f;

            Chunk chunk1 = new Chunk("Fence Alarm List", bold_underlined);

            paragraph.Add(chunk1);
            paragraph.Add(Chunk.NEWLINE);
            document.Add(paragraph);
            //

            int ColCount = 0;

            //根据数据表内容创建一个PDF格式的表
            for (int j = 0; j < datagridview.Columns.Count; j++)
            {
                if (datagridview.Columns[j].Visible == true)
                {
                    ColCount++;
                }
            }
            PdfPTable table = new PdfPTable(ColCount);
            table.TotalWidth = document.PageSize.Width;
            table.SetWidths(new int[] { 1,1,1,1,1,1,1});    // 设置各列宽度，单位是百分比

            // GridView的所有数据全输出
            //datagridview.AllowPaging = false;

            // ---------------------------------------------------------------------------
            // 添加表头
            // ---------------------------------------------------------------------------
            // 设置表头背景色 
            //table.DefaultCell.BackgroundColor = Color.GRAY;  // OK
            //table.DefaultCell.BackgroundColor = (iTextSharp.text.Color)System.Drawing.Color.FromName("#3399FF"); // NG
            //table.DefaultCell.BackgroundColor = iTextSharp.text.Color;

            table.DefaultCell.BackgroundColor = BaseColor.ORANGE;
            // 添加表头，每一页都有表头
            for (int j = 0; j < datagridview.Columns.Count; j++)
            {
                if (datagridview.Columns[j].Visible == true)
                {
                    table.AddCell(new Phrase(datagridview.Columns[j].HeaderText, font));
                }
            }

            // 告诉程序这行是表头，这样页数大于1时程序会自动为你加上表头。
            table.HeaderRows = 1;
            //
            // ---------------------------------------------------------------------------
            // 添加数据
            // ---------------------------------------------------------------------------
            // 设置表体背景色
            table.DefaultCell.BackgroundColor = BaseColor.WHITE;
            //遍历原gridview的数据行
            //写内容   
            for (int j = 0; j < datagridview.Rows.Count; j++)
            {
                for (int k = 0; k < datagridview.Columns.Count; k++)
                {
                    if (datagridview.Rows[j].Cells[k].Visible == true)
                    {
                        try
                        {
                            string value = "";
                            if (datagridview.Rows[j].Cells[k].Value != null)
                            {
                                value = datagridview.Rows[j].Cells[k].Value.ToString();

                            }
                            table.AddCell(new Phrase(value, font));
                        }
                        catch (Exception e)
                        {

                            //MessageBox.Show(e.Message);
                            cc = true;
                        }

                    }
                }

            }


            //在目标文档中添加转化后的表数据
            document.Add(table);

            //关闭目标文件
            document.Close();

            //关闭写入流
            writer.Close();

            // Dialog
            if (!cc)
            {
                MessageBox.Show(WinFormsStringResource.PDFExportSuccess,WinFormsStringResource.PromptStr ,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

