using System;
using System.Data;
using System.Reflection;
using Lavie.Infrastructure.FastReflectionLib;

namespace Lavie.Extensions
{
    public static class DataReaderExtensions
    {
        public static string GetNullableString(this IDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
                return reader.GetString(index);
            else
                return null;
        }
        public static T ToModel<T>(this IDataReader reader) where T : new()
        {
            if (reader == null) return default(T);

            Type type = typeof(T);
            T instance = new T();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                PropertyInfo propertyInfo = type.GetProperty(reader.GetName(i), BindingFlags.Instance | BindingFlags.Public);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    var instancePropertyAccessor = new PropertyAccessor(propertyInfo);
                    if (!reader.IsDBNull(i))
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                            instancePropertyAccessor.SetValue(instance, Enum.ToObject(propertyInfo.PropertyType, reader.GetValue(i)));
                        else
                            instancePropertyAccessor.SetValue(instance, reader.GetValue(i));
                    }
                }
            }
            return instance;
        }
    }

}
