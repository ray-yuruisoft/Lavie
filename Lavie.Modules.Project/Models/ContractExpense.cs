using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public class ContractExpense
    {


        [DisplayName("序号")]
        public int Index { get; set; }

        [DisplayName("饿了么站点ID")]
        public int TeamID { get; set; }

        [DisplayName("站点")]
        public string StationName { get; set; }

        [DisplayName("骑手饿了么ID")]
        public int RidderEleID { get; set; }

        [DisplayName("在职状态")]
        public string RidderStatus { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("应出勤")]
        public double AttendanceDaysDesigned { get; set; }

        [DisplayName("实际出勤")]
        public double AttendanceDaysActual { get; set; }

        [DisplayName("总单量")]
        public int TotalAmount { get; set; }


        [DisplayName("无故缺岗,天数")]
        public double Days { get; set; }

        [DisplayName("无故缺岗,单量")]
        public double Amount { get; set; }


        [DisplayName("单量提成部分（含450单任务单）,实际正常单")]
        public int ActualNormalBill { get; set; }

        [DisplayName("单量提成部分（含450单任务单）,单量提成")]
        public double Commissions { get; set; }

        [DisplayName("单量提成部分（含450单任务单）,超时订单")]
        public int TimeoutOrders { get; set; }

        [DisplayName("单量提成部分（含450单任务单）,超时提成")]
        public double TimeoutCommissions { get; set; }


        [DisplayName("无效订单")]
        public int VoidedOrders { get; set; }


        [DisplayName("综合合作费用,基础费")]
        public double BasicFee { get; set; }

        [DisplayName("综合合作费用,话补")]
        public double TelephoneSubsidy { get; set; }

        [DisplayName("综合合作费用,餐补")]
        public double DineSubsidy { get; set; }

        [DisplayName("综合合作费用,车贴")]
        public double CarSubsidy { get; set; }

        [DisplayName("综合合作费用,全勤奖励")]
        public double PerfectAttendance { get; set; }


        [DisplayName("各类奖励,星级奖励")]
        public double StarAwards { get; set; }

        [DisplayName("各类奖励,转介绍奖励")]
        public double ReferralAwards { get; set; }


        [DisplayName("早班补贴")]
        public double EarliesSubsidy { get; set; }

        [DisplayName("晚班补贴")]
        public double LatesSubsidy { get; set; }


        [DisplayName("恶单人效奖励")]
        public double HeavyWeatherSubsidy { get; set; }

        [DisplayName("其他")]
        public double ElseSubsidy { get; set; }

        [DisplayName("合计")]
        public double Total { get; set; }


        [DisplayName("扣款项,物资押金")]
        public double MaterialDeposit { get; set; }

        [DisplayName("扣款项,站点")]
        public double StationCutPayment { get; set; }

        [DisplayName("扣款项,违规操作")]
        public double ViolationCutPayment { get; set; }

        [DisplayName("扣款项,上月索赔补扣")]
        public double LastMonthClaimsCutPayment { get; set; }

        [DisplayName("扣款项,物资清算")]
        public double MaterialLiquidation { get; set; }

        [DisplayName("扣款项,装备扣款")]
        public double EquipmentCutPayment { get; set; }

        [DisplayName("扣款项,住宿")]
        public double LodgingCutPayment { get; set; }

        [DisplayName("扣款项,电瓶车")]
        public double BatteryBikeCutPayment { get; set; }

        [DisplayName("扣款项,保险")]
        public double Insurance { get; set; }

        [DisplayName("扣款项,其他")]
        public double ElseCutPayment { get; set; }

        [DisplayName("扣款合计")]
        public double TotalCutPayment { get; set; }

        [DisplayName("协议费用实发金额")]
        public double ContractExpenseActualIssuance { get; set; }

        [DisplayName("最后跑单时间")]
        public DateTime? LastRunningTime { get; set; }

        [DisplayName("备注")]
        public string Remark { get; set; }

    }
}
