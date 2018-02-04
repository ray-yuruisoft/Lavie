using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.ActionFilters.Action;
using Lavie.ActionResults;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Models;
using Lavie.Modules.Admin.Models.Api;
using Lavie.Modules.Admin.Services;
using Lavie.Modules.Project.Models;
using XMP = Lavie.Modules.Project.Models;
using Lavie.Modules.Project.Repositories;
using XM = Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Project.Controllers
{
    [AllowCrossSiteJson]
    public partial class FlowController : Controller
    {
        private readonly IFlowRepository _flowRepository;
        private readonly IGroupService _groupService;
        private readonly IRoleService _roleService;
        private readonly LavieContext _context;
        private readonly XM.User _currentUser;

        public FlowController(IFlowRepository flowRepository
            , IGroupService groupService
            , IRoleService roleService
            , LavieContext context)
        {
            _roleService = roleService;
            _groupService = groupService;
            _flowRepository = flowRepository;
            _context = context;
            _currentUser = _context.User.As<XM.User>();
        }

        //站级请假
        public async Task<object> RequestStationLeave(RequestStationLeaveInput input)
        {

            if (!ModelState.IsValid) return ErrorReturn(ModelState.FirstErrorMessage());
            // TODO: 临时代码，加上权限 Filter 后清除
            if (_context.User.As<XM.User>() == null) { return ErrorReturn("未登陆用户"); }
            // 加上权限 Filter 后，本用户一定是站长
            input.RequestStaffID = _context.User.As<XM.User>().UserInfo.UserID;
            if (await _flowRepository.RequestStationLeave(input, ModelState))
            {
                return SuccessReturn();
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }
        //获取站级请假列表
        public async Task<object> GetStationLeaveList(StaffLeaveSearchCriteria criteria, PagingInfo pagingInfo)
        {
            var temp = JudgeRole(_context, out Guid groupID, out bool isHR);
            if (!temp.IsNullOrWhiteSpace()) return ErrorReturn(temp);
            return this.DateTimeJson(new ApiPageResult
            {
                Code = 200,
                Message = "成功",
                Page = await _flowRepository.GetStationLeaveList(_currentUser, groupID, isHR, criteria, pagingInfo)
            });

        }
        //审核站级请假
        public async Task<object> AuditStationLeave(List<AuditStationLeaveInput> input)
        {

            if (!ModelState.IsValid) return ErrorReturn(ModelState.FirstErrorMessage());
            var temp = JudgeRole(_context, out Guid groupID, out bool isHR);
            if (!temp.IsNullOrWhiteSpace()) return ErrorReturn(temp);
            if (!await _flowRepository.AuditStationLeave(input, _context, ModelState))
                return ErrorReturn(ModelState.FirstErrorMessage());
            return SuccessReturn();

        }


        #region Private Methods

        private string JudgeRole(LavieContext _context, out Guid groupID, out bool isHR)
        {
            if (_context.User.As<XM.User>() == null)
            {
                groupID = new Guid("00000000-0000-0000-0000-000000000000");
                isHR = false;
                return "未登陆用户";
            }

            var roleID = _context.User.As<XM.User>().UserInfo.Roles.First().RoleID;
            groupID = _context.User.As<XM.User>().UserInfo.Group.GroupID;

            if (!(roleID == new Guid("ECE3FBF5-F55E-4111-A653-6015CFD803BF") || //站长
               roleID == new Guid("4D9ECE5C-C807-4009-B298-8ED016DF88B6") || //区域经理
               roleID == new Guid("9851828F-FA57-4CB2-BCAA-7E3C855C1C95")) ||//城市经理
               roleID == new Guid("6672379B-B66A-46F5-AC1C-590047968E09") //人事主管
               )
            {
                isHR = false;
                return "除站长，区域经理、城市经理外不可审核";
            }
            else if (roleID == new Guid("6672379B-B66A-46F5-AC1C-590047968E09"))
            {
                isHR = true;
            }
            else { isHR = false; }
            return null;
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
//todo 人事可以看请假列表 人事在员工列表 可以查看具体员工的详情