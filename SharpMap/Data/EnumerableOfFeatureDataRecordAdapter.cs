using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Data
{
    public class EnumerableOfFeatureDataRecordAdapter : IFeatureDataReader
    {
        private IEnumerator<IFeatureDataRecord> _enumerator;
        private IFeatureDataRecord record = null;
        private bool _closed = false;
        public EnumerableOfFeatureDataRecordAdapter(IEnumerable<IFeatureDataRecord> enumerable)
        {
            _enumerator = enumerable.GetEnumerator();
        }

        #region IDataReader Members

        public void Close()
        {
            _closed = true;
        }

        public int Depth
        {
            get { return 0; }
        }

        public System.Data.DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get { return _closed; }
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            if (_enumerator.MoveNext())
            {
                record = _enumerator.Current;
                return true;
            }
            return false;
        }

        public int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get { return record.FieldCount; }
        }

        public bool GetBoolean(int i)
        {
            return record.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return record.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return record.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return record.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return record.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public System.Data.IDataReader GetData(int i)
        {
            return record.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return record.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return record.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return record.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return record.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return record.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return record.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return record.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return record.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return record.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return record.GetInt64(i);
        }

        public string GetName(int i)
        {
            return record.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return record.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return record.GetString(i);
        }

        public object GetValue(int i)
        {
            return record.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return record.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return record.IsDBNull(i);
        }

        public object this[string name]
        {
            get { return record[name]; }
        }

        public object this[int i]
        {
            get { return record[i]; }
        }

        #endregion

        #region IFeatureDataRecord Members

        public GeoAPI.Geometries.IGeometry Geometry
        {
            get { return record.Geometry; }
        }

        public GeoAPI.Geometries.IExtents Extents
        {
            get { return record.Extents; }
        }

        public object GetOid()
        {
            return record.GetOid();
        }

        public bool HasOid
        {
            get { return record.HasOid; }
        }

        public bool IsFullyLoaded
        {
            get { return record.IsFullyLoaded; }
        }

        public Type OidType
        {
            get { return record.OidType; }
        }

        public GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation CoordinateTransformation
        {
            get
            {
                return record.CoordinateTransformation;
            }
            set
            {
                record.CoordinateTransformation = value;
            }
        }

        #endregion

        #region IEnumerable<IFeatureDataRecord> Members

        public IEnumerator<IFeatureDataRecord> GetEnumerator()
        {
            while (Read())
                yield return this;
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
