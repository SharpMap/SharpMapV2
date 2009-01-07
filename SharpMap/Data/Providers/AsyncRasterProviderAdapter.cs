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
using System.ComponentModel;
using System.IO;
using System.Threading;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public class AsyncRasterProviderAdapter : AsyncProviderAdapter, ICoverageProvider
    {
        protected ICoverageProvider InnerRasterProvider
        {
            get { return _innerProvider as ICoverageProvider; }
        }

        public AsyncRasterProviderAdapter(ICoverageProvider provider)
            : base(provider)
        {
        }

        #region IAsyncProvider implementation

        public override IAsyncResult BeginExecuteQuery(Expression query, AsyncCallback callback)
        {
            CoverageQueryExpression rasterQuery = query as CoverageQueryExpression;
            if (rasterQuery == null)
            {
                throw new ArgumentException("query must be non-null and of type RasterQueryExpression.", "query");
            }

            AsyncResult<Stream> asyncResult = new AsyncResult<Stream>(callback, query);
            ThreadPool.QueueUserWorkItem(QueueableBeginQuery, asyncResult);
            return asyncResult;
        }

        public override Object EndExecuteQuery(IAsyncResult asyncResult)
        {
            AsyncResult<Stream> typedAsyncResult = asyncResult as AsyncResult<Stream>;
            if (typedAsyncResult == null)
            {
                throw new ArgumentException("Result must be of type AsyncResult<Stream>", "asyncResult");
            }

            return typedAsyncResult.EndInvoke();
        }

        private void QueueableBeginQuery(Object asyncResult)
        {
            AsyncResult<Stream> typedAsyncResult = asyncResult as AsyncResult<Stream>;
            CoverageQueryExpression query = typedAsyncResult.AsyncState as CoverageQueryExpression;

            try
            {
                typedAsyncResult.SetComplete(InnerRasterProvider.ExecuteCoverageQuery(query), false);
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

        public Stream ExecuteCoverageQuery(CoverageQueryExpression query)
        {
            return InnerRasterProvider.ExecuteCoverageQuery(query);
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
