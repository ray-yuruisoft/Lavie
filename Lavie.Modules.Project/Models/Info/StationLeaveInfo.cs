using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class StationLeaveList
    {
        public List<StationLeaveInfo> StationLeaveInfos { get; set; }
    }

    public class StationLeaveInfo
    {
        public Guid? GroupID { get; set; }
        public string Name { get; set; }
        public IEnumerable<TargetStaff> TargetStaffs { get; set; }
    }

    public class TargetStaff
    {
        public int StaffID { get; set; }
        public string Name { get; set; }
        public List<StaffLeaveBase> StaffLeaveBases { get; set; }
    }


}
