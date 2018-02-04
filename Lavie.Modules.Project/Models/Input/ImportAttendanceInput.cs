using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Project.Extentions;

namespace Lavie.Modules.Project.Models
{

    #region Obsolescence
    public class ImportAttendanceInputObsolescence
    {
        [Required(ErrorMessage = "请提供城市账单路径")]
        public string CityBillExcelPath { get; set; }

        public int? CityBillExcelSheetIndex { get; set; }

        string CityBillColNameRiderEleID { get; set; }

        string CityBillColNameName { get; set; }

        string CityBillColNameTrackingNO { get; set; }

        [Required(ErrorMessage = "请提供运单数据路径")]
        public string TrackingBillCsvPath { get; set; }

        public string TrackingBillColNameTrackingNO { get; set; }

        public string TrackingBillColNameOrderTime { get; set; }

    }
    #endregion

    public class ImportAttendanceInput
    {

        [Required(ErrorMessage = "请提供配送费账单路径")]
        public string DeliveryFeeBillCsvPath { get; set; }

        [GuidIsEmpty(ErrorMessage = "CityName值为空")]
        public Guid CityName { get; set; }

        public string RiderEleIDColName { get; set; }

        public string NameColName { get; set; }

        public string RiderPickUpTimeColName { get; set; }

        public string TeamIDColName { get; set; }

        public string TeamNameColName { get; set; }


    }


}
