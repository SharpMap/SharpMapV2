using System;
using System.Threading;
using SharpMap.Data.Providers;

namespace SharpMap.Tests.Data.Providers
{
    internal class AsynchronousTask<TResult>
    {
        private readonly Int32 _wait;
        private readonly Exception _terminatingException;
        private readonly TResult _result;

        public AsynchronousTask(TResult result, Int32 seconds)
        {
            _wait = seconds * 1000;
            _result = result;
        }

        public AsynchronousTask(Int32 seconds, Exception terminatingException)
        {
            _wait = seconds * 1000;
            _terminatingException = terminatingException;
        }

        public IAsyncResult BeginDoTask(AsyncCallback callback, Object state)
        {
            AsyncResult<TResult> ar = new AsyncResult<TResult>(callback, state);

            if (_wait > 0)
            {
                ThreadPool.QueueUserWorkItem(QueueableDoTask, ar);
            }
            else
            {
                if (_terminatingException == null)
                {
                    ar.SetComplete(_result, true);
                }
                else
                {
                    ar.SetComplete(true, _terminatingException);                    
                }
            }
            return ar;
        }

        public TResult EndDoTask(IAsyncResult asyncResult)
        {
            AsyncResult<TResult> ar = asyncResult as AsyncResult<TResult>;
            if (ar == null)
                throw new ArgumentException("Non-matching result type", "asyncResult");

            return ar.EndInvoke();
        }

        private void QueueableDoTask(Object asyncResult)
        {
            AsyncResult<TResult> ar = asyncResult as AsyncResult<TResult>;

            if (_terminatingException != null)
            {
                ar.SetComplete(false, _terminatingException);
            }
            else
            {
                    Thread.Sleep(_wait);
                    ar.SetComplete(_result, false);
            }
        }
    }
}