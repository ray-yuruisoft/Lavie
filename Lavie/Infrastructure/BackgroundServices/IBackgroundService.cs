using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Infrastructure
{
    public interface IBackgroundService
    {
        string ServiceName { get; set; }
        AppSettingsHelper Settings { get; set; }
        TimeSpan Interval { get; set; }
        void Initialize();
        void Run();
        void Unload();
    }
}
