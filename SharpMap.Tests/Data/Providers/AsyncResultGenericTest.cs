using System;
using System.Diagnostics;
using System.Threading;

using SharpMap.Data.Providers;
using Xunit;

namespace SharpMap.Tests.Data.Providers
{
    public class AsyncResultGenericTest
    {
        [Fact]
        public void Constructs()
        {
            AsyncResult<String> ar = new AsyncResult<String>(null, this);
        }

        [Fact]
        public void GetState()
        {
            AsyncResult<String> ar = new AsyncResult<String>(null, this);

            Assert.Same(this, ar.AsyncState);
        }

        [Fact]
        public void AsyncCallBackNotification()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            String expectedResult = "Result we expected";
            AsynchronousTask<String> task = new AsynchronousTask<String>(expectedResult, 1);

            IAsyncResult ar = task.BeginDoTask(callBackRecorder.CallBack, this);
            Thread.Sleep(2000);

            Assert.Same(ar, callBackRecorder.Result);
            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);
            
            String actualResult = task.EndDoTask(ar);  // should not throw
            Assert.Same(expectedResult, actualResult);
        }

        [Fact]
        public void AsyncCallBackNotificationWithException()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask<String> task = new AsynchronousTask<String>(1, terminatingException);

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
            String expectedResult = "Result we expected";
            AsynchronousTask<String> task = new AsynchronousTask<String>(expectedResult, 1);

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

            String actualResult = task.EndDoTask(ar);  // should not throw
            Assert.Same(expectedResult, actualResult);
        }

        [Fact]
        public void AsyncPollingNotificationWithException()
        {
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask<String> task = new AsynchronousTask<String>(1, terminatingException);

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
            String expectedResult = "Result we expected";
            AsynchronousTask<String> task = new AsynchronousTask<String>(expectedResult, 1);
            IAsyncResult ar = task.BeginDoTask(null, this);

            Stopwatch timer = Stopwatch.StartNew();
            String actualResult = task.EndDoTask(ar);
            timer.Stop();

            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);
            Assert.Same(expectedResult, actualResult);
            Assert.True(timer.ElapsedMilliseconds > 500L);
        }

        [Fact]
        public void AsyncBlockUntilCompleteWithException()
        {
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask<String> task = new AsynchronousTask<String>(1, terminatingException);
            IAsyncResult ar = task.BeginDoTask(null, this);

            Assert.Throws<InvalidOperationException>(delegate { task.EndDoTask(ar); });
        }

    }
}