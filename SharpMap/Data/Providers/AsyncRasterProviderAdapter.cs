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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public class AsyncRasterProviderAdapter : AsyncProviderAdapter, IRasterProvider
    {
        protected IRasterProvider InnerRasterProvider
        {
            get { return _innerProvider as IRasterProvider; }
        }

        public AsyncRasterProviderAdapter(IRasterProvider provider)
            : base(provider)
        {
        }

        #region IAsyncProvider implementation

        public override IAsyncResult BeginExecuteQuery(Expression query, AsyncCallback callback)
        {
            RasterQueryExpression rasterQuery = query as RasterQueryExpression;
            if (rasterQuery == null)
            {
                throw new ArgumentException("query must be non-null and of type RasterQueryExpression.", "query");
            }

            AsyncResult<IEnumerable<IRasterRecord>> asyncResult = new AsyncResult<IEnumerable<IRasterRecord>>(callback, query);
            ThreadPool.QueueUserWorkItem(QueueableBeginQuery, asyncResult);
            return asyncResult;
        }

        public override Object EndExecuteQuery(IAsyncResult asyncResult)
        {
            AsyncResult<IEnumerable<IRasterRecord>> typedAsyncResult = asyncResult as AsyncResult<IEnumerable<IRasterRecord>>;
            if (typedAsyncResult == null)
            {
                throw new ArgumentException("Result must be of type AsyncResult<IEnumerable<IRasterRecord>>", "asyncResult");
            }

            return typedAsyncResult.EndInvoke();
        }

        private void QueueableBeginQuery(Object asyncResult)
        {
            AsyncResult<IEnumerable<IRasterRecord>> typedAsyncResult = asyncResult as AsyncResult<IEnumerable<IRasterRecord>>;
            RasterQueryExpression query = typedAsyncResult.AsyncState as RasterQueryExpression;

            try
            {
                typedAsyncResult.SetComplete(InnerRasterProvider.ExecuteRasterQuery(query), false);
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
            InnerRasterProvider.Dispose();
        }

        #endregion

        #region wrapped IRasterProvider methods

        public IEnumerable<IRasterRecord> ExecuteRasterQuery(RasterQueryExpression query)
        {
            return InnerRasterProvider.ExecuteRasterQuery(query);
        }

        #endregion

        #region IHasDynamicProperties Members

        public int AddProperty(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public int AddProperty<TValue>(PropertyDescriptor property, TValue value)
        {
            throw new NotImplementedException();
        }

        public Object GetPropertyValue(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public TValue GetPropertyValue<TValue>(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void SetPropertyValue(PropertyDescriptor property, Object value)
        {
            throw new NotImplementedException();
        }

        public void SetPropertyValue<TValue>(PropertyDescriptor property, TValue value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string GetClassName()
        {
            throw new NotImplementedException();
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public Object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            throw new NotImplementedException();
        }

        public Object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
