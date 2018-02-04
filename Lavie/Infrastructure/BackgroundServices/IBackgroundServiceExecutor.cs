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