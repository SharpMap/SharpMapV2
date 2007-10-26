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
using System.Threading;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers.GeometryProvider
{
    /// <summary>
    /// Provides a fast-forward, read-only data stream to geometry data
    /// from a <see cref="GeometryProvider"/>.
    /// </summary>
    public class GeometryDataReader : IFeatureDataReader
    {
        #region Instance fields
        private GeometryProvider _provider;
        private readonly BoundingBox _bounds;
        private Int32 _currentIndex = -1;
        private bool _isDisposed; 
        #endregion

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
		
        public Geometry Geometry
		{
			get
			{
				checkState();
				return _provider.Geometries[_currentIndex].Clone();
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

        public Int32 RecordsAffected
        {
            get
            {
                Int32 count = 0;

                foreach (Geometry geometry in _provider.Geometries)
                {
                    if (_bounds.Intersects(geometry)) count++;
                }

                return count;
            }
        }

        #endregion

        #region IDataRecord Members

        public Int32 FieldCount
        {
            get { return 1; }
        }

        public bool GetBoolean(Int32 i)
        {
            throw new NotSupportedException();
        }

        public byte GetByte(Int32 i)
        {
            checkState();
            checkIndex(i);
            return (byte)_currentIndex;
        }

        public long GetBytes(Int32 i, long fieldOffset, byte[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(Int32 i)
        {
            checkState();
            checkIndex(i);
            return (char)_currentIndex;
        }

        public long GetChars(Int32 i, long fieldoffset, char[] buffer, Int32 bufferoffset, Int32 length)
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
            return typeof(Int32).ToString();
        }

        public DateTime GetDateTime(Int32 i)
        {
            throw new NotSupportedException();
        }

        public decimal GetDecimal(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public Double GetDouble(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public Type GetFieldType(Int32 i)
        {
            checkState();
            checkIndex(i);
            return typeof(Int32);
        }

        public float GetFloat(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public Guid GetGuid(Int32 i)
        {
            throw new NotSupportedException();
        }

        public short GetInt16(Int32 i)
        {
            checkState();
            checkIndex(i);
            return (short)_currentIndex;
        }

        public Int32 GetInt32(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public long GetInt64(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public String GetName(Int32 i)
        {
            checkState();
            checkIndex(i);
            return "Oid";
        }

        public Int32 GetOrdinal(String name)
        {
            checkState();

            if (String.Compare(name, "Oid", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new ArgumentException("Column name must be 'Oid'; it was: " + name, "name");
            }

            return 0;
        }

        public String GetString(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex.ToString();
        }

        public object GetValue(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentIndex;
        }

        public Int32 GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        public bool IsDBNull(Int32 i)
        {
            checkState();
            checkIndex(i);
            return false;
        }

        public object this[String name]
        {
            get
            {
                checkState();
                Int32 ordinal = GetOrdinal(name);
                return GetInt32(ordinal);
            }
        }

        public object this[Int32 i]
        {
            get
            {
                checkState();
                checkIndex(i);
                return GetValue(i);
            }
        }

        #endregion

        #region IFeatureDataRecord Members

        public Type OidType
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

		#region IEnumerable<IFeatureDataRecord> Members

		public IEnumerator<IFeatureDataRecord> GetEnumerator()
		{
            while(Read())
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

        #region Private helper methods
        private void checkState()
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_currentIndex < 0 || _currentIndex >= _provider.Geometries.Count)
            {
                throw new InvalidOperationException("No data available at this index.");
            }
        }

        private static void checkIndex(Int32 i)
        {
            if (i >= 1) throw new IndexOutOfRangeException("Column index is out of range.");
        }
        #endregion
	}
}
