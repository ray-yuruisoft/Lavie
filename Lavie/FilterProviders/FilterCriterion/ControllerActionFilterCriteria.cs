using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Lavie.FilterProviders.FilterCriterion
{
    public class ControllerActionFilterCriteria : IFilterCriteria
    {
        private readonly List<ReflectedActionDescriptor> _methods = new List<ReflectedActionDescriptor>();
        private readonly List<TaskAsyncActionDescriptor> _asyncMethods = new List<TaskAsyncActionDescriptor>();

        public void AddMethod<T>(Expression<Func<T, object>> method)
        {
            var methodCall = method.Body as MethodCallExpression;

            if (methodCall == null || methodCall.Object == null)
                throw new InvalidOperationException();

            var controllerType = methodCall.Object.Type;
            if (IsAsyncMethod(controllerType, methodCall.Method))
                _asyncMethods.Add(new TaskAsyncActionDescriptor(methodCall.Method, methodCall.Method.Name, new ReflectedAsyncControllerDescriptor(methodCall.Object.Type)));
            else
                _methods.Add(new ReflectedActionDescriptor(methodCall.Method, methodCall.Method.Name, new ReflectedControllerDescriptor(methodCall.Object.Type)));

        }

        #region IFilterCriteria Members

        public bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var reflectedDescriptor = actionDescriptor as ReflectedActionDescriptor;
            if (reflectedDescriptor != null)
                return _methods.Any(a => a.MethodInfo == reflectedDescriptor.MethodInfo);

            var taskDescriptor = actionDescriptor as TaskAsyncActionDescriptor;
            if (taskDescriptor != null)
                return _asyncMethods.Any(a => a.MethodInfo == taskDescriptor.MethodInfo);

            return false;
            //return _methods.Any(a => a.ControllerDescriptor.FindAction(controllerContext, actionDescriptor.ActionName) != null);
        }

        #endregion

        #region Private Mothods

        private static bool IsAsyncMethod(Type classType, string methodName)
        {
            // Obtain the method with the specified name. 
            MethodInfo method = classType.GetMethod(methodName);

            return IsAsyncMethod(classType, method);
        }
        private static bool IsAsyncMethod(Type classType, MethodInfo method)
        {
            Type attType = typeof(AsyncStateMachineAttribute);

            // Obtain the custom attribute for the method. 
            // The value returned contains the StateMachineType property. 
            // Null is returned if the attribute isn't present for the method. 
            var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(attType);

            return (attrib != null);
        }

        #endregion
    }
}
