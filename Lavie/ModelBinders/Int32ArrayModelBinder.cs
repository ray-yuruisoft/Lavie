using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lavie.ModelBinders
{
    /// <summary>
    /// 解析逗号分隔的Int32列表
    /// 如：Data=1,2&Data=4,4,5
    /// 注意：对于 JSON 格式，不能使用该 ModelBinder
    /// </summary>
    public class Int32ArrayModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = new List<int>(0);
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != null)
            {
                var values = valueProviderResult.RawValue as string[];
                if (values != null && values.Length > 0)
                {
                    foreach (var value in values)
                    {
                        var array = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                        if (array.Length > 0)
                        {
                            int tryInt = 0;
                            var arrayInt = from i in array
                                where Int32.TryParse(i, out tryInt)
                                select tryInt;
                            if (arrayInt.Any())
                            {
                                result.AddRange(from i in array
                                                where Int32.TryParse(i, out tryInt)
                                                select tryInt);
                            }
                        }

                    }
                }
            }

            return result.ToArray();
        }
    }
}