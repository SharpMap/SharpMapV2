// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using SharpMap.Geometries;

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
        private readonly QueryExecutionOptions _options;
        private DataTable _schemaTable;
        private readonly BoundingBox _queryRegion;
        private Int32 _currentRow = -1;
        private bool _isDisposed; 
        #endregion

        #region Object Construction / Disposal

        internal FeatureDataReader(FeatureDataTable source, BoundingBox queryRegion, QueryExecutionOptions options)
        {
            if (source == null) throw new ArgumentNullException("source");

            if (options != QueryExecutionOptions.FullFeature)
            {
                throw new ArgumentException("Only QueryExecutionOptions.All is supported.", "options");
            }

            _options = options;
            _table = source.Clone();
            _queryRegion = queryRegion;

			foreach (FeatureDataRow row in source.Select(_queryRegion))
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

        private void Dispose(bool disposing)
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

        private bool IsDisposed
        {
            get { return _isDisposed; }
            set { _isDisposed = value; }
        }

        #endregion

		#endregion

		#region IFeatureDataRecord Members
		public Geometry Geometry
		{
			get
			{
				checkDisposed();
				checkReadState();
				return _table[_currentRow].Geometry.Clone();
			}
        }

        public object GetOid()
        {
            checkDisposed();
            checkReadState();
            return _table[_currentRow][FeatureProvider.OidColumnName];
        }

        public bool HasOid
        {
            get
            {
                checkDisposed();
                return true;
            }
        }

        public bool IsFullyLoaded
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

        public bool IsClosed
        {
            get { return IsDisposed; }
        }

        public bool NextResult()
        {
            checkDisposed();
            return false;
        }

        public bool Read()
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

        public bool GetBoolean(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToBoolean(_table[_currentRow][i]);
        }

        public byte GetByte(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToByte(_table[_currentRow][i]);
        }

        public long GetBytes(Int32 i, long fieldOffset, byte[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return Convert.ToChar(_table[_currentRow][i]);
        }

        public long GetChars(Int32 i, long fieldoffset, char[] buffer, Int32 bufferoffset, Int32 length)
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

        public float GetFloat(Int32 i)
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

        public short GetInt16(Int32 i)
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

        public long GetInt64(Int32 i)
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

        public object GetValue(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return _table[_currentRow][i];
        }

        public Int32 GetValues(object[] values)
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

        public bool IsDBNull(Int32 i)
        {
            checkDisposed();
            checkReadState();
            checkIndex(i);
            return _table[_currentRow].IsNull(i);
        }

        public object this[String name]
        {
            get
            {
                checkDisposed();
                checkReadState();
                return _table[_currentRow][name];
            }
        }

        public object this[Int32 i]
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