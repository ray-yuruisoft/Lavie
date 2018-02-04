using System;
using Lavie.Configuration;

namespace Lavie.Configuration.Extensions
{
    public static class LavieModuleConfigurationElementExtensions
    {
        public static LavieModuleConfigurationElement Module(this LavieModuleConfigurationElementCollection modules, Type moduleType)
        {
            foreach (LavieModuleConfigurationElement element in modules)
                if (element.Type.StartsWith(moduleType.FullName))
                    return element;

            return null;
        }
    }
}