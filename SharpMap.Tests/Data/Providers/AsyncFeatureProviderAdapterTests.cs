using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
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
    public class AsyncFeatureProviderAdapterTests
    {
        private IFeatureDataReader _returnReaderStub;
        private Int32 _sleepInterval;

        [TearDown]
        public void TearDown()
        {
            _returnReaderStub = null;
            _sleepInterval = 0;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorFailsOnNullProvider()
        {
            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(null);
        }

        [Test]
        public void ConstructorSucceeds()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = mocks.CreateMock<IFeatureProvider>();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
        }

        [Test]
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
                adapted.Srid = 1;
                Expect.Call(adapted.Srid)
                    .Return(2);
            }
            mocks.ReplayAll();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            adapter.Close();
            Assert.AreEqual("connectionid", adapter.ConnectionId);
            adapter.CoordinateTransformation = transformStub;
            Assert.AreSame(transformStub, adapter.CoordinateTransformation);
            Assert.AreSame(objectStub, adapter.ExecuteQuery(queryStub));
            Assert.AreSame(extentsStub, adapter.GetExtents());
            Assert.IsTrue(adapter.IsOpen);
            adapter.Open();
            Assert.AreSame(coordinateSystemStub, adapter.SpatialReference);
            adapter.Srid = 1;
            Assert.AreEqual(2, adapter.Srid);

            mocks.VerifyAll();
        }

        [Test]
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

        [Test]
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
            Assert.AreSame(featureDataTableStub, adapter.CreateNewTable());
            Assert.AreSame(iFeatureDataReaderStub,
                adapter.ExecuteFeatureQuery(featureQueryExpressionStub, featureQueryExecutionOptionsStub));
            Assert.AreSame(iFeatureDataReaderStub,
                adapter.ExecuteFeatureQuery(featureQueryExpressionStub));
            adapter.GeometryFactory = iGeometryFactoryStub;
            Assert.AreSame(iGeometryFactoryStub, adapter.GeometryFactory);
            Assert.AreEqual(3, adapter.GetFeatureCount());
            Assert.AreSame(dataTableStub, adapter.GetSchemaTable());
            Assert.AreSame(cultureInfoStub, adapter.Locale);
            adapter.SetTableSchema(featureDataTableStub);

            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BeginExecuteQueryThrowsOnInvalidExpression()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = MockRepository.GenerateStub<IFeatureProvider>();
            Expression expressionStub =
                MockRepository.GenerateStub<Expression>();

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            adapter.BeginExecuteQuery(expressionStub, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void EndExecuteQueryThrowsOnInvalidExpression()
        {
            MockRepository mocks = new MockRepository();
            IFeatureProvider adapted = MockRepository.GenerateStub<IFeatureProvider>();
            AsyncResult result = new AsyncResult(null, this);

            AsyncFeatureProviderAdapter adapter = new AsyncFeatureProviderAdapter(adapted);
            adapter.EndExecuteQuery(result);
        }

        private delegate IFeatureDataReader ExecuteQueryDelegate(FeatureQueryExpression query);
        private IFeatureDataReader executeQueryMock(FeatureQueryExpression query)
        {
            Thread.Sleep(_sleepInterval);
            return _returnReaderStub;
        }

        [Test]
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

            Assert.IsNotNull(ar);
            Assert.IsFalse(ar.IsCompleted);

            Thread.Sleep(1000);

            Assert.AreSame(ar, callBackRecorder.Result);
            Assert.IsTrue(ar.IsCompleted);
            Assert.AreSame(_returnReaderStub, adapter.EndExecuteQuery(ar));

            mocks.VerifyAll();
        }

        [Test]
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

            Assert.IsNotNull(ar);
            Assert.IsFalse(ar.IsCompleted);
            while (!ar.IsCompleted)
            {
                Thread.Sleep(350);
            }

            Assert.IsTrue(ar.IsCompleted);
            Assert.AreSame(_returnReaderStub, adapter.EndExecuteQuery(ar));

            mocks.VerifyAll();
        }
        
        [Test]
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

            Assert.IsNotNull(ar);

            Stopwatch timer = Stopwatch.StartNew();
            Assert.AreSame(_returnReaderStub, adapter.EndExecuteQuery(ar));
            timer.Stop();

            Assert.IsTrue(ar.IsCompleted);
            Assert.IsFalse(ar.CompletedSynchronously);
            Assert.IsTrue(timer.ElapsedMilliseconds > 500L);

            mocks.VerifyAll();
        }

        private IFeatureDataReader executeQueryExceptionMock(FeatureQueryExpression query)
        {
            Thread.Sleep(100);
            throw new InvalidOperationException("Failure succeeded");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Failure succeeded")]
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

            Assert.IsNotNull(ar);

            adapter.EndExecuteQuery(ar);
        }

    }
}
