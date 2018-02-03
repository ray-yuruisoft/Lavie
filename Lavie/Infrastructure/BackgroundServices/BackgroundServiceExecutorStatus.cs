using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Infrastructure
{
    public enum BackgroundServiceExecutorStatus
    {
        Stopped,
        Starting,
        Running,
        Paused
    }
}
