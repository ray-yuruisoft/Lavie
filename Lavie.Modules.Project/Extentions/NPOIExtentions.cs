using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lavie.Infrastructure.FastReflectionLib;

namespace Lavie.Modules.Project.Extentions
{
    public class NPOIExtentions
    {
        public static XSSFWorkbook WriteExcel2007<T>(T Input) where T : IWriteExcelInput
        {

            NPOI.XSSF.UserModel.XSSFWorkbook book = new NPOI.XSSF.UserModel.XSSFWorkbook();
            foreach (var top in Input.Sheets)
            {

                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet(top.Key);
                NPOI.SS.UserModel.IRow rowT1 = sheet.CreateRow(0);
                NPOI.SS.UserModel.IRow rowT2 = sheet.CreateRow(1);

                #region style

                ICellStyle style = book.CreateCellStyle();
                //设置单元格的样式：水平对齐居中
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                //新建一个字体样式对象
                IFont font = book.CreateFont();
                //设置字体加粗样式
                font.Boldweight = short.MaxValue;
                //使用SetFont方法将字体样式添加到单元格样式中 
                style.SetFont(font);

                #endregion

                Type type = top.Value[0].GetType();
                PropertyInfo[] properties = type.GetProperties();

                string[] LastValue = { "", "" };
                int step = 0;
                int begin = 0;
                for (var i = 0; i < properties.Length; i++)
                {

                    var value = properties[i].GetCustomAttribute<DisplayNameAttribute>().DisplayName.Split(',');
                    if (step == 0 && i != 0) begin = i - 1;

                    rowT1.CreateCell(i).SetCellValue(value[0]);
                    rowT2.CreateCell(i).SetCellValue(value.Length > 1 ? value[1] : "");

                    #region style

                    rowT1.GetCell(i).CellStyle = style;
                    rowT2.GetCell(i).CellStyle = style;
                    sheet.SetColumnWidth(i, (Encoding.Default.GetBytes(value.Length > 1 ? value[1] : value[0]).Length + 1) * 256);

                    #endregion

                    if (LastValue[0] == value[0]) step++;
                    else if (step != 0)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, begin, begin + step));
                        step = 0;
                    }
                    LastValue[0] = value[0];
                    LastValue[1] = value.Length > 1 ? value[1] : "";
                    if (value.Length == 1)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(0, 1, i, i));
                    }

                }

                var k = 2;
                foreach (var item in top.Value)
                {
                    NPOI.SS.UserModel.IRow row2 = sheet.CreateRow(k);
                    var j = 0;
                    foreach (var bottom in properties)
                    {

                        var accessor = FastReflectionCaches.PropertyAccessorCache.Get(bottom);
                        object value = accessor.GetValue(item);

                        dynamic temp;
                        if (bottom.PropertyType.Name == "Int32")
                        {
                            temp = Int32.Parse(value.ToString());
                        }
                        else if (bottom.PropertyType.Name == "DateTime")
                        {
                            temp = DateTime.Parse(value.ToString());
                        }
                        else if (bottom.PropertyType.Name == "Double")
                        {
                            temp = Double.Parse(value.ToString());
                        }
                        else if (bottom.PropertyType.Name == "Boolean")
                        {
                            temp = Boolean.Parse(value.ToString());
                        }
                        else if (bottom.Name == "StaffID")
                        {
                            if (value != null)
                            {
                                temp = Int32.Parse(value.ToString());
                            }
                            else
                            {
                                temp = "";
                            }
                        }
                        else if (bottom.Name == "LastRunningTime")
                        {
                            if (value != null)
                            {
                                temp = DateTime.Parse(value.ToString());
                            }
                            else
                            {
                                temp = "";
                            }
                        }
                        else
                        {
                            temp = value == null ? "" : value.ToString();
                        }


                        row2.CreateCell(j).SetCellValue(temp);

                        if (temp is DateTime)
                        {
                            ICellStyle cellStyle = book.CreateCellStyle();
                            IDataFormat dataFormat = book.CreateDataFormat();
                            cellStyle.DataFormat = dataFormat.GetFormat("yyyy/MM/dd HH:mm:ss");
                            row2.GetCell(j).CellStyle = cellStyle;
                        }
                      
                        j++;
                    }
                    k++;
                }

            }
            return book;

        }
        public interface IWriteExcelInput
        {

            Dictionary<string, ArrayList> Sheets { get; set; }

        }
    }
}
