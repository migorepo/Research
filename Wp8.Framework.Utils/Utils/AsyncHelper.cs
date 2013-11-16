using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wp8.Framework.Utils.Utils
{
    #region from http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx Building Async Coordination Primitives
    public class AsyncSemaphore
    {
        private readonly static Task SCompleted = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _mWaiters = new Queue<TaskCompletionSource<bool>>();
        private int _mCurrentCount;


        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            _mCurrentCount = initialCount;
        }



        public Task WaitAsync()
        {
            lock (_mWaiters)
            {
                if (_mCurrentCount > 0)
                {
                    --_mCurrentCount;
                    return SCompleted;
                }
                var waiter = new TaskCompletionSource<bool>();
                _mWaiters.Enqueue(waiter);
                return waiter.Task;
            }
        }
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (_mWaiters)
            {
                if (_mWaiters.Count > 0)
                    toRelease = _mWaiters.Dequeue();
                else
                    ++_mCurrentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
    public class AsyncLock
    {


        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _mToRelease;

            internal Releaser(AsyncLock toRelease) { _mToRelease = toRelease; }

            public void Dispose()
            {
                if (_mToRelease != null)
                    _mToRelease._mSemaphore.Release();
            }
        }

        private readonly AsyncSemaphore _mSemaphore;
        private readonly Task<Releaser> _mReleaser;


        public AsyncLock()
        {
            _mSemaphore = new AsyncSemaphore(1);
            _mReleaser = Task.FromResult(new Releaser(this));
        }
        public Task<Releaser> LockAsync()
        {
            var wait = _mSemaphore.WaitAsync();
            return wait.IsCompleted ?
                _mReleaser :
                wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }


    }
    #endregion
}

