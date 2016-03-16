using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace ZoDream.Number.Helper.Export
{
    public class Excel
    {
        //先上导出代码
        /// <summary>
        /// 导出速度最快
        /// </summary>
        /// <param name="list">Key, Value</param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public bool Export(List<DictionaryEntry> list, string filepath)
        {
            var bSuccess = true;
            var miss = System.Reflection.Missing.Value;
            var appexcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbookdata = null;

            workbookdata = appexcel.Workbooks.Add();

            //设置对象不可见
            appexcel.Visible = false;
            appexcel.DisplayAlerts = false;
            try
            {
                foreach (var lv in list)
                {
                    var keys = lv.Key as List<string>;
                    var values = lv.Value as List<IList<object>>;
                    var worksheetdata = (Worksheet)workbookdata.Worksheets.Add(miss, workbookdata.ActiveSheet);

                    Debug.Assert(keys != null, "keys != null");
                    for (var i = 0; i < keys.Count - 1; i++)
                    {
                        //给工作表赋名称
                        worksheetdata.Name = keys[0];//列名的第一个数据位表名
                        worksheetdata.Cells[1, i + 1] = keys[i + 1];
                    }

                    //因为第一行已经写了表头，所以所有数据都应该从a2开始

                    //irowcount为实际行数，最大行
                    if (values == null) continue;
                    {
                        var irowcount = values.Count;
                        int iparstedrow = 0, icurrsize = 0;

                        //ieachsize为每次写行的数值，可以自己设置
                        const int ieachsize = 10000;

                        //icolumnaccount为实际列数，最大列数
                        var icolumnaccount = keys.Count - 1;

                        //在内存中声明一个ieachsize×icolumnaccount的数组，ieachsize是每次最大存储的行数，icolumnaccount就是存储的实际列数
                        var objval = new object[ieachsize, icolumnaccount];
                        icurrsize = ieachsize;

                        while (iparstedrow < irowcount)
                        {
                            if ((irowcount - iparstedrow) < ieachsize)
                                icurrsize = irowcount - iparstedrow;

                            //用for循环给数组赋值
                            for (var i = 0; i < icurrsize; i++)
                            {
                                for (var j = 0; j < icolumnaccount; j++)
                                {
                                    var v = values[i + iparstedrow][j];
                                    objval[i, j] = v?.ToString() ?? "";
                                }
                            }
                            var x = "A" + ((int)(iparstedrow + 2)).ToString();
                            var col = "";
                            if (icolumnaccount <= 26)
                            {
                                col = ((char)('A' + icolumnaccount - 1)).ToString() + ((int)(iparstedrow + icurrsize + 1)).ToString();
                            }
                            else
                            {
                                col = ((char)('A' + (icolumnaccount / 26 - 1))).ToString() + ((char)('A' + (icolumnaccount % 26 - 1))).ToString() + ((int)(iparstedrow + icurrsize + 1)).ToString();
                            }
                            var xlrang = worksheetdata.Range[x, col];
                            xlrang.NumberFormat = "@";
                            // 调用range的value2属性，把内存中的值赋给excel
                            xlrang.Value2 = objval;
                            iparstedrow = iparstedrow + icurrsize;
                        }
                    }
                }
                ((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet1"]).Delete();
                ((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet2"]).Delete();
                ((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet3"]).Delete();
                //保存工作表
                workbookdata.SaveAs(filepath, miss, miss, miss, miss, miss, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, miss, miss, miss);
                workbookdata.Close(false, miss, miss);
                appexcel.Workbooks.Close();
                appexcel.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbookdata);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appexcel.Workbooks);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appexcel);
                GC.Collect();
            }
            catch (Exception)
            {
                //ErrorMsg = ex.Message;
                bSuccess = false;
            }
            finally
            {
                //ExcelImportHelper.KillSpecialExcel(appexcel);
            }
            return bSuccess;
        }

        /*
range.NumberFormatLocal = "@";     //设置单元格格式为文本   
  
range = (Range)worksheet.get_Range("A1", "E1");     //获取Excel多个单元格区域：本例做为Excel表头   
  
range.Merge(0);     //单元格合并动作   
  
worksheet.Cells[1, 1] = "Excel单元格赋值";     //Excel单元格赋值   
  
range.Font.Size = 15;     //设置字体大小   
  
range.Font.Underline=true;     //设置字体是否有下划线   
  
range.Font.Name="黑体";       设置字体的种类   
  
range.HorizontalAlignment=XlHAlign.xlHAlignCenter;     //设置字体在单元格内的对其方式   
  
range.ColumnWidth=15;     //设置单元格的宽度   
  
range.Cells.Interior.Color=System.Drawing.Color.FromArgb(255,204,153).ToArgb();     //设置单元格的背景色   
  
range.Borders.LineStyle=1;     //设置单元格边框的粗细   
  
range.BorderAround(XlLineStyle.xlContinuous,XlBorderWeight.xlThick,XlColorIndex.xlColorIndexAutomatic,System.Drawing.Color.Black.ToArgb());     //给单元格加边框   
  
range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone; //设置单元格上边框为无边框   
  
range.EntireColumn.AutoFit();     //自动调整列宽   
  
Range.HorizontalAlignment= xlCenter;     // 文本水平居中方式   
  
Range.VerticalAlignment= xlCenter     //文本垂直居中方式   
  
Range.WrapText=true;     //文本自动换行   
  
Range.Interior.ColorIndex=39;     //填充颜色为淡紫色   
  
Range.Font.Color=clBlue;     //字体颜色   
  
xlsApp.DisplayAlerts=false;   //对Excel的操作 不弹出提示信息
ApplicationClass xlsApp = new ApplicationClass(); // 1. 创建Excel应用程序对象的一个实例，相当于我们从开始菜单打开Excel应用程序。
if (xlsApp == null)
{
//对此实例进行验证，如果为null则表示运行此代码的机器可能未安装Excel
}

1. 打开现有的Excel文件

Workbook workbook = xlsApp.Workbooks.Open(excelFilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
Worksheet mySheet = workbook.Sheets[1] as Worksheet; //第一个sheet页
mySheet.Name = "testsheet"; //这里修改sheet名称


2.复制sheet页

mySheet.Copy(Type.Missing, workbook.Sheets[1]); //复制mySheet成一个新的sheet页，复制完后的名称是mySheet页名称后加一个(2)，这里就是testsheet(2)，复制完后，Worksheet的数量增加一个


注意 这里Copy方法的两个参数，指是的复制出来新的sheet页是在指定sheet页的前面还是后面，上面的例子就是指复制的sheet页在第一个sheet页的后面。
3.删除sheet页
    
xlsApp.DisplayAlerts = false; //如果想删除某个sheet页，首先要将此项设为fasle。
(xlsApp.ActiveWorkbook.Sheets[1] as Worksheet).Delete();


4.选中sheet页
    
(xlsApp.ActiveWorkbook.Sheets[1] as Worksheet).Select(Type.Missing); //选中某个sheet页


5.另存excel文件

workbook.Saved = true;
workbook.SaveCopyAs(filepath);


6.释放excel资源
    
workbook.Close(true, Type.Missing, Type.Missing);
workbook = null;
xlsApp.Quit();
xlsApp = null;

        */
    }
}
