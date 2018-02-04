
using LumenWorks.Framework.IO.Csv;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Infrastructure.FastReflectionLib;
using Lavie.Modules.Admin.Repositories;
using Lavie.Modules.Admin.Services;
using Lavie.Modules.Project.Models;
using Lavie.Modules.Project.Extentions;
using System.Collections;
using Lavie.Modules.Project.FastRiderPayPolicy;

namespace Lavie.Modules.Project.Repositories
{

    public partial interface IReportRepository
    {
        //Task<bool> ImportAttendanceObsolescence(ImportAttendanceInputObsolescence input, ModelStateDictionary modelState, NpoiMemoryStream OutputStream);
        Task<Tuple<bool, byte[]>> ImportAttendance(ImportAttendanceInput input, ModelStateDictionary modelState);
        Task<Tuple<bool, byte[]>> Calculate(CalculateInput input, ModelStateDictionary modelState);
    }
    public partial class ReportRepository : RepositoryBase, IReportRepository
    {
        private readonly IGroupService _groupService;
        public ReportRepository(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<Tuple<bool, byte[]>> ImportAttendance(ImportAttendanceInput input, ModelStateDictionary modelState)
        {

            HashSet<ExceptionReportInfo> exceptionReports = new HashSet<ExceptionReportInfo>();
            var reFromFeeBillCsv = await ImportDeliveryFeeBillCsv(input.DeliveryFeeBillCsvPath, input.CityName, exceptionReports, modelState);
            if (reFromFeeBillCsv == null) return new Tuple<bool, byte[]>(false, null);

            #region StaffWorkUpdate

            int count = 1;
            string[] lastTemp = { "", "", "" };
            string v = null;
            foreach (var item in reFromFeeBillCsv.Item1)
            {

                string[] temp = item.Key.Split(',');
                if (lastTemp[0] == "")
                {
                    lastTemp[0] = temp[0];
                    lastTemp[1] = temp[1];
                    lastTemp[2] = temp[2];
                }

                #region methord 3 3s

                v += temp[0] + ",";
                foreach (var bottom in item.Value)
                {
                    v += bottom.Key.Substring(3) + ",";
                }
                v += ";";

                if (count == 100 || lastTemp[1] != temp[1] || lastTemp[2] != temp[2])
                {
                    count = 0;

                    await DbContext.Database.ExecuteSqlCommandAsync("EXEC sp_updatestaffworks @Year ,@Month ,@v",
                        new SqlParameter("Year", temp[1]),
                        new SqlParameter("Month", temp[2]),
                        new SqlParameter("v", v));

                    v = null;
                }
                count++;
                lastTemp[0] = temp[0];
                lastTemp[1] = temp[1];
                lastTemp[2] = temp[2];

                #endregion

            }
            if (!string.IsNullOrEmpty(v))
            {
                await DbContext.Database.ExecuteSqlCommandAsync("EXEC sp_updatestaffworks @Year ,@Month ,@v",
                             new SqlParameter("Year", lastTemp[1]),
                             new SqlParameter("Month", lastTemp[2]),
                             new SqlParameter("v", v));
            }

            #endregion

            #region RidderLastWorkDateUpdate

            var staffsFromFeeBillCsv = reFromFeeBillCsv.Item2.Select(c => c.Key).ToList();
            var staffsNeedToSave = await DbContext.Staffs.Where(c => staffsFromFeeBillCsv.Contains(c.StaffID)).ToListAsync();
            foreach (var item in reFromFeeBillCsv.Item2)
            {
                var staff = staffsNeedToSave.FirstOrDefault(c => c.StaffID == item.Key);
                if (staff.RiderFirstWorkDate == null || staff.RiderFirstWorkDate > item.Value.RidderFirstWorkDate)
                    staff.RiderFirstWorkDate = item.Value.RidderFirstWorkDate;
                if (staff.RiderLastWorkDate == null || staff.RiderLastWorkDate < item.Value.RidderLastWorkDate)
                    staff.RiderLastWorkDate = item.Value.RidderLastWorkDate;
            }
            await DbContext.SaveChangesAsync();

            #endregion

            byte[] Bytes = null;
            if (exceptionReports.Count != 0)
            {

                WriteExcelInput writeExcelInputBase = new WriteExcelInput
                {
                    Sheets = new Dictionary<string, ArrayList>()
                };
                var exceptionReportsOutPut = new ArrayList();
                foreach (var item in exceptionReports)
                {
                    exceptionReportsOutPut.Add(item);
                }
                writeExcelInputBase.Sheets.Add("异常信息", exceptionReportsOutPut);
                var workbook = NPOIExtentions.WriteExcel2007(writeExcelInputBase);
                using (var OutputStream = new NpoiMemoryStream())
                {
                    OutputStream.AllowClose = false;
                    workbook.Write(OutputStream);
                    OutputStream.AllowClose = true;
                    Bytes = OutputStream.GetBuffer();
                }

            }
            return new Tuple<bool, byte[]>(true, Bytes);

        }

        public async Task<Tuple<bool, byte[]>> Calculate(CalculateInput input, ModelStateDictionary modelState)
        {

            #region StaffWorks

            #region stations

            var groups = await _groupService.GetListAsync();
            var theCity = groups.FirstOrDefault(c => c.GroupID == input.GroupID);
            if (theCity == null)
            {
                modelState.AddModelError("GroupID", "城市ID有误");
                return new Tuple<bool, byte[]>(false, null);
            }
            int displayOrderOfNextParentOrNextBrother = GetDisplayOrderOfNextParentOrNextBrother(groups, theCity.DisplayOrder, theCity.Level);
            if (displayOrderOfNextParentOrNextBrother == 0)
            {
                modelState.AddModelError("GroupID", "城市ID有误");
                return new Tuple<bool, byte[]>(false, null);
            }
            var stations = groups.Where(m => m.DisplayOrder >= theCity.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother && m.Level == 5).Select(c => c.GroupID).ToList();

            #endregion
            //当前城市下所有骑手
            var CurrentCityRidders = await DbContext.Staffs.Where(c => stations.Contains(c.User.GroupID)).ToListAsync();
            var StaffIDs = CurrentCityRidders.Select(c => c.StaffID).ToList();
            var StaffWorks = await DbContext.StaffWorks.Where(c => StaffIDs.Contains(c.StaffID) && c.Year == input.Year && c.Month == input.Month).ToListAsync();

            #endregion

            #region monthBillCsv

            var monthBillCsv = ImportMonthBillCsv(input.CityMonthBillCsvPath, modelState);
            if (monthBillCsv == null) return new Tuple<bool, byte[]>(false, null);

            #endregion

            ArrayList exceptionReports = new ArrayList();

            #region RiderReferrer

            var Referrered = CurrentCityRidders.Where(c => c.RecruitChannelID == 1 && (!c.RiderReferrerAwardsProvideTime.HasValue || c.RiderReferrerAwardsProvideTime.Value.Month == input.Month)).ToList();

            var ReferreredHashSet = new HashSet<int>();
            var ReferreringDic = new Dictionary<int, int>();

            foreach (var item in Referrered)
            {
                var RiderReferrerStaff = CurrentCityRidders.FirstOrDefault(c => c.StaffID == item.RiderReferrerStaffID);

                #region 该骑手的转介绍人ID在系统内找不到，请修改

                if (RiderReferrerStaff == null)
                {
                    ExceptionReportInfo exceptionReportInfo = new ExceptionReportInfo
                    {

                        GroupID = item.User.GroupID,
                        StaffID = item.StaffID,
                        StationName = item.User.Group.Name,
                        RiderName = item.Name,
                        StaffMobile = item.StaffMobile,
                        StaffStatus = item.StaffStatu.Name,
                        RiderPosition = item.User.Role.Name,
                        RiderJobType = item.RiderJobType.Name,
                        RiderEleID = item.RiderEleID ?? 0,
                        ExceptionReason = "错误代码101",
                        Remark = "该骑手的转介绍人ID在系统内找不到，请修改骑手转介绍信息。 错误来自：系统数据库"

                    };
                    continue;
                }

                #endregion
                var ReferreringRiderEleID = RiderReferrerStaff.RiderEleID ?? 0;
                var ReferreredRiderEleID = item.RiderEleID ?? 0;
                if (ReferreringRiderEleID != 0 && ReferreredRiderEleID != 0 && monthBillCsv.ContainsKey(ReferreringRiderEleID))
                {

                    #region 被介绍人

                    var ReferreredOrders = monthBillCsv[ReferreredRiderEleID].TotalAmount - monthBillCsv[ReferreredRiderEleID].VoidedOrders - monthBillCsv[ReferreredRiderEleID].TimeoutOrders;
                    var ReferreredStaffWork = StaffWorks.FirstOrDefault(c => c.StaffID == item.StaffID);
                    var ReferreredStaffWorkDays = GetAttendanceDaysActual(ReferreredStaffWork);

                    #endregion

                    #region 介绍人

                    var RiderReferrerStaffOrders = monthBillCsv[ReferreringRiderEleID].TotalAmount - monthBillCsv[ReferreringRiderEleID].TimeoutOrders - monthBillCsv[ReferreringRiderEleID].VoidedOrders;
                    var RiderReferrerStaffStaffWork = StaffWorks.FirstOrDefault(c => c.StaffID == RiderReferrerStaff.StaffID);
                    var RiderReferrerStaffWorkDays = GetAttendanceDaysActual(RiderReferrerStaffStaffWork);

                    #endregion

                    if (ReferreredOrders >= 150
                    && ReferreredStaffWorkDays >= 15
                    && RiderReferrerStaffOrders >= 150
                    && RiderReferrerStaffWorkDays >= 15
                    )
                    {
                        ReferreredHashSet.Add(item.StaffID);
                        if (ReferreringDic != null && ReferreringDic.ContainsKey(RiderReferrerStaff.StaffID))
                        {
                            ReferreringDic[RiderReferrerStaff.StaffID]++;
                        }
                        else
                        {
                            ReferreringDic.Add(RiderReferrerStaff.StaffID, 1);
                        }
                    }

                }

            }

            #endregion

            #region EvaluateCsv

            var evaluateCsv = ImportEvaluateCsv(input.EvaluateCsvPath, exceptionReports, CurrentCityRidders, modelState);
            if (evaluateCsv == null) return new Tuple<bool, byte[]>(false, null);

            #endregion

            #region TeamID

            var teams = await DbContext.Stations.Where(c => true).ToListAsync();

            #endregion

            #region staffLeaves

            var ridderIDs = CurrentCityRidders.Select(c => c.StaffID).ToList();
            var staffLeaves = await DbContext.StaffLeaves.Where(a => ridderIDs.Contains(a.TargetStaffID)).ToListAsync();

            #endregion

            ArrayList contractExpenses = new ArrayList();
            ComputeData data = new ComputeData();
            var policies = PolicyCollectionFactory.CreatePolicy(input.GroupID);

            int Index = 1;
            foreach (var item in CurrentCityRidders)
            {

                #region 系统内骑手没有饿了么ID，请先导入，才能参与运算
                if (!item.RiderEleID.HasValue)
                {

                    ExceptionReportInfo exceptionReportInfo = new ExceptionReportInfo
                    {
                        StaffID = item.StaffID,
                        StaffStatus = item.StaffStatu.Name,
                        GroupID = item.User.GroupID,
                        StationName = item.User.Group.Name,
                        RiderName = item.Name,
                        RiderPosition = item.User.Roles.Count == 0 ? "" : item.User.Roles.First().Name,
                        RiderJobType = item.RiderJobTypeID.HasValue ? item.RiderJobType.Name : "",
                        ExceptionReason = "错误代码102",
                        Remark = "该骑手在系统内没有饿了吗ID，请修改骑手员工信息。错误源来自：系统数据库"
                    };

                    exceptionReports.Add(exceptionReportInfo);
                    continue;

                }
                #endregion

                #region 系统内骑手没有在导入“城市月账单”中出现，请检查是否遗漏

                if (!monthBillCsv.ContainsKey(item.RiderEleID.Value))
                {

                    ExceptionReportInfo exceptionReportInfo = new ExceptionReportInfo
                    {
                        StaffID = item.StaffID,
                        StaffStatus = item.StaffStatu.Name,
                        GroupID = item.User.GroupID,
                        StationName = item.User.Group.Name,
                        RiderName = item.Name,
                        RiderPosition = item.User.Roles.Count == 0 ? "" : item.User.Roles.First().Name,
                        RiderJobType = item.RiderJobTypeID.HasValue ? item.RiderJobType.Name : "",
                        RiderEleID = item.RiderEleID.Value,
                        TeamID = teams.FirstOrDefault(c => c.StationID == item.User.GroupID).StationEleID,
                        TeamName = item.User.Group.Name,
                        ExceptionReason = "错误代码201",
                        Remark = "系统内骑手没有在导入“城市月账单”中出现，请检查是否遗漏。错误源来自：城市月账单CSV表格"
                    };
                    exceptionReports.Add(exceptionReportInfo);
                    continue;

                }

                #endregion

                #region 系统内该骑手没有考勤记录，请先考勤，才能参与运算

                var staffWork = StaffWorks.FirstOrDefault(c => c.StaffID == item.StaffID && c.Month == input.Month && c.Year == input.Year);
                if (staffWork == null)
                {
                    ExceptionReportInfo exceptionReportInfo = new ExceptionReportInfo
                    {
                        StaffID = item.StaffID,
                        StaffStatus = item.StaffStatu.Name,
                        GroupID = item.User.GroupID,
                        StationName = item.User.Group.Name,
                        RiderName = item.Name,
                        RiderPosition = item.User.Roles.Count == 0 ? "" : item.User.Roles.First().Name,
                        RiderJobType = item.RiderJobTypeID.HasValue ? item.RiderJobType.Name : "",
                        RiderEleID = item.RiderEleID.Value,
                        TeamID = teams.FirstOrDefault(c => c.StationID == item.User.GroupID).StationEleID,
                        TeamName = item.User.Group.Name,
                        ExceptionReason = "错误代码103",
                        Remark = "该骑手在系统内没有考勤记录，请增加骑手的考勤记录。 错误源来自:系统数据库"
                    };
                    exceptionReports.Add(exceptionReportInfo);
                    continue;

                }

                #endregion

                #region Policy

                //总单量
                data.TotalAmount = monthBillCsv.ContainsKey(item.RiderEleID ?? 0) ? monthBillCsv[item.RiderEleID.Value].TotalAmount : 0;
                //无效单
                data.VoidedOrders = monthBillCsv.ContainsKey(item.RiderEleID ?? 0) ? monthBillCsv[item.RiderEleID.Value].VoidedOrders : 0;
                //超时单
                data.TimeoutOrders = monthBillCsv.ContainsKey(item.RiderEleID ?? 0) ? monthBillCsv[item.RiderEleID.Value].TimeoutOrders : 0;

                data.AttendanceDaysDesigned = input.AttendanceDaysDesigned;
                data.AttendanceDaysActual = GetAttendanceDaysActual(staffWork);

                //早班天数
                data.Earlies = monthBillCsv.ContainsKey(item.RiderEleID ?? 0) ? monthBillCsv[item.RiderEleID.Value].Earlies : 0;

                if (evaluateCsv != null && evaluateCsv.ContainsKey(item.StaffID))
                {
                    data.GoodEvaluate = evaluateCsv[item.StaffID].GoodEvaluate;
                    data.BadEvaluate = evaluateCsv[item.StaffID].BadEvaluate;
                }
                else
                {
                    data.GoodEvaluate = 0;
                    data.BadEvaluate = 0;
                }

                #region 计算无故缺岗天数

                var offDays = input.AttendanceDaysDesigned - data.AttendanceDaysActual;
                if (offDays > 0)
                {

                    DateTime Begin = new DateTime(input.Year, input.Month, 1, 9, 0, 0);
                    DateTime End = new DateTime(Begin.Year, Begin.Month, DateTime.DaysInMonth(Begin.Year, Begin.Month), 22, 0, 0);

                    var staffLeavesCurrent = staffLeaves.Where(c => (c.BeginDate > Begin) && (c.EndDate < End) && (c.TargetStaffID == item.StaffID)).ToList();
                    if (staffLeavesCurrent == null)
                    {
                        data.NoreasonAbsence = offDays;
                    }
                    else
                    {
                        double daysCount = 0;
                        foreach (var bottom in staffLeavesCurrent)
                        {
                            daysCount += (offDays - bottom.HalfDays * 0.5);
                        }
                        var days = offDays - daysCount;
                        if (days > 0)
                        {
                            data.NoreasonAbsence = days;
                        }
                    }
                }

                #endregion

                #region 转介绍奖励

                if (ReferreredHashSet != null && ReferreredHashSet.Contains(item.StaffID))
                {
                    data.IsReferrered = true;
                    //更新数据库
                    item.RiderReferrerAwardsProvideTime = new DateTime(input.Year, input.Month, 1);
                }
                else
                {
                    data.IsReferrered = false;
                }
                if (ReferreringDic != null && ReferreringDic.ContainsKey(item.StaffID))
                {
                    data.ReferreringCount = ReferreringDic[item.StaffID];
                }
                else
                {
                    data.ReferreringCount = 0;
                }

                #endregion

                policies.Compute(data);

                #endregion

                #region contractExpense

                ContractExpense contractExpense = new ContractExpense
                {

                    Index = Index,
                    TeamID = teams.FirstOrDefault(c => c.StationID == item.User.GroupID).StationEleID,
                    StationName = item.User.Group.Name,
                    Name = item.Name,
                    RidderEleID = item.RiderEleID ?? 0,
                    RidderStatus = item.StaffStatu.Name,
                    AttendanceDaysDesigned = data.AttendanceDaysDesigned,
                    AttendanceDaysActual = data.AttendanceDaysActual,
                    TotalAmount = data.TotalAmount,
                    VoidedOrders = data.VoidedOrders,
                    TimeoutOrders = data.TotalAmount,
                    ActualNormalBill = data.ActualNormalBill,
                    Commissions = data.Commissions,
                    TimeoutCommissions = data.TimeoutCommissions,
                    Days = data.NoreasonAbsence,
                    Amount = data.Amount,
                    EarliesSubsidy = data.EarliesSubsidy,
                    StarAwards = data.StarAwards,
                    ReferralAwards = data.ReferralAwards,

                    BasicFee = data.BasicFee,
                    TelephoneSubsidy = data.TelephoneSubsidy,
                    DineSubsidy = data.DineSubsidy,
                    CarSubsidy = data.CarSubsidy,
                    PerfectAttendance = data.PerfectAttendance,
                    LastRunningTime = item.RiderLastWorkDate

                };
                if (contractExpenses.Add(contractExpense) >= 0) { Index++; }
                monthBillCsv.Remove(item.RiderEleID.Value);

                #endregion

            }

            #region 导入“城市月账单”中骑手并未在系统内登录，请先登录
            if (monthBillCsv.Count != 0)
            {
                foreach (var item in monthBillCsv)
                {
                    ExceptionReportInfo exceptionReportInfo = new ExceptionReportInfo
                    {
                        RiderName = item.Value.Name,
                        RiderEleID = item.Key,
                        ExceptionReason = "错误代码104",
                        Remark = "导入“城市月账单”中骑手在系统内并不存在，请增加骑手员工信息。 错误源来自:系统数据库"
                    };
                    exceptionReports.Add(exceptionReportInfo);
                }
            }
            #endregion

            #region NPOI Import

            if (contractExpenses.Count == 0 && exceptionReports.Count == 0)
            {
                modelState.AddModelError("Sheets", "导出内容为空");
                return new Tuple<bool, byte[]>(false, null);
            }
            WriteExcelInput writeExcelInputBase = new WriteExcelInput
            {
                Sheets = new Dictionary<string, ArrayList>()
            };
            if (contractExpenses.Count != 0)
                writeExcelInputBase.Sheets.Add("工资信息", contractExpenses);
            if (exceptionReports.Count != 0)
                writeExcelInputBase.Sheets.Add("异常信息", exceptionReports);
            var workbook = NPOIExtentions.WriteExcel2007(writeExcelInputBase);

            #endregion

            byte[] Bytes;
            using (var OutputStream = new NpoiMemoryStream())
            {
                OutputStream.AllowClose = false;
                workbook.Write(OutputStream);
                OutputStream.AllowClose = true;
                Bytes = OutputStream.GetBuffer();
            }
            if (Bytes != null && Bytes.Length > 0) await DbContext.SaveChangesAsync();
            return new Tuple<bool, byte[]>(true, Bytes);

        }


        #region Private Methods

        private int GetAttendanceDaysDesigned(int year, int month)
        {


            // 某年某月的第一天
            DateTime t = new DateTime(year, month, 1);

            // 记录周末数量
            int weekendCount = 0;

            do
            {
                // 星期天为周末
                if (t.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekendCount++;
                }
                t = t.AddDays(1);

            } while (t.Month == month);

            return DateTime.DaysInMonth(year, month) - weekendCount;

        }
        private double GetAttendanceDaysActual(StaffWork StaffWork)
        {
            //00:00 - 02:00 深夜班在岗 1
            //06:00 - 09:00 早班在岗 2
            //09:00 - 16:00 上半天在岗 3
            //16:00 - 22:00 下半天在岗 4
            //22:00 - 24:00 夜班在岗 5
            double count = 0;
            if (StaffWork.Day0103 > 0) count += 0.5;
            if (StaffWork.Day0104 > 0) count += 0.5;
            if (StaffWork.Day0203 > 0) count += 0.5;
            if (StaffWork.Day0204 > 0) count += 0.5;
            if (StaffWork.Day0303 > 0) count += 0.5;
            if (StaffWork.Day0304 > 0) count += 0.5;
            if (StaffWork.Day0403 > 0) count += 0.5;
            if (StaffWork.Day0404 > 0) count += 0.5;
            if (StaffWork.Day0503 > 0) count += 0.5;
            if (StaffWork.Day0504 > 0) count += 0.5;
            if (StaffWork.Day0603 > 0) count += 0.5;
            if (StaffWork.Day0604 > 0) count += 0.5;
            if (StaffWork.Day0703 > 0) count += 0.5;
            if (StaffWork.Day0704 > 0) count += 0.5;
            if (StaffWork.Day0803 > 0) count += 0.5;
            if (StaffWork.Day0804 > 0) count += 0.5;
            if (StaffWork.Day0903 > 0) count += 0.5;
            if (StaffWork.Day0904 > 0) count += 0.5;
            if (StaffWork.Day1003 > 0) count += 0.5;
            if (StaffWork.Day1004 > 0) count += 0.5;
            if (StaffWork.Day1103 > 0) count += 0.5;
            if (StaffWork.Day1104 > 0) count += 0.5;
            if (StaffWork.Day1203 > 0) count += 0.5;
            if (StaffWork.Day1204 > 0) count += 0.5;
            if (StaffWork.Day1303 > 0) count += 0.5;
            if (StaffWork.Day1304 > 0) count += 0.5;
            if (StaffWork.Day1403 > 0) count += 0.5;
            if (StaffWork.Day1404 > 0) count += 0.5;
            if (StaffWork.Day1503 > 0) count += 0.5;
            if (StaffWork.Day1504 > 0) count += 0.5;
            if (StaffWork.Day1603 > 0) count += 0.5;
            if (StaffWork.Day1604 > 0) count += 0.5;
            if (StaffWork.Day1703 > 0) count += 0.5;
            if (StaffWork.Day1704 > 0) count += 0.5;
            if (StaffWork.Day1803 > 0) count += 0.5;
            if (StaffWork.Day1804 > 0) count += 0.5;
            if (StaffWork.Day1903 > 0) count += 0.5;
            if (StaffWork.Day1904 > 0) count += 0.5;
            if (StaffWork.Day2003 > 0) count += 0.5;
            if (StaffWork.Day2004 > 0) count += 0.5;
            if (StaffWork.Day2103 > 0) count += 0.5;
            if (StaffWork.Day2104 > 0) count += 0.5;
            if (StaffWork.Day2203 > 0) count += 0.5;
            if (StaffWork.Day2204 > 0) count += 0.5;
            if (StaffWork.Day2303 > 0) count += 0.5;
            if (StaffWork.Day2304 > 0) count += 0.5;
            if (StaffWork.Day2403 > 0) count += 0.5;
            if (StaffWork.Day2404 > 0) count += 0.5;
            if (StaffWork.Day2503 > 0) count += 0.5;
            if (StaffWork.Day2504 > 0) count += 0.5;
            if (StaffWork.Day2603 > 0) count += 0.5;
            if (StaffWork.Day2604 > 0) count += 0.5;
            if (StaffWork.Day2703 > 0) count += 0.5;
            if (StaffWork.Day2704 > 0) count += 0.5;
            if (StaffWork.Day2803 > 0) count += 0.5;
            if (StaffWork.Day2804 > 0) count += 0.5;
            if (StaffWork.Day2903 > 0) count += 0.5;
            if (StaffWork.Day2904 > 0) count += 0.5;
            if (StaffWork.Day3003 > 0) count += 0.5;
            if (StaffWork.Day3004 > 0) count += 0.5;
            if (StaffWork.Day3103 > 0) count += 0.5;
            if (StaffWork.Day3104 > 0) count += 0.5;
            return count;
        }
        private void StaffWorkUpdateDays(StaffWork staffWork, DateTime orderTime)
        {

            string propertyName;
            string oneDayPropertyName = OneDayPropertyName(orderTime);
            if (oneDayPropertyName == null) return;

            staffWork.Year = orderTime.Year;
            staffWork.Month = orderTime.Month;
            if (orderTime.Day < 10)
            {
                propertyName = "Day0" + orderTime.Day.ToString() + oneDayPropertyName;
            }
            else
            {
                propertyName = "Day" + orderTime.Day.ToString() + oneDayPropertyName;
            }
            Type type = staffWork.GetType();


            PropertyInfo propertyInfo = type.GetProperties().FirstOrDefault(c => c.Name == propertyName);

            FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).SetValue(staffWork, 1);

            //  propertyInfo.SetValue(staffWork, 1);

        }
        private string OneDayPropertyName(DateTime orderTime)
        {
            //00:00 - 02:00 深夜班在岗
            //06:00 - 09:00 早班在岗
            //09:00 - 16:00 上半天在岗
            //16:00 - 22:00 下半天在岗
            //22:00 - 24:00 夜班在岗
            if ((orderTime.Hour >= 0 && orderTime.Hour < 2)
                 || (orderTime.Hour == 2 && orderTime.Minute == 0))
            {
                return "01";
            }
            else if (orderTime.Hour >= 6 && orderTime.Hour < 9)
            {
                return "02";
            }
            else if (orderTime.Hour >= 9 && orderTime.Hour < 16)
            {
                return "03";
            }
            else if (orderTime.Hour >= 16 && orderTime.Hour < 22)
            {
                return "04";
            }
            else if (orderTime.Hour >= 22 && orderTime.Hour < 24)
            {
                return "05";
            }
            else return null;

        }
        private Admin.Models.Group RecursionToGetCity(List<Admin.Models.Group> groups, Admin.Models.Group group)
        {
            if (group.Level == 3)
                return group;
            return RecursionToGetCity(groups, groups.FirstOrDefault(c => c.GroupID == group.ParentID));
        }
        private int GetDisplayOrderOfNextParentOrNextBrother(List<Admin.Models.Group> groups, int displayOrder, int level)
        {
            return groups.Where(m => m.Level <= level && m.DisplayOrder > displayOrder)
                .OrderBy(m => m.DisplayOrder)
                .Select(m => m.DisplayOrder)
                .FirstOrDefault();
        }

        #endregion

        #region EXCEL与CSV处理

        public Dictionary<int, CityMonthBillStatisticsRidder> ImportMonthBillCsv(string MonthBillCsvPath, ModelStateDictionary modelState)
        {

            Dictionary<int, CityMonthBillStatisticsRidder> Statistics = new Dictionary<int, CityMonthBillStatisticsRidder>();
            try
            {
                using (var file = new FileStream(MonthBillCsvPath, FileMode.Open, FileAccess.Read))
                {

                    using (var read = new StreamReader(file, Encoding.GetEncoding("gb2312")))//TODO:需改回utf-8
                    {

                        using (var csv = new CsvReader(read, false))
                        {

                            List<string> ColNames = new List<string>()
                            {
                                "日期",
                                "骑手ID",
                                "骑手",
                                "是否超时",
                                "配送状态",
                                "骑手接单时间"
                            };

                            var titles = CsvExtentions.GetTitleCoords(csv, ColNames);

                            #region 检查列名错位 - 暂不支持错位列

                            int temp = -1;
                            foreach (var item in titles)
                            {
                                if (temp == -1)
                                {
                                    temp = item.Value.Item1;
                                }
                                else
                                {
                                    if (temp != item.Value.Item1) throw new Exception("列名不在同一行，请检查导入CSV数据");
                                }
                            }

                            #endregion

                            while (csv.ReadNextRecord())
                            {

                                if (csv.CurrentRecordIndex > titles["骑手ID"].Item1)
                                {

                                    if (!Int32.TryParse(csv[titles["骑手ID"].Item2], out int ridderID)) throw new Exception("骑手ID列内容格式不正确");

                                    if (ridderID == 0) continue;

                                    if (Statistics.ContainsKey(ridderID))
                                    {

                                        if (!DateTime.TryParse(csv[titles["日期"].Item2], out DateTime datetime)) throw new Exception("日期列内容格式不正确");
                                        if (!DateTime.TryParse(csv[titles["骑手接单时间"].Item2], out DateTime RiderPickupTime)) throw new Exception("骑手接单时间列内容格式不正确");
                                        if (Statistics[ridderID].Month != datetime.Month) throw new Exception("日期列存在跨月数据");

                                        #region Earlies
                                        //06:00 - 09:00 早班在岗
                                        if (RiderPickupTime.Hour > 6 && RiderPickupTime.Hour < 9)
                                        {
                                            Statistics[ridderID].Earlies++;
                                        }
                                        #endregion

                                        var someDay = datetime.Day;
                                        if (csv[titles["是否超时"].Item2] == "是")
                                        {
                                            Statistics[ridderID].TimeoutOrders++;
                                            if (Statistics[ridderID].StatisticAsDays.ContainsKey(someDay))
                                            {
                                                Statistics[ridderID].StatisticAsDays[someDay].TimeoutOrders++;
                                            }
                                            else
                                            {
                                                AsDays asDays = new AsDays() { TimeoutOrders = 1 };
                                                Statistics[ridderID].StatisticAsDays.Add(someDay, asDays);
                                            }

                                        }
                                        else if (csv[titles["是否超时"].Item2] != "否")
                                        {
                                            throw new Exception("“是否超时”列存在空白项,无法计算超时单");
                                        }

                                        if (csv[titles["配送状态"].Item2] == "异常")
                                        {
                                            Statistics[ridderID].VoidedOrders++;

                                            if (Statistics[ridderID].StatisticAsDays.ContainsKey(someDay))
                                            {
                                                Statistics[ridderID].StatisticAsDays[someDay].VoidedOrders++;
                                            }
                                            else
                                            {
                                                AsDays asDays = new AsDays() { VoidedOrders = 1 };
                                                Statistics[ridderID].StatisticAsDays.Add(someDay, asDays);
                                            }
                                        }
                                        else if (csv[titles["配送状态"].Item2] != "正常")
                                        {
                                            throw new Exception("“配送状态”列存在空白项,无法计算无效单");
                                        }

                                        Statistics[ridderID].TotalAmount++;
                                        if (Statistics[ridderID].StatisticAsDays.ContainsKey(someDay))
                                        {
                                            Statistics[ridderID].StatisticAsDays[someDay].TotalAmount++;
                                        }
                                        else
                                        {
                                            AsDays asDays = new AsDays() { TotalAmount = 1 };
                                            Statistics[ridderID].StatisticAsDays.Add(someDay, asDays);
                                        }
                                    }
                                    else
                                    {

                                        var cityMonthBillStatisticsRidder = new CityMonthBillStatisticsRidder()
                                        {
                                            StatisticAsDays = new Dictionary<int, AsDays>()
                                        };

                                        if (!DateTime.TryParse(csv[titles["日期"].Item2], out DateTime datetime)) throw new Exception("日期列内容格式不正确");
                                        cityMonthBillStatisticsRidder.Month = datetime.Month;

                                        var someDay = datetime.Day;

                                        if (csv[titles["是否超时"].Item2] == "是")
                                        {
                                            cityMonthBillStatisticsRidder.TimeoutOrders = 1;

                                            if (cityMonthBillStatisticsRidder.StatisticAsDays.ContainsKey(someDay))
                                            {
                                                cityMonthBillStatisticsRidder.StatisticAsDays[someDay].TimeoutOrders = 1;
                                            }
                                            else
                                            {
                                                AsDays asDays = new AsDays() { TimeoutOrders = 1 };
                                                cityMonthBillStatisticsRidder.StatisticAsDays.Add(someDay, asDays);
                                            }

                                        }
                                        else if (csv[titles["是否超时"].Item2] == "否")
                                        {
                                            cityMonthBillStatisticsRidder.TimeoutOrders = 0;
                                            if (cityMonthBillStatisticsRidder.StatisticAsDays.ContainsKey(someDay))
                                            {
                                                cityMonthBillStatisticsRidder.StatisticAsDays[someDay].TimeoutOrders = 0;
                                            }
                                            else
                                            {
                                                AsDays asDays = new AsDays() { TimeoutOrders = 0 };
                                                cityMonthBillStatisticsRidder.StatisticAsDays.Add(someDay, asDays);
                                            }

                                        }
                                        else throw new Exception("“是否超时”列存在空白项,无法计算超时单");

                                        if (csv[titles["配送状态"].Item2] == "异常")
                                        {
                                            cityMonthBillStatisticsRidder.VoidedOrders = 1;

                                            if (cityMonthBillStatisticsRidder.StatisticAsDays.ContainsKey(someDay))
                                            {
                                                cityMonthBillStatisticsRidder.StatisticAsDays[someDay].VoidedOrders = 1;
                                            }
                                            else
                                            {
                                                AsDays asDays = new AsDays() { VoidedOrders = 1 };
                                                cityMonthBillStatisticsRidder.StatisticAsDays.Add(someDay, asDays);
                                            }
                                        }
                                        else if (csv[titles["配送状态"].Item2] == "正常")
                                        {
                                            cityMonthBillStatisticsRidder.VoidedOrders = 0;

                                            if (cityMonthBillStatisticsRidder.StatisticAsDays.ContainsKey(someDay))
                                            {
                                                cityMonthBillStatisticsRidder.StatisticAsDays[someDay].VoidedOrders = 0;
                                            }
                                            else
                                            {
                                                AsDays asDays = new AsDays() { VoidedOrders = 0 };
                                                cityMonthBillStatisticsRidder.StatisticAsDays.Add(someDay, asDays);
                                            }
                                        }
                                        else throw new Exception("“配送状态”列存在空白项,无法计算无效单");

                                        cityMonthBillStatisticsRidder.Name = csv[titles["骑手"].Item2];
                                        cityMonthBillStatisticsRidder.TotalAmount = 1;
                                        if (cityMonthBillStatisticsRidder.StatisticAsDays.ContainsKey(someDay))
                                        {
                                            cityMonthBillStatisticsRidder.StatisticAsDays[someDay].TotalAmount = 0;
                                        }
                                        else
                                        {
                                            AsDays asDays = new AsDays() { TotalAmount = 0 };
                                            cityMonthBillStatisticsRidder.StatisticAsDays.Add(someDay, asDays);
                                        }

                                        Statistics.Add(ridderID, cityMonthBillStatisticsRidder);

                                    }

                                }

                            }

                        }

                    }

                }
            }
            catch (Exception e)
            {

                modelState.AddModelError("ImportMonthBillCsv", e.Message);
                return null;

            }
            return Statistics;

        }

        public async Task<Tuple<Dictionary<string, Dictionary<string, int>>, Dictionary<int, RidderWorkDate>>> ImportDeliveryFeeBillCsv(string DeliveryFeeBillCsvPath, Guid CityGroupID, HashSet<ExceptionReportInfo> exceptionReports, ModelStateDictionary modelState)
        {

            #region 按用户组筛选

            //缓存
            // List<Station> stationsBase = await DbContext.Stations.Where(c => true).ToListAsync();
            List<Admin.Models.Group> groups = await _groupService.GetListAsync();
            var theCity = groups.FirstOrDefault(c => c.GroupID == CityGroupID);
            if (theCity == null) { modelState.AddModelError("theCity", "城市ID不正确"); return null; }
            int displayOrderOfNextParentOrNextBrother = GetDisplayOrderOfNextParentOrNextBrother(groups, theCity.DisplayOrder, theCity.Level);
            if (displayOrderOfNextParentOrNextBrother == 0)
            {
                modelState.AddModelError("teamID", "导入EXCEL表第一行团队ID有误");
                return null;
            }
            var stations = groups.Where(m => m.DisplayOrder >= theCity.DisplayOrder && m.DisplayOrder < displayOrderOfNextParentOrNextBrother && m.Level == 5).Select(c => c.GroupID);

            List<Staff> staffs;
            staffs = await DbContext.Staffs.Where(c => stations.Contains(c.User.GroupID)).ToListAsync();


            #endregion

            #region StaffTurnover

            var staffIDs = staffs.Select(c => c.StaffID).ToList();
            var staffTurnovers = await DbContext.StaffTurnovers.Where(c => staffIDs.Contains(c.TargetStaffID)).ToListAsync();

            #endregion

            Dictionary<string, Dictionary<string, int>> NeedsaveDic = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<int, RidderWorkDate> RidderWorkDates = new Dictionary<int, RidderWorkDate>();
            Dictionary<int, List<Turnover>> TurnoverCache = new Dictionary<int, List<Turnover>>();

            try
            {
                using (var file = new FileStream(DeliveryFeeBillCsvPath, FileMode.Open, FileAccess.Read))
                {

                    using (var read = new StreamReader(file, Encoding.GetEncoding("gb2312")))//TODO:需改回utf-8
                    {

                        using (var csv = new CsvReader(read, false))
                        {

                            #region ready

                            List<string> ColNames = new List<string>()
                            {

                                "骑手ID",
                                "骑手",
                                "骑手接单时间",
                                "团队ID",
                                "团队名称"

                            };
                            var titles = CsvExtentions.GetTitleCoords(csv, ColNames);

                            #region 检查列名错位 - 暂不支持错位列

                            int temp = -1;
                            foreach (var item in titles)
                            {
                                if (temp == -1)
                                {
                                    temp = item.Value.Item1;
                                }
                                else
                                {
                                    if (temp != item.Value.Item1) throw new Exception("列名不在同一行，请检查导入CSV数据");
                                }
                            }

                            #endregion

                            #endregion

                            while (csv.ReadNextRecord())
                            {

                                if (csv.CurrentRecordIndex > titles["骑手ID"].Item1)
                                {

                                    #region 取值

                                    if (!Int32.TryParse(csv[titles["骑手ID"].Item2], out int RiderEleID)) throw new Exception("骑手ID列内容格式不正确");
                                    if (!Int32.TryParse(csv[titles["团队ID"].Item2], out int TeamID)) throw new Exception("团队ID列内容格式不正确");
                                    string TeamName = csv[titles["团队名称"].Item2];
                                    string RiderName = csv[titles["骑手"].Item2];
                                    if (!DateTime.TryParse(csv[titles["骑手接单时间"].Item2], out DateTime RiderPickUpTime)) throw new Exception("骑手接单时间列内容格式不正确");

                                    #endregion

                                    #region 校验

                                    //只判断一次，就认为所有都是正确的
                                    if (csv.CurrentRecordIndex == titles["骑手ID"].Item1 + 1)
                                    {

                                        var station = groups.FirstOrDefault(c => c.Name == TeamName);
                                        if (station == null) throw new Exception("选择城市与提供数据解析的城市不一致");
                                        if (RecursionToGetCity(groups, groups.FirstOrDefault(c => c.GroupID == station.GroupID)).GroupID != CityGroupID) { throw new Exception("选择城市与提供数据解析的城市不一致"); }

                                    }
                                    if (RiderEleID == 0) continue;//ridderID等于0的特殊情况，需注意

                                    #endregion

                                    var staff = staffs.FirstOrDefault(c => c.RiderEleID == RiderEleID);

                                    #region exceptionReport

                                    if (staff == null)
                                    {

                                        #region 系统没有骑手饿了么ID对应的员工

                                        ExceptionReportInfo exceptionReport = new ExceptionReportInfo
                                        {
                                            RiderEleID = RiderEleID,
                                            RiderName = RiderName,
                                            ExceptionReason = "错误代码105",
                                            TeamID = TeamID,
                                            TeamName = TeamName,
                                            Remark = "系统没有骑手饿了么ID对应的员工（可能需要补充骑手饿了么ID，这种情况必须补充完整）。 错误源来自:系统数据库"
                                        };
                                        exceptionReports.Add(exceptionReport);

                                        #endregion

                                        continue;
                                    }
                                    else
                                    {

                                        #region 员工不处于在职状态

                                        if (staff.StaffStatusID != 2)
                                        {

                                            ExceptionReportInfo exceptionReportStaffStatusID = new ExceptionReportInfo
                                            {
                                                StaffID = staff.StaffID,
                                                GroupID = staff.User.GroupID,
                                                StationName = staff.User.Group.Name,
                                                RiderName = staff.Name,
                                                StaffStatus = staff.StaffStatu.Name,
                                                RiderPosition = staff.User.Roles == null ? "" : staff.User.Roles.First().Name,
                                                RiderJobType = staff.RiderJobType.Name,
                                                RiderEleID = staff.RiderEleID.Value,
                                                TeamID = TeamID,
                                                TeamName = TeamName,
                                                ExceptionReason = "错误代码106",
                                                Remark = "该骑手不处于在职状态（可能需要入职，也可以不入职）。 错误源来自:系统数据库"
                                            };
                                            exceptionReports.Add(exceptionReportStaffStatusID);

                                        }

                                        #endregion

                                        #region EXCEL表中反应出员工所处于的站点错误

                                        var group = groups.FirstOrDefault(c => c.Name == TeamName);
                                        var ToGroupID = group.GroupID;


                                        ExceptionReportInfo exceptionReport = new ExceptionReportInfo
                                        {
                                            RiderEleID = RiderEleID,
                                            GroupID = staff.User.GroupID,
                                            StationName = staff.User.Group.Name,
                                            StaffID = staff.StaffID,
                                            RiderName = staff.Name,
                                            StaffStatus = staff.StaffStatu.Name,
                                            RiderJobType = staff.RiderJobType.Name,
                                            ExceptionReason = "错误代码107",
                                            Remark = "该骑手在系统内的“站点名称”与导入表不一致。系统已将该骑手从“" + staff.User.Group.Name + "”调整到“" + TeamName + "”。生效时间为“" + RiderPickUpTime.ToString() + "”",
                                            RiderPosition = staff.User.Roles == null ? "" : staff.User.Roles.First().Name,
                                            TeamID = TeamID,
                                            TeamName = TeamName
                                        };

                                        if (TurnoverCache.ContainsKey(staff.StaffID))
                                        {

                                            var count = TurnoverCache[staff.StaffID].Count;
                                            var FromGroupID = TurnoverCache[staff.StaffID][count - 1].ToGroupID;
                                            if (groups.FirstOrDefault(c => c.GroupID == FromGroupID).Name != TeamName)
                                            {
                                                TurnoverCache[staff.StaffID].Add(new Turnover
                                                {
                                                    FromGroupID = FromGroupID,
                                                    ToGroupID = ToGroupID,
                                                    EffectiveDate = RiderPickUpTime,
                                                    ExceptionReport = exceptionReport
                                                });
                                            }

                                        }
                                        else
                                        {

                                            if (staff.User.Group.Name != TeamName)
                                            {
                                                var FromGroupID = staff.User.GroupID;
                                                TurnoverCache.Add(staff.StaffID, new List<Turnover>() {
                                                new Turnover(){
                                                    FromGroupID =FromGroupID,
                                                    ToGroupID = ToGroupID,
                                                    EffectiveDate = RiderPickUpTime,
                                                    ExceptionReport = exceptionReport
                                                }
                                            });

                                            }
                                        }

                                        #endregion

                                    }

                                    #endregion

                                    #region RidderWorkDate


                                    if (RidderWorkDates.ContainsKey(staff.StaffID))
                                    {
                                        if (RidderWorkDates[staff.StaffID].RidderLastWorkDate < RiderPickUpTime)
                                        {
                                            RidderWorkDates[staff.StaffID].RidderLastWorkDate = RiderPickUpTime;
                                        }
                                        else if (RidderWorkDates[staff.StaffID].RidderFirstWorkDate > RiderPickUpTime)
                                        {
                                            RidderWorkDates[staff.StaffID].RidderFirstWorkDate = RiderPickUpTime;

                                        }
                                    }
                                    else
                                    {
                                        RidderWorkDates.Add(staff.StaffID, new RidderWorkDate()
                                        {
                                            RidderFirstWorkDate = RiderPickUpTime,
                                            RidderLastWorkDate = RiderPickUpTime
                                        });
                                    }


                                    #endregion

                                    #region StaffWorkData

                                    var keyDic = staff.StaffID.ToString() + "," + RiderPickUpTime.Year.ToString() + "," + RiderPickUpTime.Month;
                                    var day = RiderPickUpTime.Day.ToString().PadLeft(2, '0');
                                    var oneDayPropertyName = OneDayPropertyName(RiderPickUpTime);
                                    if (oneDayPropertyName == null) continue;
                                    var keyDays = "Day" + day + oneDayPropertyName;
                                    if (!NeedsaveDic.ContainsKey(keyDic))
                                    {

                                        Dictionary<string, int> DaysDic = new Dictionary<string, int>
                    {
                        { keyDays, 1 }
                    };
                                        NeedsaveDic.Add(keyDic, DaysDic);

                                    }
                                    else
                                    {

                                        if (!NeedsaveDic[keyDic].ContainsKey(keyDays))
                                        {
                                            NeedsaveDic[keyDic].Add(keyDays, 1);
                                        }

                                    }

                                    #endregion

                                }

                            }

                        }

                    }

                }
            }
            catch (Exception e)
            {

                modelState.AddModelError("ImportDeliveryFeeBillCsv", e.Message);
                return null;

            }



            if (TurnoverCache.Count != 0)
            {

                #region excel异常导出

                foreach (var item in TurnoverCache)
                {


                    foreach (var bottom in item.Value)
                    {

                        var begin = new DateTime(bottom.EffectiveDate.Year, bottom.EffectiveDate.Month, 1);
                        var end = new DateTime(bottom.EffectiveDate.Year, bottom.EffectiveDate.Month + 1, 1);

                        var FromGroupID = bottom.FromGroupID;
                        var ToGroupID = bottom.ToGroupID;
                        var currentStaffTurnovers = staffTurnovers
                            .Where(c => c.TargetStaffID == item.Key
                            && c.EffectiveDate > begin
                            && c.EffectiveDate < end
                            && c.ToGroupID == ToGroupID
                            && c.RequestRemark == "考勤异常-系统自动添加"
                            ).ToList();

                        if (currentStaffTurnovers.Count == 0)
                        {

                            if (exceptionReports.Add(bottom.ExceptionReport))
                            {


                                #region 系统自动调站，并更新数据库记录

                                //系统自动调站，并更新数据库记录
                                //1、更新staff表
                                //2、更新staffTurnover表

                                string cmd = @"UPDATE [FastOA].[dbo].[User] SET GroupID = @GroupID WHERE UserID = @UserID
                                           INSERT INTO [FastOA].[dbo].[StaffTurnover]
                                            (
                                                 [StaffTurnoverTypeID]
                                                ,[RequestStaffID]
                                                ,[TargetStaffID]
                                                ,[FromGroupID]
                                                ,[ToGroupID]
                                                ,[RequestRemark]
                                                ,[CreationDate]
                                                ,[EffectiveDate]
                                                ,[AuditStatus]
                                                ,[AuditStaffLevelCurrent]
                                                ,[AuditStaffLevelMax]
                                                ,[AuditStaffID1]
                                                ,[AuditDate1]
                                                ,[AuditRemark1]
                                                ) VALUES(
                                                 4
                                                ,5
                                                ,@UserID
                                                ,@FromGroupID
                                                ,@GroupID
                                                ,'考勤异常-系统自动添加'
                                                ,@RiderPickUpTime
                                                ,@RiderPickUpTime
                                                ,1
                                                ,1
                                                ,1
                                                ,5
                                                ,@RiderPickUpTime
                                                ,'考勤异常-系统自动添加'  )";

                                await DbContext.Database.ExecuteSqlCommandAsync(cmd,

                                    new SqlParameter("GroupID", ToGroupID),
                                    new SqlParameter("UserID", item.Key),
                                    new SqlParameter("FromGroupID", FromGroupID),
                                    new SqlParameter("RiderPickUpTime", bottom.EffectiveDate)

                                    );

                                #endregion


                            }

                        }

                    }



                }

                #endregion

            }




            return new Tuple<Dictionary<string, Dictionary<string, int>>, Dictionary<int, RidderWorkDate>>(NeedsaveDic, RidderWorkDates);

        }

        public Dictionary<int, RidderEvaluate> ImportEvaluateCsv(string EvaluateCsvPath, ArrayList exceptionReports, List<Staff> staffs, ModelStateDictionary modelState)
        {

            Dictionary<int, RidderEvaluate> RidderEvaluate = new Dictionary<int, RidderEvaluate>();
            Dictionary<string, ExceptionReportInfo> exceptionReportHashSet = new Dictionary<string, ExceptionReportInfo>();

            try
            {
                using (var file = new FileStream(EvaluateCsvPath, FileMode.Open, FileAccess.Read))
                {

                    using (var read = new StreamReader(file, Encoding.GetEncoding("gb2312")))//TODO:需改回utf-8
                    {

                        using (var csv = new CsvReader(read, false))
                        {

                            List<string> ColNames = new List<string>()
                            {
                                "骑手信息",
                                "骑手手机",
                                "评价星级"
                            };
                            var titles = CsvExtentions.GetTitleCoords(csv, ColNames);
                            #region 检查列名错位 - 暂不支持错位列

                            int temp = -1;
                            foreach (var item in titles)
                            {
                                if (temp == -1)
                                {
                                    temp = item.Value.Item1;
                                }
                                else
                                {
                                    if (temp != item.Value.Item1) throw new Exception("列名不在同一行，请检查导入CSV数据");
                                }
                            }

                            #endregion
                            while (csv.ReadNextRecord())
                            {

                                if (csv.CurrentRecordIndex > titles["骑手信息"].Item1)
                                {

                                    #region 取值

                                    var RiderName = csv[titles["骑手信息"].Item2].ToString();

                                    var RiderTelNO = csv[titles["骑手手机"].Item2].ToString();

                                    var EvaluateStars = csv[titles["评价星级"].Item2].ToString();

                                    #endregion

                                    var staff = staffs.FirstOrDefault(c => c.Name == RiderName && c.StaffMobile == RiderTelNO);
                                    if (staff == null)
                                    {

                                        ExceptionReportInfo exceptionReportInfo = new ExceptionReportInfo
                                        {
                                            RiderName = RiderName,
                                            StaffMobile = RiderTelNO,
                                            ExceptionReason = "错误代码202",
                                            Remark = "该骑手在系统内没有找到。 错误源来自：城市好评单CSV表格"
                                        };

                                        if (exceptionReportHashSet.ContainsKey(RiderName + RiderTelNO))
                                        {
                                            exceptionReportHashSet.Add(RiderName + RiderTelNO, exceptionReportInfo);

                                        }
                                        continue;
                                    }

                                    if (RidderEvaluate != null && RidderEvaluate.ContainsKey(staff.StaffID))
                                    {
                                        if (EvaluateStars == "1")
                                        {
                                            RidderEvaluate[staff.StaffID].BadEvaluate++;
                                        }
                                        else if (EvaluateStars == "5")
                                        {
                                            RidderEvaluate[staff.StaffID].GoodEvaluate++;
                                        }
                                    }
                                    else
                                    {
                                        RidderEvaluate dictionary = new RidderEvaluate();
                                        if (EvaluateStars == "1")
                                        {
                                            dictionary.BadEvaluate = 1;
                                        }
                                        else if (EvaluateStars == "5")
                                        {
                                            dictionary.GoodEvaluate = 1;
                                        }
                                        RidderEvaluate.Add(staff.StaffID, dictionary);
                                    }

                                }

                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                modelState.AddModelError("ImportEvaluateCsv", e.Message);
                return null;
            }

            foreach (var item in exceptionReportHashSet)
            {
                exceptionReports.Add(item.Value);
            }



            return RidderEvaluate;
        }

        #endregion

    }

    public class NpoiMemoryStream : MemoryStream
    {
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
    public class RidderWorkDate
    {
        public DateTime? RidderFirstWorkDate { get; set; }
        public DateTime? RidderLastWorkDate { get; set; }
    }
    public class RidderEvaluate
    {
        public int GoodEvaluate { get; set; }
        public int BadEvaluate { get; set; }
    }
    public class Turnover
    {
        public Guid FromGroupID { get; set; }
        public Guid ToGroupID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public ExceptionReportInfo ExceptionReport { get; set; }
    }

}
