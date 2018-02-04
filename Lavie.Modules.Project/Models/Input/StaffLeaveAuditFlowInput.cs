using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Project.Extentions;

namespace Lavie.Modules.Project.Models
{
    public class StaffLeaveAuditFlowInput
    {

        [GuidIsEmpty(ErrorMessage = "StaffLeaveAuditFlowID值为空")]
        public Guid StaffLeaveAuditFlowID { get; set; }

        [GuidIsEmpty(ErrorMessage = "RequestRoleID值为空")]
        public Guid RequestRoleID { get; set; }

        [GuidIsEmpty(ErrorMessage = "AuditGroupID1值为空")]
        [Required(ErrorMessage = "AuditGroupID1值为空")]
        public Guid? AuditGroupID1 { get; set; }
        public Guid? AuditGroupID2 { get; set; }
        public Guid? AuditGroupID3 { get; set; }
        public Guid? AuditGroupID4 { get; set; }

        public Guid? AuditRoleID1 { get; set; }
        public Guid? AuditRoleID2 { get; set; }
        public Guid? AuditRoleID3 { get; set; }
        public Guid? AuditRoleID4 { get; set; }

        public Int32? AuditStaffLevelMaxDays1 { get; set; }
        public Int32? AuditStaffLevelMaxDays2 { get; set; }
        public Int32? AuditStaffLevelMaxDays3 { get; set; }
        public Int32? AuditStaffLevelMaxDays4 { get; set; }

    }


}
