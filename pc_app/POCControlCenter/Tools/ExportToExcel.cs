using System;
using System.Collections;
using System.Text;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace POCControlCenter
{
    public class ExportToExcel
    {
        public Excel.Application m_xlApp = null;

        public void OutputAsExcelFile(DataGridView dataGridView)
        {
            if (dataGridView.Rows.Count <= 0)
            {
                MessageBox.Show(WinFormsStringResource.NoData,WinFormsStringResource.PromptStr,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;

            }
            string filePath = "";
            SaveFileDialog s = new SaveFileDialog();
            s.Title = WinFormsStringResource.SaveExcelFile;
            s.Filter = "Excel file(*.xls)|*.xls";
            s.FilterIndex = 1;
            if (s.ShowDialog() == DialogResult.OK)
                filePath = s.FileName;
            else
                return;

            //第一步：将dataGridView转化为dataTable,这样可以过滤掉dataGridView中的隐藏列

            DataTable tmpDataTable = new DataTable("tmpDataTable");
            DataTable modelTable = new DataTable("ModelTable");
            for (int column = 0; column < dataGridView.Columns.Count; column++)
            {
                if (dataGridView.Columns[column].Visible == true)
                {
                    DataColumn tempColumn = new DataColumn(dataGridView.Columns[column].HeaderText, typeof(string));
                    tmpDataTable.Columns.Add(tempColumn);
                    DataColumn modelColumn = new DataColumn(dataGridView.Columns[column].Name, typeof(string));
                    modelTable.Columns.Add(modelColumn);
                }
            }
            for (int row = 0; row < dataGridView.Rows.Count; row++)
            {
                if (dataGridView.Rows[row].Visible == false)
                    continue;
                DataRow tempRow = tmpDataTable.NewRow();
                for (int i = 0; i < tmpDataTable.Columns.Count; i++)
                    tempRow[i] = dataGridView.Rows[row].Cells[modelTable.Columns[i].ColumnName].Value;
                tmpDataTable.Rows.Add(tempRow);
            }
            if (tmpDataTable == null)
            {
                return;
            }

            //第二步：导出dataTable到Excel
            long rowNum = tmpDataTable.Rows.Count;//行数
            int columnNum = tmpDataTable.Columns.Count;//列数
            Excel.Application m_xlApp = new Excel.Application();
            m_xlApp.DisplayAlerts = false;//不显示更改提示
            m_xlApp.Visible = false;

            Excel.Workbooks workbooks = m_xlApp.Workbooks;
            Excel.Workbook workbook = workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets[1];//取得sheet1

            try
            {
                string[,] datas = new string[rowNum + 1, columnNum];
                for (int i = 0; i < columnNum; i++) //写入字段
                    datas[0, i] = tmpDataTable.Columns[i].Caption;
                //Excel.Range range = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[1, columnNum]);
                Excel.Range range = m_xlApp.Range[worksheet.Cells[1, 1], worksheet.Cells[1, columnNum]];
                range.Interior.ColorIndex = 15;//15代表灰色
                range.Font.Bold = true;
                range.Font.Size = 10;

                int r = 0;
                for (r = 0; r < rowNum; r++)
                {
                    for (int i = 0; i < columnNum; i++)
                    {
                        object obj = tmpDataTable.Rows[r][tmpDataTable.Columns[i].ToString()];
                        datas[r + 1, i] = obj == null ? "" : "'" + obj.ToString().Trim();//在obj.ToString()前加单引号是为了防止自动转化格式
                    }
                    System.Windows.Forms.Application.DoEvents();
                    //添加进度条
                }
                //Excel.Range fchR = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[rowNum + 1, columnNum]);
                Excel.Range fchR = m_xlApp.Range[worksheet.Cells[1, 1], worksheet.Cells[rowNum + 1, columnNum]];
                fchR.Value2 = datas;

                worksheet.Columns.EntireColumn.AutoFit();//列宽自适应。
                                                         //worksheet.Name = "dd";

                //m_xlApp.WindowState = Excel.XlWindowState.xlMaximized;
                m_xlApp.Visible = false;

                // = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[rowNum + 1, columnNum]);
                range = m_xlApp.Range[worksheet.Cells[1, 1], worksheet.Cells[rowNum + 1, columnNum]];

                //range.Interior.ColorIndex = 15;//15代表灰色
                range.Font.Size = 9;
                range.RowHeight = 14.25;
                range.Borders.LineStyle = 1;
                range.HorizontalAlignment = 1;
                workbook.Saved = true;
                workbook.SaveCopyAs(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show( WinFormsStringResource.ExcelExportFail + ex.Message,
                    WinFormsStringResource.ExcelExportFail, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                EndReport();
            }

            m_xlApp.Workbooks.Close();
            m_xlApp.Workbooks.Application.Quit();
            m_xlApp.Application.Quit();
            m_xlApp.Quit();
            MessageBox.Show(WinFormsStringResource.ExcelExportSuccess, WinFormsStringResource.PromptStr,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void EndReport()
        {
            object missing = System.Reflection.Missing.Value;
            try
            {
                //m_xlApp.Workbooks.Close();
                //m_xlApp.Workbooks.Application.Quit();
                //m_xlApp.Application.Quit();
                //m_xlApp.Quit();
            }
            catch { }
            finally
            {
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_xlApp.Workbooks);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_xlApp.Application);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_xlApp);
                    m_xlApp = null;
                }
                catch { }
                try
                {
                    //清理垃圾进程
                    this.killProcessThread();
                }
                catch { }
                GC.Collect();
            }
        }

        private void killProcessThread()
        {
            ArrayList myProcess = new ArrayList();
            for (int i = 0; i < myProcess.Count; i++)
            {
                try
                {
                    System.Diagnostics.Process.GetProcessById(int.Parse((string)myProcess[i])).Kill();
                }
                catch { }
            }
        }
    }
}
