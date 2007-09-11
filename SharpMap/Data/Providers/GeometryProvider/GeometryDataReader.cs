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
using System.Threading;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers.GeometryProvider
{
    public class GeometryDataReader : IFeatureDataReader
    {
        private GeometryProvider _provider;
        private readonly BoundingBox _bounds;
        private int _currentIndex = -1;
        private bool _isDisposed;

        #region Object construction and disposal
        internal GeometryDataReader(GeometryProvider provider, BoundingBox bounds)
        {
            _provider = provider;
            _bounds = bounds;
        }
        #region Dispose pattern

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _provider = null;
            }
        }

        #endregion
		#endregion

		#region IFeatureDataRecord Members
		public Geometry Geometry
		{
			get
			{
				checkState();
				return _provider.Geometries[_currentIndex].Clone();
			}
		}
		#endregion

        #region IFeatureDataReader Members

        public object GetOid()
        {
            checkState();
            return _currentIndex;
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
            get { return 0; }
        }

        public DataTable GetSchemaTable()
        {
            return _provider.GetSchemaTable();
        }

        public bool IsClosed
        {
            get { return _isDisposed; }
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            do
            {
                Interlocked.Increment(ref _currentIndex);
            } while (_currentIndex < _provider.Geometries.Count &&
                     _bounds.Intersects(_provider.Geometries[_currentIndex]));

            return _currentIndex < _provider.Geometries.Count;
        }

        public int RecordsAffected
        {
            get
            {
                int count = 0;

                foreach (Geometry geometry in _provider.Geometries)
                {
                    if (_bounds.Intersects(geometry)) count++;
                }

                return count;
            }
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get { return 1; }
        }

        public bool GetBoolean(int i)
        {
            throw new NotSupportedException();
        }

        public byte GetByte(int i)
        {
            checkState();
            checkIndex(i);
            return (byte)_currentIndex;
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            checkState();
            checkIndex(i);
            return (char)_currentIndex;
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public string GetDataTypeName(int i)
        {
            checkState();
            checkIndex(i);
            return typeof(Int32).ToString();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotSupportedException();
        }

        public decimal GetDecimal(int i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public double GetDouble(int i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public Type GetFieldType(int i)
        {
            checkState();
            checkIndex(i);
            return typeof(Int32);
        }

        public float GetFloat(int i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        public short GetInt16(int i)
        {
            checkState();
            checkIndex(i);
            return (short)_currentIndex;
        }

        public int GetInt32(int i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public long GetInt64(int i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public string GetName(int i)
        {
            checkState();
            checkIndex(i);
            return "oid";
        }

        public int GetOrdinal(string name)
        {
            checkState();

            if (String.Compare(name, "oid", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new ArgumentException("Column name must be 'oid'; it was: " + name, "name");
            }

            return 0;
        }

        public string GetString(int i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex.ToString();
        }

        public object GetValue(int i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        public bool IsDBNull(int i)
        {
            checkState();
            checkIndex(i);
            return false;
        }

        public object this[string name]
        {
            get
            {
                checkState();
                int ordinal = GetOrdinal(name);
                return GetInt32(ordinal);
            }
        }

        public object this[int i]
        {
            get
            {
                checkState();
                checkIndex(i);
                return GetValue(i);
            }
        }

        #endregion

        #region Private helper methods
        private void checkState()
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_currentIndex < 0 || _currentIndex >= _provider.Geometries.Count)
            {
                throw new InvalidOperationException("No data available at this index.");
            }
        }

        private static void checkIndex(int i)
        {
            if (i >= 1) throw new IndexOutOfRangeException("Column index is out of range.");
        }
        #endregion


        #region IFeatureDataRecord Members


        public TOid GetOid<TOid>()
        {
            throw new NotImplementedException();
        }

        public Type OidType
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerator<IFeatureDataRecord> Members

        public IFeatureDataRecord Current
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { throw new NotImplementedException(); }
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
