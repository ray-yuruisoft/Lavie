using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class Station
    {
        public int StationID { get; set; } // StationID (Primary key)
        public int? ParentID { get; set; } // ParentID
        public string Name { get; set; } // Name (length: 50)
        public int Level { get; set; } // Level
        public int DisplayOrder { get; set; } // DisplayOrder
    }
}
