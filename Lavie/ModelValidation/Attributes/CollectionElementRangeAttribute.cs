using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.ModelValidation.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CollectionElementRangeAttribute : ValidationAttribute
    {
        public int Minimum { get; private set; }
        public int Maximum { get; private set; }

        public CollectionElementRangeAttribute(int minimum, int maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        public override bool IsValid(object value)
        {
            var list = value as ICollection;
            if (list != null)
            {
                return list.Count >= Minimum && list.Count <= Maximum;
            }
            return false;
        }
    }

    // MinLengthAttribute

}
