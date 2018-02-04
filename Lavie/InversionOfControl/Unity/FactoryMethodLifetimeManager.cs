using System;
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
