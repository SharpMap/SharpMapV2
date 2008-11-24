using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;

using Rhino.Mocks;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Expressions;
using Xunit;

namespace SharpMap.Tests.Data.Providers
{
    
    public class AsyncRasterProviderAdapterTests
    {
        private Stream _returnStreamStub;
        private Int32 _sleepInterval;

        //[TearDown]
        //public void TearDown()
        //{
        //    _returnStreamStub = null;
        //    _sleepInterval = 0;
        //}

        [Fact]
        public void ConstructorFailsOnNullProvider()
        {
            Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         AsyncRasterProviderAdapter adapter =
                                                             new AsyncRasterProviderAdapter(null);
                                                     });
        }

        [Fact]
        public void ConstructorSucceeds()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = mocks.CreateMock<IRasterProvider>();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
        }

        [Fact]
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
                    .Return("2");
            }

            mocks.ReplayAll();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            adapter.Close();
            Assert.Equal("connectionid", adapter.ConnectionId);
            adapter.CoordinateTransformation = transformStub;
            Assert.Same(transformStub, adapter.CoordinateTransformation);
            Assert.Same(objectStub, adapter.ExecuteQuery(queryStub));
            Assert.Same(extentsStub, adapter.GetExtents());
            Assert.True(adapter.IsOpen);
            adapter.Open();
            Assert.Same(coordinateSystemStub, adapter.SpatialReference);
            //adapter.Srid = 1;
            Assert.Equal("2", adapter.Srid);

            mocks.VerifyAll();
        }

        [Fact]
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

        [Fact]
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
            Assert.Same(streamStub, adapter.ExecuteRasterQuery(rasterQueryExpressionStub));

            mocks.VerifyAll();
        }

        [Fact]
        public void BeginExecuteQueryThrowsOnInvalidExpression()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = MockRepository.GenerateStub<IRasterProvider>();
            Expression expressionStub =
                MockRepository.GenerateStub<Expression>();

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            Assert.Throws<ArgumentException>(delegate { adapter.BeginExecuteQuery(expressionStub, null); });
        }

        [Fact]
        public void EndExecuteQueryThrowsOnInvalidExpression()
        {
            MockRepository mocks = new MockRepository();
            IRasterProvider adapted = MockRepository.GenerateStub<IRasterProvider>();
            AsyncResult result = new AsyncResult(null, this);

            AsyncRasterProviderAdapter adapter = new AsyncRasterProviderAdapter(adapted);
            Assert.Throws<ArgumentException>(delegate { adapter.EndExecuteQuery(result); });
        }


        private delegate Stream ExecuteQueryDelegate(RasterQueryExpression query);
        private Stream executeQueryMock(RasterQueryExpression query)
        {
            Thread.Sleep(_sleepInterval);
            return _returnStreamStub;
        }

        [Fact]
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

            Assert.NotNull(ar);
            Assert.False(ar.IsCompleted);

            Thread.Sleep(1000);

            Assert.Same(ar, callBackRecorder.Result);
            Assert.True(ar.IsCompleted);
            Assert.Same(_returnStreamStub, adapter.EndExecuteQuery(ar));

            mocks.VerifyAll();
        }

        [Fact]
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

            Assert.NotNull(ar);
            Assert.False(ar.IsCompleted);
            while (!ar.IsCompleted)
            {
                Thread.Sleep(350);
            }

            Assert.True(ar.IsCompleted);
            Assert.Same(_returnStreamStub, adapter.EndExecuteQuery(ar));

            mocks.VerifyAll();
        }
        
        [Fact]
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

            Assert.NotNull(ar);

            Stopwatch timer = Stopwatch.StartNew();
            Assert.Same(_returnStreamStub, adapter.EndExecuteQuery(ar));
            timer.Stop();

            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);
            Assert.True(timer.ElapsedMilliseconds > 500L);

            mocks.VerifyAll();
        }

        private Stream executeQueryExceptionMock(RasterQueryExpression query)
        {
            Thread.Sleep(100);
            throw new InvalidOperationException("Failure succeeded");
        }

        [Fact]
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

            Assert.NotNull(ar);

            Assert.Throws<InvalidOperationException>(delegate { adapter.EndExecuteQuery(ar); });
        }
    }
}