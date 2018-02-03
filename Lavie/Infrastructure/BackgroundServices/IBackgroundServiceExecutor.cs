using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Infrastructure
{
    public interface IBackgroundServiceExecutor
    {
        BackgroundServiceExecutorStatus Status { get; }
        void Start();
        void Pause();
        void Stop();
    }
}
