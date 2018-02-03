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
            _getValue = getValue;
        }

        public override object GetValue(ILifetimeContainer container = null)
        {
            return _getValue();
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return this;
        }
    }
}
