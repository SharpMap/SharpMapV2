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
using System.Data;
using SharpMap.Features;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers.FeatureProvider
{
    public class FeatureDataReader : IFeatureDataReader
    {
        private readonly FeatureDataTable _table;
        private DataTable _schemaTable;
        private readonly BoundingBox _queryRegion;
        private int _currentRow = -1;
        private bool _isDisposed;

        #region Object Construction / Disposal

        internal FeatureDataReader(FeatureDataTable source, BoundingBox queryRegion)
        {
            if (source == null) throw new ArgumentNullException("source");

            _table = new FeatureDataTable();
            source.CopyTableSchema(_table);
            _queryRegion = queryRegion;

            foreach (FeatureDataRow row in source)
            {
                if (row.Geometry != null && row.Geometry.GetBoundingBox().Intersects(_queryRegion))
                {
                    _table.ImportRow(row);
                }
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

        #region IFeatureDataReader Members

        public Geometry GetGeometry()
        {
            checkState();
            return _table[_currentRow].Geometry.Clone();
        }

        public object GetOid()
        {
            checkState();
            return _table[_currentRow][FeatureProvider.OidColumnName];
        }

        public bool HasOid
        {
            get
            {
                checkState();
                return true;
            }
        }

        #endregion

        #region IDataReader Members

        public void Close()
        {
            Dispose();
        }

        public int Depth
        {
            get
            {
                checkState();
                return 0;
            }
        }

        public DataTable GetSchemaTable()
        {
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
            return false;
        }

        public bool Read()
		{
			if (IsDisposed) throw new ObjectDisposedException(GetType().ToString());

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

        public int RecordsAffected
        {
            get { return _table.FeatureCount; }
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(GetType().ToString());
                return _table.Columns.Count;
            }
        }

        public bool GetBoolean(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToBoolean(_table[_currentRow][i]);
        }

        public byte GetByte(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToByte(_table[_currentRow][i]);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToChar(_table[_currentRow][i]);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            checkState();
            checkIndex(i);
            return _table.Columns[i].DataType.ToString();
        }

        public DateTime GetDateTime(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToDateTime(_table[_currentRow][i]);
        }

        public decimal GetDecimal(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToDecimal(_table[_currentRow][i]);
        }

        public double GetDouble(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToDouble(_table[_currentRow][i]);
        }

        public Type GetFieldType(int i)
        {
            checkState();
            checkIndex(i);
            return _table.Columns[i].DataType;
        }

        public float GetFloat(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToSingle(_table[_currentRow][i]);
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToInt16(_table[_currentRow][i]);
        }

        public int GetInt32(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToInt32(_table[_currentRow][i]);
        }

        public long GetInt64(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToInt64(_table[_currentRow][i]);
        }

        public string GetName(int i)
        {
            checkState();
            checkIndex(i);
            return _table.Columns[i].ColumnName;
        }

        public int GetOrdinal(string name)
        {
            checkState();
            return _table.Columns[name].Ordinal;
        }

        public string GetString(int i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToString(_table[_currentRow][i]);
        }

        public object GetValue(int i)
        {
            checkState();
            checkIndex(i);
            return _table[_currentRow][i];
        }

        public int GetValues(object[] values)
        {
            checkState();

            if (values == null) throw new ArgumentNullException("values");

            int count = values.Length > FieldCount ? FieldCount : values.Length;

            for (int i = 0; i < count; i++)
            {
                values[i] = _table[_currentRow][i];
            }

            return count;
        }

        public bool IsDBNull(int i)
        {
            checkState();
            checkIndex(i);
            return _table[_currentRow].IsNull(i);
        }

        public object this[string name]
        {
            get
            {
                checkState();
                return _table[_currentRow][name];
            }
        }

        public object this[int i]
        {
            get
            {
                checkState();
                checkIndex(i);
                return _table[_currentRow][i];
            }
        }

        #endregion

        #region Private helper methods

        private void checkIndex(int i)
        {
            if (i < 0 || i >= FieldCount)
            {
                throw new IndexOutOfRangeException("Column index out of range: " + i);
            }
        }

        private void checkState()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_currentRow < 0 || _currentRow >= RecordsAffected)
            {
                throw new InvalidOperationException("Attempt to read beyond end of records.");
            }
        }

        #endregion
    }
}