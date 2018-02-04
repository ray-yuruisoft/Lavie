using Lavie.Configuration;
using System;

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