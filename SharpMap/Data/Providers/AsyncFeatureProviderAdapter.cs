// Copyright 2008: Ron Emmert (justsome.handle@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Data;
using System.Globalization;
using System.Threading;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public class AsyncFeatureProviderAdapter : AsyncProviderAdapter, IFeatureProvider
    {
        protected IFeatureProvider InnerFeatureProvider
        {
            get { return _innerProvider as IFeatureProvider; }
        }

        public AsyncFeatureProviderAdapter(IFeatureProvider provider)
            : base(provider)
        { }


        #region IAsyncProvider implementation

        public override IAsyncResult BeginExecuteQuery(Expression query, AsyncCallback callback)
        {
            FeatureQueryExpression featureQuery = query as FeatureQueryExpression;
            if (featureQuery == null)
            {
                throw new ArgumentException("query must be non-null and of type FeatureQueryExpression.", "query");
            }
            
            AsyncResult<IFeatureDataReader> asyncResult = new AsyncResult<IFeatureDataReader>(callback, query);
            ThreadPool.QueueUserWorkItem(QueueableBeginQuery, asyncResult);
            return asyncResult;
        }

        public override Object EndExecuteQuery(IAsyncResult asyncResult)
        {
            AsyncResult<IFeatureDataReader> typedAsyncResult = asyncResult as AsyncResult<IFeatureDataReader>;
            if (typedAsyncResult == null)
            {
                throw new ArgumentException("Result must be of type AsyncResult<IFeatureDataReader>", "asyncResult");
            }

            return typedAsyncResult.EndInvoke();
        }
        
        private void QueueableBeginQuery(Object asyncResult)
        {
            AsyncResult<IFeatureDataReader> typedAsyncResult = asyncResult as AsyncResult<IFeatureDataReader>;
            FeatureQueryExpression query = typedAsyncResult.AsyncState as FeatureQueryExpression;

            try
            {
                typedAsyncResult.SetComplete(InnerFeatureProvider.ExecuteFeatureQuery(query), false);
            }
            catch (Exception terminatingException)
            {
                typedAsyncResult.SetComplete(false, terminatingException);
            }
        }

        #endregion

        #region wrapped IDisposable methods

        public override void Dispose()
        {
            InnerFeatureProvider.Dispose();
        }

        #endregion

        #region wrapped IFeatureProvider methods and properties

        public FeatureDataTable CreateNewTable()
        {
            return InnerFeatureProvider.CreateNewTable();
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query, FeatureQueryExecutionOptions options)
        {
            return InnerFeatureProvider.ExecuteFeatureQuery(query, options);
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query)
        {
            return InnerFeatureProvider.ExecuteFeatureQuery(query);
        }

        public IGeometryFactory GeometryFactory
        {
            get { return InnerFeatureProvider.GeometryFactory; }
            set { InnerFeatureProvider.GeometryFactory = value; }
        }

        public int GetFeatureCount()
        {
            return InnerFeatureProvider.GetFeatureCount();
        }

        public DataTable GetSchemaTable()
        {
            return InnerFeatureProvider.GetSchemaTable();
        }

        public CultureInfo Locale
        {
            get { return InnerFeatureProvider.Locale; }
        }

        public void SetTableSchema(FeatureDataTable table)
        {
            InnerFeatureProvider.SetTableSchema(table);
        }

        #endregion
    }
}
