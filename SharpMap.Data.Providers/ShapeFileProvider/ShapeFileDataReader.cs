// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Data;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.ShapeFile
{
    /// <summary>
    /// Provides a fast-forward, read-only data stream to feature data
    /// from a <see cref="ShapeFile"/>.
    /// </summary>
	public class ShapeFileDataReader : IFeatureDataReader
	{
        #region Instance fields
        private readonly ShapeFileProvider _shapeFile;
        private readonly FeatureQueryExecutionOptions _options;
        private readonly DataTable _schemaTable;
        private IFeatureDataRecord _currentFeature;
        private Boolean _isDisposed;
        private readonly IEnumerator<UInt32> _objectEnumerator;
        #endregion

		#region Object Construction / Disposal

        internal ShapeFileDataReader(ShapeFileProvider source, 
                                     FeatureQueryExpression query, 
                                     FeatureQueryExecutionOptions options)
		{
            if (options != FeatureQueryExecutionOptions.FullFeature)
            {
                throw new ArgumentException("Only QueryExecutionOptions.All is supported.", "options");
            }

			_shapeFile = source;
		    _options = options;
			_schemaTable = source.GetSchemaTable();

            // TODO: now that we are accessing the geometry each time, perhaps a feature
            // query here would save a disk access
			_objectEnumerator = source.ExecuteOidQuery(query.SpatialPredicate).GetEnumerator();
		}

		#region Dispose Pattern

		~ShapeFileDataReader()
		{
			Dispose(false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			Dispose(true);
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		#endregion

		private void Dispose(Boolean disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				_shapeFile.Close();
			}

			OnDisposed();
		}

        /// <summary>
        /// Gets a value indicating whether the ShapeFileDataReader has been disposed or not.
        /// </summary>
		public Boolean IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		private void OnDisposed()
		{
			EventHandler e = Disposed;

			if (e != null)
			{
				e(this, EventArgs.Empty);
			}
		}

		public event EventHandler Disposed;

		#endregion

		#endregion

		#region IFeatureDataRecord Members
		public IGeometry Geometry
		{
			get
			{
			    checkState();

			    return _currentFeature.Geometry == null
			               ? null
			               : _currentFeature.Geometry.Clone();
			}
        }

        public IExtents Extents
        {
            get
            {
                checkState();

                return _currentFeature.Extents == null
                           ? null
                           : _currentFeature.Extents.Clone() as IExtents;
            }
        }

		public object GetOid()
		{
			checkState();
			return _objectEnumerator.Current;
		}

		public Boolean HasOid
		{
			get
			{
                checkDisposed();
                return true;
			}
		}

        public Boolean IsFullyLoaded
        {
            get
            {
                return (_options | FeatureQueryExecutionOptions.BoundingBoxes) == 
                       (FeatureQueryExecutionOptions.FullFeature | FeatureQueryExecutionOptions.BoundingBoxes);
            }
        }

        public Type OidType
        {
            get { return typeof(UInt32); }
        }

		#endregion

		#region IDataReader Members

		public void Close()
		{
			Dispose();
		}

		public Int32 Depth
		{
			get
			{
                checkDisposed();
                return 0;
			}
		}

		public DataTable GetSchemaTable()
		{
            checkDisposed();
            return _schemaTable.Copy();
		}

		public Boolean IsClosed
		{
			get { return IsDisposed; }
		}

		public Boolean NextResult()
		{
			checkState();
			return false;
		}

		public Boolean Read()
		{
            checkDisposed();

			Boolean reading = _objectEnumerator.MoveNext();

			if (reading)
			{
				_currentFeature = _shapeFile.GetFeatureByOid(_objectEnumerator.Current);
			}

			return reading;
		}

		public Int32 RecordsAffected
		{
			get { return -1; }
		}

		#endregion

		#region IDataRecord Members

		public Int32 FieldCount
		{
			get
			{
			    checkDisposed();
			    return _schemaTable.Rows.Count;
			}
		}

        public Boolean GetBoolean(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToBoolean(_currentFeature[i]);
		}

		public Byte GetByte(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToByte(_currentFeature[i]);
		}

		public Int64 GetBytes(Int32 i, Int64 fieldOffset, Byte[] buffer, Int32 bufferoffset, Int32 length)
		{
            throw new NotSupportedException();
		}

		public Char GetChar(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToChar(_currentFeature[i]);
		}

		public Int64 GetChars(Int32 i, Int64 fieldoffset, Char[] buffer, Int32 bufferoffset, Int32 length)
		{
            throw new NotSupportedException();
		}

		public IDataReader GetData(Int32 i)
		{
			throw new NotSupportedException();
		}

		public String GetDataTypeName(Int32 i)
        {
            checkState();
            checkIndex(i);

		    return GetFieldType(i).Name;
		}

		public DateTime GetDateTime(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToDateTime(_currentFeature[i]);
		}

		public decimal GetDecimal(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToDecimal(_currentFeature[i]);
		}

		public Double GetDouble(Int32 i)
        {
            checkState();
            checkIndex(i);

            return Convert.ToDouble(_currentFeature[i]);
		}

		public Type GetFieldType(Int32 i)
        {
            checkState();
            checkIndex(i);
		    return _currentFeature.GetFieldType(i);
		}

		public Single GetFloat(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToSingle(_currentFeature[i]);
		}

		public Guid GetGuid(Int32 i)
        {
            checkState();
            checkIndex(i);

		    Object feature = _currentFeature[i];

            if (feature is Guid)
            {
                return (Guid)feature;
            }

		    String featureAsString = feature as String;

            if (featureAsString != null)
            {
                return new Guid(featureAsString);
            }
            
            Byte[] featureAsBytes = feature as Byte[];

            if (featureAsBytes != null)
            {
                return new Guid(featureAsBytes);
            }

		    throw new InvalidCastException(
                String.Format("Invalid cast from '{0}' to 'Guid'", feature.GetType()));
		}

		public Int16 GetInt16(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToInt16(_currentFeature[i]);
		}

		public Int32 GetInt32(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToInt32(_currentFeature[i]);
		}

		public Int64 GetInt64(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToInt64(_currentFeature[i]);
		}

		public String GetName(Int32 i)
		{
            checkDisposed();
            return _schemaTable.Rows[i][0] as String;
		}

		public Int32 GetOrdinal(String name)
		{
            checkDisposed();

			if (name == null) throw new ArgumentNullException("name");

			Int32 fieldCount = FieldCount;

			for (Int32 i = 0; i < fieldCount; i++)
			{
				if (name.Equals(GetName(i)))
				{
					return i;
				}
			}

			for (Int32 i = 0; i < fieldCount; i++)
			{
				if (String.Compare(name, GetName(i), StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return i;
				}
			}

			throw new IndexOutOfRangeException("Column name not found");
		}

		public String GetString(Int32 i)
		{
			checkState();
			checkIndex(i);

			return Convert.ToString(_currentFeature[i]);
		}

		public object GetValue(Int32 i)
		{
			checkState();
			checkIndex(i);

			return _currentFeature[i];
		}

		public Int32 GetValues(object[] values)
		{
			checkState();

			if (values == null) throw new ArgumentNullException("values");

			Int32 count = values.Length > FieldCount ? FieldCount : values.Length;

			for (Int32 i = 0; i < count; i++)
			{
				values[i] = this[i];
			}

			return count;
		}

		public Boolean IsDBNull(Int32 i)
		{
			checkState();
			checkIndex(i);

			return _currentFeature.IsDBNull(i);
		}

		public object this[String name]
		{
			get
			{
				checkState();
				return _currentFeature[name];
			}
		}

		public object this[Int32 i]
		{
			get
			{
				checkState();
				checkIndex(i);

				return _currentFeature[i];
			}
		}

		#endregion

		#region Private helper methods

		private void checkIndex(Int32 i)
		{
			if (i >= FieldCount)
			{
				throw new IndexOutOfRangeException("Index must be less than FieldCount.");
			}
		}

		private void checkState()
		{
			checkDisposed();
			if (_currentFeature == null)
			{
				throw new InvalidOperationException("The Read method must be called before accessing values.");
			}
		}

        private void checkDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().ToString());
        }
        
        #endregion

		#region IEnumerable<IFeatureDataRecord> Members

		public IEnumerator<IFeatureDataRecord> GetEnumerator()
        {
            while (Read())
            {
                yield return this;
            }
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}