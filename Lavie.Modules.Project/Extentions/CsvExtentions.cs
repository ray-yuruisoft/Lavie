using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Extentions
{
    public class CsvExtentions
    {

        public static Dictionary<string, Tuple<int, int>> GetTitleCoords(CsvReader csv, List<string> ColNames)
        {

            Dictionary<string, Tuple<int, int>> results = new Dictionary<string, Tuple<int, int>>();
            int row = 0;
            while (csv.ReadNextRecord())
            {
                for (var i = 0; i < csv.FieldCount; i++)
                {
                    var temp = csv[i].ToString()
                                     .Replace("=", "")
                                     .Replace("\"", "")
                                     ;
                    string ColName = null;
                    foreach (var item in ColNames)
                    {
                        if (temp == item)
                        {
                            results.Add(item, new Tuple<int, int>(row, i));
                            ColName = item;
                            break;
                        }
                    }
                    ColNames.Remove(ColName);
                    if (ColNames.Count == 0) goto Re;

                }
                row++;
            }
            Re: return results;

        }

    }
}
