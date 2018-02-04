using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Lavie.Utilities.Exceptions;

namespace Lavie.FilterProviders.FilterCriterion
{
    public class ControllerTypeFilterCriteria : IFilterCriteria
    {
        private readonly List<Type> _types = new List<Type>();
        private readonly List<ReflectedActionDescriptor> _methods = new List<ReflectedActionDescriptor>();
        private readonly List<TaskAsyncActionDescriptor> _asyncMethods = new List<TaskAsyncActionDescriptor>();

        public void AddType(Type controllerType)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");

            if (!typeof(IController).IsAssignableFrom(controllerType))
                throw new ArgumentOutOfRangeException("controllerType");

            _types.Add(controllerType);
        }
        public void AddType<T>() where T : IController
        {
            _types.Add(typeof(T));
        }

        public void AddType<T>(params Expression<Func<T, object>>[] excludeMethods) where T : IController
        {
            _types.Add(typeof(T));
            if (excludeMethods.Length == 0) return;

            foreach (var method in excludeMethods)
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
        }

        #region IFilterCriteria Members

        public bool Match(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            Guard.ArgumentNotNull(controllerContext, "controllerContext");
            Guard.ArgumentNotNull(actionDescriptor, "actionDescriptor");

            IController currentController = controllerContext.Controller;

            if (currentController == null)
                throw new ArgumentException("The Controller is null", "controllerContext");

            if (!_types.Contains(currentController.GetType())) return false; // 类型不匹配
            if (_methods.Count == 0) return true; // 类型匹配，没有需要排除的方法

            var reflectedDescriptor = actionDescriptor as ReflectedActionDescriptor;
            if (reflectedDescriptor != null && _methods.Any(a => a.MethodInfo == reflectedDescriptor.MethodInfo))
                return false; // 某个同步方法匹配，反而不匹配

            var taskDescriptor = actionDescriptor as TaskAsyncActionDescriptor;
            if (taskDescriptor != null && _asyncMethods.Any(a => a.MethodInfo == taskDescriptor.MethodInfo))
                return false; // 某个异步方法匹配，反而不匹配

            return true;
        }

        #endregion

        #region Private Methods
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
