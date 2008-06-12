using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Expressions;

namespace SharpMap.Tests.Data.Providers
{
    [TestFixture]
    public class AsyncRasterProviderAdapterTests
    {
        private Stream _returnStreamStub;
        private Int32 _sleepInterval;

        [TearDown]
        public void TearDown()
        {
            _returnStreamStub = null;
            _sleepInterval = 0;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorFailsOnNullProvider()
        {
            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(null);
        }

        [Test]
        public void ConstructorSucceeds()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
        }

        [Test]
        public void IProviderMethodsPassThroughAdapter()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();
            ICoordinateTransformation transformStub =
                MockRepository.GenerateStub<ICoordinateTransformation>();
            Expression queryStub =
                MockRepository.GenerateStub<Expression>();
            Object objectStub =
                MockRepository.GenerateStub<Object>();
            IExtents extentsStub =
                MockRepository.GenerateStub<IExtents>();
            ICoordinateSystem coordinateSystemStub =
                MockRepository.GenerateStub<ICoordinateSystem>();

            using (mocks.Unordered())
            {
                adapted.Close();
                Expect.Call(adapted.ConnectionId)
                    .Return("connectionid");
                adapted.CoordinateTransformation = transformStub;
                Expect.Call(adapted.CoordinateTransformation)
                    .Return(transformStub);
                Expect.Call(adapted.ExecuteQuery(queryStub))
                    .Return(objectStub);
                Expect.Call(adapted.GetExtents())
                    .Return(extentsStub);
                Expect.Call(adapted.IsOpen)
                    .Return(true);
                adapted.Open();
                Expect.Call(adapted.SpatialReference)
                    .Return(coordinateSystemStub);
                //adapted.Srid = 1;
                Expect.Call(adapted.Srid)
                    .Return(2);
            }
            mocks.ReplayAll();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            adapter.Close();
            Assert.AreEqual("connectionid", adapter.ConnectionId);
            adapter.CoordinateTransformation = transformStub;
            Assert.AreSame(transformStub, adapter.CoordinateTransformation);
            Assert.AreSame(objectStub, adapter.ExecuteQuery(queryStub));
            Assert.AreSame(extentsStub, adapter.GetExtents());
            Assert.IsTrue(adapter.IsOpen);
            adapter.Open();
            Assert.AreSame(coordinateSystemStub, adapter.SpatialReference);
            //adapter.Srid = 1;
            Assert.AreEqual(2, adapter.Srid);

            mocks.VerifyAll();
        }

        [Test]
        public void IDisposableMethodsPassThroughAdapter()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();

            using (mocks.Unordered())
            {
                adapted.Dispose();
            }
            mocks.ReplayAll();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            adapter.Dispose();

            mocks.VerifyAll();
        }

        [Test]
        public void IRasterProviderMethodsPassThroughAdapter()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();
            Stream streamStub =
                MockRepository.GenerateStub<Stream>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            RasterQueryExpression rasterQueryExpressionStub =
                MockRepository.GenerateStub<RasterQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteRasterQuery(rasterQueryExpressionStub))
                    .Return(streamStub);
            }
            mocks.ReplayAll();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            Assert.AreSame(streamStub, adapter.ExecuteRasterQuery(rasterQueryExpressionStub));

            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BeginExecuteQueryThrowsOnInvalidExpression()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = MockRepository.GenerateStub<IRasterProvider>();
            Expression expressionStub =
                MockRepository.GenerateStub<Expression>();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            adapter.BeginExecuteQuery(expressionStub, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void EndExecuteQueryThrowsOnInvalidExpression()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = MockRepository.GenerateStub<IRasterProvider>();
            AsyncResult result = new AsyncResult(null, this);

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            adapter.EndExecuteQuery(result);
        }


        private delegate Stream ExecuteQueryDelegate(RasterQueryExpression query);
        private Stream executeQueryMock(RasterQueryExpression query)
        {
            Thread.Sleep(_sleepInterval);
            return _returnStreamStub;
        }

        [Test]
        public void BeginExecuteQueryCallBackNotification()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();

            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            RasterQueryExpression RasterQueryExpressionStub =
                MockRepository.GenerateStub<RasterQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            _returnStreamStub = MockRepository.GenerateStub<Stream>();
            _sleepInterval = 500;

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteRasterQuery(RasterQueryExpressionStub))
                    .Do(new ExecuteQueryDelegate(executeQueryMock));
            }
            mocks.ReplayAll();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            IAsyncResult ar = adapter.BeginExecuteQuery(RasterQueryExpressionStub, callBackRecorder.CallBack);

            Assert.IsNotNull(ar);
            Assert.IsFalse(ar.IsCompleted);

            Thread.Sleep(1000);

            Assert.AreSame(ar, callBackRecorder.Result);
            Assert.IsTrue(ar.IsCompleted);
            Assert.AreSame(_returnStreamStub, adapter.EndExecuteQuery(ar));

            mocks.VerifyAll();
        }

        [Test]
        public void BeginExecuteQueryPollingNotification()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            RasterQueryExpression RasterQueryExpressionStub =
                MockRepository.GenerateStub<RasterQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            _returnStreamStub = MockRepository.GenerateStub<Stream>();
            _sleepInterval = 1000;

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteRasterQuery(RasterQueryExpressionStub))
                    .Do(new ExecuteQueryDelegate(executeQueryMock));
            }
            mocks.ReplayAll();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            IAsyncResult ar = adapter.BeginExecuteQuery(RasterQueryExpressionStub, null);

            Assert.IsNotNull(ar);
            Assert.IsFalse(ar.IsCompleted);
            while (!ar.IsCompleted)
            {
                Thread.Sleep(350);
            }

            Assert.IsTrue(ar.IsCompleted);
            Assert.AreSame(_returnStreamStub, adapter.EndExecuteQuery(ar));

            mocks.VerifyAll();
        }
        
        [Test]
        public void BeginExecuteQueryBlockUntilDone()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            RasterQueryExpression RasterQueryExpressionStub =
                MockRepository.GenerateStub<RasterQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            _returnStreamStub = MockRepository.GenerateStub<Stream>();
            _sleepInterval = 750;

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteRasterQuery(RasterQueryExpressionStub))
                    .Do(new ExecuteQueryDelegate(executeQueryMock));
            }
            mocks.ReplayAll();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            IAsyncResult ar = adapter.BeginExecuteQuery(RasterQueryExpressionStub, null);

            Assert.IsNotNull(ar);

            Stopwatch timer = Stopwatch.StartNew();
            Assert.AreSame(_returnStreamStub, adapter.EndExecuteQuery(ar));
            timer.Stop();

            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);
            Assert.IsTrue(timer.ElapsedMilliseconds > 500L);

            mocks.VerifyAll();
        }

        private Stream executeQueryExceptionMock(RasterQueryExpression query)
        {
            Thread.Sleep(100);
            throw new InvalidOperationException("Failure succeeded");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Failure succeeded")]
        public void EndExecuteQueryThrowsTerminatingException()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            RasterQueryExpression RasterQueryExpressionStub =
                MockRepository.GenerateStub<RasterQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteRasterQuery(RasterQueryExpressionStub))
                    .Do(new ExecuteQueryDelegate(executeQueryExceptionMock));
            }
            mocks.ReplayAll();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            IAsyncResult ar = adapter.BeginExecuteQuery(RasterQueryExpressionStub, null);

            Assert.IsNotNull(ar);

            adapter.EndExecuteQuery(ar);
        }

    }
}