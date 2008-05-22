using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using SharpMap.Data.Providers;

namespace SharpMap.Tests.Data.Providers
{

    [TestFixture]
    public class AsyncResultGenericTest
    {
        [Test]
        public void Constructs()
        {
            AsyncResult<String> ar = new AsyncResult<String>(null, this);
        }

        [Test]
        public void GetState()
        {
            AsyncResult<String> ar = new AsyncResult<String>(null, this);

            Assert.AreSame(this, ar.AsyncState);
        }

        [Test]
        public void AsyncCallBackNotification()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            String expectedResult = "Result we expected";
            AsynchronousTask<String> task = new AsynchronousTask<String>(expectedResult, 1);

            IAsyncResult ar = task.BeginDoTask(callBackRecorder.CallBack, this);
            Thread.Sleep(2000);

            Assert.AreSame(ar, callBackRecorder.Result);
            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);
            
            String actualResult = task.EndDoTask(ar);  // should not throw
            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException),ExpectedMessage = "Failure succeeded")]
        public void AsyncCallBackNotificationWithException()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask<String> task = new AsynchronousTask<String>(1, terminatingException);

            IAsyncResult ar = task.BeginDoTask(callBackRecorder.CallBack, this);
            Thread.Sleep(2000);

            Assert.AreSame(ar, callBackRecorder.Result);
            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);

            String actualResult = task.EndDoTask(ar);  // should throw
        }


        [Test]
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

            Assert.Greater(count, 0);
            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);

            String actualResult = task.EndDoTask(ar);  // should not throw
            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Failure succeeded")]
        public void AsyncPollingNotificationWithException()
        {
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask<String> task = new AsynchronousTask<String>(1, terminatingException);

            IAsyncResult ar = task.BeginDoTask(null, this);

            while (!ar.IsCompleted)
            {
                Thread.Sleep(500);
            }

            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);

            String actualResult = task.EndDoTask(ar);  // should throw
        }

        [Test]
        public void AsyncBlockUntilComplete()
        {
            String expectedResult = "Result we expected";
            AsynchronousTask<String> task = new AsynchronousTask<String>(expectedResult, 1);
            IAsyncResult ar = task.BeginDoTask(null, this);

            Stopwatch timer = Stopwatch.StartNew();
            String actualResult = task.EndDoTask(ar);
            timer.Stop();

            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);
            Assert.AreSame(expectedResult, actualResult);
            Assert.IsTrue(timer.ElapsedMilliseconds > 500L);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Failure succeeded")]
        public void AsyncBlockUntilCompleteWithException()
        {
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask<String> task = new AsynchronousTask<String>(1, terminatingException);
            IAsyncResult ar = task.BeginDoTask(null, this);

            String actualResult = task.EndDoTask(ar);  // should throw
        }

    }
}