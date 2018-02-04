using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Models
{
    public class PullInput
    {
        [DisplayName("客户端最大ID")]
        [Range(0, Int32.MaxValue, ErrorMessage = "请输入合法的客户端最大ID")]
        public int? MaxID { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "请输入合法的客户端最小ID")]
        //[Mutex("MaxID",false/*, ErrorMessage = "客户端最大ID和客户端最小ID只能二选一"*/)]
        [DisplayName("客户端最小ID")]
        public int? MinID { get; set; }

        [DisplayName("拉取数量")]
        [Range(1, 1000, ErrorMessage = "拉取数量请保持在1-1000之间")]
        public int PullCount { get; set; }
    }
}
