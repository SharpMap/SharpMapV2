using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
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
    
    public class AsyncFeatureProviderAdapterTests
    {
        private IFeatureDataReader _returnReaderStub;
        private Int32 _sleepInterval;

        //[TearDown]
        //public void TearDown()
        //{
        //    _returnReaderStub = null;
        //    _sleepInterval = 0;
        //}

        [Fact]
        public void ConstructorFailsOnNullProvider()
        {
            Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         AsyncFeatureProviderAdapter adapter =
                                                             new AsyncFeatureProviderAdapter(null);
                                                     });
        }

        [Fact]
        public void ConstructorSucceeds()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
        }

        [Fact]
        public void IProviderMethodsPassThroughAdapter()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();
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

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
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
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();

            using (mocks.Unordered())
            {
                adapted.Dispose();
            }
            mocks.ReplayAll();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            adapter.Dispose();

            mocks.VerifyAll();
        }

        [Fact]
        public void IFeatureProviderMethodsPassThroughAdapter()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();
            IGeometryFactory iGeometryFactoryStub =
                MockRepository.GenerateStub<IGeometryFactory>();
            FeatureDataTable featureDataTableStub =
                MockRepository.GenerateStub<FeatureDataTable>(iGeometryFactoryStub);
            IFeatureDataReader iFeatureDataReaderStub =
                MockRepository.GenerateStub<IFeatureDataReader>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            FeatureQueryExpression featureQueryExpressionStub =
                MockRepository.GenerateStub<FeatureQueryExpression>(iExtentsStub, SpatialOperation.Intersects);
            FeatureQueryExecutionOptions featureQueryExecutionOptionsStub =
                new FeatureQueryExecutionOptions();
            DataTable dataTableStub =
                MockRepository.GenerateStub<DataTable>();
            CultureInfo cultureInfoStub =
                MockRepository.GenerateStub<CultureInfo>(1);

            using (mocks.Unordered())
            {
                Expect.Call(adapted.CreateNewTable())
                    .Return(featureDataTableStub);
                Expect.Call(adapted.ExecuteFeatureQuery(featureQueryExpressionStub, featureQueryExecutionOptionsStub))
                    .Return(iFeatureDataReaderStub);
                Expect.Call(adapted.ExecuteFeatureQuery(featureQueryExpressionStub))
                    .Return(iFeatureDataReaderStub);
                adapted.GeometryFactory = iGeometryFactoryStub;
                Expect.Call(adapted.GeometryFactory)
                    .Return(iGeometryFactoryStub);
                Expect.Call(adapted.GetFeatureCount())
                    .Return(3);
                Expect.Call(adapted.GetSchemaTable())
                    .Return(dataTableStub);
                Expect.Call(adapted.Locale)
                    .Return(cultureInfoStub);
                adapted.SetTableSchema(featureDataTableStub);
            }
            mocks.ReplayAll();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            Assert.Same(featureDataTableStub, adapter.CreateNewTable());
            Assert.Same(iFeatureDataReaderStub,
                adapter.ExecuteFeatureQuery(featureQueryExpressionStub, featureQueryExecutionOptionsStub));
            Assert.Same(iFeatureDataReaderStub,
                adapter.ExecuteFeatureQuery(featureQueryExpressionStub));
            adapter.GeometryFactory = iGeometryFactoryStub;
            Assert.Same(iGeometryFactoryStub, adapter.GeometryFactory);
            Assert.Equal(3, adapter.GetFeatureCount());
            Assert.Same(dataTableStub, adapter.GetSchemaTable());
            Assert.Same(cultureInfoStub, adapter.Locale);
            adapter.SetTableSchema(featureDataTableStub);

            mocks.VerifyAll();
        }

        [Fact]
        public void BeginExecuteQueryThrowsOnInvalidExpression()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = MockRepository.GenerateStub<IFeatureProvider>();
            Expression expressionStub =
                MockRepository.GenerateStub<Expression>();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            Assert.Throws<ArgumentException>(delegate { adapter.BeginExecuteQuery(expressionStub, null); });
        }

        [Fact]
        public void EndExecuteQueryThrowsOnInvalidExpression()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = MockRepository.GenerateStub<IFeatureProvider>();
            AsyncResult result = new AsyncResult(null, this);

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            Assert.Throws<ArgumentException>(delegate { adapter.EndExecuteQuery(result); });
        }

        private delegate IFeatureDataReader ExecuteQueryDelegate(FeatureQueryExpression query);
        private IFeatureDataReader executeQueryMock(FeatureQueryExpression query)
        {
            Thread.Sleep(_sleepInterval);
            return _returnReaderStub;
        }

        [Fact]
        public void BeginExecuteQueryCallBackNotification()
        {
            CallBackHelper callBackRecorder = new CallBackHelper();

            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            FeatureQueryExpression featureQueryExpressionStub =
                MockRepository.GenerateStub<FeatureQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            _returnReaderStub = MockRepository.GenerateStub<IFeatureDataReader>();
            _sleepInterval = 500;

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteFeatureQuery(featureQueryExpressionStub))
                    .Do(new ExecuteQueryDelegate(executeQueryMock));
            }
            mocks.ReplayAll();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            IAsyncResult ar = adapter.BeginExecuteQuery(featureQueryExpressionStub, callBackRecorder.CallBack);

            Assert.NotNull(ar);
            Assert.False(ar.IsCompleted);

            Thread.Sleep(1000);

            Assert.Same(ar, callBackRecorder.Result);
            Assert.True(ar.IsCompleted);
            Assert.Same(_returnReaderStub, adapter.EndExecuteQuery(ar));

            mocks.VerifyAll();
        }

        [Fact]
        public void BeginExecuteQueryPollingNotification()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            FeatureQueryExpression featureQueryExpressionStub =
                MockRepository.GenerateStub<FeatureQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            _returnReaderStub = MockRepository.GenerateStub<IFeatureDataReader>();
            _sleepInterval = 1000;

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteFeatureQuery(featureQueryExpressionStub))
                    .Do(new ExecuteQueryDelegate(executeQueryMock));
            }
            mocks.ReplayAll();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            IAsyncResult ar = adapter.BeginExecuteQuery(featureQueryExpressionStub, null);

            Assert.NotNull(ar);
            Assert.False(ar.IsCompleted);
            while (!ar.IsCompleted)
            {
                Thread.Sleep(350);
            }

            Assert.True(ar.IsCompleted);
            Assert.Same(_returnReaderStub, adapter.EndExecuteQuery(ar));

            mocks.VerifyAll();
        }
        
        [Fact]
        public void BeginExecuteQueryBlockUntilDone()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            FeatureQueryExpression featureQueryExpressionStub =
                MockRepository.GenerateStub<FeatureQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            _returnReaderStub = MockRepository.GenerateStub<IFeatureDataReader>();
            _sleepInterval = 750;

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteFeatureQuery(featureQueryExpressionStub))
                    .Do(new ExecuteQueryDelegate(executeQueryMock));
            }
            mocks.ReplayAll();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            IAsyncResult ar = adapter.BeginExecuteQuery(featureQueryExpressionStub, null);

            Assert.NotNull(ar);

            Stopwatch timer = Stopwatch.StartNew();
            Assert.Same(_returnReaderStub, adapter.EndExecuteQuery(ar));
            timer.Stop();

            Assert.True(ar.IsCompleted);
            Assert.False(ar.CompletedSynchronously);
            Assert.True(timer.ElapsedMilliseconds > 500L);

            mocks.VerifyAll();
        }

        private IFeatureDataReader executeQueryExceptionMock(FeatureQueryExpression query)
        {
            Thread.Sleep(100);
            throw new InvalidOperationException("Failure succeeded");
        }

        [Fact]
        public void EndExecuteQueryThrowsTerminatingException()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();
            IExtents iExtentsStub =
                MockRepository.GenerateStub<IExtents>();
            FeatureQueryExpression featureQueryExpressionStub =
                MockRepository.GenerateStub<FeatureQueryExpression>(iExtentsStub, SpatialOperation.Intersects);

            using (mocks.Unordered())
            {
                Expect.Call(adapted.ExecuteFeatureQuery(featureQueryExpressionStub))
                    .Do(new ExecuteQueryDelegate(executeQueryExceptionMock));
            }
            mocks.ReplayAll();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            IAsyncResult ar = adapter.BeginExecuteQuery(featureQueryExpressionStub, null);

            Assert.NotNull(ar);

            Assert.Throws<InvalidOperationException>(delegate { adapter.EndExecuteQuery(ar); });
        }

    }
}
