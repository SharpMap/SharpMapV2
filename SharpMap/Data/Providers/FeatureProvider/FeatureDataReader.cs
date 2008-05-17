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

namespace SharpMap.Data.Providers.FeatureProvider
{
    /// <summary>
    /// Provides a fast-forward, read-only data stream to feature data
    /// from a <see cref="FeatureProvider"/>.
    /// </summary>
    public class FeatureDataReader : IFeatureDataReader
    {
        #region Instance fields
        private readonly FeatureDataTable _table;
        private readonly FeatureQueryExecutionOptions _options;
        private DataTable _schemaTable;
        private readonly FeatureQueryExpression _query;
        private Int32 _currentRow = -1;
        private Boolean _isDisposed;
        private IGeometryFactory _factory;
        #endregion

        #region Object Construction / Disposal

        internal FeatureDataReader(IGeometryFactory factory, 
                                   FeatureDataTable source, 
                                   FeatureQueryExpression query, 
                                   FeatureQueryExecutionOptions options)
        {
            if (source == null) throw new ArgumentNullException("source");

            if (options != FeatureQueryExecutionOptions.FullFeature)
            {
                throw new ArgumentException("Only QueryExecutionOptions.All is supported.", 
                                            "options");
            }

            _factory = factory;
            _options = options;
            _query = query;
            _table = source.Clone();

            foreach (FeatureDataRow row in source.Select(query.SpatialPredicate))
			{
				_table.ImportRow(row);
            }
        }

        #region Dispose pattern

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

        private void Dispose(Boolean disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (_table != null)
                {
                    _table.Dispose();
                }
            }
        }

        #endregion

        private Boolean IsDisposed
        {
            get { return _isDisposed; }
            set { _isDisposed = value; }
        }

        #endregion

		#endregion

		#region IFeatureDataRecord Members
		public IGeometry Geometry
		{
			get
			{
				checkDisposed();
				checkReadState();
				return _table[_currentRow].Geometry.Clone();
			}
        }

        public Object GetOid()
        {
            checkDisposed();
            checkReadState();
            return _table[_currentRow][FeatureProvider.OidColumnName];
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
            get { return true; }
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

            if (_schemaTable == null)
            {
                DataTableReader reader = new DataTableReader(_table);
                _schemaTable = reader.GetSchemaTable();
            }

            return _schemaTable;
        }

        public Boolean IsClosed
        {
            get { return IsDisposed; }
        }

        public Boolean NextResult()
        {
            checkDisposed();
            return false;
        }

        public Boolean Read()
        {
            checkDisposed();

            if (_currentRow < _table.FeatureCount - 1)
            {
                _currentRow++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Int32 RecordsAffected
        {
            get
            {
                checkDisposed();
                return _table.FeatureCount;
            }
        }

        #endregion

        #region IDataRecord Members

        public Int32 FieldCount
        {
            get
            {
                checkDisposed();
                return _table.Columns.Count;
            }
        }

        public Boolean GetBoolean(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToBoolean(_table[_currentRow][i]);
        }

        public Byte GetByte(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToByte(_table[_currentRow][i]);
        }

        public Int64 GetBytes(Int32 i, Int64 fieldOffset, Byte[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public Char GetChar(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToChar(_table[_currentRow][i]);
        }

        public Int64 GetChars(Int32 i, Int64 fieldoffset, Char[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(Int32 i)
        {
            throw new NotImplementedException();
        }

        public String GetDataTypeName(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return _table.Columns[i].DataType.ToString();
        }

        public DateTime GetDateTime(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToDateTime(_table[_currentRow][i]);
        }

        public decimal GetDecimal(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToDecimal(_table[_currentRow][i]);
        }

        public Double GetDouble(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToDouble(_table[_currentRow][i]);
        }

        public Type GetFieldType(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return _table.Columns[i].DataType;
        }

        public Single GetFloat(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToSingle(_table[_currentRow][i]);
        }

        public Guid GetGuid(Int32 i)
        {
            throw new NotImplementedException();
        }

        public Int16 GetInt16(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToInt16(_table[_currentRow][i]);
        }

        public Int32 GetInt32(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToInt32(_table[_currentRow][i]);
        }

        public Int64 GetInt64(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToInt64(_table[_currentRow][i]);
        }

        public String GetName(Int32 i)
        {
            checkDisposed();
            checkIndex(i);
            return _table.Columns[i].ColumnName;
        }

        public Int32 GetOrdinal(String name)
        {
            checkDisposed();
            checkReadState();
            return _table.Columns[name].Ordinal;
        }

        public String GetString(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToString(_table[_currentRow][i]);
        }

        public Object GetValue(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return _table[_currentRow][i];
        }

        public Int32 GetValues(Object[] values)
        {
            checkDisposed();
            checkReadState();

            if (values == null) throw new ArgumentNullException("values");

            Int32 count = values.Length > FieldCount ? FieldCount : values.Length;

            for (Int32 i = 0; i < count; i++)
            {
                values[i] = _table[_currentRow][i];
            }

            return count;
        }

        public Boolean IsDBNull(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return _table[_currentRow].IsNull(i);
        }

        public Object this[String name]
        {
            get
            {
                checkDisposed();
                checkReadState();
                return _table[_currentRow][name];
            }
        }

        public Object this[Int32 i]
        {
            get
            {
                checkDisposed();
                checkReadState();
                checkIndex(i);
                return _table[_currentRow][i];
            }
        }

        #endregion

        #region Private helper methods

        private void checkIndex(Int32 i)
        {
            if (i < 0 || i >= FieldCount)
            {
                throw new IndexOutOfRangeException("Column index out of range: " + i);
            }
        }

        private void checkDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().ToString());
        }

        private void checkReadState()
        {
            if (_currentRow < 0 || _currentRow >= RecordsAffected)
            {
                throw new InvalidOperationException("Attempt to read when data " +
                    "isn't available: either Read() wasn't called or it returned 'false'.");
            }
        }
        #endregion

        #region IFeatureDataRecord Members

        public Type OidType
        {
            get { return typeof(Guid); }
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