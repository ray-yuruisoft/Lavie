using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lavie.ModelBinders
{
    /// <summary>
    /// 解析逗号分隔的Int32列表
    /// 如：Data=1,2&Data=3,4&Data=5,6
    /// </summary>
    public class Int32KeyValuePairArrayModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != null)
            {
                var array = valueProviderResult.RawValue as string[];
                if (array != null&&array.Length>1)
                {
                    var result = new List<KeyValuePair<int, int>>(array.Length / 2);
                    for (int i = 0; i < array.Length - 1; i = i + 2)
                    {
                        int tryInt1;
                        int tryInt2;
                        if (Int32.TryParse(array[i], out tryInt1) && Int32.TryParse(array[i + 1], out tryInt2))
                        {
                            result.Add(new KeyValuePair<int, int>(tryInt1, tryInt2));
                        }
                    }
                }
            }

            return new KeyValuePair<int, int>[0];
        }
    }
}