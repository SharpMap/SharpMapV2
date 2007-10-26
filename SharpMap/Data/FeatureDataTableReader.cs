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
using SharpMap.Data;
using System.Data;
using SharpMap.Geometries;

namespace SharpMap.Data
{
    /// <summary>
    /// Provides a fast-forward, read-only data stream to feature data
    /// from a <see cref="FeatureDataTable"/>.
    /// </summary>
    public class FeatureDataTableReader : IFeatureDataReader
    {
        private readonly FeatureDataTable _table;

        public FeatureDataTableReader(FeatureDataTable table)
        {
            _table = table;
        }

        #region IFeatureDataRecord Members

        public Geometry Geometry
        {
            get { throw new NotImplementedException(); }
        }

        public object GetOid()
        {
            throw new NotImplementedException();
        }

        public bool HasOid
        {
            get { throw new NotImplementedException(); }
        }

        public Type OidType
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsFullyLoaded
        {
            get { return true; }
        }

        #endregion

        #region IDataReader Members

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Int32 Depth
        {
            get { throw new NotImplementedException(); }
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            throw new NotImplementedException();
        }

        public Int32 RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDataRecord Members

        public Int32 FieldCount
        {
            get { throw new NotImplementedException(); }
        }

        public bool GetBoolean(Int32 i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(Int32 i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(Int32 i, long fieldOffset, byte[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(Int32 i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(Int32 i, long fieldoffset, char[] buffer, Int32 bufferoffset, Int32 length)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDataReader GetData(Int32 i)
        {
            throw new NotImplementedException();
        }

        public String GetDataTypeName(Int32 i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(Int32 i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(Int32 i)
        {
            throw new NotImplementedException();
        }

        public Double GetDouble(Int32 i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(Int32 i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(Int32 i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(Int32 i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(Int32 i)
        {
            throw new NotImplementedException();
        }

        public Int32 GetInt32(Int32 i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(Int32 i)
        {
            throw new NotImplementedException();
        }

        public String GetName(Int32 i)
        {
            throw new NotImplementedException();
        }

        public Int32 GetOrdinal(String name)
        {
            throw new NotImplementedException();
        }

        public String GetString(Int32 i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(Int32 i)
        {
            throw new NotImplementedException();
        }

        public Int32 GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(Int32 i)
        {
            throw new NotImplementedException();
        }

        public object this[String name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[Int32 i]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerable<IFeatureDataRecord> Members

        public IEnumerator<IFeatureDataRecord> GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}