using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Data.Entity;
using Lavie.Models;
using Lavie.Extensions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.TenPayLibV3;
using Lavie.Modules.Admin.Repositories;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Models;
using System.Globalization;
using Lavie.Modules.Admin.Services;
using System.Net.Http;
using Lavie.Extensions.Object;
using Lavie.Infrastructure;
using System.Threading;
using System.Data.SqlClient;
using Microsoft.VisualStudio.Threading;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using XM = Lavie.Modules.Project.Models;
using XHM = Lavie.Modules.Admin.Models;
using System.IO;
using Lavie.Modules.Project.Models;

namespace Lavie.Modules.Project.Repositories
{
    public partial interface IStaffRepository
    {
        #region Staff

        Task<XM.StaffDictionary> GetDictionaryList();
        Task<List<XM.EducationInfo>> GetEducationInfoBaseList();
        Task<List<XM.RecruitChannelInfo>> GetRecruitChannelInfoBaseList();
        Task<List<XM.JobNatureInfo>> GetJobNatureInfoBaseList();
        Task<List<XM.BankTypeInfo>> GetBankTypeInfoBaseList();
        Task<List<XM.ProtocolTimeInfo>> GetProtocolTimeInfoBaseList();
        Task<List<XM.ProtocolTypeInfo>> GetProtocolTypeInfoBaseList();
        Task<List<XM.InsuranceNatureInfo>> GetInsuranceNatureInfoBaseList();
        Task<List<XM.RiderJobTypeInfo>> GetRiderJobTypeInfoBaseList();
        Task<List<XM.GroupInfoBase>> HRGetGroupInfoBaseList(Guid groupID);
        Task<List<XM.ExitReasonInfoBase>> GetExitReasonInfoBaseList();
        Task<List<XM.StaffTurnoverTypeInfoBase>> GetStaffTurnoverTypeInfoBaseList();
        Task<List<XM.StaffLeaveTypeInfoBase>> GetStaffLeaveTypeInfoBaseList();

        Task<Page<XM.StaffInfoBaseList>> GetStaffInfoBaseList(XM.GetStaffInfoBaseListPar par, List<Guid> Groups, PagingInfo pagingInfo);
        Task<Page<XM.StaffBaseList>> GetStaffBaseList(XM.GetStaffInfoListPar par, List<Guid> Groups, PagingInfo pagingInfo);

        Task<XM.StaffInfo> GetStaffInfo(XM.GetStaffInfoPar par);
        Task<bool> AddStaffInfo(XHM.User currentUser, XM.AddStaffInfoPar par, XM.UserAddPar user, ModelStateDictionary modelState);
        Task<XM.StaffInfoBaseList> EditStaffInfo(XM.EditStaffInfoPar par, XM.UserEditPar user, ModelStateDictionary modelState);
        Task<bool> Entry(XM.EntryPar par, LavieContext _context, ModelStateDictionary modelState);
        Task<bool> EditRiderReferrer(XM.RiderReferrePar par, ModelStateDictionary modelState);
        Task<bool> EditRecruitChannel(XM.RecruitChannelInput input, ModelStateDictionary modelState);
        Task<bool> EditBank(XM.EditBankInput input, ModelStateDictionary modelState);
        Task<bool> EditInsurance(XM.EditInsuranceInput input, ModelStateDictionary modelState);
        Task<bool> EditRiderEleID(XM.EditRiderEleIDInput input, ModelStateDictionary modelState);
        Task<bool> Exit(XM.ExitInput input, LavieContext _context, ModelStateDictionary modelState);
        Task<bool> InBlackList(XM.InBlackListInput input, ModelStateDictionary modelState);
        Task<bool> Turnover(XM.TurnoverInput input, LavieContext _context, ModelStateDictionary modelState);
        Task<bool> ChangeRiderJobType(XM.ChangeRiderJobTypeInput input, LavieContext _context, ModelStateDictionary modelState);

        Task<bool> GetHrGroupPermitAsync(Guid currentGroupID, Guid groupID);
        Task<bool> IsExistsIDCardNOAsync(string IDCardNO);
        Task<bool> VerifyExistsIDCardNOAsync(int staffID, string IDCardNO);
        Task<List<XM.StaffStatusInfoBase>> GetStaffStatus();

        Task<object> GetStation();

        #endregion
    }

    public partial class StaffRepository : RepositoryBase, IStaffRepository
    {

        public readonly int stationLevel = 5;
        private readonly Expression<Func<Staff, XM.StaffInfoBaseList>> _staffInfoBaseListSelector;
        private readonly Expression<Func<Staff, XM.StaffBaseList>> _staffBaseListSelector;//需扩展字段
        private readonly Expression<Func<Staff, XM.StaffInfo>> _staffInfoSelector;//需扩展字段

        private readonly IGroupRepository _groupRepository;
        public StaffRepository(IGroupRepository groupRepository)
        {

            _staffInfoBaseListSelector = c => new XM.StaffInfoBaseList
            {
                StaffID = c.StaffID,
                Name = c.Name,
                Groups = new GroupInfo
                {
                    GroupID = c.User.Group.GroupID,
                    Name = c.User.Group.Name
                },
                Role = (from a in c.User.Roles
                        select new RoleBase
                        {
                            RoleID = a.RoleID,
                            Name = a.Name,
                            IsSystem = a.IsSystem,
                            DisplayOrder = a.DisplayOrder
                        }).FirstOrDefault(),
                StaffStatus = new XM.StaffStatusInfoBase
                {
                    StaffStatusID = c.StaffStatusID,
                    Name = c.StaffStatu.Name
                }
            };
            _staffBaseListSelector = c => new XM.StaffBaseList
            {
                StaffID = c.StaffID,
                Name = c.Name,
                Groups = new GroupInfo
                {
                    GroupID = c.User.Group.GroupID,
                    Name = c.User.Group.Name
                },
                Role = (from a in c.User.Roles
                        select new RoleBase
                        {
                            RoleID = a.RoleID,
                            Name = a.Name,
                            IsSystem = a.IsSystem,
                            DisplayOrder = a.DisplayOrder
                        }).FirstOrDefault(),
                StaffStatus = new XM.StaffStatusInfoBase
                {
                    StaffStatusID = c.StaffStatusID,
                    Name = c.StaffStatu.Name
                },
                Sex = c.Sex.Value,
                Age = c.Age.Value,
                Birthday = c.Birthday.Value,
                StaffMobile = c.StaffMobile,
                MaritalStatus = c.MaritalStatus,
                IDCardNO = c.IDCardNO,
                Education = new XM.EducationInfo
                {
                    EducationID = c.EducationID ?? 0,
                    Name = c.EducationID.HasValue ? c.Education.Name : null
                },
                School = c.School,
                Major = c.Major,
                Household = c.Household,
                Residence = c.Residence,
                EmergencyContact = c.EmergencyContact,
                EmergencyContactRelationship = c.EmergencyContactRelationship,
                EmergencyContactMobile = c.EmergencyContactMobile,
                RiderJobType = new XM.RiderJobTypeInfo
                {
                    RiderJobTypeID = c.RiderJobTypeID ?? 0,
                    Name = c.RiderJobTypeID.HasValue ? c.RiderJobType.Name : null
                },
                User = new XM.UserBase
                {
                    UserID = c.User.UserID,
                    Status = c.User.Status,
                    Username = c.User.Username,
                    DisplayName = c.User.DisplayName,
                    HeadURL = c.User.HeadURL,
                    LogoURL = c.User.LogoURL,
                    RealName = c.User.RealName,
                    RealNameIsValid = c.User.RealNameIsValid,
                    Email = c.User.Email,
                    EmailIsValid = c.User.EmailIsValid,
                    Mobile = c.User.Mobile,
                    MobileIsValid = c.User.MobileIsValid,
                    Description = c.User.Description
                },
                EntryRemark = c.EntryRemark,
                WorkNO = c.WorkNO,
                RecruitChannel = new XM.RecruitChannelBaseInfo
                {
                    RecruitChannelID = c.RecruitChannelID ?? 0,
                    Name = c.RecruitChannelID.HasValue ? c.RecruitChannel.Name : null
                },
                RiderReferrerStaff = new StaffInfoBase
                {
                    Name = c.RiderReferrerStaffID.HasValue ? c.RiderReferrerStaff.Name : null,
                    StaffID = c.RiderReferrerStaffID ?? 0
                },
                RiderReferrerDate = c.RiderReferrerDate,
                RiderReferrerAttachmentURL = c.RiderReferrerAttachmentURL,
                JobNature = new XM.JobNatureInfo
                {

                    JobNatureID = c.JobNatureID.HasValue ? c.JobNatureID : 0,
                    Name = c.JobNature.Name
                },
                ProtocolNO = c.ProtocolNO,
                ProtocolTime = new XM.ProtocolTimeBaseInfo
                {
                    ProtocolTimeID = c.ProtocolTimeID.HasValue ? c.ProtocolTimeID : 0,
                    Name = c.ProtocolTimeID.HasValue ? c.ProtocolTime.Name : null
                },
                ProtocolType = new XM.ProtocolTypeInfo
                {
                    ProtocolTypeID = c.ProtocolTypeID ?? 0,
                    Name = c.ProtocolTypeID.HasValue ? c.ProtocolType.Name : null
                },
                ProtocolSignedDate = c.ProtocolSignedDate,
                ProtocolBeginDate = c.ProtocolBeginDate,
                ProtocolEndDate = c.ProtocolEndDate,
                InsuranceNature = new XM.InsuranceNatureBaseInfo
                {
                    InsuranceNatureID = c.InsuranceNatureID ?? 0,
                    Name = c.InsuranceNatureID.HasValue ? c.InsuranceNature.Name : null
                },
                InsuranceStartBuyDate = c.InsuranceStartBuyDate,
                InsuranceStopBuyDate = c.InsuranceStopBuyDate,
                BankType = new XM.BankTypeInfo
                {

                    BankTypeID = c.BankTypeID.HasValue ? c.BankTypeID : 0,
                    Name = c.BankTypeID.HasValue ? c.BankType.Name : null
                },
                BankNO = c.BankNO,
                ExitReason = new XM.ExitReasonInfoBase
                {
                    ExitReasonID = c.ExitReasonID,
                    Name = c.Name
                },
                IsInBlackList = c.IsInBlackList,
                RiderEleID = c.RiderEleID,
                ExitRemark = c.ExitRemark,
                RiderReferrerRemark = c.RiderReferrerRemark

            };
            _staffInfoSelector = c => new XM.StaffInfo
            {
                StaffID = c.StaffID,
                Name = c.Name,
                Group = new XM.GroupBaseInfo
                {
                    GroupID = c.User.Group.GroupID,
                    Name = c.User.Group.Name
                },
                Role = (from a in c.User.Roles
                        select new XM.RoleBaseInfo
                        {
                            RoleID = a.RoleID,
                            Name = a.Name,
                        }).FirstOrDefault(),
                RecruitChannel = new XM.RecruitChannelBaseInfo
                {
                    RecruitChannelID = c.RecruitChannelID ?? 0,
                    Name = c.RecruitChannelID.HasValue ? c.RecruitChannel.Name : null
                },
                Sex = c.Sex,
                StaffMobile = c.StaffMobile,
                EntryDate = c.EntryDate,
                RiderJobType = new XM.RiderJobTypeInfo
                {
                    RiderJobTypeID = c.RiderJobTypeID.HasValue ? c.RiderJobType.RiderJobTypeID : 0,
                    Name = c.RiderJobTypeID.HasValue ? c.RiderJobType.Name : null
                },
                RiderLastWorkDate = c.RiderLastWorkDate,
                Age = c.Age,
                MaritalStatus = c.MaritalStatus,
                BirthDay = c.Birthday,
                BankNO = c.BankNO,
                BankType = new XM.BankTypeInfo
                {
                    BankTypeID = c.BankTypeID.HasValue ? c.BankType.BankTypeID : 0,
                    Name = c.BankTypeID.HasValue ? c.BankType.Name : null
                },
                Education = new XM.EducationInfo
                {
                    EducationID = c.EducationID.HasValue ? c.Education.EducationID : 0,
                    Name = c.EducationID.HasValue ? c.Education.Name : null
                },
                School = c.School,
                Major = c.Major,
                IDCardNO = c.IDCardNO,
                Household = c.Household,
                Residence = c.Residence,
                EmergencyContact = c.EmergencyContact,
                EmergencyContactRelationship = c.EmergencyContactRelationship,
                EmergencyContactMobile = c.EmergencyContactMobile,
                JobNature = new XM.JobNatureInfo
                {
                    JobNatureID = c.JobNatureID.HasValue ? c.JobNature.JobNatureID : 0,
                    Name = c.JobNatureID.HasValue ? c.JobNature.Name : null
                },
                ProtocolNO = c.ProtocolNO,
                ProtocolTime = new XM.ProtocolTimeBaseInfo
                {
                    ProtocolTimeID = c.ProtocolTimeID.HasValue ? c.ProtocolTime.ProtocolTimeID : 0,
                    Name = c.ProtocolTimeID.HasValue ? c.ProtocolTime.Name : null
                },
                ProtocolType = new XM.ProtocolType
                {
                    ProtocolTypeID = c.ProtocolTypeID.HasValue ? c.ProtocolType.ProtocolTypeID : 0,
                    Name = c.ProtocolTypeID.HasValue ? c.ProtocolType.Name : null
                },
                ProtocolSignedDate = c.ProtocolSignedDate,
                ProtocolBeginDate = c.ProtocolBeginDate,
                ProtocolEndDate = c.ProtocolEndDate,
                InsuranceNature = new XM.InsuranceNatureBaseInfo
                {
                    InsuranceNatureID = c.InsuranceNatureID.HasValue ? c.InsuranceNature.InsuranceNatureID : 0,
                    Name = c.InsuranceNatureID.HasValue ? c.InsuranceNature.Name : null
                },
                StaffStatus = new XM.StaffStatusBaseInfo
                {
                    StaffStatusID = c.StaffStatu.StaffStatusID,
                    Name = c.StaffStatu.Name
                },
                InsuranceStartBuyDate = c.InsuranceStartBuyDate,
                InsuranceStopBuyDate = c.InsuranceStopBuyDate,
                EntryRemark = c.EntryRemark,
                ExitDate = c.ExitDate,
                ExitReason = new XM.ExitReasonBaseInfo
                {
                    ExitReasonID = c.ExitReasonID.HasValue ? c.ExitReason.ExitReasonID : 0,
                    Name = c.ExitReasonID.HasValue ? c.ExitReason.Name : null,
                },
                ExitRemark = c.ExitRemark
            };
            _groupRepository = groupRepository;

        }


        #region 法思特OA Api

        #region 字典

        //获取员工添加编辑需要使用的数据字典
        public async Task<XM.StaffDictionary> GetDictionaryList()
        {
            var staffDictionary = new XM.StaffDictionary();
            var userStatus = typeof(UserStatus).GetEnumDictionary<UserStatus>().Select(m => new XM.UserStatusInfoBase
            {
                UserStatus = m.Key,
                Name = m.Value,
            }).ToList();

            staffDictionary.UserStatus = userStatus;
            staffDictionary.ProtocolTimes = await GetProtocolTimeInfoBaseList();
            staffDictionary.ProtocolTypes = await GetProtocolTypeInfoBaseList();
            staffDictionary.JobNatures = await GetJobNatureInfoBaseList();
            staffDictionary.RecruitChannels = await GetRecruitChannelInfoBaseList();
            staffDictionary.InsuranceNatures = await GetInsuranceNatureInfoBaseList();
            staffDictionary.RiderJobTypes = await GetRiderJobTypeInfoBaseList();
            staffDictionary.Educations = await GetEducationInfoBaseList();
            staffDictionary.BankTypes = await GetBankTypeInfoBaseList();

            return staffDictionary;
        }
        //获取学历列表
        public async Task<List<XM.EducationInfo>> GetEducationInfoBaseList()
        {
            return await DbContext.Set<Education>()
                .Where(c => !c.IsDisabled)
                .OrderBy(c => c.EducationID)
                .Select(c => new XM.EducationInfo
                {
                    EducationID = c.EducationID,
                    Name = c.Name
                })
                .ToListAsync();
        }
        //获取招聘渠道列表
        public async Task<List<XM.RecruitChannelInfo>> GetRecruitChannelInfoBaseList()
        {
            return await DbContext.Set<RecruitChannel>()
                .Where(c => !c.IsDisabled)
                .OrderBy(c => c.RecruitChannelID)
                .Select(c => new XM.RecruitChannelInfo
                {
                    RecruitChannelID = c.RecruitChannelID,
                    Name = c.Name
                })
                .ToListAsync();
        }
        //获取用工性质列表 
        public async Task<List<XM.JobNatureInfo>> GetJobNatureInfoBaseList()
        {
            return await DbContext.Set<JobNature>()
                .Where(c => !c.IsDisabled)
                .OrderBy(c => c.JobNatureID)
                .Select(c => new XM.JobNatureInfo
                {
                    JobNatureID = c.JobNatureID,
                    Name = c.Name
                })
                .ToListAsync();
        }
        //获取合同期次列表
        public async Task<List<XM.ProtocolTimeInfo>> GetProtocolTimeInfoBaseList()
        {
            return await DbContext.Set<ProtocolTime>()
                .Where(c => !c.IsDisabled)
                .OrderBy(c => c.ProtocolTimeID)
                .Select(c => new XM.ProtocolTimeInfo
                {
                    ProtocolTimeID = c.ProtocolTimeID,
                    Name = c.Name
                })
                .ToListAsync();
        }
        //获取合同类型列表
        public async Task<List<XM.ProtocolTypeInfo>> GetProtocolTypeInfoBaseList()
        {
            return await DbContext.Set<ProtocolType>()
                .Where(c => !c.IsDisabled)
                .OrderBy(c => c.ProtocolTypeID)
                .Select(c => new XM.ProtocolTypeInfo
                {
                    ProtocolTypeID = c.ProtocolTypeID,
                    Name = c.Name
                })
                .ToListAsync();
        }
        //获取银行类型列表
        public async Task<List<XM.BankTypeInfo>> GetBankTypeInfoBaseList()
        {
            return await DbContext.Set<BankType>()
                .Where(c => !c.IsDisabled)
                .OrderBy(c => c.BankTypeID)
                .Select(c => new XM.BankTypeInfo
                {
                    BankTypeID = c.BankTypeID,
                    Name = c.Name
                })
                .ToListAsync();
        }
        //获取保险性质列表
        public async Task<List<XM.InsuranceNatureInfo>> GetInsuranceNatureInfoBaseList()
        {
            return await DbContext.Set<InsuranceNature>()
                .Where(c => !c.IsDisabled)
                .OrderBy(c => c.InsuranceNatureID)
                .Select(c => new XM.InsuranceNatureInfo
                {
                    InsuranceNatureID = c.InsuranceNatureID,
                    Name = c.Name
                })
                .ToListAsync();
        }
        //获取骑手职位类型列表
        public async Task<List<XM.RiderJobTypeInfo>> GetRiderJobTypeInfoBaseList()
        {
            return await DbContext.Set<RiderJobType>()
                .Where(c => true)
                .OrderBy(c => c.RiderJobTypeID)
                .Select(c => new XM.RiderJobTypeInfo
                {
                    RiderJobTypeID = c.RiderJobTypeID,
                    Name = c.Name
                })
                .ToListAsync();
        }
        //人事获取部门列表（含职位）
        public async Task<List<XM.GroupInfoBase>> HRGetGroupInfoBaseList(Guid groupID)
        {
            var group = await DbContext.Set<Group>().FirstOrDefaultAsync(c => c.GroupID == groupID);
            if (group.Level == 1)
            {
                return (await GetTreeAsync(groupID, true)).Select(c => new XM.GroupInfoBase
                {
                    GroupID = c.GroupID,
                    ParentID = c.ParentID,
                    Level = c.Level,
                    Name = c.Name,
                    IsIncludeUser = c.IsIncludeUser,
                    Roles = c.Role.Select(a => new XM.RoleInfoBase
                    {
                        RoleID = a.RoleID,
                        Name = a.Name.Replace("\n", string.Empty).Replace("\r", string.Empty)
                    }).ToList()
                }).ToList();
            }
            var temp = await _groupRepository.GetBasePathAsync(group.GroupID);
            var groupT = temp[temp.Count - 2].GroupID;
            var result = (await GetTreeAsync(groupT, true)).Select(c => new XM.GroupInfoBase
            {
                GroupID = c.GroupID,
                ParentID = c.ParentID,
                Level = c.Level,
                Name = c.Name,
                IsIncludeUser = c.IsIncludeUser,
                Roles = c.Role.Select(a => new XM.RoleInfoBase
                {
                    RoleID = a.RoleID,
                    Name = a.Name.Replace("\n", string.Empty).Replace("\r", string.Empty)
                }).ToList()
            }).ToList();
            return result;
        }
        //离职原因
        public async Task<List<XM.ExitReasonInfoBase>> GetExitReasonInfoBaseList()
        {
            return await DbContext.Set<ExitReason>()
                .Where(c => !c.IsDisabled)
                .OrderBy(c => c.ExitReasonID)
                .Select(c => new XM.ExitReasonInfoBase
                {
                    ExitReasonID = c.ExitReasonID,
                    Name = c.Name
                })
                .ToListAsync();

        }
        //员工异动类型
        public async Task<List<XM.StaffTurnoverTypeInfoBase>> GetStaffTurnoverTypeInfoBaseList()
        {

            return await DbContext.Set<StaffTurnoverType>()
                .Where(c => true)
                .OrderBy(c => c.StaffTurnoverTypeID)
                .Select(c => new XM.StaffTurnoverTypeInfoBase
                {
                    StaffTurnoverTypeID = c.StaffTurnoverTypeID,
                    Name = c.Name
                })
                .ToListAsync();

        }
        //员工请假类型
        public async Task<List<XM.StaffLeaveTypeInfoBase>> GetStaffLeaveTypeInfoBaseList()
        {

            return await DbContext.Set<StaffLeaveType>()
                .Where(c => true)
                .OrderBy(c => c.StaffLeaveTypeID)
                .Select(c => new XM.StaffLeaveTypeInfoBase
                {
                    StaffLeaveTypeID = c.StaffLeaveTypeID,
                    Name = c.Name
                })
                .ToListAsync();

        }
        //员工状态
        public async Task<List<XM.StaffStatusInfoBase>> GetStaffStatus()
        {
            return await DbContext.StaffStatus.Where(c => true).Select(c => new XM.StaffStatusInfoBase
            {
                StaffStatusID = c.StaffStatusID,
                Name = c.Name
            }).ToListAsync();
        }
        //自动化导入使用
        public async Task<object> GetStation()
        {

            return await DbContext.Stations.Where(c => true).Select(c => new
            {
                GroupID = c.StationID,
                TeamID = c.StationEleID
            }).ToListAsync();

        }
        #endregion

        #region 人事管理

        //获取员工基本信息列表(用于添加其他信息时选择)
        public async Task<Page<XM.StaffInfoBaseList>> GetStaffInfoBaseList(XM.GetStaffInfoBaseListPar par, List<Guid> Groups, PagingInfo pagingInfo)
        {

            IQueryable<Staff> query = DbContext.Staffs.AsNoTracking();
            query = query.Where(c => Groups.Contains(c.User.GroupID));
            
            if (!par.GroupIDs.IsNullOrEmpty())
            {
                query = query.Where(m => par.GroupIDs.Contains(m.User.GroupID));
            }
            if (!par.RoleIDs.IsNullOrEmpty())
            {
                query = query.Where(c => par.RoleIDs.Contains(c.User.RoleID ?? Guid.Empty));
            }
            if (!par.StaffStatusIDs.IsNullOrEmpty())
            {
                query = query.Where(c => par.StaffStatusIDs.Contains(c.StaffStatusID));
            }
            if (!par.Keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(c => c.Name.Contains(par.Keyword)
                                    || c.StaffMobile.Contains(par.Keyword)
                                    || c.User.Group.Name.Contains(par.Keyword));
            }
            if (par.IsInBlackList.HasValue)
            {
                query = query.Where(c => c.IsInBlackList == par.IsInBlackList);
            }
            if (par.IsViaReferral.HasValue)
            {
                query = query.Where(c => c.RecruitChannel.RecruitChannelID == 1);
            }

            IOrderedQueryable<Staff> orderedQuery;
            if (pagingInfo.SortInfo != null && !pagingInfo.SortInfo.Sort.IsNullOrWhiteSpace())
            {
                orderedQuery = query.Order(pagingInfo.SortInfo.Sort, pagingInfo.SortInfo.SortDir == SortDir.DESC);
            }
            else
            {
                // 默认排序
                orderedQuery = query.OrderBy(m => m.StaffID);
            }
            var page = await orderedQuery.Select(_staffInfoBaseListSelector).GetPageAsync(pagingInfo);
            return page;

        }
        //获取员工基本详细信息列表
        public async Task<Page<XM.StaffBaseList>> GetStaffBaseList(XM.GetStaffInfoListPar par, List<Guid> Groups, PagingInfo pagingInfo)
        {

            IQueryable<Staff> query = DbContext.Staffs.AsNoTracking();
            query = query.Where(c => Groups.Contains(c.User.GroupID));

            if (!par.GroupIDs.IsNullOrEmpty())
            {
                query = query.Where(m => par.GroupIDs.Contains(m.User.GroupID));
            }
            if (!par.RoleIDs.IsNullOrEmpty())
            {
                query = query.Where(c => par.RoleIDs.Contains(c.User.RoleID??Guid.Empty));
            }
            if (!par.StaffStatusIDs.IsNullOrEmpty())
            {
                query = query.Where(c => par.StaffStatusIDs.Contains(c.StaffStatusID));
            }
            if (!par.Keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(c => c.Name.Contains(par.Keyword)
                                    || c.StaffMobile.Contains(par.Keyword)
                                    || c.User.Group.Name.Contains(par.Keyword)
                                    );
            }
            if (par.RiderEleIDIsNull.HasValue)
            {
                if (par.RiderEleIDIsNull.Value)
                {
                    query = query.Where(c => c.RiderEleID == null);
                }
                else
                {
                    query = query.Where(c => c.RiderEleID != null);
                }
            }
            if (par.IsInBlackList.HasValue)
            {
                query = query.Where(c => c.IsInBlackList == par.IsInBlackList);
            }
            if (par.IsViaReferral.HasValue)
            {
                query = query.Where(c => c.RecruitChannel.RecruitChannelID == 1);
            }

            IOrderedQueryable<Staff> orderedQuery;
            if (pagingInfo.SortInfo != null && !pagingInfo.SortInfo.Sort.IsNullOrWhiteSpace())
            {
                orderedQuery = query.Order(pagingInfo.SortInfo.Sort, pagingInfo.SortInfo.SortDir == SortDir.DESC);
            }
            else
            {
                // 默认排序
                orderedQuery = query.OrderBy(m => m.StaffID);
            }
            var page = await orderedQuery.Select(_staffBaseListSelector).GetPageAsync(pagingInfo);
            return page;

        }
        //获取员工详细信息
        public async Task<XM.StaffInfo> GetStaffInfo(XM.GetStaffInfoPar par)
        {
            return await DbContext.Set<Staff>()
                .Select(_staffInfoSelector)
                .FirstOrDefaultAsync(c => c.StaffID == par.StaffID);
        }
        //添加员工信息--验证
        public async Task<bool> AddStaffInfo(XHM.User currentUser, XM.AddStaffInfoPar par, XM.UserAddPar user, ModelStateDictionary modelState)
        {
            //骑手职位类型ID如果是站点级别的员工，需要录入职位类型
            var group = await DbContext.Set<Group>().FirstOrDefaultAsync(c => c.GroupID == user.GroupID);

            #region 特殊

            if (group.Level == stationLevel && !par.RiderJobTypeID.HasValue)
            {
                modelState.AddModelError("Level", "站点级别的员工，需要录入职位类型");
                return false;
            }
            else if (group.Level != stationLevel && par.RiderJobTypeID.HasValue)
            {
                modelState.AddModelError("Level", "非站点级别的员工，不需要录入职位类型");
                return false;
            }

            #endregion

            #region user

            var inputToSaveUser = DbContext.Set<User>().Create();
            DbContext.Set<User>().Add(inputToSaveUser);

            if (!await VerifyGroupID(user.GroupID, modelState)) { return false; }
            inputToSaveUser.GroupID = user.GroupID;
            if (!await VerifyRoleID(user.RoleID, modelState)) { return false; }
            inputToSaveUser.Roles.Add(DbContext.Set<Role>().FirstOrDefault(c => c.RoleID == user.RoleID));

            if (await IsExistsUsernameAsync(user.Username, modelState)) return false;
            inputToSaveUser.Username = user.Username;

            inputToSaveUser.DisplayName = user.DisplayName;
            inputToSaveUser.RealName = user.RealName;
            inputToSaveUser.RealNameIsValid = user.RealNameIsValid;

            if (!user.Email.IsNullOrWhiteSpace())
            {
                if (await IsExistsEmailAsync(user.Email, modelState)) return false;
                inputToSaveUser.Email = user.Email;
                inputToSaveUser.EmailIsValid = user.EmailIsValid;
            }

            if (!user.Mobile.IsNullOrWhiteSpace())
            {
                if (await IsExistsMobileAsync(user.Mobile, modelState)) return false;
                inputToSaveUser.Mobile = user.Mobile;
                inputToSaveUser.MobileIsValid = user.MobileIsValid;
            }


            //生成密码
            user.Password = user.PasswordConfirm = UserRepository.GeneratePassword(user.Password);
            inputToSaveUser.Password = user.Password;

            //userStatus特殊处理
            inputToSaveUser.Status = user.Status;

            inputToSaveUser.CreationDate = DateTime.Now;
            inputToSaveUser.HeadURL = user.HeadURL;
            inputToSaveUser.LogoURL = user.LogoURL;
            inputToSaveUser.Description = user.Description;

            #endregion

            #region staff

            if (par.Name.IsNullOrWhiteSpace())
            {
                modelState.AddModelError("StaffName", "员工姓名不能为空");
                return false;
            }
            var inputToSaveStaff = DbContext.Set<Staff>().Create();
            DbContext.Set<Staff>().Add(inputToSaveStaff);

            //特殊处理后的 RiderJobTypeID 赋值
            if (par.RiderJobTypeID.HasValue)
                if (!await VerifyRiderJobTypeID(par.RiderJobTypeID.Value, modelState)) return false;
            inputToSaveStaff.RiderJobTypeID = par.RiderJobTypeID;

            inputToSaveStaff.StaffID = inputToSaveUser.UserID;
            inputToSaveStaff.Name = par.Name;
            inputToSaveStaff.Sex = par.Sex;
            inputToSaveStaff.Age = par.Age;
            inputToSaveStaff.Birthday = par.Birthday;
            if (await IsExistsStaffMobileAsync(par.StaffMobile, modelState)) return false;
            inputToSaveStaff.StaffMobile = par.StaffMobile;

            if (par.MaritalStatus.HasValue)
            {
                if (!Enum.IsDefined(typeof(XM.MaritalStatus), par.MaritalStatus))
                {
                    modelState.AddModelError("MaritalStatus", "婚姻状况不正确");
                    return false;
                }
                inputToSaveStaff.MaritalStatus = par.MaritalStatus;
            }

            if (await IsExistsIDCardNOAsync(par.IDCardNO, modelState)) return false;
            inputToSaveStaff.IDCardNO = par.IDCardNO;

            if (par.EducationID.HasValue)
            {
                if (!await VerifyEducationID(par.EducationID.Value, modelState)) return false;
                inputToSaveStaff.EducationID = par.EducationID;
            }

            inputToSaveStaff.School = par.School;
            inputToSaveStaff.Major = par.Major;
            inputToSaveStaff.Household = par.Household;
            inputToSaveStaff.Residence = par.Residence;
            inputToSaveStaff.EmergencyContact = par.EmergencyContact;
            inputToSaveStaff.EmergencyContactRelationship = par.EmergencyContactRelationship;
            inputToSaveStaff.EmergencyContactMobile = par.EmergencyContactMobile;
            inputToSaveStaff.StaffStatusID = (int)XM.StaffStatus.NoEntry;

            #endregion

            #region staffTurnover

            var staffTurnover = DbContext.StaffTurnovers.Create();
            DbContext.StaffTurnovers.Add(staffTurnover);

            staffTurnover.StaffTurnoverTypeID = 1;//录入
            if (!await IsExistsStaffIDAsync(currentUser.UserInfo.UserID, modelState)) return false;
            staffTurnover.RequestStaffID = currentUser.UserInfo.UserID;
            staffTurnover.TargetStaffID = inputToSaveUser.UserID;
            staffTurnover.ToGroupID = user.GroupID;
            staffTurnover.ToRoleID = user.RoleID;

            if (par.RiderJobTypeID.HasValue)
                staffTurnover.ToRiderJobTypeID = par.RiderJobTypeID.Value;

            staffTurnover.CreationDate = DateTime.Now;
            staffTurnover.EffectiveDate = DateTime.Now;
            staffTurnover.AuditStatus = 1;
            staffTurnover.AuditStaffLevelCurrent = 1;
            staffTurnover.AuditStaffLevelMax = 1;
            staffTurnover.AuditStaffID1 = currentUser.UserInfo.UserID;
            staffTurnover.AuditDate1 = DateTime.Now;

            staffTurnover.RequestRemark = "新增-系统自动添加";
            staffTurnover.AuditRemark1 = "新增-系统自动添加";

            #endregion

            //测试使用
            inputToSaveStaff.RiderEleID = par.RiderEleID;

            if (await DbContext.SaveChangesAsync() > 0) return true;
            modelState.AddModelError("DbContext", "数据库写入失败");
            return false;

        }
        //编辑员工信息-修改-验证
        public async Task<XM.StaffInfoBaseList> EditStaffInfo(XM.EditStaffInfoPar par, XM.UserEditPar user, ModelStateDictionary modelState)
        {

            var inputToSave = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.User.UserID == user.UserID);

            #region 特殊

            if (inputToSave == null)
            {
                modelState.AddModelError("UserID", "尝试编辑不存在的记录");
                return null;
            }

            #endregion

            #region user

            if (await DbContext.Set<User>().AnyAsync(c => c.UserID != user.UserID && c.Username == user.Username))
            {
                modelState.AddModelError("Username", "用户名重复");
                return null;
            }
            inputToSave.User.Username = user.Username;
            inputToSave.User.DisplayName = user.DisplayName;
            inputToSave.User.RealName = user.RealName;
            inputToSave.User.RealNameIsValid = user.RealNameIsValid;
            if (!user.Email.IsNullOrWhiteSpace())
            {
                if (await DbContext.Set<User>().AnyAsync(c => c.UserID != user.UserID && c.Email == user.Email))
                {
                    modelState.AddModelError("Email", "邮箱重复");
                    return null;
                }
            }
            inputToSave.User.Email = user.Email;
            inputToSave.User.EmailIsValid = user.EmailIsValid;
            if (!user.Mobile.IsNullOrWhiteSpace())
            {

                if (await DbContext.Set<User>().AnyAsync(c => c.UserID != user.UserID && c.Mobile == user.Mobile))
                {
                    modelState.AddModelError("Mobile", "手机号重复");
                    return null;
                }

            }
            inputToSave.User.Mobile = user.Mobile;
            inputToSave.User.MobileIsValid = user.MobileIsValid;
            if (!user.Password.IsNullOrWhiteSpace())
            {
                //生成密码
                user.Password = user.PasswordConfirm = UserRepository.GeneratePassword(user.Password);
                inputToSave.User.Password = user.Password;
            }
            if (!Enum.IsDefined(typeof(UserStatus), user.Status))
            {
                modelState.AddModelError("UserStatus", "用户状态不正确");
                return null;
            }
            inputToSave.User.Status = user.Status;
            inputToSave.User.HeadURL = user.HeadURL;
            inputToSave.User.LogoURL = user.LogoURL;
            inputToSave.User.Description = user.Description;

            #endregion

            #region Staff

            inputToSave.Name = par.Name;
            inputToSave.Sex = par.Sex;
            inputToSave.Age = par.Age;
            inputToSave.Birthday = par.Birthday;

            //员工手机号码重复
            if (!par.StaffMobile.IsNullOrWhiteSpace())
            {

                if (await DbContext.Set<Staff>().AnyAsync(c => c.StaffMobile == par.StaffMobile && c.StaffMobile != inputToSave.StaffMobile))
                {
                    modelState.AddModelError("StaffMobile", "员工手机号已存在");
                    return null;
                }

            }
            inputToSave.StaffMobile = par.StaffMobile;

            if (par.MaritalStatus.HasValue)
            {
                if (!Enum.IsDefined(typeof(XM.MaritalStatus), par.MaritalStatus))
                {
                    modelState.AddModelError("MaritalStatus", "婚姻状况不正确");
                    return null;
                }
            }
            inputToSave.MaritalStatus = par.MaritalStatus;

            //身份证号码重复
            if (!par.IDCardNO.IsNullOrWhiteSpace())
            {

                if (await DbContext.Set<Staff>().AnyAsync(c => c.IDCardNO == par.IDCardNO && c.IDCardNO != inputToSave.IDCardNO))
                {
                    modelState.AddModelError("IDCardNO", "身份证号码已存在");
                    return null;
                }

            }
            inputToSave.IDCardNO = par.IDCardNO;
            if (par.EducationID.HasValue)
            {
                if (!await VerifyEducationID(par.EducationID.Value, modelState)) return null;
                inputToSave.EducationID = par.EducationID;
            }

            inputToSave.School = par.School;
            inputToSave.Major = par.Major;
            inputToSave.Household = par.Household;
            inputToSave.Residence = par.Residence;
            inputToSave.EmergencyContact = par.EmergencyContact;
            inputToSave.EmergencyContactRelationship = par.EmergencyContactRelationship;
            inputToSave.EmergencyContactMobile = par.EmergencyContactMobile;

            #endregion

            if (await DbContext.SaveChangesAsync() > 0)
            {
                var result = (new Staff[] { inputToSave }).Select(_staffInfoBaseListSelector.Compile()).First();
                return result;
            }
            return null;

        }
        //员工入职--验证
        public async Task<bool> Entry(XM.EntryPar par, LavieContext _context, ModelStateDictionary modelState)
        {

            #region 非空字段-数据库验证

            var staff = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => par.StaffID == c.StaffID);
            if (staff == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }
            //GroupID
            if (!await VerifyGroupID(par.GroupID, modelState)) return false;
            //roleID
            if (!await VerifyRoleID(par.RoleID, modelState)) return false;

            //默认生效的内容：1、员工再次入职，允许更改招聘渠道，包括以前是转介绍，现在也是转介绍并且介绍人不是同一人也允许；
            //默认生效的内容：2、如果之前有ExitDate、ExitRemark，也保留着。
            //默认生效的内容：3、入职时协议内容是必须填写的（除了ProtocolEndDate）。

            #endregion

            #region 特殊性

            #region 验证

            if (staff.StaffStatusID == (int)XM.StaffStatus.OnJob)
            {
                modelState.AddModelError("StaffStatusID", "不允许处于“在职”状态的员工重复入职");
                return false;
            }
            if (staff.IsInBlackList == true)
            {
                modelState.AddModelError("IsInBlackList", "员工在黑名单，不允许入职");
                return false;
            }

            #endregion

            #region 修改

            //StaffStatusID 要改为“在职”
            staff.StaffStatusID = (int)XM.StaffStatus.OnJob;
            //EntryDate入职时间更新
            staff.EntryDate = DateTime.Now;
            //RiderEleID清除骑手饿了么ID
            staff.RiderEleID = null;
            //入职的部门、职位、骑手职位，需要在StaffTurnover表进行记录
            var staffTurnover = DbContext.Set<StaffTurnover>().Create();
            DbContext.Set<StaffTurnover>().Add(staffTurnover);
            staffTurnover.StaffTurnoverTypeID = (int)XM.StaffTurnoverType.Entry;
            staffTurnover.FromGroupID = staff.User.GroupID;//FromGroupID ToGroupID 的类型需修改
            staffTurnover.ToGroupID = par.GroupID;
            if (staff.User.RoleID.HasValue)
            {
                staffTurnover.FromRoleID = staff.User.RoleID;
            }
            staffTurnover.ToRoleID = par.RoleID;
            staffTurnover.FromRiderJobTypeID = staff.RiderJobTypeID;
            staffTurnover.ToRiderJobTypeID = par.RiderJobTypeID;
            staffTurnover.EffectiveDate = DateTime.Now;
            staffTurnover.RequestStaffID = _context.User.As<XHM.User>().UserInfo.UserID;
            staffTurnover.TargetStaffID = staff.StaffID;

            #endregion

            #endregion

            #region staff

            staff.EntryRemark = par.EntryRemark;
            staff.WorkNO = par.WorkNO;

            if (par.RecruitChannelID.HasValue)
            {
                if (!await VerifyRecruitChannelID(par.RecruitChannelID.Value, modelState))
                    return false;
                staff.RecruitChannelID = par.RecruitChannelID.Value;
                if (par.RecruitChannelID.Value == 1)
                {
                    //关联staff表 内部转介绍必须选择转介绍人、介绍日期、附件
                    if (!(par.RiderReferrerStaffID.HasValue
                        && par.RiderReferrerDate.HasValue
                        && (par.RiderReferrerAttachmentURL != null)))
                    {
                        modelState.AddModelError("RiderReferrerStaffID", "内部转介绍必须选择转介绍人、介绍日期、附件");
                        return false;
                    }
                    else
                    {
                        //内部转介绍必须选择转介绍人、介绍日期、附件
                        if (!await VerifyRiderReferrerStaffID(par.RiderReferrerStaffID.Value, modelState))
                            return false;
                        staff.RiderReferrerStaffID = par.RiderReferrerStaffID.Value;
                        staff.RiderReferrerDate = par.RiderReferrerDate.Value;
                        staff.RiderReferrerAttachmentURL = par.RiderReferrerAttachmentURL;
                    }

                }
                else
                {
                    if (par.RiderReferrerStaffID.HasValue
                        && par.RiderReferrerDate.HasValue
                        && (par.RiderReferrerAttachmentURL != null))
                    {
                        modelState.AddModelError("RiderReferrerStaffID", "除内部转介绍，不能选择转介绍人、介绍日期、附件");
                        return false;
                    }
                }
            }


            //数据库可空，模型非空字段
            //关联JobNature表验证
            if (!await VerifyJobNatureID(par.JobNatureID, modelState))
                return false;
            staff.JobNatureID = par.JobNatureID;

            staff.ProtocolNO = par.ProtocolNO;
            staff.ProtocolTimeID = par.ProtocolTimeID;
            staff.ProtocolTypeID = par.ProtocolTypeID;
            staff.ProtocolSignedDate = par.ProtocolSignedDate;
            staff.ProtocolBeginDate = par.ProtocolBeginDate;

            if (par.InsuranceNatureID != null)
            {
                //关联InsuranceNature表
                if (!await VerifyInsuranceNatureID(par.InsuranceNatureID.Value, modelState))
                    return false;
                staff.InsuranceNatureID = par.InsuranceNatureID;
            }
            if (par.InsuranceStartBuyDate != null)
            {
                staff.InsuranceNatureID = par.InsuranceNatureID;
            }
            if (par.BankTypeID.HasValue)
            {
                //关联BankTypeID表
                if (!await VerifyBankTypeID(par.BankTypeID.Value, modelState))
                    return false;
                staff.BankTypeID = par.BankTypeID;
            }
            if (par.BankNO != null)
            {
                //如果可能，验证卡号正确性 -- 放入模型正则
                staff.BankNO = par.BankNO;
            }

            //非空字段,验证字段的合法性         
            staff.User.GroupID = par.GroupID;
            if (staff.User.Roles.IsNullOrEmpty())
            {
                var temp = await DbContext.Set<Role>().FirstOrDefaultAsync(c => c.RoleID == par.RoleID);
                staff.User.Roles.Add(temp);
            }
            else
            {
                var temp = await DbContext.Set<Role>().FirstOrDefaultAsync(c => c.RoleID == par.RoleID);
                staff.User.Roles.Clear();
                staff.User.Roles.Add(temp);
            }


            //RiderJobTypeID 如果是站点级别的员工，需要录入职位类型
            if (par.RiderJobTypeID.HasValue)
            {
                if ((await DbContext.Set<Group>().FirstOrDefaultAsync(c => c.GroupID == par.GroupID)).Level == 5)
                {
                    if (!await VerifyRiderJobTypeID(par.RiderJobTypeID.Value, modelState)) { return false; }
                    staff.RiderJobTypeID = par.RiderJobTypeID;
                }
            }
            #endregion



            if (staff.User.RoleID.HasValue && staff.User.RoleID != new Guid("99D9B82A-5796-4A64-8724-D090CB85B4AE") && staff.User.RoleID != new Guid("4E3D798B-16A9-4987-91E7-40B730D755E2") && staff.User.RoleID != new Guid("ECE3FBF5-F55E-4111-A653-6015CFD803BF") && staff.User.RoleID != new Guid("9F30903B-C782-4719-BC52-A58C89425D53") && staff.User.RoleID != new Guid("7CC6A55E-60CE-4733-9E1A-C5E5A3B20FD7") && staff.User.RoleID != new Guid("C22F5628-B146-4CE9-B5B7-69300820C600") && staff.RiderJobTypeID != null)
            {
                staff.RiderJobTypeID = null;
            }

            await DbContext.SaveChangesAsync();
            return true;

        }
        //员工转介绍信息补充
        public async Task<bool> EditRiderReferrer(XM.RiderReferrePar par, ModelStateDictionary modelState)
        {
            var staff = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == par.StaffID);
            if (staff == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }
            if (!await VerifyRecruitChannelID(par.RecruitChannelID, modelState)) { return false; }

            if (par.RecruitChannelID == 1)
            {
                //内部转介绍必须选择转介绍人、介绍日期、附件
                if (!(par.RiderReferrerStaffID.HasValue && par.RiderReferrerDate.HasValue && !par.RiderReferrerAttachmentURL.IsNullOrWhiteSpace()))
                {
                    modelState.AddModelError("RiderReferrerStaffID", "内部转介绍必须选择转介绍人、介绍日期、附件");
                    return false;
                }
            }
            else
            {
                //内部转介绍必须选择转介绍人、介绍日期、附件
                if (par.RiderReferrerStaffID.HasValue && par.RiderReferrerDate.HasValue && !par.RiderReferrerAttachmentURL.IsNullOrWhiteSpace())
                {
                    modelState.AddModelError("RiderReferrerStaffID", "除内部转介绍外不能选择转介绍人、介绍日期、附件");
                    return false;
                }
            }

            //介绍人员工ID(用户ID、员工ID)关联本表必须是跑单者
            if (!await DbContext.Set<Staff>().AnyAsync(c => c.User.Group.Level == stationLevel))
            {
                modelState.AddModelError("RiderReferrerStaffID", "介绍人员工ID(用户ID、员工ID)关联本表必须是跑单者");
                return false;
            }

            #region 特殊

            //1、StaffStatusID 为“在职”是才可补充
            if (!await DbContext.Set<Staff>()
                    .AnyAsync(c => c.StaffID == par.StaffID && c.StaffStatusID == (int)XM.StaffStatus.OnJob))
            {
                modelState.AddModelError("StaffStatusID", "StaffStatusID 为“在职”才可补充");
                return false;
            }
            //2、介绍人当前StaffStatusID 必须处于“在职”
            if (!await DbContext.Set<Staff>()
                    .AnyAsync(c => c.StaffID == par.RiderReferrerStaffID
                    && c.StaffStatusID == (int)XM.StaffStatus.OnJob))
            {
                modelState.AddModelError("StaffStatusID", "介绍人当前“员工状态”必须处于“在职”");
                return false;
            }
            //3、如果之前招聘渠道不是“内部转介绍”,可转为“内部转介绍”且必须填写RiderReferrerStaffID、RiderReferrerDate和RiderReferrerAttachmentURL；
            if (par.RecruitChannelID == (int)XM.RecruitChannelEnum.InnerReferrer)
            {
                //关联staff表 内部转介绍必须选择转介绍人、介绍日期、附件
                if (!(par.RiderReferrerStaffID.HasValue
                    && par.RiderReferrerDate.HasValue
                    && (par.RiderReferrerAttachmentURL != null)))
                {
                    modelState.AddModelError("RiderReferrerStaffID", "内部转介绍必须选择转介绍人、介绍日期、附件");
                    return false;
                }
                else
                {
                    //内部转介绍必须选择转介绍人、介绍日期、附件
                    if (!await VerifyRiderReferrerStaffID(par.RiderReferrerStaffID.Value, modelState))
                        return false;
                    staff.RiderReferrerStaffID = par.RiderReferrerStaffID.Value;
                    staff.RiderReferrerDate = par.RiderReferrerDate.Value;
                    staff.RiderReferrerAttachmentURL = par.RiderReferrerAttachmentURL;
                }
            }
            //4、如果之前招聘渠道是“内部转介绍”，则只可更改RiderReffererRemark； 默认实现

            #endregion

            staff.RecruitChannelID = par.RecruitChannelID;
            await DbContext.SaveChangesAsync();
            return true;

        }
        //员工协议信息补充
        public async Task<bool> EditRecruitChannel(XM.RecruitChannelInput input, ModelStateDictionary modelState)
        {
            var staff = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == input.StaffID);
            if (staff == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }

            if (!await VerifyJobNatureID(input.JobNatureID, modelState)) return false;

            staff.ProtocolNO = input.ProtocolNO;
            staff.ProtocolTimeID = input.ProtocolTimeID;
            staff.ProtocolTypeID = input.ProtocolTypeID;
            staff.ProtocolSignedDate = input.ProtocolSignedDate;
            staff.ProtocolBeginDate = input.ProtocolBeginDate;
            staff.ProtocolEndDate = input.ProtocolEndDate;

            if (!await VerifyJobNatureID(input.JobNatureID, modelState)) return false;
            staff.JobNatureID = input.JobNatureID;



            #region 特殊

            #endregion

            if (await DbContext.SaveChangesAsync() > 0) { return true; }
            else
            {
                modelState.AddModelError("数据库写入失败", "");
                return false;
            }

        }
        //员工银行账号信息补充
        public async Task<bool> EditBank(XM.EditBankInput input, ModelStateDictionary modelState)
        {

            var needToSave = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == input.StaffID);
            if (needToSave == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }
            if (input.BankTypeID.HasValue)
                needToSave.BankTypeID = input.BankTypeID;
            if (!input.BankNO.IsNullOrWhiteSpace())
                needToSave.BankNO = input.BankNO;
            if (await DbContext.SaveChangesAsync() > 0) { return true; }
            else
            {
                modelState.AddModelError("数据库写入失败", "");
                return false;
            }
        }
        //员工保险信息补充
        public async Task<bool> EditInsurance(XM.EditInsuranceInput input, ModelStateDictionary modelState)
        {

            var needToSave = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == input.StaffID);
            if (needToSave == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }
            if (input.InsuranceNatureID.HasValue)
            {
                if (!await VerifyInsuranceNatureID(input.InsuranceNatureID.Value, modelState)) return false;
                needToSave.InsuranceNatureID = input.InsuranceNatureID;
            }

            if (input.InsuranceStartBuyDate.HasValue)
                needToSave.InsuranceStartBuyDate = input.InsuranceStartBuyDate;
            if (input.InsuranceStopBuyDate.HasValue)
                needToSave.InsuranceStopBuyDate = input.InsuranceStopBuyDate;

            await DbContext.SaveChangesAsync();

            return true;
        }
        //员工骑手饿了么ID补充
        public async Task<bool> EditRiderEleID(XM.EditRiderEleIDInput input, ModelStateDictionary modelState)
        {

            var needToSave = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == input.StaffID);
            if (needToSave == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }
            needToSave.RiderEleID = input.RiderEleID;

            if (await DbContext.SaveChangesAsync() > 0) { return true; }
            else
            {
                modelState.AddModelError("数据库写入失败", "");
                return false;
            }

        }
        //员工离职
        public async Task<bool> Exit(XM.ExitInput input, LavieContext _context, ModelStateDictionary modelState)
        {
            var curentStaffID = _context.User.As<XHM.User>().UserInfo.UserID;
            var staff = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == input.StaffID);
            if (staff == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }

            #region 特殊

            //1、StaffStatusID仅处于“在职”状态的员工才可离职；
            if (staff.StaffStatusID != (int)XM.StaffStatus.OnJob)
            {
                modelState.AddModelError("StaffStatu", "仅处于“在职”状态的员工才可离职");
                return false;
            }
            //2、StaffStatusID 需要更改为“离职”状态;
            staff.StaffStatusID = (int)XM.StaffStatus.Resign;
            //3、需要在StaffTurnover表进行记录。
            #region StaffTurnover

            var StaffTurnoverSave = DbContext.Set<StaffTurnover>().Create();
            DbContext.Set<StaffTurnover>().Add(StaffTurnoverSave);
            StaffTurnoverSave.StaffTurnoverTypeID = (int)XM.StaffTurnoverType.Resign;
            StaffTurnoverSave.RequestStaffID = curentStaffID;
            StaffTurnoverSave.TargetStaffID = input.StaffID;
            StaffTurnoverSave.CreationDate = DateTime.Now;
            StaffTurnoverSave.AuditStatus = 1;// 一级审核通过
            StaffTurnoverSave.AuditStaffLevelCurrent = 1;//当前审核层级
            StaffTurnoverSave.AuditStaffLevelMax = 1;//最大审核层级

            #endregion
            //4、RiderFirstWorkDate（首单时间）要清除；RiderLastWorkDate（末单时间）保留。

            //5、转介绍信息清理。离职本人的转介绍人置空，离职本人介绍的人的转介绍人也置空。
            staff.RiderReferrerStaffID = null;
            staff.RiderReferrerDate = null;
            staff.RiderReferrerRemark = null;
            staff.RiderReferrerAttachmentURL = null;
            var list = DbContext.Set<Staff>().Where(c => c.RiderReferrerStaffID == input.StaffID);
            foreach (var item in list)
            {
                item.RiderReferrerStaffID = null;
                item.RiderReferrerRemark = null;
                item.RiderReferrerDate = null;
                item.RiderReferrerAttachmentURL = null;
            }

            staff.RiderFirstWorkDate = null;

            #endregion

            staff.ExitDate = input.ExitDate;
            staff.ExitRemark = input.ExitRemark;
            if (!await VerifyExitReasonID(input.ExitReasonID, modelState)) return false;
            staff.ExitReasonID = input.ExitReasonID;

            if (await DbContext.SaveChangesAsync() > 0) { return true; }
            else
            {
                modelState.AddModelError("数据库写入失败", "");
                return false;
            }

        }
        //员工加入黑名单
        public async Task<bool> InBlackList(XM.InBlackListInput input, ModelStateDictionary modelState)
        {

            var staff = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == input.StaffID);
            if (staff == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }
            #region 特殊
            //1、StaffStatusID仅处于非“在职”状态的员工才可加入黑名单；
            if (staff.StaffStatusID == (int)XM.StaffStatus.OnJob)
            {
                modelState.AddModelError("StaffID", "仅处于非“在职”状态的员工才可加入黑名单");
                return false;
            }
            //2、IsInBlackList不能和当前黑名单状态相同。
            if (staff.IsInBlackList == input.IsInBlackList)
            {
                modelState.AddModelError("StaffID", "不能和当前黑名单状态相同");
                return false;
            }

            #endregion
            staff.IsInBlackList = input.IsInBlackList;
            staff.InBlackListRemark = input.InBlackListRemark;

            if (await DbContext.SaveChangesAsync() > 0) { return true; }
            else
            {
                modelState.AddModelError("数据库写入失败", "");
                return false;
            }

        }
        //员工部门、职位调整
        public async Task<bool> Turnover(XM.TurnoverInput input, LavieContext _context, ModelStateDictionary modelState)
        {

            var staff = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == input.StaffID);
            var curentStaffID = _context.User.As<XHM.User>().UserInfo.UserID;
            if (staff == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }
            //if (!(new HashSet<int> { 4, 5, 6, 7 }.Any(c => c == input.StaffTurnoverTypeID)))
            //{
            //    modelState.AddModelError("StaffTurnoverTypeID", "可用范围：“平调、晋升、降级 、骑手职位类型调整”");
            //    return false;
            //}

            #region StaffTurnover

            var StaffTurnoverSave = DbContext.Set<StaffTurnover>().Create();
            DbContext.Set<StaffTurnover>().Add(StaffTurnoverSave);
            StaffTurnoverSave.StaffTurnoverTypeID = input.StaffTurnoverTypeID;
            StaffTurnoverSave.RequestStaffID = curentStaffID;
            StaffTurnoverSave.TargetStaffID = input.StaffID;
            StaffTurnoverSave.CreationDate = DateTime.Now;
            StaffTurnoverSave.AuditStatus = 1;// 一级审核通过
            StaffTurnoverSave.AuditStaffLevelCurrent = 1;//当前审核层级
            StaffTurnoverSave.AuditStaffLevelMax = 1;//最大审核层级

            #endregion

            #region 特殊

            //3、GroupID和RoleID不能和当前部门和职位完全相同
            if (!staff.User.Roles.IsNullOrEmpty())
            {
                if ((staff.User.GroupID == input.GroupID) && (staff.User.Roles.Any(c => c.RoleID == input.RoleID)))
                {
                    modelState.AddModelError("GroupID", "用户组和职位不能和当前完全相同");
                    return false;
                }
            }



            #endregion

            if (!await VerifyGroupID(input.GroupID, modelState)) return false;
            staff.User.GroupID = input.GroupID;
            if (!await VerifyRoleID(input.RoleID, modelState)) return false;

            staff.User.Roles.Clear();
            staff.User.Roles.Add(DbContext.Set<Role>().FirstOrDefault(c => c.RoleID == input.RoleID));

            if (staff.User.RoleID.HasValue && staff.User.RoleID != new Guid("99D9B82A-5796-4A64-8724-D090CB85B4AE") && staff.User.RoleID != new Guid("4E3D798B-16A9-4987-91E7-40B730D755E2") && staff.User.RoleID != new Guid("ECE3FBF5-F55E-4111-A653-6015CFD803BF") && staff.User.RoleID != new Guid("9F30903B-C782-4719-BC52-A58C89425D53") && staff.User.RoleID != new Guid("7CC6A55E-60CE-4733-9E1A-C5E5A3B20FD7") && staff.User.RoleID != new Guid("C22F5628-B146-4CE9-B5B7-69300820C600") && staff.RiderJobTypeID != null)
            {
                staff.RiderJobTypeID = null;
            }

            await DbContext.SaveChangesAsync();
            return true;


        }
        //员工骑手职位类型调整
        public async Task<bool> ChangeRiderJobType(XM.ChangeRiderJobTypeInput input, LavieContext _context, ModelStateDictionary modelState)
        {
            var staff = await DbContext.Set<Staff>().FirstOrDefaultAsync(c => c.StaffID == input.StaffID);

            if (staff.User.Group.Level != stationLevel)
            {

                modelState.AddModelError("GroupID", "必须是站点级别的：站长、副站长、骑手等才可调整职位类型");
                return false;

            }

            var curentStaffID = _context.User.As<XHM.User>().UserInfo.UserID;

            if (staff == null)
            {
                modelState.AddModelError("StaffID", "尝试编辑不存在的记录");
                return false;
            }
            #region 特殊

            if (staff.RiderJobTypeID == input.RiderJobTypeID)
            {
                modelState.AddModelError("RiderJobTypeID", "RiderJobTypeID不能和当前职位类型相同");
                return false;
            }

            if (staff.User.Group.Level != stationLevel)
            {

                modelState.AddModelError("Level", "必须是站点级别的：站长、副站长、骑手等才可调整职位类型");
                return false;
            }

            #region StaffTurnover
            var StaffTurnoverSave = DbContext.Set<StaffTurnover>().Create();
            DbContext.Set<StaffTurnover>().Add(StaffTurnoverSave);
            StaffTurnoverSave.StaffTurnoverTypeID = (int)XM.StaffTurnoverType.RiderTypeMove;
            StaffTurnoverSave.RequestStaffID = curentStaffID;
            StaffTurnoverSave.TargetStaffID = input.StaffID;
            StaffTurnoverSave.CreationDate = DateTime.Now;
            StaffTurnoverSave.AuditStatus = 1;// 一级审核通过
            StaffTurnoverSave.AuditStaffLevelCurrent = 1;//当前审核层级
            StaffTurnoverSave.AuditStaffLevelMax = 1;//最大审核层级
            #endregion

            #endregion
            if (await DbContext.SaveChangesAsync() > 0) { return true; }
            else
            {
                modelState.AddModelError("数据库写入失败", "");
                return false;
            }
        }

        //验证
        public async Task<bool> VerifyExistsIDCardNOAsync(int staffID, string IDCardNO)
        {
            if (IDCardNO.IsNullOrWhiteSpace()) return false;
            return await DbContext.Set<Staff>().AnyAsync(m => m.StaffID != staffID && m.IDCardNO == IDCardNO);
        }
        public async Task<bool> IsExistsIDCardNOAsync(string IDCardNO)
        {
            if (IDCardNO.IsNullOrWhiteSpace()) return false;
            return await DbContext.Set<Staff>().AnyAsync(m => m.IDCardNO == IDCardNO);
        }

        #endregion

        #region StaffTurnover

        public async Task<IPagedList<XM.StaffTurnoverInfoBase>> GetStaffTurnoverList(XM.StaffTurnoverSearchCriteria criteria, PagingInfo pagingInfo)
        {
            Expression<Func<StaffTurnover, XM.StaffTurnoverInfoBase>> selector = u => new XM.StaffTurnoverInfoBase
            {
                StaffTurnoverID = u.StaffTurnoverID,
                StaffTurnoverType = new XM.StaffTurnoverTypeInfoBase
                {
                    StaffTurnoverTypeID = u.StaffTurnoverTypeID,
                    Name = u.StaffTurnoverType.Name
                },
                RequestStaff = new XM.StaffInfoBase
                {
                    StaffID = u.RequestStaff.StaffID,
                    Name = u.RequestStaff.Name
                },
                TargetStaff = new XM.StaffInfoBase
                {
                    StaffID = u.TargetStaff.StaffID,
                    Name = u.TargetStaff.Name
                },
                FromGroup = new XM.GroupBaseInfo
                {
                    GroupID = u.FromGroupID ?? Guid.Empty,
                    Name = u.FromGroupID.HasValue ? u.FromGroup.Name : null
                },
                FromRole = new XM.RoleBaseInfo
                {
                    RoleID = u.FromRoleID ?? Guid.Empty,
                    Name = u.FromRoleID.HasValue ? u.FromRole.Name : null
                },
                FromRiderJobType = new XM.RiderJobTypeInfo
                {
                    RiderJobTypeID = u.FromRiderJobTypeID ?? 0,
                    Name = u.FromRiderJobTypeID.HasValue ? u.FromRiderJobType.Name : null
                },
                ToGroup = new XM.GroupBaseInfo
                {
                    GroupID = u.ToGroupID ?? Guid.Empty,
                    Name = u.ToGroupID.HasValue ? u.ToGroup.Name : null
                },
                ToRole = new XM.RoleBaseInfo
                {
                    RoleID = u.ToRoleID ?? Guid.Empty,
                    Name = u.ToRoleID.HasValue ? u.ToRole.Name : null
                },
                ToRiderJobType = new XM.RiderJobTypeInfo
                {
                    RiderJobTypeID = u.ToRiderJobTypeID ?? 0,
                    Name = u.ToRiderJobTypeID.HasValue ? u.ToRiderJobType.Name : null
                },
                RequestRemark = u.RequestRemark,
                CreationDate = u.CreationDate,
                EffectiveDate = u.EffectiveDate,
                AuditStatus = u.AuditStatus,
                AuditStaffLevelCurrent = u.AuditStaffLevelCurrent,
                AuditStaffLevelMax = u.AuditStaffLevelMax,
                AuditStaff1 = new XM.StaffInfoBase
                {
                    StaffID = u.AuditStaffID1 ?? 0,
                    Name = u.AuditStaffID1.HasValue ? u.Staff_AuditStaffID1.Name : null
                },
                AuditStaff2 = new XM.StaffInfoBase
                {
                    StaffID = u.AuditStaffID2 ?? 0,
                    Name = u.AuditStaffID2.HasValue ? u.Staff_AuditStaffID2.Name : null
                },
                AuditStaff3 = new XM.StaffInfoBase
                {
                    StaffID = u.AuditStaffID3 ?? 0,
                    Name = u.AuditStaffID3.HasValue ? u.Staff_AuditStaffID3.Name : null
                },
                AuditStaff4 = new XM.StaffInfoBase
                {
                    StaffID = u.AuditStaffID4 ?? 0,
                    Name = u.AuditStaffID4.HasValue ? u.Staff_AuditStaffID4.Name : null
                },
                AuditDate1 = u.AuditDate2,
                AuditDate2 = u.AuditDate2,
                AuditDate3 = u.AuditDate3,
                AuditDate4 = u.AuditDate4,
                AuditRemark1 = u.AuditRemark1,
                AuditRemark2 = u.AuditRemark2,
                AuditRemark3 = u.AuditRemark4,
                AuditRemark4 = u.AuditRemark4
            };

            IQueryable<StaffTurnover> list = DbContext.Set<StaffTurnover>();
            if (criteria != null)
            {
                if (criteria.RequestStaff != null)
                    list = list.Where(m => m.RequestStaffID == criteria.RequestStaff.StaffID);
                //条件拼接
            }
            int count = await list.CountAsync();

            return new PagedList<XM.StaffTurnoverInfoBase>(await list
                .OrderBy(m => m.CreationDate)
                .Skip(pagingInfo.PageIndex * pagingInfo.PageSize)
                .Take(pagingInfo.PageSize)
                .Select(selector)
                .AsNoTracking()
                .ToListAsync(),
                pagingInfo.PageIndex,
                pagingInfo.PageSize,
                count);

        }


        #endregion

        #endregion
        public async Task<bool> GetHrGroupPermitAsync(Guid currentGroupID, Guid groupID)
        {
            var group = await DbContext.Set<Group>().FirstOrDefaultAsync(c => c.GroupID == currentGroupID);
            if (group.Level == 1)
                return (await GetTreeAsync(currentGroupID, true))
                      .Select(c => c.GroupID)
                      .ToList().Contains(groupID);
            var temp = await _groupRepository.GetBasePathAsync(currentGroupID);
            var groupT = temp[temp.Count - 2].GroupID;
            return (await GetTreeAsync(groupT, true))
                      .Select(c => c.GroupID)
                      .ToList().Contains(groupID);
        }

        #region private Methords

        private void Log(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                var temp = sql;
                temp = DateTime.Now.ToString("HH:mm:ss") + "--" + temp + "--";
                string fileLogPath = @"C:\Users\Administrator\Desktop\efLog";
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                File.AppendAllText(fileLogPath + fileName, temp, Encoding.Default);
            }
        }


        //机构含子树的占位
        private async Task<bool> MoveAsync(Guid sourceGroupID, Guid targetGroupID)
        {
            //sourceGroupID = new Guid("35DEA0E5-9B9F-4B8B-981E-2E1120B1C317"); //测试
            //targetGroupID = new Guid("28C9B6D8-DD00-4BC9-BD3E-E06174DFAAEB"); 
            Group sourceGroup = await DbContext.Set<Group>().FirstOrDefaultAsync(m => m.GroupID == sourceGroupID);
            Group targetGroup = await DbContext.Set<Group>().FirstOrDefaultAsync(m => m.GroupID == targetGroupID);
            List<Group> sourceTree = new List<Group> { sourceGroup };
            //List<Group> targetTree = new List<Group> { targetGroup };
            sourceTree.AddRange(await GetCurrentTree(sourceGroup));
            //targetTree.AddRange(await GetCurrentTree1(targetGroup));

            //改变层级
            var distant = targetGroup.Level - sourceGroup.Level;
            if (distant != 0) foreach (var item in sourceTree) { item.Level += distant; };
            //重新排序       
            List<Group> needToSort = new List<Group>();
            if (targetGroup.DisplayOrder < sourceGroup.DisplayOrder)//上移
            {

                needToSort.AddRange(sourceTree);//第一部分
                needToSort.AddRange(await DbContext.Set<Group>().Where(c => c.DisplayOrder >= targetGroup.DisplayOrder && c.DisplayOrder < sourceGroup.DisplayOrder)
                                               .OrderBy(c => c.DisplayOrder)
                                               .ToListAsync()
                                               );//第二部分
                var temp = sourceTree[sourceTree.Count - 1].DisplayOrder;
                needToSort.AddRange(await DbContext.Set<Group>().Where(c => c.DisplayOrder > temp)
                                               .OrderBy(c => c.DisplayOrder)
                                               .ToListAsync());//第三部分
                var i = 0;
                var begin = targetGroup.DisplayOrder;
                foreach (var item in needToSort)
                {
                    item.DisplayOrder = i + begin;
                    i++;
                }
            }
            else//下移
            {
                var temp = sourceTree[sourceTree.Count - 1].DisplayOrder;
                needToSort.AddRange(await DbContext.Set<Group>().Where(c => c.DisplayOrder > temp && c.DisplayOrder < targetGroup.DisplayOrder)
                                               .OrderBy(c => c.DisplayOrder)
                                               .ToListAsync());//第一部分
                needToSort.AddRange(sourceTree);//第二部分

                needToSort.AddRange(await DbContext.Set<Group>().Where(c => c.DisplayOrder >= targetGroup.DisplayOrder)
                                               .OrderBy(c => c.DisplayOrder)
                                               .ToListAsync());//第三部分
                var i = 0;
                var begin = sourceGroup.DisplayOrder;
                foreach (var item in needToSort)
                {
                    item.DisplayOrder = i + begin;
                    i++;
                }
            }
            await DbContext.SaveChangesAsync();
            return true;
        }
        private async Task<List<Group>> GetCurrentTree(Group current)
        {
            int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(current.DisplayOrder, current.Level);
            return await DbContext.Set<Group>().Where(c => c.Level > current.Level && c.DisplayOrder > current
            .DisplayOrder && c.DisplayOrder < displayOrderOfNextParentOrNextBrother)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        }
        private async Task<List<Group>> GetTreeAsync(Guid groupID, bool isIncludeSelf)
        {
            var group = await DbContext.Set<Group>().FirstOrDefaultAsync(m => m.GroupID == groupID);
            if (group == null)
                return new List<Group>(0);
            else
            {
                int displayOrderOfNextParentOrNextBrother = await GetDisplayOrderOfNextParentOrNextBrother(group.DisplayOrder, group.Level);
                if (displayOrderOfNextParentOrNextBrother != 0)
                    return await DbContext.Set<Group>().Where(m => ((isIncludeSelf && m.DisplayOrder >= group.DisplayOrder) || (!isIncludeSelf && m.DisplayOrder > group.DisplayOrder)) && m.DisplayOrder < displayOrderOfNextParentOrNextBrother)
                        .OrderBy(m => m.DisplayOrder)
                        .ToListAsync();
                else
                    return await DbContext.Set<Group>().Where(m => (isIncludeSelf && m.DisplayOrder >= group.DisplayOrder) || (!isIncludeSelf && m.DisplayOrder > group.DisplayOrder))
                        .OrderBy(m => m.DisplayOrder)
                        .ToListAsync();
            }
        }
        private async Task<int> GetDisplayOrderOfNextParentOrNextBrother(int displayOrder, int groupLevel)
        {
            return await DbContext.Set<Group>().Where(m => m.Level <= groupLevel && m.DisplayOrder > displayOrder)
                .OrderBy(m => m.DisplayOrder)
                .Select(m => m.DisplayOrder)
                .FirstOrDefaultAsync();
        }



        private async Task<bool> IsExistsStaffIDAsync(int StaffID, ModelStateDictionary modelState)
        {

            if (!await DbContext.Set<Staff>().AnyAsync(c => c.StaffID == StaffID))
            {
                modelState.AddModelError("StaffID", "登录用户没有StaffID");
                return false;
            }
            return true;

        }
        private async Task<bool> IsExistsIDCardNOAsync(string IDCardNO, ModelStateDictionary modelState)
        {
            if (await DbContext.Set<Staff>().AnyAsync(c => c.IDCardNO == IDCardNO))
            {
                modelState.AddModelError("IDCardNO", "身份证已存在");
                return true;
            }
            return false;
        }
        private async Task<bool> IsExistsEmailAsync(string Email, ModelStateDictionary modelState)
        {
            if (await DbContext.Set<User>().AnyAsync(c => c.Email == Email))
            {
                modelState.AddModelError("Email", "邮箱已存在");
                return true;
            }
            return false;
        }
        private async Task<bool> IsExistsMobileAsync(string Mobile, ModelStateDictionary modelState)
        {
            if (await DbContext.Set<User>().AnyAsync(c => c.Mobile == Mobile))
            {
                modelState.AddModelError("Mobile", "手机号已存在");
                return true;
            }
            return false;
        }
        private async Task<bool> IsExistsUsernameAsync(string Username, ModelStateDictionary modelState)
        {
            if (await DbContext.Set<User>().AnyAsync(c => c.Username == Username))
            {
                modelState.AddModelError("Username", "用户名已存在");
                return true;
            }
            return false;
        }
        private async Task<bool> IsExistsStaffMobileAsync(string StaffMobile, ModelStateDictionary modelState)
        {
            if (await DbContext.Set<Staff>().AnyAsync(c => c.StaffMobile == StaffMobile))
            {
                modelState.AddModelError("StaffMobile", "员工手机号已存在");
                return true;
            }
            return false;
        }
        private async Task<bool> VerifyGroupID(Guid GroupID, ModelStateDictionary modelState)
        {
            if (GroupID == new Guid("00000000-0000-0000-0000-000000000000"))
            {
                modelState.AddModelError("GroupID", "用户组ID异常");
                return false;
            }

            var temp = await DbContext.Set<Group>().FirstOrDefaultAsync(c => c.GroupID == GroupID);
            if (temp == null)
            {
                modelState.AddModelError("GroupID", "用户组不存在");
                return false;
            }
            if (!temp.IsIncludeUser)
            {
                modelState.AddModelError("GroupID", "用户组【{0}】不允许包含用户".FormatWith(temp.Name));
                return false;
            }
            if (temp.IsDisabled)
            {
                modelState.AddModelError("GroupID", "用户组【{0}】已经停用".FormatWith(temp.Name));
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyRoleID(Guid RoleID, ModelStateDictionary modelState)
        {
            if (!await DbContext.Set<Role>().AnyAsync(c => c.RoleID == RoleID))
            {
                modelState.AddModelError("RoleID", "职位不存在");
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyRiderJobTypeID(Int32 RiderJobTypeID, ModelStateDictionary modelState)
        {
            if (!await DbContext.Set<RiderJobType>().AnyAsync(c => c.RiderJobTypeID == RiderJobTypeID))
            {
                modelState.AddModelError("RiderJobTypeID", "骑手职位类型不存在");
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyRecruitChannelID(Int32 RecruitChannelID, ModelStateDictionary modelState)
        {
            if (!await DbContext.Set<RecruitChannel>().AnyAsync(c => c.RecruitChannelID == RecruitChannelID))
            {
                modelState.AddModelError("RecruitChannelID", "招聘渠道不存在");
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyRiderReferrerStaffID(int RiderReferrerStaffID, ModelStateDictionary modelState)
        {
            //介绍人员工ID(必须是跑单者，用户ID、员工ID)
            if (!(await DbContext.Set<Staff>().AnyAsync(c => c.StaffID == RiderReferrerStaffID)
                || await DbContext.Set<User>().AnyAsync(c => c.UserID == RiderReferrerStaffID)
                || await DbContext.Set<Staff>().AnyAsync(c => c.User.Group.Level == 5)
                ))
            {
                modelState.AddModelError("StaffID", "介绍人员工ID(必须是跑单者，用户ID、员工ID)");
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyJobNatureID(Int32 JobNatureID, ModelStateDictionary modelState)
        {
            if (!await DbContext.Set<JobNature>().AnyAsync(c => c.JobNatureID == JobNatureID))
            {
                modelState.AddModelError("JobNatureID", "用工性质不存在");
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyInsuranceNatureID(Int32 InsuranceNatureID, ModelStateDictionary modelState)
        {
            if (!await DbContext.Set<InsuranceNature>().AnyAsync(c => c.InsuranceNatureID == InsuranceNatureID))
            {
                modelState.AddModelError("InsuranceNatureID", "保险性质不存在");
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyBankTypeID(Int32 BankTypeID, ModelStateDictionary modelState)
        {
            if (!await DbContext.Set<BankType>().AnyAsync(c => c.BankTypeID == BankTypeID))
            {
                modelState.AddModelError("BankTypeID", "银行类型不存在");
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyExitReasonID(Int32 ExitReasonID, ModelStateDictionary modelState)
        {
            if (!await DbContext.Set<ExitReason>().AnyAsync(c => c.ExitReasonID == ExitReasonID))
            {
                modelState.AddModelError("ExitReasonID", "离职原因类型不存在");
                return false;
            }
            return true;
        }
        private async Task<bool> VerifyStaffTurnoverTypeID(Int32 StaffTurnoverTypeID, ModelStateDictionary modelState)
        {

            if (!await DbContext.Set<StaffTurnoverType>().AnyAsync(c => c.StaffTurnoverTypeID == StaffTurnoverTypeID))
            {
                modelState.AddModelError("StaffTurnoverTypeID", "员工流转类型不存在");
                return false;
            }
            return true;

        }
        private async Task<bool> VerifyEducationID(Int32 EducationID, ModelStateDictionary modelState)
        {
            if (!await DbContext.Set<Education>().AnyAsync(c => c.EducationID == EducationID))
            {
                modelState.AddModelError("EducationID", "学历类型不存在");
                return false;
            }
            return true;

        }



        #endregion

    }
}
