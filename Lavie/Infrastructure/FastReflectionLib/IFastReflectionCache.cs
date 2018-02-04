using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lavie.Infrastructure.FastReflectionLib
{
    public interface IFastReflectionCache<TKey, TValue>
    {
        TValue Get(TKey key);
    }
}
