using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Admin.Models.InputModels
{
    public class ParentIDInput
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "请输入ParentID")]
        public int ParentID { get; set; }
    }
}
