using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Lavie.ActionResults;
using Lavie.Extensions;
using Lavie.Infrastructure.XmlRpc;

namespace Lavie.ActionInvokers
{
    public class LavieAsyncControllerActionInvoker : AsyncControllerActionInvoker
    {
        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
        {
            // 当Action返回值是void或null时，actionReturnValue将为null
            // 详见ActionMethodDispatcher.GetExecutor方法
            if (actionReturnValue == null)
            {
                return new NotFoundResult();
            }

            // 判断actionReturnValue的类型是否继承自ActionResult，也可以用is判断
            //if (typeof(ActionResult).IsAssignableFrom(actionReturnValue.GetType()))
            if (actionReturnValue is ActionResult)
                return actionReturnValue as ActionResult;

            // 如果是简单类型，如int,string等。
            if (!actionReturnValue.GetType().IsComplexType())
                return new ContentResult { Content = Convert.ToString(actionReturnValue, CultureInfo.InvariantCulture) };

            if (controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = actionReturnValue
                };
            }
            else
            {
                controllerContext.Controller.ViewData.Model = actionReturnValue;
                return new ViewResult
                {
                    ViewData = controllerContext.Controller.ViewData,
                    TempData = controllerContext.Controller.TempData,
                };
            }
        }

        /// <summary>
        /// 获取Action参数值字典
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        protected override IDictionary<string, object> GetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (controllerContext.RouteData.DataTokens.ContainsKey("IsXmlRpc") &&
                (bool)controllerContext.RouteData.DataTokens["IsXmlRpc"])
            {
                var parameters =
                    controllerContext.RouteData.Values["parameters"] as IList<XmlRpcParameter>;
                IDictionary<string, object> mappedParameters =
                    XmlRpcParameterMapper.Map(actionDescriptor.GetParameters(), parameters);
                foreach (var mappedParameter in mappedParameters)
                {
                    controllerContext.RouteData.Values.Add(mappedParameter.Key, mappedParameter.Value);
                }
            }
            return base.GetParameterValues(controllerContext, actionDescriptor);
        }
    }
}
