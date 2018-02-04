using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class GetStaffInfoBaseListPar
    {
        [StringLength(20)]
        public string Keyword { get; set; }

        public List<Int32> StaffStatusIDs { get; set; }

        public List<Guid> GroupIDs { get; set; }

        public List<Guid> RoleIDs { get; set; }

        public bool? IsInBlackList { get; set; }

        public bool? IsViaReferral { get; set; }

    }
}
