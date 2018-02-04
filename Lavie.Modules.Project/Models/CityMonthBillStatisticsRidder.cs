using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class CityMonthBillStatisticsRidder
    {

        public string Name { get; set; }

        public int Month { get; set; }

        public int TotalAmount { get; set; }

        public int TimeoutOrders { get; set; }

        public int VoidedOrders { get; set; }

        public int Earlies { get; set; }

        public Dictionary<int, AsDays> StatisticAsDays { get; set; }

    }

    public class AsDays
    {
        public int TotalAmount { get; set; }
        public int TimeoutOrders { get; set; }
        public int VoidedOrders { get; set; }
    }
   
}
