using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Modules.Admin.Services;
using Lavie.Modules.Project.Models;
using Lavie.Infrastructure;
using XM = Lavie.Modules.Admin.Models;
using Lavie.Models;
using Lavie.Modules.Admin.Models.Api;
using Lavie.Extensions;

namespace Lavie.Modules.Project.Repositories
{

    public partial interface IFlowRepository
    {
        #region Flow

        Task<bool> RequestStationLeave(RequestStationLeaveInput input, ModelStateDictionary modelState);
        Task<Page<StaffLeaveInfoExtention>> GetStationLeaveList(XM.User currentUser, Guid groupID, bool isHR, StaffLeaveSearchCriteria criteria, PagingInfo pagingInfo);
        Task<bool> AuditStationLeave(List<AuditStationLeaveInput> input, LavieContext _context, ModelStateDictionary modelState);
        Task<List<StaffLeaveAuditFlowBase>> GetStaffLeaveAuditFlowList();
        Task<bool> ConfigStaffLeaveAuditFlow(StaffLeaveAuditFlowInput input, ModelStateDictionary modelState);

        #endregion
    }

    public partial class FlowRepository : RepositoryBase, IFlowRepository
    {

        private IGroupService _groupService;
        private readonly Expression<Func<StaffLeave, StaffLeaveInfoExtention>> _stationLeaveInfoSelector;
        private readonly Expression<Func<StaffLeaveAuditFlow, StaffLeaveAuditFlowBase>> _staffLeaveAuditFlowSelector;
        private readonly ICacheModule _cache;
        private const string GroupListCacheKey = "StaffLeaveAuditFlowList";
        private readonly Guid CityManager = new Guid("9851828F-FA57-4CB2-BCAA-7E3C855C1C95");
        private readonly Guid AreaManager = new Guid("4d9ece5c-c807-4009-b298-8ed016df88b6");
        private readonly Guid StationManager = new Guid("ECE3FBF5-F55E-4111-A653-6015CFD803BF");

        public FlowRepository(IGroupService groupService
            , IModuleRegistry moduleRegistry
            )
        {

            this._cache = moduleRegistry.GetModules<ICacheModule>().Last();
            _groupService = groupService;
            _stationLeaveInfoSelector = b => new StaffLeaveInfoExtention
            {

                StaffLeaveID = b.StaffLeaveID,
                StaffLeaveType = new Models.StaffLeaveType
                {
                    StaffLeaveTypeID = b.StaffLeaveTypeID,
                    Name = b.StaffLeaveType.Name
                },
                RequestStaff = new StaffInfoBase
                {
                    StaffID = b.RequestStaff.StaffID,
                    Name = b.RequestStaff.Name
                },
                TargetStaff = new StaffInfoBase
                {
                    StaffID = b.TargetStaff.StaffID,
                    Name = b.TargetStaff.Name
                },
                HalfDays = b.HalfDays,
                BeginDate = b.BeginDate,
                EndDate = b.EndDate,
                EndDatePartial = b.EndDatePartial,
                BeginDatePartial = b.BeginDatePartial,
                RequestRemark = b.RequestRemark,
                RequestAttachmentURL1 = b.RequestAttachmentURL1,
                RequestAttachmentURL2 = b.RequestAttachmentURL2,
                RequestAttachment1UploadTime = b.RequestAttachment1UploadTime,
                RequestAttachment2UploadTime = b.RequestAttachment2UploadTime,
                CreationDate = b.CreationDate,
                AuditStatus = new AuditStatus
                {
                    AuditStatusID = b.AuditStatus,
                    Name = b.AuditStatus
                },
                AuditStaffLevelCurrent = b.AuditStaffLevelCurrent,
                AuditStaffLevelMax = b.AuditStaffLevelMax,
                AuditStaff1 = new StaffInfoBase
                {
                    StaffID = b.AuditStaffID1 ?? 0,
                    Name = b.AuditStaffID1.HasValue ? b.Staff_AuditStaffID1.Name : null
                },
                AuditStaff2 = new StaffInfoBase
                {
                    StaffID = b.AuditStaffID2 ?? 0,
                    Name = b.AuditStaffID2.HasValue ? b.Staff_AuditStaffID2.Name : null
                },
                AuditStaff3 = new StaffInfoBase
                {
                    StaffID = b.AuditStaffID3 ?? 0,
                    Name = b.AuditStaffID3.HasValue ? b.Staff_AuditStaffID3.Name : null
                },
                AuditStaff4 = new StaffInfoBase
                {
                    StaffID = b.AuditStaffID4 ?? 0,
                    Name = b.AuditStaffID4.HasValue ? b.Staff_AuditStaffID4.Name : null
                },
                AuditDate1 = b.AuditDate1,
                AuditDate2 = b.AuditDate2,
                AuditDate3 = b.AuditDate3,
                AuditDate4 = b.AuditDate4,
                AuditRemark1 = b.AuditRemark1,
                AuditRemark2 = b.AuditRemark2,
                AuditRemark3 = b.AuditRemark3,
                AuditRemark4 = b.AuditRemark4

            };
            _staffLeaveAuditFlowSelector = c => new StaffLeaveAuditFlowBase
            {

                StaffLeaveAuditFlowID = c.StaffLeaveAuditFlowID,
                RequestRoleID = c.RequestRoleID,
                AuditGroupID1 = c.AuditGroupID1,
                AuditGroupID2 = c.AuditGroupID2,
                AuditGroupID3 = c.AuditGroupID3,
                AuditGroupID4 = c.AuditGroupID4,
                AuditRoleID1 = c.AuditRoleID1,
                AuditRoleID2 = c.AuditRoleID2,
                AuditRoleID3 = c.AuditRoleID3,
                AuditRoleID4 = c.AuditRoleID4,
                AuditStaffLevelMaxDays1 = c.AuditStaffLevelMaxDays1,
                AuditStaffLevelMaxDays2 = c.AuditStaffLevelMaxDays2,
                AuditStaffLevelMaxDays3 = c.AuditStaffLevelMaxDays3,
                AuditStaffLevelMaxDays4 = c.AuditStaffLevelMaxDays4

            };

        }

        //站级请假
        public async Task<bool> RequestStationLeave(RequestStationLeaveInput input, ModelStateDictionary modelState)
        {

            var requestStaff = await DbContext.Set<Staff>().Where(c => c.StaffID == input.RequestStaffID).FirstOrDefaultAsync();
            var targetStaff = await DbContext.Set<Staff>().Where(c => c.StaffID == input.TargetStaffID).FirstOrDefaultAsync();
            if (requestStaff == null || targetStaff == null)
            {
                modelState.AddModelError("StaffID", "StaffID有误");
                return false;
            }
            if (requestStaff.User.GroupID != targetStaff.User.GroupID)
            {
                modelState.AddModelError("GroupID", "不能跨部门申请");
                return false;
            }
            var staffLeave = DbContext.Set<StaffLeave>().Create();
            DbContext.StaffLeaves.Add(staffLeave);
            if (!await VerifyStaffLeaveType(input.StaffLeaveTypeID, modelState)) return false;
            staffLeave.StaffLeaveTypeID = input.StaffLeaveTypeID;
            staffLeave.RequestStaffID = requestStaff.StaffID;
            staffLeave.TargetStaffID = targetStaff.StaffID;

            #region 请假天数算法
            var timeDifference = (input.EndDate - input.BeginDate).Days;
            if (timeDifference < 0)
            {
                modelState.AddModelError("BeginDate", "开始时间不能小于结束时间");
                return false;
            }
            double days;
            if (input.BeginDatePartial == 1 && input.EndDatePartial == 1)
            {
                days = timeDifference + 0.5;
            }
            else if (input.BeginDatePartial == 1 && input.EndDatePartial == 2)
            {
                days = timeDifference + 1;
            }
            else if (input.BeginDatePartial == 2 && input.EndDatePartial == 1)
            {
                if (timeDifference != 0)
                {
                    days = timeDifference + 0;
                }
                else
                {
                    modelState.AddModelError("BeginDate", "请输入正确的时间");
                    return false;
                }
            }
            else
            {
                days = timeDifference + 0.5;
            }
            if ((int)(days * 2) != input.HalfDays)
            {
                modelState.AddModelError("days", "计算的请假天数有误");
                return false;
            }
            staffLeave.HalfDays = (int)(days * 2);
            staffLeave.BeginDate = input.BeginDate;
            staffLeave.EndDate = input.EndDate;
            staffLeave.BeginDatePartial = input.BeginDatePartial;
            staffLeave.EndDatePartial = input.EndDatePartial;
            staffLeave.RequestRemark = input.RequestRemark;
            #endregion

            staffLeave.RequestAttachmentURL1 = input.RequestAttachmentURL1;
            staffLeave.RequestAttachmentURL2 = input.RequestAttachmentURL2;
            staffLeave.RequestAttachment1UploadTime = DateTime.Now;
            staffLeave.RequestAttachment2UploadTime = DateTime.Now;
            staffLeave.CreationDate = DateTime.Now;
            staffLeave.AuditStatus = StaffLeaveAuditStatus.OneLevelAudited;
            staffLeave.AuditStaffLevelCurrent = 1;
            staffLeave.AuditDate1 = DateTime.Now;

            #region 最大审核层级与审核层级设置

            var staffLeaveAuditFlow = await DbContext.StaffLeaveAuditFlows.FirstOrDefaultAsync(c => c.StaffLeaveAuditFlowID == requestStaff.User.GroupID);
            if (staffLeaveAuditFlow == null)
            {
                modelState.AddModelError("StaffLeaveAuditFlow", "站点未设置审批流程");
                return false;
            }
            int auditStaffLevelMax = 0;

            // 备注：这里假设了 AuditStaffLevelMaxDays4 如有值，一定比 AuditStaffLevelMaxDays3 大；AuditStaffLevelMaxDays3、AuditStaffLevelMaxDays2等类似。
            if (auditStaffLevelMax == 0 && staffLeaveAuditFlow.AuditStaffLevelMaxDays4 != null)
            {
                if (days > staffLeaveAuditFlow.AuditStaffLevelMaxDays4)
                {
                    modelState.AddModelError("AuditStaffLevelMax", "请假天数超出最大审核层级天数");
                    return false;
                }
            }
            if (auditStaffLevelMax == 0 && staffLeaveAuditFlow.AuditStaffLevelMaxDays3 != null)
            {
                if (days > staffLeaveAuditFlow.AuditStaffLevelMaxDays3)
                {
                    auditStaffLevelMax = 4;
                }
            }
            if (auditStaffLevelMax == 0 && staffLeaveAuditFlow.AuditStaffLevelMaxDays2 != null)
            {
                if (days > staffLeaveAuditFlow.AuditStaffLevelMaxDays2)
                {
                    auditStaffLevelMax = 3;
                }
            }
            if (auditStaffLevelMax == 0 && staffLeaveAuditFlow.AuditStaffLevelMaxDays1 != null)
            {
                if (days > staffLeaveAuditFlow.AuditStaffLevelMaxDays1)
                {
                    auditStaffLevelMax = 2;
                }
            }
            if (auditStaffLevelMax == 0)
            {

                if (days > 0)
                {
                    auditStaffLevelMax = 1;
                }
                else
                {
                    modelState.AddModelError("AuditStaffLevelMax", "站点设置审批流程有误");
                    return false;
                }

            }
            staffLeave.AuditStaffLevelMax = auditStaffLevelMax;

            //if (staffLeave.AuditStaffLevelCurrent == staffLeave.AuditStaffLevelMax)
            //    staffLeave.AuditStaffLevelCurrent = -1;
            var IsSet = false;
            #region 一级审核设置
            if (auditStaffLevelMax >= 1 && (days <= staffLeaveAuditFlow.AuditStaffLevelMaxDays1))
            {

                var AuditStaffID1 = await GetAuditStaffID1(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID1.HasValue) return false;
                staffLeave.AuditStaffID1 = AuditStaffID1.Value;

                IsSet = true;
            }
            #endregion
            #region 二级审核设置
            if (auditStaffLevelMax >= 2 && (days <= staffLeaveAuditFlow.AuditStaffLevelMaxDays2) && (days > staffLeaveAuditFlow.AuditStaffLevelMaxDays1))
            {

                var AuditStaffID1 = await GetAuditStaffID1(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID1.HasValue) return false;
                staffLeave.AuditStaffID1 = AuditStaffID1.Value;

                var AuditStaffID2 = await GetAuditStaffID2(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID2.HasValue) return false;
                staffLeave.AuditStaffID2 = AuditStaffID2.Value;

                IsSet = true;
            }
            #endregion
            #region 三级审核设置
            if (auditStaffLevelMax >= 3 && (days <= staffLeaveAuditFlow.AuditStaffLevelMaxDays3) && (days > staffLeaveAuditFlow.AuditStaffLevelMaxDays2))
            {

                var AuditStaffID1 = await GetAuditStaffID1(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID1.HasValue) return false;
                staffLeave.AuditStaffID1 = AuditStaffID1.Value;

                var AuditStaffID2 = await GetAuditStaffID2(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID2.HasValue) return false;
                staffLeave.AuditStaffID2 = AuditStaffID2.Value;

                var AuditStaffID3 = await GetAuditStaffID3(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID3.HasValue) return false;
                staffLeave.AuditStaffID3 = AuditStaffID3.Value;

                IsSet = true;
            }
            #endregion
            #region 四级审核设置
            if (auditStaffLevelMax == 4 && (days <= staffLeaveAuditFlow.AuditStaffLevelMaxDays4) && (days > staffLeaveAuditFlow.AuditStaffLevelMaxDays3))
            {

                var AuditStaffID1 = await GetAuditStaffID1(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID1.HasValue) return false;
                staffLeave.AuditStaffID1 = AuditStaffID1.Value;

                var AuditStaffID2 = await GetAuditStaffID2(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID2.HasValue) return false;
                staffLeave.AuditStaffID2 = AuditStaffID2.Value;

                var AuditStaffID3 = await GetAuditStaffID3(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID3.HasValue) return false;
                staffLeave.AuditStaffID3 = AuditStaffID3.Value;

                var AuditStaffID4 = await GetAuditStaffID4(staffLeaveAuditFlow, auditStaffLevelMax, modelState);
                if (!AuditStaffID4.HasValue) return false;
                staffLeave.AuditStaffID4 = AuditStaffID4.Value;

                IsSet = true;
            }
            #endregion
            if (!IsSet)
            {

                modelState.AddModelError("AuditStaffLevelMax", "请假天数超出最大审核层级天数");
                return false;

            }

            #endregion

            if (await DbContext.SaveChangesAsync() > 0) return true;

            modelState.AddModelError("", "服务器更新错误");
            return false;

        }

        //获取站级请假列表
        public async Task<Page<StaffLeaveInfoExtention>> GetStationLeaveList(XM.User currentUser, Guid groupID, bool isHR, StaffLeaveSearchCriteria criteria, PagingInfo pagingInfo)
        {

            var stations = await GetBelowCurrentGroupStations(groupID);
            List<Guid> stationIDs;
            if (!isHR)//不是HR需筛选流程配置表
            {
                List<Guid> AuditGroupID1s = null;
                List<Guid> AuditGroupID2s = null;
                List<Guid> AuditGroupID3s = null;
                List<Guid> AuditGroupID4s = null;
                var AuditGroup1s = await DbContext.StaffLeaveAuditFlows.Where(c => c.Group_AuditGroupID1 != null).
                    Select(c => c.Group_AuditGroupID1).ToListAsync();
                if (AuditGroup1s.Count != 0)
                {
                    AuditGroupID1s = AuditGroup1s.Where(c => true).Select(c => c.GroupID).ToList();
                }
                var AuditGroup2s = await DbContext.StaffLeaveAuditFlows.Where(c => c.Group_AuditGroupID2 != null).
                    Select(c => c.Group_AuditGroupID2).ToListAsync();
                if (AuditGroup2s.Count != 0)
                {
                    AuditGroupID2s = AuditGroup2s.Where(c => true).Select(c => c.GroupID).ToList();
                }
                var AuditGroup3s = await DbContext.StaffLeaveAuditFlows.Where(c => c.Group_AuditGroupID3 != null).
                    Select(c => c.Group_AuditGroupID3).ToListAsync();
                if (AuditGroup3s.Count != 0)
                {
                    AuditGroupID3s = AuditGroup3s.Where(c => true).Select(c => c.GroupID).ToList();
                }
                var AuditGroup4s = await DbContext.StaffLeaveAuditFlows.Where(c => c.Group_AuditGroupID4 != null).
                    Select(c => c.Group_AuditGroupID4).ToListAsync();
                if (AuditGroup4s.Count != 0)
                {
                    AuditGroupID4s = AuditGroup4s.Where(c => true).Select(c => c.GroupID).ToList();
                }

                //第一审核人为空，后面必为空
                if (AuditGroupID1s == null) { stationIDs = stations.Select(m => m.GroupID).ToList(); }
                else if (AuditGroupID2s == null)//第一审核人不为空，第二审核人为空，后面必为空...依次类推
                {
                    stationIDs = stations.Where(c => AuditGroupID1s.Contains(c.GroupID))
                                         .Select(m => m.GroupID).ToList();
                }
                else if (AuditGroupID3s == null)
                {
                    stationIDs = stations.Where(c => AuditGroupID1s.Contains(c.GroupID)
                              || AuditGroupID2s.Contains(c.GroupID))
                              .Select(m => m.GroupID).ToList();
                }
                else if (AuditGroupID4s == null)
                {
                    stationIDs = stations.Where(c => AuditGroupID1s.Contains(c.GroupID)
                              || AuditGroupID2s.Contains(c.GroupID)
                              || AuditGroupID3s.Contains(c.GroupID)
                               ).Select(m => m.GroupID).ToList();
                }
                else
                {
                    stationIDs = stations.Where(c => AuditGroupID1s.Contains(c.GroupID)
                              || AuditGroupID2s.Contains(c.GroupID)
                              || AuditGroupID3s.Contains(c.GroupID)
                              || AuditGroupID4s.Contains(c.GroupID)
                               ).Select(m => m.GroupID).ToList();
                }
            }
            else
                stationIDs = stations.Select(m => m.GroupID).ToList();

            IQueryable<StaffLeaveInfoExtention> list = DbContext.StaffLeaves
               .Where(m => stationIDs.Contains(m.TargetStaff.User.GroupID))
               .Select(_stationLeaveInfoSelector)
               ;

            if (criteria != null)
            {
                if (!criteria.Keyword.IsNullOrWhiteSpace())
                    list = list.Where(m => m.TargetStaff.Name.Contains(criteria.Keyword));
                //条件拼接

                if (isHR && criteria.StaffID.HasValue)
                {
                    list = list.Where(m => m.TargetStaff.StaffID == criteria.StaffID.Value);
                }

            }

            IOrderedQueryable<StaffLeaveInfoExtention> orderedQuery;
            if (pagingInfo.SortInfo != null && !pagingInfo.SortInfo.Sort.IsNullOrWhiteSpace())
            {
                orderedQuery = list.Order(pagingInfo.SortInfo.Sort, pagingInfo.SortInfo.SortDir == SortDir.DESC);
            }
            else
            {
                // 默认排序
                orderedQuery = list.OrderBy(m => m.StaffLeaveID);
            }

            var page = await orderedQuery.GetPageAsync(pagingInfo);
            foreach (var item in page.List)
            {

                if (item.AuditStaff1.StaffID == currentUser.UserInfo.UserID)
                    item.CurrentUserAudituditStaffLevel = 1;
                else if (item.AuditStaff2.StaffID == currentUser.UserInfo.UserID)
                    item.CurrentUserAudituditStaffLevel = 2;
                else if (item.AuditStaff3.StaffID == currentUser.UserInfo.UserID)
                    item.CurrentUserAudituditStaffLevel = 3;
                else if (item.AuditStaff4.StaffID == currentUser.UserInfo.UserID)
                    item.CurrentUserAudituditStaffLevel = 4;
            }
            return page;

        }

        //审核站级请假
        public async Task<bool> AuditStationLeave(List<AuditStationLeaveInput> input, LavieContext _context, ModelStateDictionary modelState)
        {

            if (input == null)
            {
                modelState.AddModelError("List<AuditStationLeaveInput>", "提交参数错误");
                return false;
            }
            List<StaffLeave> stationLeaveList = new List<StaffLeave>();
            foreach (var item in input)
            {
                var temp = await DbContext.StaffLeaves.FirstOrDefaultAsync(c => c.StaffLeaveID == item.StaffLeaveID);
                //审核只更新 AuditStatus AuditStaffLevelCurrent AuditDate AuditRemark
                //只校验了 当前用户是否在审批流上，不对请假条正确性再次校验
                #region 特殊性

                var AuditStaffID = _context.User.As<XM.User>().UserInfo.UserID;
                if (!(temp.AuditStaffID1 == AuditStaffID ||
                      temp.AuditStaffID2 == AuditStaffID ||
                      temp.AuditStaffID3 == AuditStaffID ||
                      temp.AuditStaffID4 == AuditStaffID))
                {
                    modelState.AddModelError("AuditStaffID", "当前请假条不在账户审核范围内");
                    return false;
                }

                #endregion
                #region 审核状态校验

                //if ((int)temp.AuditStatus == 0)
                //{
                //    if (item.AuditStatus == 1)
                //    {
                //        temp.AuditStatus = StaffLeaveAuditStatus.OneLevelAudited;
                //        temp.AuditStaffLevelCurrent = 1;
                //        temp.AuditDate1 = DateTime.Now;
                //    }
                //    else if (item.AuditStatus == 2)
                //    {
                //        temp.AuditStatus = StaffLeaveAuditStatus.OneLevelDeny;
                //        temp.AuditStaffLevelCurrent = 1;
                //        temp.AuditDate1 = DateTime.Now;
                //    }
                //    else
                //    {
                //        modelState.AddModelError("AuditStatus", "审核状态不正确");
                //    }
                //}
                if ((int)temp.AuditStatus == 1)
                {
                    if (item.AuditStatus == 1)
                    {
                        temp.AuditStatus = StaffLeaveAuditStatus.TwoLevelAudited;
                        temp.AuditStaffLevelCurrent = 2;
                        temp.AuditDate2 = DateTime.Now;
                    }
                    else if (item.AuditStatus == 2)
                    {
                        temp.AuditStatus = StaffLeaveAuditStatus.TwoLevelDeny;
                        temp.AuditStaffLevelCurrent = 2;
                        temp.AuditDate2 = DateTime.Now;
                    }
                    else
                    {
                        modelState.AddModelError("AuditStatus", "审核状态不正确");
                        return false;
                    }
                }
                else if ((int)temp.AuditStatus == 3)
                {
                    if (item.AuditStatus == 1)
                    {
                        temp.AuditStatus = StaffLeaveAuditStatus.ThreeLevelAudited;
                        temp.AuditStaffLevelCurrent = 3;
                        temp.AuditDate3 = DateTime.Now;
                    }
                    else if (item.AuditStatus == 2)
                    {
                        temp.AuditStatus = StaffLeaveAuditStatus.ThreeLevelDeny;
                        temp.AuditStaffLevelCurrent = 3;
                        temp.AuditDate3 = DateTime.Now;
                    }
                    else
                    {
                        modelState.AddModelError("AuditStatus", "审核状态不正确");
                        return false;
                    }
                }
                else if ((int)temp.AuditStatus == 5)
                {
                    if (item.AuditStatus == 1)
                    {
                        temp.AuditStatus = StaffLeaveAuditStatus.FourLevelAudited;
                        temp.AuditStaffLevelCurrent = 4;
                        temp.AuditDate4 = DateTime.Now;
                    }
                    else if (item.AuditStatus == 2)
                    {
                        temp.AuditStatus = StaffLeaveAuditStatus.FourLevelDeny;
                        temp.AuditStaffLevelCurrent = 4;
                        temp.AuditDate4 = DateTime.Now;
                    }
                    else
                    {
                        modelState.AddModelError("AuditStatus", "审核状态不正确");
                        return false;
                    }
                }
                else
                {
                    modelState.AddModelError("AuditStatus", "审核状态不正确");
                    return false;
                }

                #endregion
                #region 审核备注更新

                if ((int)item.AuditStatus == 1 || (int)item.AuditStatus == 2)
                    temp.AuditRemark1 = item.AuditRemark;
                else if ((int)item.AuditStatus == 3 || (int)item.AuditStatus == 4)
                    temp.AuditRemark2 = item.AuditRemark;
                else if ((int)item.AuditStatus == 5 || (int)item.AuditStatus == 6)
                    temp.AuditRemark3 = item.AuditRemark;
                else if ((int)item.AuditStatus == 7 || (int)item.AuditStatus == 8)
                    temp.AuditRemark4 = item.AuditRemark;

                #endregion

            }
            if (await DbContext.SaveChangesAsync() > 0) { return true; }
            return false;

        }

        //获取请假流程
        public async Task<List<StaffLeaveAuditFlowBase>> GetStaffLeaveAuditFlowList()
        {

            List<StaffLeaveAuditFlowBase> staffLeaveAuditFlows = await GetListFromCacheAsync();
            if (!staffLeaveAuditFlows.IsNullOrEmpty())
                return staffLeaveAuditFlows;
            else
                return await GetStaffLeaveAuditFlowAsync();

        }

        //配置用户组请假审核流程
        public async Task<bool> ConfigStaffLeaveAuditFlow(StaffLeaveAuditFlowInput input, ModelStateDictionary modelState)
        {

            #region input 有效性校验

            #region 必须是站级才能配置

            var group = await _groupService.GetItemAsync(input.StaffLeaveAuditFlowID);
            if (group == null) { modelState.AddModelError("StaffLeaveAuditFlowID", "StaffLeaveAuditFlowID值非法"); return false; }
            var role = await DbContext.Roles.FirstOrDefaultAsync(c => c.RoleID == StationManager);
            if (group.Level != 5 || !group.LimitRoles.Any(c => c.RoleID == StationManager)) { modelState.AddModelError("staff", "必须是站级才能配置"); return false; }

            #endregion

            //用户组请假审核流程 级联groupID
            if (!await VerifyGroup(input.StaffLeaveAuditFlowID, modelState)) return false;
            //请求角色 级联roleID
            if (!await VerifyRole(input.RequestRoleID, modelState)) return false;
            //4级审核流用户组ID，要求1、高级审核流存在的情况下，低级审核流不为空2、级联用户组3、AuditGroupID、AuditRoleID、AuditStaffLevelMaxDays三个表需匹配4、检查三种字段的有效性
            if (!await VerifyStaffLeaveAuditFlow(input, modelState)) return false;

            #endregion

            #region Update

            var inputToSave = await DbContext.StaffLeaveAuditFlows.FirstOrDefaultAsync(c => c.StaffLeaveAuditFlowID == input.StaffLeaveAuditFlowID);

            if (inputToSave == null)
            {
                inputToSave = DbContext.StaffLeaveAuditFlows.Create();
                inputToSave.StaffLeaveAuditFlowID = input.StaffLeaveAuditFlowID;
                DbContext.StaffLeaveAuditFlows.Add(inputToSave);
            }

            inputToSave.RequestRoleID = input.RequestRoleID;

            inputToSave.AuditGroupID1 = input.AuditGroupID1;
            inputToSave.AuditGroupID2 = input.AuditGroupID2;
            inputToSave.AuditGroupID3 = input.AuditGroupID3;
            inputToSave.AuditGroupID4 = input.AuditGroupID4;

            inputToSave.AuditRoleID1 = input.AuditRoleID1;
            inputToSave.AuditRoleID2 = input.AuditRoleID2;
            inputToSave.AuditRoleID3 = input.AuditRoleID3;
            inputToSave.AuditRoleID4 = input.AuditRoleID4;

            inputToSave.AuditStaffLevelMaxDays1 = input.AuditStaffLevelMaxDays1;
            inputToSave.AuditStaffLevelMaxDays2 = input.AuditStaffLevelMaxDays2;
            inputToSave.AuditStaffLevelMaxDays3 = input.AuditStaffLevelMaxDays3;
            inputToSave.AuditStaffLevelMaxDays4 = input.AuditStaffLevelMaxDays4;

            #endregion

            if (await DbContext.SaveChangesAsync() > 0)
            {
                _cache.Invalidate(GroupListCacheKey);
            };

            return true;

        }

        #region Private Methods

        private async Task<List<StaffLeaveAuditFlowBase>> GetListFromCacheAsync()
        {
            if (_cache != null)
                return await _cache.GetItemAsync<List<StaffLeaveAuditFlowBase>>(
                    GroupListCacheKey,
                    async () => await GetStaffLeaveAuditFlowAsync(),
                    TimeSpan.FromDays(1)
                    );
            else
                return null;
        }
        private async Task<List<StaffLeaveAuditFlowBase>> GetStaffLeaveAuditFlowAsync()
        {

            return await DbContext.StaffLeaveAuditFlows.Select(_staffLeaveAuditFlowSelector).ToListAsync();

        }
        private async Task<int?> GetAuditStaffID1(StaffLeaveAuditFlow staffLeaveAuditFlow, int auditStaffLevelMax, ModelStateDictionary modelState)
        {

            if (!(staffLeaveAuditFlow.AuditGroupID1.HasValue && staffLeaveAuditFlow.AuditRoleID1.HasValue))
            {
                modelState.AddModelError("AuditGroupID1", "站点设置审批流程有误");
                return null;
            }
            return (await DbContext.Staffs.FirstOrDefaultAsync(c => c.User.GroupID == staffLeaveAuditFlow.AuditGroupID1 && c.User.Roles.FirstOrDefault().RoleID == staffLeaveAuditFlow.AuditRoleID1)).StaffID;

        }
        private async Task<int?> GetAuditStaffID2(StaffLeaveAuditFlow staffLeaveAuditFlow, int auditStaffLevelMax, ModelStateDictionary modelState)
        {

            if (!(staffLeaveAuditFlow.AuditGroupID2.HasValue && staffLeaveAuditFlow.AuditRoleID2.HasValue))
            {
                modelState.AddModelError("AuditGroupID2", "站点设置审批流程有误");
                return null;
            }
            var staffAudit2 = await DbContext.Staffs.FirstOrDefaultAsync(c => c.User.GroupID == staffLeaveAuditFlow.AuditGroupID2 && c.User.Roles.FirstOrDefault().RoleID == staffLeaveAuditFlow.AuditRoleID2);
            if (staffAudit2 == null)
            {

                modelState.AddModelError("AuditGroupID2", "该2级审核员对应职位不符合要求");
                return null;

            }
            return staffAudit2.StaffID;
        }
        private async Task<int?> GetAuditStaffID3(StaffLeaveAuditFlow staffLeaveAuditFlow, int auditStaffLevelMax, ModelStateDictionary modelState)
        {

            if (!(staffLeaveAuditFlow.AuditGroupID3.HasValue && staffLeaveAuditFlow.AuditRoleID3.HasValue))
            {
                modelState.AddModelError("AuditGroupID3", "站点设置审批流程有误");
                return null;
            }
            var staffAudit3 = await DbContext.Staffs.FirstOrDefaultAsync(c => c.User.GroupID == staffLeaveAuditFlow.AuditGroupID3 && c.User.Roles.FirstOrDefault().RoleID == staffLeaveAuditFlow.AuditRoleID3);
            if (staffAudit3 == null)
            {

                modelState.AddModelError("AuditGroupID3", "该3级审核员对应职位不符合要求");
                return null;

            }
            return staffAudit3.StaffID;
        }
        private async Task<int?> GetAuditStaffID4(StaffLeaveAuditFlow staffLeaveAuditFlow, int auditStaffLevelMax, ModelStateDictionary modelState)
        {

            if (!(staffLeaveAuditFlow.AuditGroupID4.HasValue && staffLeaveAuditFlow.AuditRoleID4.HasValue))
            {
                modelState.AddModelError("AuditGroupID4", "站点设置审批流程有误");
                return null;
            }
            var staffAudit4 = await DbContext.Staffs.FirstOrDefaultAsync(c => c.User.GroupID == staffLeaveAuditFlow.AuditGroupID4 && c.User.Roles.FirstOrDefault().RoleID == staffLeaveAuditFlow.AuditRoleID4);
            if (staffAudit4 == null)
            {

                modelState.AddModelError("AuditGroupID4", "该4级审核员对应职位不符合要求");
                return null;

            }
            return staffAudit4.StaffID;
        }
        private async Task<List<GroupBaseInfo>> GetBelowCurrentGroupStations(Guid groupID)
        {

            var group = await _groupService.GetItemAsync(groupID);
            if (group.LimitRoles.Any(c => c.RoleID == CityManager || c.RoleID == AreaManager))
            {
                groupID = group.ParentID ?? Guid.Empty;
            }
            var groups = await _groupService.GetListAsync(groupID);
            List<GroupBaseInfo> stations = new List<GroupBaseInfo>();
            foreach (var item in groups)
            {
                if (item.Level == 5)
                    stations.Add(
                        new GroupBaseInfo
                        {
                            GroupID = item.GroupID,
                            Name = item.Name
                        });
            }
            return stations;
        }
        private async Task<bool> VerifyStaffLeaveType(int StaffLeaveType, ModelStateDictionary modelState)
        {
            var result = await DbContext.StaffLeaveTypes.AnyAsync(c => c.StaffLeaveTypeID == StaffLeaveType);
            if (result) return true;
            modelState.AddModelError("StaffLeaveTypeID", "请假类型错误");
            return false;
        }
        private async Task<bool> VerifyGroup(Guid GroupID, ModelStateDictionary modelState)
        {

            var result = await DbContext.Groups.AnyAsync(c => c.GroupID == GroupID);
            if (result) return true;
            modelState.AddModelError("GroupID", "GroupID非法");
            return false;

        }
        private async Task<bool> VerifyRole(Guid RoleID, ModelStateDictionary modelState)
        {

            var result = await DbContext.Roles.AnyAsync(c => c.RoleID == RoleID);
            if (result) return true;
            modelState.AddModelError("RoleID", "RoleID非法");
            return false;

        }
        private async Task<bool> VerifyStaffLeaveAuditFlow(StaffLeaveAuditFlowInput input, ModelStateDictionary modelState)
        {

            var flag = 0;
            if (!input.AuditGroupID1.IsNullOrEmpty() &&
               !input.AuditRoleID1.IsNullOrEmpty() &&
               input.AuditStaffLevelMaxDays1.HasValue
                )
            {
                if (!await VerifyGroup(input.AuditGroupID1.Value, modelState)) return false;
                if (!await VerifyRole(input.AuditRoleID1.Value, modelState)) return false;
                flag = 1;
            }
            if (!input.AuditGroupID2.IsNullOrEmpty() &&
               !input.AuditRoleID2.IsNullOrEmpty() &&
               input.AuditStaffLevelMaxDays2.HasValue
                )
            {
                #region 审批流中高级配置不能完全等于低级配置

                if (input.AuditGroupID2.Value == input.AuditGroupID1
                    &&
                    input.AuditRoleID2.Value == input.AuditRoleID1
                    )
                { modelState.AddModelError("Same", "审批流中高级配置不能完全等于低级配置"); return false; }

                #endregion

                if (!await VerifyGroup(input.AuditGroupID2.Value, modelState)) return false;
                if (!await VerifyRole(input.AuditRoleID2.Value, modelState)) return false;
                if (input.AuditStaffLevelMaxDays1 >= input.AuditStaffLevelMaxDays2) { modelState.AddModelError("AuditStaffLevelMaxDays", "低级最大审核天数不能大于或等于高级最大审核天数"); return false; }

                if (flag != 1) { modelState.AddModelError("AuditFlow", "高级审核有值情况下，低级审核无值"); return false; }
                flag = 2;
            }
            if (!input.AuditGroupID3.IsNullOrEmpty() &&
               !input.AuditRoleID3.IsNullOrEmpty() &&
               input.AuditStaffLevelMaxDays3.HasValue
                )
            {
                #region 审批流中高级配置不能完全等于低级配置

                if (input.AuditGroupID3.Value == input.AuditGroupID1
                   &&
                   input.AuditRoleID3.Value == input.AuditRoleID1
                   )
                { modelState.AddModelError("Same", "审批流中高级配置不能完全等于低级配置"); return false; }
                if (input.AuditGroupID3.Value == input.AuditGroupID2
                   &&
                   input.AuditRoleID3.Value == input.AuditRoleID2
                   )
                { modelState.AddModelError("Same", "审批流中高级配置不能完全等于低级配置"); return false; }

                #endregion

                if (!await VerifyGroup(input.AuditGroupID3.Value, modelState)) return false;
                if (!await VerifyRole(input.AuditRoleID3.Value, modelState)) return false;
                if (input.AuditStaffLevelMaxDays2 >= input.AuditStaffLevelMaxDays3) { modelState.AddModelError("AuditStaffLevelMaxDays", "低级最大审核天数不能大于或等于高级最大审核天数"); return false; }

                if (flag != 2) { modelState.AddModelError("AuditFlow", "高级审核有值情况下，低级审核无值"); return false; }
                flag = 3;
            }
            if (!input.AuditGroupID4.IsNullOrEmpty() &&
               !input.AuditRoleID4.IsNullOrEmpty() &&
               input.AuditStaffLevelMaxDays4.HasValue
                )
            {

                #region 审批流中高级配置不能完全等于低级配置

                if (input.AuditGroupID4.Value == input.AuditGroupID1
                   &&
                   input.AuditRoleID4.Value == input.AuditRoleID1
                   )
                { modelState.AddModelError("Same", "审批流中高级配置不能完全等于低级配置"); return false; }
                if (input.AuditGroupID4.Value == input.AuditGroupID2
                   &&
                   input.AuditRoleID4.Value == input.AuditRoleID2
                   )
                { modelState.AddModelError("Same", "审批流中高级配置不能完全等于低级配置"); return false; }
                if (input.AuditGroupID4.Value == input.AuditGroupID3
                   &&
                   input.AuditRoleID4.Value == input.AuditRoleID3
                   )
                { modelState.AddModelError("Same", "审批流中高级配置不能完全等于低级配置"); return false; }


                #endregion

                if (!await VerifyGroup(input.AuditGroupID4.Value, modelState)) return false;
                if (!await VerifyRole(input.AuditRoleID4.Value, modelState)) return false;
                if (input.AuditStaffLevelMaxDays3 >= input.AuditStaffLevelMaxDays4) { modelState.AddModelError("AuditStaffLevelMaxDays", "低级最大审核天数不能大于或等于高级最大审核天数"); return false; }

                if (flag != 3) { modelState.AddModelError("AuditFlow", "高级审核有值情况下，低级审核无值"); return false; }
                flag = 4;
            }

            if (flag == 0) { modelState.AddModelError("AuditFlow", "审核流级别配置必须对应有值"); return false; }
            return true;

        }

        #endregion

    }




}
