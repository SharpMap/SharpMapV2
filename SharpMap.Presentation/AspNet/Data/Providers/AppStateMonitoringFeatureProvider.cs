/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// A wrapper for a feature provider which executes Monitor() while enumerating the inner provider.
    /// Monitor is a method which raises an exception when the application reaches a certain state - 
    /// for instance Http client disconnected.
    /// This prevents long running tasks continuing after the requesting client has surfed off elsewhere.
    /// </summary>
    public class AppStateMonitoringFeatureProvider : CustomTypeDescriptor, IFeatureProvider
    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _innerProvider.PropertyChanged += value; }
            remove { _innerProvider.PropertyChanged -= value; }
        }

        public Int32 AddProperty(PropertyDescriptor property)
        {
            return _innerProvider.AddProperty(property);
        }

        public Int32 AddProperty<TValue>(PropertyDescriptor property, TValue value)
        {
            return _innerProvider.AddProperty(property, value);
        }

        public Object GetPropertyValue(PropertyDescriptor property)
        {
            return _innerProvider.GetPropertyValue(property);
        }

        public TValue GetPropertyValue<TValue>(PropertyDescriptor property)
        {
            return _innerProvider.GetPropertyValue<TValue>(property);
        }

        public Boolean HasProperty(PropertyDescriptor property)
        {
            return _innerProvider.HasProperty(property);
        }

        public void SetPropertyValue<TValue>(PropertyDescriptor property, TValue value)
        {
            _innerProvider.SetPropertyValue(property, value);
        }

        public void SetPropertyValue(PropertyDescriptor property, Object value)
        {
            _innerProvider.SetPropertyValue(property, value);
        }

        public Object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _innerProvider.GetPropertyOwner(pd);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return _innerProvider.GetProperties();
        }

        private IFeatureProvider _innerProvider;

        public AppStateMonitoringFeatureProvider(IFeatureProvider innerProvider)
            : this(innerProvider, null) { }

        public AppStateMonitoringFeatureProvider(IFeatureProvider innerProvider, Action monitor)
        {
            InnerProvider = innerProvider;
            Monitor = monitor;
        }

        public IFeatureProvider InnerProvider
        {
            get { return _innerProvider; }
            protected set { _innerProvider = value; }
        }

        /// <summary>
        /// An Action which throws an exception if the application reaches a certain state. For instance Http Client is disconnected.
        /// </summary>
        public Action Monitor { get; set; }

        #region IFeatureProvider Members

        public FeatureDataTable CreateNewTable()
        {
            return InnerProvider.CreateNewTable();
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query)
        {
            return new MonitoringFeatureDataReader(InnerProvider.ExecuteFeatureQuery(query), Monitor);
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query, FeatureQueryExecutionOptions options)
        {
            return new MonitoringFeatureDataReader(InnerProvider.ExecuteFeatureQuery(query, options), Monitor);
        }

        public IGeometryFactory GeometryFactory
        {
            get { return InnerProvider.GeometryFactory; }
            set { InnerProvider.GeometryFactory = value; }
        }

        public Int32 GetFeatureCount()
        {
            return InnerProvider.GetFeatureCount();
        }

        public DataTable GetSchemaTable()
        {
            return InnerProvider.GetSchemaTable();
        }

        public CultureInfo Locale
        {
            get { return InnerProvider.Locale; }
        }

        public void SetTableSchema(FeatureDataTable table)
        {
            InnerProvider.SetTableSchema(table);
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { return InnerProvider.CoordinateTransformation; }
            set { InnerProvider.CoordinateTransformation = value; }
        }

        public ICoordinateSystem SpatialReference
        {
            get { return InnerProvider.SpatialReference; }
        }

        public Boolean IsOpen
        {
            get { return InnerProvider.IsOpen; }
        }

        public String Srid
        {
            get { return InnerProvider.Srid; }
        }

        public IExtents GetExtents()
        {
            return InnerProvider.GetExtents();
        }

        public String ConnectionId
        {
            get { return InnerProvider.ConnectionId; }
        }

        public void Open()
        {
            InnerProvider.Open();
        }

        public void Close()
        {
            InnerProvider.Close();
        }

        public Object ExecuteQuery(Expression query)
        {
            return InnerProvider.ExecuteQuery(query);
        }

        public void Dispose()
        {
            InnerProvider.Dispose();
        }

        #endregion

        #region Nested type: MonitoringFeatureDataReader

        private class MonitoringFeatureDataReader : IFeatureDataReader
        {
            public MonitoringFeatureDataReader(IFeatureDataReader innerReader, Action monitor)
            {
                InnerReader = innerReader;
                Monitor = monitor ?? new Action(() => { });
            }

            private IFeatureDataReader InnerReader { get; set; }
            private Action Monitor { get; set; }

            #region IFeatureDataReader Members

            public void Close()
            {
                InnerReader.Close();
            }

            public Int32 Depth
            {
                get { return InnerReader.Depth; }
            }

            public DataTable GetSchemaTable()
            {
                return InnerReader.GetSchemaTable();
            }

            public Boolean IsClosed
            {
                get { return InnerReader.IsClosed; }
            }

            public Boolean NextResult()
            {
                return InnerReader.NextResult();
            }

            public Boolean Read()
            {
                return InnerReader.Read();
            }

            public Int32 RecordsAffected
            {
                get { return InnerReader.RecordsAffected; }
            }

            public void Dispose()
            {
                InnerReader.Dispose();
            }

            public Int32 FieldCount
            {
                get { return InnerReader.FieldCount; }
            }

            public Boolean GetBoolean(Int32 i)
            {
                return InnerReader.GetBoolean(i);
            }

            public byte GetByte(Int32 i)
            {
                return InnerReader.GetByte(i);
            }

            public long GetBytes(Int32 i, long fieldOffset, byte[] buffer, Int32 bufferoffset, Int32 length)
            {
                return InnerReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
            }

            public char GetChar(Int32 i)
            {
                return InnerReader.GetChar(i);
            }

            public long GetChars(Int32 i, long fieldoffset, char[] buffer, Int32 bufferoffset, Int32 length)
            {
                return InnerReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
            }

            public IDataReader GetData(Int32 i)
            {
                return InnerReader.GetData(i);
            }

            public String GetDataTypeName(Int32 i)
            {
                return InnerReader.GetDataTypeName(i);
            }

            public DateTime GetDateTime(Int32 i)
            {
                return InnerReader.GetDateTime(i);
            }

            public decimal GetDecimal(Int32 i)
            {
                return InnerReader.GetDecimal(i);
            }

            public double GetDouble(Int32 i)
            {
                return InnerReader.GetDouble(i);
            }

            public Type GetFieldType(Int32 i)
            {
                return InnerReader.GetFieldType(i);
            }

            public float GetFloat(Int32 i)
            {
                return InnerReader.GetFloat(i);
            }

            public Guid GetGuid(Int32 i)
            {
                return InnerReader.GetGuid(i);
            }

            public short GetInt16(Int32 i)
            {
                return InnerReader.GetInt16(i);
            }

            public Int32 GetInt32(Int32 i)
            {
                return InnerReader.GetInt32(i);
            }

            public long GetInt64(Int32 i)
            {
                return InnerReader.GetInt64(i);
            }

            public String GetName(Int32 i)
            {
                return InnerReader.GetName(i);
            }

            public Int32 GetOrdinal(String name)
            {
                return InnerReader.GetOrdinal(name);
            }

            public String GetString(Int32 i)
            {
                return InnerReader.GetString(i);
            }

            public Object GetValue(Int32 i)
            {
                return InnerReader.GetValue(i);
            }

            public Int32 GetValues(Object[] values)
            {
                return InnerReader.GetValues(values);
            }

            public Boolean IsDBNull(Int32 i)
            {
                return InnerReader.IsDBNull(i);
            }

            public Object this[String name]
            {
                get { return InnerReader[name]; }
            }

            public Object this[Int32 i]
            {
                get { return InnerReader[i]; }
            }

            public IGeometry Geometry
            {
                get { return InnerReader.Geometry; }
            }

            public Object GetOid()
            {
                return InnerReader.GetOid();
            }

            public Boolean HasOid
            {
                get { return InnerReader.HasOid; }
            }

            public Boolean IsFullyLoaded
            {
                get { return InnerReader.IsFullyLoaded; }
            }

            public Type OidType
            {
                get { return InnerReader.OidType; }
            }

            public IEnumerator<IFeatureDataRecord> GetEnumerator()
            {
                try
                {
                    while (InnerReader.Read())
                    {
                        Monitor();
                        yield return this;
                    }
                }
                finally
                {
                    InnerReader.Close();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region IFeatureDataRecord Members


            public IExtents Extents
            {
                get { return InnerReader.Extents; }
            }

            #endregion

            #region IFeatureDataRecord Members


            public ICoordinateTransformation CoordinateTransformation
            {
                get { return InnerReader.CoordinateTransformation; }
                set { InnerReader.CoordinateTransformation = value; }

            }

            #endregion
        }

        #endregion
    }

    public static class AppStateMonitoringFeatureLayerProperties
    {
        private static readonly PropertyDescriptor _appStateMonitor =
            new DataSourcePassThroughPropertyDescriptor("AppStateMonitor", typeof(Action));

        public static PropertyDescriptor AppStateMonitor
        {
            get { return _appStateMonitor; }
        }
    }
}
