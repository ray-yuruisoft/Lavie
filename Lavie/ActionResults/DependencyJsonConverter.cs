using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Infrastructure.FastReflectionLib;

namespace Lavie.ActionResults
{
    public class DependencyJsonConverterGuid : DependencyJsonConverter<Guid>
    {
        public DependencyJsonConverterGuid(string propertyName, string equaValue) : base(propertyName, new Guid(equaValue))
        {
        }
    }

    public class DependencyJsonConverter<T> : JsonConverter where T: IEquatable<T>
    {
        private readonly string _propertyName;
        private readonly T _equalValue;
        public DependencyJsonConverter(string propertyName, T equalValue)
        {
            _propertyName = propertyName;
            _equalValue = equalValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            var propertyInfo = value.GetType().GetProperty(_propertyName);
            var accessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);

            IEquatable<T> pValue = (IEquatable<T>)accessor.GetValue(value);
            if (pValue.Equals(_equalValue))
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, value);
        }

    }
}
