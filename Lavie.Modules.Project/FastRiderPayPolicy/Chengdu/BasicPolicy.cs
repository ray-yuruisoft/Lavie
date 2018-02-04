using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Project.Extentions;

namespace Lavie.Modules.Project.FastRiderPayPolicy.Chengdu
{

    //基础配送费
    public class BasicDeliveryFee : IPolicy
    {
        //实际正常单
        public void Ready(ComputeData data)
        {

            data.ActualNormalBill = data.TotalAmount - data.VoidedOrders - data.TimeoutOrders;
            //- (int)Math.Floor(data.NoreasonAbsence * 20);
            //todo:无故缺岗，需结合请假流程来计算
        }
        //单量提成
        public void Step1(ComputeData data)
        {

            if (data.ActualNormalBill >= 450)
            {
                data.Commissions = (data.ActualNormalBill - 450) * 6.5;
            }
            else
            {
                data.Commissions = data.ActualNormalBill * 5;
            }

        }
        //超时提成
        public void Step2(ComputeData data)
        {
            data.TimeoutCommissions = data.TimeoutOrders * 3;
        }
        //基础费用 话补	餐补 车贴
        public void Step3(ComputeData data)
        {

            if (data.ActualNormalBill >= 450)
            {
                data.BasicFee = 1500;
                data.TelephoneSubsidy = 200;
                data.DineSubsidy = 300;
                data.CarSubsidy = 150;
            }
            else
            {
                data.BasicFee = 0;
                data.TelephoneSubsidy = 0;
                data.DineSubsidy = 0;
                data.CarSubsidy = 0;
            }

        }
        //全勤奖励
        public void Step4(ComputeData data)
        {
            if (data.ActualNormalBill >= 450 && data.AttendanceDaysDesigned <= data.AttendanceDaysActual)
            {
                data.PerfectAttendance = 300;
            }
            else
            {
                data.PerfectAttendance = 0;
            }
        }
        //无故缺岗
        public void Step5(ComputeData data)
        {
            data.Amount = data.NoreasonAbsence * (-20);
        }

        public void Compute(ComputeData data)
        {
            Ready(data);
            Step1(data);
            Step2(data);
            Step3(data);
            Step4(data);
            Step5(data);
        }

    }

    //工龄
    public class SeniorityFee : IPolicy
    {
        public void Compute(ComputeData data)
        {
            throw new NotImplementedException();
        }
    }

    //扣款
    public class CutPayment : IPolicy
    {
        public void Compute(ComputeData data)
        {
            throw new NotImplementedException();
        }

    }

    //早晚班补贴 TODO：晚班，深夜班
    public class EarliesAndLatesSubsidy : IPolicy
    {
        public void Compute(ComputeData data)
        {
            Step1(data);
        }

        //早班补贴1.5元/单
        public void Step1(ComputeData data)
        {
            data.EarliesSubsidy = data.Earlies * 1.5;
        }

    }
    //评价政策
    public class Evaluate : IPolicy
    {

        public void Compute(ComputeData data)
        {
            Step1(data);
        }

        //差评扣 5元/单
        public void Step1(ComputeData data)
        {
            data.StarAwards = data.BadEvaluate * (-5);
        }

    }
    //转介绍政策
    public class Referral : IPolicy
    {
        public void Compute(ComputeData data)
        {
            Step1(data);
        }

        public void Step1(ComputeData data)
        {
            if (data.IsReferrered)
            {
                data.ReferralAwards = 200;
                data.ReferralAwards += data.ReferreringCount * 600;
            }
            else
            {
                data.ReferralAwards = data.ReferreringCount * 600;
            }
        }

    }

}
