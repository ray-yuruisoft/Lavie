using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.FastRiderPayPolicy
{
    public class ComputeData
    {

        //应出勤
        public double AttendanceDaysDesigned { get; set; }

        //实际出勤
        public double AttendanceDaysActual { get; set; }


        //实际正常单
        public int ActualNormalBill { get; set; }

        //总单
        public int TotalAmount { get; set; }
        //无效单
        public int VoidedOrders { get; set; }
        //超时单
        public int TimeoutOrders { get; set; }

        //无故缺岗天数
        public double NoreasonAbsence { get; set; }
        //无故缺岗单量
        public double Amount { get; set; }

        //早班数量
        public int Earlies { get; set; }
        //早班补贴
        public double EarliesSubsidy { get; set; }

        //单量提成
        public double Commissions { get; set; }

        //超时提成
        public double TimeoutCommissions { get; set; }

        //基础费用
        public double BasicFee { get; set; }
        //话补
        public double TelephoneSubsidy { get; set; }
        //餐贴
        public double DineSubsidy { get; set; }
        //车补
        public double CarSubsidy { get; set; }
        //全勤奖励
        public double PerfectAttendance { get; set; }

        //好评数
        public double GoodEvaluate { get; set; }
        //差评数
        public double BadEvaluate { get; set; }

        //星级奖励
        public double StarAwards { get; set; }

        //是否被转介绍
        public bool IsReferrered { get; set; }
        //介绍的人数
        public int ReferreringCount { get; set; }
        //转介绍奖励
        public double ReferralAwards { get; set; }
    }
}
