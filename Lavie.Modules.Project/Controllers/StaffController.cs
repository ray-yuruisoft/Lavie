using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Models;
using Lavie.Modules.Admin.Extensions;
using XM = Lavie.Modules.Admin.Models;
using Lavie.Modules.Admin.Models.InputModels;
using Lavie.Modules.Admin.Models.ViewModels;
using Lavie.Modules.Admin.Services;
using Lavie.Modules.Project.Models;
using Lavie.Modules.Project.Repositories;
using CM = Lavie.Modules.Admin.Models;
using Lavie.ActionResults;
using Lavie.ActionFilters.Action;

namespace Lavie.Modules.Project.Controllers
{
    [AllowCrossSiteJson]
    public partial class StaffController : Controller
    {

        private readonly IStaffRepository _staffRepository;
        private readonly IGroupService _groupService;
        private readonly LavieContext _context;
        private readonly XM.User _currentUser;

        private readonly Guid RoleIDIsHR = new Guid("6672379B-B66A-46F5-AC1C-590047968E09");//人事主管
        private readonly Guid GroupIDIsHeadOffice = new Guid("BF872ED0-EE64-484F-9518-7F4ECF9926C4");//总部人事行政部

        public StaffController(IStaffRepository staffRepository,
            IGroupService groupService,
            LavieContext context)
        {
            _staffRepository = staffRepository;
            _groupService = groupService;
            _context = context;
            _currentUser = _context.User.As<XM.User>();
        }

        #region 法思特OA Api

        #region 字典
        //获取员工添加编辑需要使用的数据字典
        public async Task<object> GetDictionaryList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetDictionaryList()
            });
        }
        //获取学历列表
        public async Task<object> GetEducationInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetEducationInfoBaseList()
            });
        }
        //获取招聘渠道列表
        public async Task<object> GetRecruitChannelInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetRecruitChannelInfoBaseList()
            });
        }
        //获取用工性质列表
        public async Task<object> GetJobNatureInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetJobNatureInfoBaseList()
            });
        }
        //获取合同期次列表
        public async Task<object> GetProtocolTimeInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetProtocolTimeInfoBaseList()
            });
        }
        //获取合同类型列表
        public async Task<object> GetProtocolTypeInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetProtocolTypeInfoBaseList()
            });
        }
        //获取保险性质列表
        public async Task<object> GetBankTypeInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetBankTypeInfoBaseList()
            });
        }
        //获取骑手职位类型列表
        public async Task<object> GetInsuranceNatureInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetInsuranceNatureInfoBaseList()
            });
        }
        //获取骑手职位类型列表
        public async Task<object> GetRiderJobTypeInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetRiderJobTypeInfoBaseList()
            });
        }
        //离职原因
        public async Task<object> GetExitReasonInfoBaseList()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetExitReasonInfoBaseList()
            });
        }
        //员工异动类型
        public async Task<object> GetStaffTurnoverTypeInfoBaseList()
        {

            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetStaffTurnoverTypeInfoBaseList()
            });

        }
        //员工请假类型
        public async Task<object> GetStaffLeaveTypeInfoBaseList()
        {

            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetStaffLeaveTypeInfoBaseList()
            });

        }
        //人事获取部门列表（含职位）
        public async Task<object> HRGetGroupInfoBaseList()
        {

            if (!_context.User.IsAuthenticated) { return ErrorReturn("未登陆用户访问！"); }
            var groupID = _context.User.As<XM.User>().UserInfo.Group.GroupID;

            //var groupID = new Guid("35DEA0E5-9B9F-4B8B-981E-2E1120B1C317");

            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.HRGetGroupInfoBaseList(groupID)
            });
        }

        public async Task<object> GetStaffStatus()
        {
            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = await _staffRepository.GetStaffStatus()
            });
        }

        public async Task<object> GetStation()
        {
            return this.DateTimeJson(await _staffRepository.GetStation());
        }

        #endregion

        #region 人事管理

        //获取员工基本信息列表(用于添加其他信息时选择)
        public async Task<object> GetStaffInfoBaseList(GetStaffInfoBaseListPar par, PagingInfo pagingInfo)
        {
            if (!ModelState.IsValid) return ErrorReturn(ModelState.FirstErrorMessage());
            //TODO:测试使用，需删除
            if (_currentUser == null) { return ErrorReturn("未登陆用户！"); }


            if (!par.GroupIDs.IsNullOrEmpty() && par.GroupIDs.Count == 1)
            {
                par.GroupIDs = (await _groupService.GetListAsync(par.GroupIDs.FirstOrDefault())).Select(c => c.GroupID).ToList();
            }

            var page = await _staffRepository.GetStaffInfoBaseList(par, await JudgeRole(_currentUser.UserInfo.Group.GroupID, _currentUser.UserInfo.Role!=null? _currentUser.UserInfo.Role.RoleID:Guid.Empty), pagingInfo);
            return this.DateTimeJson(new ApiPageResult
            {
                Code = 200,
                Message = "成功",
                Page = page
            });

        }
        //获取部门列表(包含职位，用于添加其他信息时选择)
        public async Task<object> GetStaffBaseList(GetStaffInfoListPar par, PagingInfo pagingInfo)
        {
            //.Headers.GetValues("Set-Cookie")[0].Split(';')[0].Split('=');
            var a = HttpContext.Request.Headers.GetValues("Cookie");



            if (!ModelState.IsValid) return ErrorReturn(ModelState.FirstErrorMessage());
            if (_currentUser == null) { return ErrorReturn("未登陆用户！"); }
            if (!par.GroupIDs.IsNullOrEmpty() && par.GroupIDs.Count == 1)
            {
                par.GroupIDs = (await _groupService.GetListAsync(par.GroupIDs.FirstOrDefault())).Select(c => c.GroupID).ToList();
            }
            var temp = await JudgeRole(_currentUser.UserInfo.Group.GroupID, _currentUser.UserInfo.Role!=null? _currentUser.UserInfo.Role.RoleID:Guid.Empty);
            var page = await _staffRepository.GetStaffBaseList(par, temp, pagingInfo);
            return this.DateTimeJson(new ApiPageResult
            {
                Code = 200,
                Message = "成功",
                Page = page
            });

        }
        //获取员工详细信息
        public async Task<object> GetStaffInfo(GetStaffInfoPar par)
        {
            var temp = await _staffRepository.GetStaffInfo(par);
            if (temp != null)
            {
                var tempList = await _groupService.GetBasePathAsync(temp.Group.GroupID);
                for (var i = tempList.Count - 1; i > 0; i--)
                {
                    temp.Groups.Add(new GroupBaseInfo
                    {
                        GroupID = tempList[i].GroupID,
                        Name = tempList[i].Name
                    });
                }
            }

            return this.DateTimeJson(new ApiListResult
            {
                Code = 200,
                Message = "成功",
                List = new[] { temp }
            });
        }
        //添加员工信息
        public async Task<object> Add(AddStaffInfoPar par, UserAddPar user)
        {

            if (user.UserID.HasValue) return ErrorReturn("新增员工信息不能提供UserID");
            //模型验证失败直接返回
            if (!ModelState.IsValid)
            {
                return ErrorReturn(ModelState.FirstErrorMessage());
            }

            //当前用户检查
            // TODO：
            if (_currentUser == null)
            {
                return ErrorReturn("未登陆用户非法操作");
            }


            var currentGroupID = _currentUser.UserInfo.Group.GroupID;
            if (!await _staffRepository.GetHrGroupPermitAsync(currentGroupID, user.GroupID))
            {
                return ErrorReturn("当前用户不具备操作参数组权限");
            }
            if (await _staffRepository.AddStaffInfo(_currentUser, par, user, ModelState))
            {
                return this.DateTimeJson(new ApiResult
                {
                    Code = 200,
                    Message = "成功"
                });
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //编辑员工信息
        public async Task<object> Edit(EditStaffInfoPar par, UserEditPar user)
        {
            //模型验证失败直接返回
            if (!ModelState.IsValid)
            {
                return ErrorReturn(ModelState.FirstErrorMessage());
            }
            var result = await _staffRepository.EditStaffInfo(par, user, ModelState);
            if (result != null)
            {
                return this.DateTimeJson(new ApiListResult
                {
                    Code = 200,
                    Message = "编辑成功",
                    List = new StaffInfoBaseList[] { result }
                });
            }

            if (ModelState.IsValid) { return SuccessReturn(); }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工入职
        public async Task<object> Entry(EntryPar par)
        {
            //模型验证失败直接返回
            if (!ModelState.IsValid)
            {
                return ErrorReturn(ModelState.FirstErrorMessage());
            }
            //TODO:
            if (_currentUser == null)
            {
                return ErrorReturn("当前账户未登陆");
            }

            if (await _staffRepository.Entry(par, _context, ModelState))
            {
                return this.DateTimeJson(new ApiResult
                {
                    Code = 200,
                    Message = "成功"
                });
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工转介绍信息补充
        public async Task<object> EditRiderReferrer(RiderReferrePar par)
        {

            if (await _staffRepository.EditRiderReferrer(par, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工协议信息补充
        public async Task<object> EditRecruitChannel(RecruitChannelInput input)
        {

            if (await _staffRepository.EditRecruitChannel(input, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工银行账号信息补充
        public async Task<object> EditBank(EditBankInput input)
        {

            if (await _staffRepository.EditBank(input, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工保险信息补充
        public async Task<object> EditInsurance(EditInsuranceInput input)
        {

            if (await _staffRepository.EditInsurance(input, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工骑手饿了么ID补充
        public async Task<object> EditRiderEleID(EditRiderEleIDInput input)
        {

            if (await _staffRepository.EditRiderEleID(input, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工离职
        public async Task<object> Exit(ExitInput input)
        {
            //TODO:
            if (_currentUser == null)
            {
                return ErrorReturn("当前账户未登陆");
            }
            if (await _staffRepository.Exit(input, _context, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工加入黑名单
        public async Task<object> InBlackList(InBlackListInput input)
        {
            if (await _staffRepository.InBlackList(input, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工部门、职位调整
        public async Task<object> ChangeRiderJobType(ChangeRiderJobTypeInput input)
        {
            if (await _staffRepository.ChangeRiderJobType(input, _context, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //员工骑手职位类型调整
        public async Task<object> Turnover(TurnoverInput input)
        {

            if (!ModelState.IsValid) return ErrorReturn(ModelState.FirstErrorMessage());

            #region 权限判断

            var temp = _currentUser.UserInfo.Roles.FirstOrDefault();
            if (temp == null) return ErrorReturn("无权限访问！");//系统管理员等
            var current = (temp.RoleID == RoleIDIsHR) && (_currentUser.UserInfo.Group.GroupID == GroupIDIsHeadOffice);//总部“人事主管”
            if (!current)
            {
                var currentGroup = await _groupService.GetItemAsync(_currentUser.UserInfo.Group.GroupID);
                current = (temp.RoleID == RoleIDIsHR) && ((currentGroup.Level == 4) && currentGroup.LimitRoles.Any(c => c.RoleID == RoleIDIsHR));//城市“人事主管”
            }
            if (!current)
            {
                return ErrorReturn("无权限访问！");
            }

            #endregion

            if (await _staffRepository.Turnover(input, _context, ModelState))
            {
                return SuccessReturn();
            }

            return ErrorReturn(ModelState.FirstErrorMessage());
        }

        #endregion

        #endregion
    

        #region Private Methods

        private async Task<List<Guid>> JudgeRole(Guid groupID, Guid RoleID)
        {

            if (RoleID == new Guid("6672379B-B66A-46F5-AC1C-590047968E09"))//人事主管
            {

                // var a = (await _staffRepository.HRGetGroupInfoBaseList(groupID)).Select(c => c.GroupID).ToList();

                return (await _staffRepository.HRGetGroupInfoBaseList(groupID)).Select(c => c.GroupID).ToList();
            }
            return (await _groupService.GetListAsync(groupID)).Select(c => c.GroupID).ToList();

        }
        private DateTimeJsonResult ErrorReturn(string message)
        {
            return this.DateTimeJson(new
            {
                code = 400,
                message = message
            });
        }
        private DateTimeJsonResult SuccessReturn()
        {
            return this.DateTimeJson(new
            {
                code = 200,
                message = "成功"
            });
        }

        #endregion

    }
}
