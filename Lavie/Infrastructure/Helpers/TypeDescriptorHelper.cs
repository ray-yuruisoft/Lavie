using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lavie.Infrastructure.Helpers
{
    internal static class TypeDescriptorHelper
    {
        public static ICustomTypeDescriptor Get(Type type)
        {
            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        }

    }
}
