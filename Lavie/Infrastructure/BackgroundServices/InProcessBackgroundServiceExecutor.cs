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
        private bool disposedValue = false; // Ҫ����������

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: �ͷ��й�״̬(�йܶ���)��
                    Stop();
                }

                // TODO: �ͷ�δ�йܵ���Դ(δ�йܵĶ���)������������������ս�����
                // TODO: �������ֶ�����Ϊ null��

                disposedValue = true;
            }
        }

        // TODO: �������� Dispose(bool disposing) ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս�����
        // ~InProcessBackgroundServiceExecutor() {
        //   // ������Ĵ˴��롣���������������� Dispose(bool disposing) �С�
        //   Dispose(false);
        // }

        // ��Ӵ˴�������ȷʵ�ֿɴ���ģʽ��
        public void Dispose()
        {
            // ������Ĵ˴��롣���������������� Dispose(bool disposing) �С�
            Dispose(true);
            // TODO: ���������������������ս�������ȡ��ע�������С�
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}