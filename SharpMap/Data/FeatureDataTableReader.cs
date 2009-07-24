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
using GeoAPI.CoordinateSystems.Transformations;
using System.Data;
using GeoAPI.Geometries;

namespace SharpMap.Data
{
    /// <summary>
    /// Provides a fast-forward, read-only data stream to feature data
    /// from a <see cref="FeatureDataTable"/>.
    /// </summary>
    public class FeatureDataTableReader : IFeatureDataReader
    {
        private readonly FeatureDataTable _table;
        private FeatureDataRow _currentRow;
        private int _nextRowIndex = 0;
        private ICoordinateTransformation _coordinateTransform;
        private bool _isDisposed = false;

        #region Contruction and Disposal function

        public FeatureDataTableReader(FeatureDataTable table)
        {
            _table = table;
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
                //Clean up anything that needs freeing up here
                          
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

        #region IFeatureDataRecord Members

        public IGeometry Geometry
        {
            get 
            {
                checkState();
                return _currentRow.Geometry == null ? null : _currentRow.Geometry.Clone();
            }
        }

        public IExtents Extents
        {
            get {
                checkState();
                return _currentRow.Geometry == null ? null : _currentRow.Geometry.Extents.Clone() as IExtents;
            }
        }

        public Object GetOid()
        {
            throw new NotImplementedException();
        }

        public Boolean HasOid
        {
            get { throw new NotImplementedException(); }
        }

        public Type OidType
        {
            get { throw new NotImplementedException(); }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { return _coordinateTransform; }
            set { _coordinateTransform = value; }
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
            //Not too sure what Depth is refering to?
            get { return 0; }
        }

        public DataTable GetSchemaTable()
        {
            return _table.Copy();
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
            bool read = false;
            if (_nextRowIndex < _table.Rows.Count)
            {
                _currentRow = _table[_nextRowIndex++];
                read = true;
            }
            
            return read;
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
                return _table.Columns.Count;    
            }
        }

        public Boolean GetBoolean(Int32 i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToBoolean(_currentRow[i]);
            
        }

        public Byte GetByte(Int32 i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToByte(_currentRow[i]);
        }

        public Int64 GetBytes(Int32 i, Int64 fieldOffset, Byte[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public Char GetChar(Int32 i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToChar(_currentRow[i]);
        }

        public Int64 GetChars(Int32 i, Int64 fieldoffset, Char[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDataReader GetData(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentRow.GetData(i);
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
            return Convert.ToDateTime(_currentRow[i]);
        }

        public decimal GetDecimal(Int32 i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToDecimal(_currentRow[i]);
        }

        public Double GetDouble(Int32 i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToDouble(_currentRow[i]);
        }

        public Type GetFieldType(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentRow.GetFieldType(i);
        }

        public Single GetFloat(Int32 i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToSingle(_currentRow[i]);
        }

        public Guid GetGuid(Int32 i)
        {
            checkState();
            checkIndex(i);

            Object feature = _currentRow[i];

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
            return Convert.ToInt16(_currentRow[i]);
        }

        public Int32 GetInt32(Int32 i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToInt32(_currentRow[i]);
        }

        public Int64 GetInt64(Int32 i)
        {
            checkState();
            checkIndex(i);
            return Convert.ToInt64(_currentRow[i]);
        }

        public String GetName(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _table.Rows[i][0] as String;
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
            return Convert.ToString(_currentRow[i]);
        }

        public Object GetValue(Int32 i)
        {
            checkState();
            checkIndex(i);
            return _currentRow[i];
        }

        public Int32 GetValues(Object[] values)
        {
            checkState();

            if (values == null) 
                throw new ArgumentNullException("values");

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
            return _currentRow.IsDBNull(i);
        }

        public Object this[String name]
        {
            get { return _currentRow[name]; }
        }

        public Object this[Int32 i]
        {
            get 
            {
                checkState();
                checkIndex(i);
                return _currentRow[i]; 
            }
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
            if (_currentRow == null)
            {
                throw new InvalidOperationException("The Read method must be called before accessing values.");
            }
        }

        private void checkDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().ToString());
        }

        #endregion
    }
}