using System;

namespace Lavie.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsNullOrEmpty(this Guid? source)
        {
            return !source.HasValue || source.Value == Guid.Empty;
        }
    }
}
