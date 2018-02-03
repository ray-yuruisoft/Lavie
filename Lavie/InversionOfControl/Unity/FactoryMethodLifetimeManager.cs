using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Lifetime;

namespace Lavie.InversionOfControl.Unity
{
    public class FactoryMethodLifetimeManager : LifetimeManager
    {
        private readonly Func<object> _getValue;

        public FactoryMethodLifetimeManager(Func<object> getValue)
        {
            this._getValue = getValue;
        }

        public override object GetValue()
        {
            return _getValue();
        }

        public override void RemoveValue()
        {
        }

        public override void SetValue(object newValue)
        {
        }
    }
}
