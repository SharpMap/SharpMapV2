using System;
using System.Threading;
using SharpMap.Data.Providers;

namespace SharpMap.Tests.Data.Providers
{
    internal class AsynchronousTask
    {
        private readonly Int32 _wait;
        private readonly Exception _terminatingException;

        public AsynchronousTask(Int32 seconds)
        {
            _wait = seconds * 1000;
        }

        public AsynchronousTask(Int32 seconds, Exception terminatingException)
            : this(seconds)
        {
            _terminatingException = terminatingException;
        }

        public virtual IAsyncResult BeginDoTask(AsyncCallback callback, Object state)
        {
            AsyncResult ar = new AsyncResult(callback, state);

            if (_wait > 0)
            {
                ThreadPool.QueueUserWorkItem(QueueableDoTask, ar);
            }
            else
            {
                ar.SetComplete(true, _terminatingException);
            }
            return ar;
        }

        public void EndDoTask(IAsyncResult asyncResult)
        {
            AsyncResult ar = asyncResult as AsyncResult;
            if (ar == null)
                throw new ArgumentException("Non-matching result type", "asyncResult");
            ar.EndInvoke();
        }

        private void QueueableDoTask(Object asyncResult)
        {
            AsyncResult ar = asyncResult as AsyncResult;

            if (_terminatingException != null)
            {
                ar.SetComplete(false, _terminatingException);
            }
            else
            {
                    Thread.Sleep(_wait);
                    ar.SetComplete();
            }
        }
    }
}