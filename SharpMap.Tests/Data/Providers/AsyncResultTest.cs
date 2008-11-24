using System;
using System.Diagnostics;
using System.Threading;

using SharpMap.Data.Providers;
using Xunit;

namespace SharpMap.Tests.Data.Providers
{
    public class AsyncResultTest
    {
        [Fact]
        public void Constructs()
        {
            AsyncResult ar = new AsyncResult(null, this);
        }

        [Fact]
        public void GetState()
        {
            AsyncResult ar = new AsyncResult(null, this);

            Assert.Same(this, ar.AsyncState);
        }

        [Fact]
        public void CallBack()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            AsyncResult ar = new AsyncResult(callBackRecorder.CallBack, this);
            ar.SetComplete();

            Assert.Same(ar, callBackRecorder.Result);
        }

        [Fact]
        public void Incomplete()
        {
            AsyncResult ar = new AsyncResult(null, this);

            Assert.False(ar.IsCompleted);
        }

        [Fact]
        public void Completed()
        {
            AsyncResult ar = new AsyncResult(null, this);
            ar.SetComplete();

            Assert.True(ar.IsCompleted);
        }

        [Fact]
        public void CompletedSynchronouslyPropertyFalse()
        {
            AsyncResult ar = new AsyncResult(null, this);
            ar.SetComplete();

            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);
        }

        [Fact]
        public void CompletedSynchronouslyProperty()
        {
            AsyncResult ar = new AsyncResult(null, this);
            ar.SetComplete(true, null);

            Assert.True(ar.IsCompleted);
            Assert.True(ar.CompletedSynchronously);
        }

        [Fact]
        public void AsyncWaitHandle()
        {
            AsyncResult ar = new AsyncResult(null, this);

            Assert.NotNull(ar.AsyncWaitHandle);
        }

        [Fact]
        public void AsyncCallBackNotification()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            AsynchronousTask task = new AsynchronousTask(1);

            IAsyncResult ar = task.BeginDoTask(callBackRecorder.CallBack, this);
            Thread.Sleep(2000);

            Assert.Same(ar, callBackRecorder.Result);
            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);
            
            task.EndDoTask(ar);  // should not throw
        }

        [Fact]
        public void AsyncCallBackNotificationWithException()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask task = new AsynchronousTask(1, terminatingException);

            IAsyncResult ar = task.BeginDoTask(callBackRecorder.CallBack, this);
            Thread.Sleep(2000);

            Assert.Same(ar, callBackRecorder.Result);
            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);

            Assert.Throws<InvalidOperationException>(delegate { task.EndDoTask(ar); });
        }


        [Fact]
        public void AsyncPollingNotification()
        {
            AsynchronousTask task = new AsynchronousTask(1);

            IAsyncResult ar = task.BeginDoTask(null, this);

            Int32 count = 0;
            while (!ar.IsCompleted)
            {
                count++;
                Thread.Sleep(500);
            }

            Assert.True(count > 0);
            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);

            task.EndDoTask(ar);  // should not throw
        }

        [Fact]
        public void AsyncPollingNotificationWithException()
        {
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask task = new AsynchronousTask(1, terminatingException);

            IAsyncResult ar = task.BeginDoTask(null, this);

            while (!ar.IsCompleted)
            {
                Thread.Sleep(500);
            }

            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);

            Assert.Throws<InvalidOperationException>(delegate { task.EndDoTask(ar); });
        }

        [Fact]
        public void AsyncBlockUntilComplete()
        {
            AsynchronousTask task = new AsynchronousTask(1);
            IAsyncResult ar = task.BeginDoTask(null, this);

            Stopwatch timer = Stopwatch.StartNew();
            task.EndDoTask(ar);
            timer.Stop();

            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);
            Assert.True(timer.ElapsedMilliseconds > 500L);
        }

        [Fact]
        public void AsyncBlockUntilCompleteWithException()
        {
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask task = new AsynchronousTask(1, terminatingException);
            IAsyncResult ar = task.BeginDoTask(null, this);

            Assert.Throws<InvalidOperationException>(delegate { task.EndDoTask(ar); });
        }

    }
}