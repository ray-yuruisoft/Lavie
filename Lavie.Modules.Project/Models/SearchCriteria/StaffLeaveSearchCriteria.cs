using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class StaffLeaveSearchCriteria
    {

        public Guid? GroupID { get; set; }

        [StringLength(20)]
        public string Keyword { get; set; }

        public int? StaffID { get; set; }


    }
}
