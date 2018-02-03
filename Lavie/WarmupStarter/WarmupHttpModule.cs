using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Lavie.WarmupStarter
{
    public class WarmupHttpModule : IHttpModule
    {
        private HttpApplication _context;
        private static object _synLock = new object();
        private static IList<Action> _awaiting = new List<Action>();

        public void Init(HttpApplication context)
        {
            _context = context;
            context.AddOnBeginRequestAsync(BeginBeginRequest, EndBeginRequest, null);
        }

        public void Dispose()
        {
        }

        private static bool InWarmup()
        {
            lock (_synLock)
            {
                return _awaiting != null;
            }
        }

        /// <summary>
        /// Warmup code is about to start: Any new incoming request is queued 
        /// until "SignalWarmupDone" is called.
        /// </summary>
        public static void SignalWarmupStart()
        {
            lock (_synLock)
            {
                if (_awaiting == null)
                {
                    _awaiting = new List<Action>();
                }
            }
        }

        /// <summary>
        /// Warmup code just completed: All pending requests in the "_await" queue are processed, 
        /// and any new incoming request is now processed immediately.
        /// </summary>
        public static void SignalWarmupDone()
        {
            IList<Action> temp;

            lock (_synLock)
            {
                temp = _awaiting;
                _awaiting = null;
            }

            if (temp != null)
            {
                foreach (var action in temp)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Enqueue or directly process action depending on current mode.
        /// </summary>
        private void Await(Action action)
        {
            Action temp = action;

            lock (_synLock)
            {
                if (_awaiting != null)
                {
                    temp = null;
                    _awaiting.Add(action);
                }
            }

            if (temp != null)
            {
                temp();
            }
        }

        private IAsyncResult BeginBeginRequest(object sender, EventArgs e, AsyncCallback cb, object extradata)
        {
            // host is available, process every requests, or file is processed
            if (!InWarmup() || WarmupUtility.DoBeginRequest(_context))
            {
                var asyncResult = new DoneAsyncResult(extradata);
                cb(asyncResult);
                return asyncResult;
            }
            else
            {
                // this is the "on hold" execution path
                var asyncResult = new WarmupAsyncResult(cb, extradata);
                Await(asyncResult.Completed);
                return asyncResult;
            }
        }

        private static void EndBeginRequest(IAsyncResult ar)
        {
        }

        /// <summary>
        /// AsyncResult for "on hold" request (resumes when "Completed()" is called)
        /// </summary>
        private class WarmupAsyncResult : IAsyncResult, IDisposable
        {
            private readonly EventWaitHandle _eventWaitHandle = new AutoResetEvent(false/*initialState*/);
            private readonly AsyncCallback _cb;
            private readonly object _asyncState;
            private bool _isCompleted;

            public WarmupAsyncResult(AsyncCallback cb, object asyncState)
            {
                _cb = cb;
                _asyncState = asyncState;
                _isCompleted = false;
            }

            public void Completed()
            {
                _isCompleted = true;
                _eventWaitHandle.Set();
                _cb(this);
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return false; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return _isCompleted; }
            }

            object IAsyncResult.AsyncState
            {
                get { return _asyncState; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return _eventWaitHandle; }
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
                        if (_eventWaitHandle != null)
                        {
                            _eventWaitHandle.Dispose();
                        }
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                    // TODO: 将大型字段设置为 null。

                    disposedValue = true;
                }
            }

            // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
            //~WarmupAsyncResult()
            //{
            //    // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            //    Dispose(false);
            //}

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

        /// <summary>
        /// Async result for "ok to process now" requests
        /// </summary>
        private class DoneAsyncResult : IAsyncResult
        {
            private readonly object _asyncState;
            private static readonly WaitHandle _waitHandle = new ManualResetEvent(true/*initialState*/);

            public DoneAsyncResult(object asyncState)
            {
                _asyncState = asyncState;
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return true; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return true; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return _waitHandle; }
            }

            object IAsyncResult.AsyncState
            {
                get { return _asyncState; }
            }
        }
    }
}
