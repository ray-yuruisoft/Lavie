using System;
using System.Threading;
using System.Web.Mvc;
using Lavie.Utilities.Exceptions;

namespace Lavie.Infrastructure
{

    public class InProcessBackgroundServiceExecutor : IDisposable, IBackgroundServiceExecutor
    {
        private readonly IDependencyResolver _dependencyResolver;
       
        private readonly TimeSpan _interval;
        private readonly Timer _timer;
        private IBackgroundService _backgroundService;

        public InProcessBackgroundServiceExecutor(IDependencyResolver dependencyResolver, IBackgroundService backgroundService)
        {
            Guard.ArgumentNotNull(dependencyResolver, "dependencyResolver");
            Guard.ArgumentNotNull(backgroundService, "backgroundService");

            _dependencyResolver = dependencyResolver;
            _backgroundService = backgroundService;
            _interval = backgroundService.Interval;
            _timer = new Timer(TimerCallback);
        }

        public BackgroundServiceExecutorStatus Status { get; private set; }

        public void Start()
        {
            lock (_timer)
            {
                if (Status == BackgroundServiceExecutorStatus.Paused || Status == BackgroundServiceExecutorStatus.Stopped)
                {
                    if (Status == BackgroundServiceExecutorStatus.Stopped)
                    {
                        Status = BackgroundServiceExecutorStatus.Starting;
                        _backgroundService.Initialize();
                    }

                    Status = BackgroundServiceExecutorStatus.Running;

                    _backgroundService.Run();

                    _timer.Change(_interval, new TimeSpan(0, 0, 0, 0, -1));
                }
            }
        }

        public void Pause()
        {
            lock (_timer)
            {
                if (Status == BackgroundServiceExecutorStatus.Running)
                {
                    //TODO: (erikpo) Instead of calling Stop, fire a pause event to the currently running background service to give it a chance to hault without stopping

                    Stop();
                }
            }
        }

        public void Stop()
        {
            lock (_timer)
            {
                if (Status != BackgroundServiceExecutorStatus.Stopped)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer.Dispose();

                    _backgroundService = null;

                    Status = BackgroundServiceExecutorStatus.Stopped;
                }
            }
        }

        private void TimerCallback(object state)
        {
            lock (_timer)
            {
                //TODO: (erikpo) Instead of eating the exception, log it
                try
                {
                    _backgroundService.Run();
                }
                catch { }

                _timer.Change(_interval, new TimeSpan(0, 0, 0, 0, -1));
            }

            //TODO: (erikpo) Once background services have a cancel state and timeout interval, check their state and cancel if appropriate
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    Stop();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~InProcessBackgroundServiceExecutor() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}