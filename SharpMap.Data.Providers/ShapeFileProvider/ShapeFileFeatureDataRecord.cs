using System;
using System.Collections.Generic;
using System.Data;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal class ShapeFileFeatureDataRecord : IFeatureDataRecord
    {
        private readonly Dictionary<Int32, Object> _rowValues = new Dictionary<Int32, Object>();
        private readonly Dictionary<Int32, String> _rowColumns = new Dictionary<Int32, String>();
        private IGeometry _geometry;
        private readonly Int32 _oidColumn;
        private readonly Int32 _columnCount;

        public ShapeFileFeatureDataRecord(Int32 oidColumn, Int32 columnCount)
        {
            if (oidColumn < 0 || oidColumn >= columnCount)
            {
                throw new ArgumentOutOfRangeException("oidColumn",
                                                      oidColumn,
                                                      "Parameter must be between 0 and 'columnCount'.");
            }
            _oidColumn = oidColumn;
            _columnCount = columnCount;
        }

        public void AddColumnValue(Int32 columnOrdinal, String columnName, Object value)
        {
            if (_rowValues.ContainsKey(columnOrdinal) || _rowColumns.ContainsKey(columnOrdinal))
            {
                throw new InvalidOperationException("Column with ordinal " + columnOrdinal + " already exists.");
            }

            _rowValues[columnOrdinal] = value;
            _rowColumns[columnOrdinal] = columnName;
        }

        #region Implementation of IDataRecord

        public string GetName(int i)
        {
            checkIndex(i);
            return _rowColumns[i];
        }

        private void checkIndex(int i)
        {
            if (i < 0 || i >= _columnCount)
            {
                throw new IndexOutOfRangeException();
            }
        }

        public string GetDataTypeName(int i)
        {
            throw new System.NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new System.NotImplementedException();
        }

        public object GetValue(int i)
        {
            throw new System.NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (values.Length == 0)
            {
                return 0;
            }

            Int32 objectCount = Math.Min(values.Length, _columnCount);

            List<KeyValuePair<Int32, Object>> sortedValues = new List<KeyValuePair<Int32, Object>>(_rowValues);
            sortedValues.Sort(delegate(KeyValuePair<Int32, Object> a, KeyValuePair<Int32, Object> b)
            {
                return a.Key.CompareTo(b.Key);
            });

            for (int i = 0; i < objectCount; i++)
            {
                values[i] = sortedValues[i].Value;
            }

            return objectCount;
        }

        public int GetOrdinal(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            foreach (KeyValuePair<int, string> column in _rowColumns)
            {
                if (name.Equals(column.Value))
                {
                    return column.Key;
                }
            }

            return -1;
        }

        public bool GetBoolean(int i)
        {
            checkIndex(i);

            return (Boolean)_rowValues[i];
        }

        public byte GetByte(int i)
        {
            checkIndex(i);

            return (Byte)_rowValues[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new System.NotImplementedException();
        }

        public char GetChar(int i)
        {
            checkIndex(i);

            return (Char)_rowValues[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new System.NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            checkIndex(i);

            return (Guid)_rowValues[i];
        }

        public short GetInt16(int i)
        {
            checkIndex(i);

            return (Int16)_rowValues[i];
        }

        public int GetInt32(int i)
        {
            checkIndex(i);

            return (Int32)_rowValues[i];
        }

        public long GetInt64(int i)
        {
            checkIndex(i);

            return (Int64)_rowValues[i];
        }

        public float GetFloat(int i)
        {
            checkIndex(i);

            return (Single)_rowValues[i];
        }

        public double GetDouble(int i)
        {
            checkIndex(i);

            return (Double)_rowValues[i];
        }

        public string GetString(int i)
        {
            checkIndex(i);

            return (String)_rowValues[i];
        }

        public decimal GetDecimal(int i)
        {
            checkIndex(i);

            return (Decimal)_rowValues[i];
        }

        public DateTime GetDateTime(int i)
        {
            checkIndex(i);

            return (DateTime)_rowValues[i];
        }

        public IDataReader GetData(int i)
        {
            throw new System.NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            checkIndex(i);

            Object value;
            return !_rowValues.TryGetValue(i, out value) && DBNull.Value.Equals(value);
        }

        public int FieldCount
        {
            get { return _columnCount; }
        }

        object IDataRecord.this[int i]
        {
            get
            {
                checkIndex(i);
                return _rowValues[i];
            }
        }

        object IDataRecord.this[string name]
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion

        #region Implementation of IFeatureDataRecord

        public IGeometry Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
        }

        public IExtents Extents
        {
            get { return _geometry == null ? null : _geometry.Extents; }
        }

        public object GetOid()
        {
            return _rowValues[_oidColumn];
        }

        public bool HasOid
        {
            get
            {
                return true;
            }
        }

        public bool IsFullyLoaded
        {
            get
            {
                return true;
            }
        }

        public Type OidType
        {
            get
            {
                return typeof(UInt32);
            }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}
