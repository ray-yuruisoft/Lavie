using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class StaffLeaveAuditFlowSiteConfigInput
    {
        public Guid GroupID { get; set; }
        public int? AuditStaffID1 { get; set; }
        public int? AuditStaffID2 { get; set; }
        public int? AuditStaffID3 { get; set; }
        public int? AuditStaffID4 { get; set; }
        public string AuditStaffLevelMaxDays1 { get; set; }
        public string AuditStaffLevelMaxDays2 { get; set; }
        public string AuditStaffLevelMaxDays3 { get; set; }
        public string AuditStaffLevelMaxDays4 { get; set; }
    }
}
