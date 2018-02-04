using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lavie.ModelBinders
{
    /// <summary>
    /// 解析逗号分隔的Guid列表
    /// 注意：对于 JSON 格式，不能使用该 ModelBinder
    /// </summary>
    public class GuidArrayModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = new List<Guid>(0);
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != null)
            {
                var values = valueProviderResult.RawValue as string[];
                if (values != null && values.Length > 0)
                {
                    foreach (var value in values)
                    {
                        var array = value.Split(',');
                        Guid tryGuid = Guid.Empty;
                        result.AddRange(from i in array
                                        where Guid.TryParse(i, out tryGuid)
                                        select tryGuid);
                    }
                }
            }

            return result.ToArray();

        }
    }
}