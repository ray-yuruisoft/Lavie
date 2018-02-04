using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lavie.Modules.Project.Extentions.NPOIExtentions;

namespace Lavie.Modules.Project.Models
{
    public class WriteExcelInput : IWriteExcelInput
    {
        public Dictionary<string, ArrayList> Sheets { get; set; }
    }
}
