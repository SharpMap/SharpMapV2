/*
 *  The attached / following is part of SharpMap.Data.Providers.Db
 *  SharpMap.Data.Providers.Db is free software © 2008 Newgrove Consultants Limited, 
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
using System.Data;
using GeoAPI.Geometries;

namespace SharpMap.Data.Providers.Database
{
    internal class SpatialDbFeatureDataReader : IFeatureDataReader
    {
        private readonly int _geomColumnIndex;
        private readonly IGeometryFactory _geomFactory;
        private readonly IDataReader _internalReader;
        private readonly string _oidColumn;

        private readonly int _oidColumnIndex;
        private string _geometryColumn;

        public SpatialDbFeatureDataReader(IGeometryFactory geomFactory, IDataReader internalReader,
                                          string geometryColumn, string oidColumn)
        {
            _geomFactory = geomFactory;
            _internalReader = internalReader;
            _geometryColumn = geometryColumn;
            _oidColumn = oidColumn;

            _geomColumnIndex = GetOrdinal(geometryColumn);
            _oidColumnIndex = GetOrdinal(oidColumn);
        }

        #region IFeatureDataReader Members

        public void Close()
        {
            _internalReader.Close();
        }

        public int Depth
        {
            get { return _internalReader.Depth; }
        }

        public DataTable GetSchemaTable()
        {
            return _internalReader.GetSchemaTable();
        }

        public bool IsClosed
        {
            get { return _internalReader.IsClosed; }
        }

        public bool NextResult()
        {
            return _internalReader.NextResult();
        }

        public bool Read()
        {
            return _internalReader.Read();
        }

        public int RecordsAffected
        {
            get { return _internalReader.RecordsAffected; }
        }

        public void Dispose()
        {
            _internalReader.Dispose();
        }

        public int FieldCount
        {
            get { return _internalReader.FieldCount; }
        }

        public bool GetBoolean(int i)
        {
            return _internalReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _internalReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _internalReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _internalReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _internalReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return _internalReader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return _internalReader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _internalReader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return _internalReader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return _internalReader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return _internalReader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return _internalReader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return _internalReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _internalReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _internalReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _internalReader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return _internalReader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return _internalReader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return _internalReader.GetString(i);
        }

        public object GetValue(int i)
        {
            return _internalReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _internalReader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return _internalReader.IsDBNull(i);
        }

        public object this[string name]
        {
            get { return _internalReader[name]; }
        }

        public object this[int i]
        {
            get { return _internalReader[i]; }
        }

        public IGeometry Geometry
        {
            get
            {
                if (_geomColumnIndex > -1)
                    return _geomFactory.WkbReader.Read((byte[]) this[_geomColumnIndex]);

                return null;
            }
        }

        public object GetOid()
        {
            if (HasOid)
                return this[_oidColumn];
            return null;
        }

        public bool HasOid
        {
            get { return _oidColumnIndex > -1; }
        }

        public bool IsFullyLoaded
        {
            get { return false; }
        }

        public Type OidType
        {
            get { return typeof (long); }
        }

        public IEnumerator<IFeatureDataRecord> GetEnumerator()
        {
            try
            {
                while (_internalReader.Read())
                    yield return this;
            }
            finally
            {
                _internalReader.Close();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}