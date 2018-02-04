using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    //public class AttendanceInfo
    //{
    //    public int? RiderEleID { get; set; }

    //    public string Name { get; set; }

    //    public string TrackingNO { get; set; }

    //    public DateTime? OrderTime { get; set; }
    //}

    public class StaffWorkInfoFromCsv
    {

        public int RiderEleID { get; set; }

        public string Name { get; set; }

        public DateTime RiderPickUpTime { get; set; }

        public int TeamID { get; set; }

        public string TeamName { get; set; }

    }



}
