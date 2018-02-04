using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lavie.ModelBinders
{
    /// <summary>
    /// 解析分隔分隔的键值对。键值类型都为String，以分号分隔
    /// 如：Data=1,2;3,4;5,6，不支持Data=1,2;3,4;5,6&Data=7,8;9,10;11,12
    /// </summary>
    public class StringPairArrayModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != null)
            {
                var value = valueProviderResult.AttemptedValue;
                if (value != null)
                {
                    Boolean isSuccessed = true;
                    var itemArray = value.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries);
                    if (itemArray.Length > 0)
                    {
                        var result = new List<Tuple<String, String>>(itemArray.Length);
                        for (int i = 0; i < itemArray.Length; i++)
                        {
                            var keyValueArray = itemArray[i].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                            if (keyValueArray.Length != 2)
                            {
                                isSuccessed = false;
                                break;
                            }
                            result.Add(new Tuple<String, String>(keyValueArray[0], keyValueArray[1]));

                        }
                        if (isSuccessed)
                        {
                            return result.ToArray();
                        }
                    }

                }
            }
            return new Tuple<String, String>[0];
        }
    }
}