using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lavie.ActionFilters.Action;
using Lavie.ActionResults;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Modules.Project.Models;
using Lavie.Modules.Project.Repositories;

namespace Lavie.Modules.Project.Controllers
{
    [AllowCrossSiteJson]
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly LavieContext _context;
        public ReportController(IReportRepository reportRepository,
            LavieContext context)
        {
            _reportRepository = reportRepository;
            _context = context;
        }


        public async Task<object> ImportAttendance(ImportAttendanceInput input)
        {
            
            if (!ModelState.IsValid) return ErrorReturn(ModelState.FirstErrorMessage());
            input.DeliveryFeeBillCsvPath = Server.MapPath("~\\" + HttpUtility.UrlDecode(input.DeliveryFeeBillCsvPath));
            var tuple = await _reportRepository.ImportAttendance(input, ModelState);
            if (tuple.Item1)
            {
                if (tuple.Item2.IsNullOrEmpty()) return SuccessReturn();
                return File(tuple.Item2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            return ErrorReturn(ModelState.FirstErrorMessage());

        }

        public async Task<object> Calculate(CalculateInput input)
        {
            if (!ModelState.IsValid) return ErrorReturn(ModelState.FirstErrorMessage());
            input.CityMonthBillCsvPath = Server.MapPath("~\\" + HttpUtility.UrlDecode(input.CityMonthBillCsvPath));
            input.EvaluateCsvPath = Server.MapPath("~\\" + HttpUtility.UrlDecode(input.EvaluateCsvPath));

            var tuple = await _reportRepository.Calculate(input, ModelState);
            if (tuple.Item1)
            {
                if (tuple.Item2.IsNullOrEmpty()) return SuccessReturn();
                return File(tuple.Item2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            return ErrorReturn(ModelState.FirstErrorMessage());
        }

        #region Private Methods

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
