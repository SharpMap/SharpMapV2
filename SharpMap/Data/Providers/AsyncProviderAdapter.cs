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
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public abstract class AsyncProviderAdapter : IAsyncProvider
    {
        /// <summary>
        /// IProvider providing actual services
        /// </summary>
        protected readonly IProvider _innerProvider;


        protected AsyncProviderAdapter(IProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            _innerProvider = provider;
        }

        #region wrapped IProvider methods and properties

        public void Close()
        {
            _innerProvider.Close();
        }

        public String ConnectionId
        {
            get { return _innerProvider.ConnectionId; }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { return _innerProvider.CoordinateTransformation; }
            set { _innerProvider.CoordinateTransformation = value; }
        }

        public Object ExecuteQuery(Expression query)
        {
            return _innerProvider.ExecuteQuery(query);
        }

        public IExtents GetExtents()
        {
            return _innerProvider.GetExtents();
        }

        public Boolean IsOpen
        {
            get { return _innerProvider.IsOpen; }
        }

        public void Open()
        {
            _innerProvider.Open();
        }

        public ICoordinateSystem SpatialReference
        {
            get { return _innerProvider.SpatialReference; }
        }

        public Int32? Srid
        {
            get { return _innerProvider.Srid; }
        }

        #endregion

        #region IDisposableMembers

        public abstract void Dispose();

        #endregion

        #region IAsyncProvider Members

        public abstract IAsyncResult BeginExecuteQuery(Expression query, AsyncCallback callback);

        public abstract Object EndExecuteQuery(IAsyncResult asyncResult);

        #endregion
    }
}