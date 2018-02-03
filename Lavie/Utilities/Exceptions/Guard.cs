using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Utilities.Exceptions
{
    public static class Guard
    {
        //Guard.ArgumentNotNull(any, "any");
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
        //Guard.ArgumentNotNullOrEmpty(any, "any");
        public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
            if (argumentValue.Length == 0)
            {
                throw new ArgumentException("The provided string argument must be not empty.", argumentName);
            }
        }

    }

}
