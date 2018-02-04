using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Admin.Models;



namespace Lavie.Modules.Project.Models
{
    public class StaffInfoBaseList : StaffBase
    {
        public Int32 StaffID { get; set; }
        new public string Name { get; set; }
        public GroupInfo Groups { get; set; }
        public RoleBase Role { get; set; }

        public StaffStatusInfoBase StaffStatus { get; set; }

    }
}
