using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using SharpMap.Data.Providers;

namespace SharpMap.Tests.Data.Providers
{

    [TestFixture]
    public class AsyncResultTest
    {
        [Test]
        public void Constructs()
        {
            AsyncResult ar = new AsyncResult(null, this);
        }

        [Test]
        public void GetState()
        {
            AsyncResult ar = new AsyncResult(null, this);

            Assert.AreSame(this, ar.AsyncState);
        }

        [Test]
        public void CallBack()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            AsyncResult ar = new AsyncResult(callBackRecorder.CallBack, this);
            ar.SetComplete();

            Assert.AreSame(ar, callBackRecorder.Result);
        }

        [Test]
        public void Incomplete()
        {
            AsyncResult ar = new AsyncResult(null, this);

            Assert.IsFalse(ar.IsCompleted);
        }

        [Test]
        public void Completed()
        {
            AsyncResult ar = new AsyncResult(null, this);
            ar.SetComplete();

            Assert.IsTrue(ar.IsCompleted);
        }

        [Test]
        public void CompletedSynchronouslyPropertyFalse()
        {
            AsyncResult ar = new AsyncResult(null, this);
            ar.SetComplete();

            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);
        }

        [Test]
        public void CompletedSynchronouslyProperty()
        {
            AsyncResult ar = new AsyncResult(null, this);
            ar.SetComplete(true, null);

            Assert.IsTrue(ar.IsCompleted);
            Assert.IsTrue(ar.CompletedSynchronously);
        }

        [Test]
        public void AsyncWaitHandle()
        {
            AsyncResult ar = new AsyncResult(null, this);

            Assert.IsNotNull(ar.AsyncWaitHandle);
        }

        [Test]
        public void AsyncCallBackNotification()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            AsynchronousTask task = new AsynchronousTask(1);

            IAsyncResult ar = task.BeginDoTask(callBackRecorder.CallBack, this);
            Thread.Sleep(2000);

            Assert.AreSame(ar, callBackRecorder.Result);
            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);
            
            task.EndDoTask(ar);  // should not throw
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException),ExpectedMessage = "Failure succeeded")]
        public void AsyncCallBackNotificationWithException()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask task = new AsynchronousTask(1, terminatingException);

            IAsyncResult ar = task.BeginDoTask(callBackRecorder.CallBack, this);
            Thread.Sleep(2000);

            Assert.AreSame(ar, callBackRecorder.Result);
            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);
        
            task.EndDoTask(ar);  // should throw
        }


        [Test]
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

            Assert.Greater(count, 0);
            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);

            task.EndDoTask(ar);  // should not throw
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Failure succeeded")]
        public void AsyncPollingNotificationWithException()
        {
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask task = new AsynchronousTask(1, terminatingException);

            IAsyncResult ar = task.BeginDoTask(null, this);

            while (!ar.IsCompleted)
            {
                Thread.Sleep(500);
            }

            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);

            task.EndDoTask(ar);  // should throw
        }

        [Test]
        public void AsyncBlockUntilComplete()
        {
            AsynchronousTask task = new AsynchronousTask(1);
            IAsyncResult ar = task.BeginDoTask(null, this);

            Stopwatch timer = Stopwatch.StartNew();
            task.EndDoTask(ar);
            timer.Stop();

            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);
            Assert.IsTrue(timer.ElapsedMilliseconds > 500L);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Failure succeeded")]
        public void AsyncBlockUntilCompleteWithException()
        {
            InvalidOperationException terminatingException = new InvalidOperationException("Failure succeeded");
            AsynchronousTask task = new AsynchronousTask(1, terminatingException);
            IAsyncResult ar = task.BeginDoTask(null, this);

            task.EndDoTask(ar);  // should throw
        }

    }
}