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

/*
 * Portions by Felix Obermaier
 * Ingenieurgruppe IVV GmbH & Co. KG
 * www.ivv-aachen.de
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Data.Providers.Db
{
    public class SpatialDbFeatureDataReader : IFeatureDataReader
    {
        protected readonly int _geomColumnIndex = -1;
        private readonly string _geometryColumn;
        protected readonly IGeometryFactory _geomFactory;
        protected readonly IDataReader _internalReader;
        private readonly string _oidColumn;

        private readonly int _oidColumnIndex = -1;
        private readonly Type _oidType;
        protected IGeometry _currentGeometry;
        private bool _disposed;
        private DataTable _schema;

        private IGeometryFactory _transFactory;

        protected internal SpatialDbFeatureDataReader(IGeometryFactory geomFactory, IDataReader internalReader,
                                                      string geometryColumn, string oidColumn)
        {
            _geomFactory = geomFactory;
            _internalReader = internalReader;
            _geometryColumn = geometryColumn;
            _oidColumn = oidColumn;


            for (int i = 0; i < internalReader.FieldCount; i++)
                // note: GetOrdinal crashes if the column does not exist so loop through fields
            {
                string name = internalReader.GetName(i);
                if (name == geometryColumn)
                    _geomColumnIndex = i;

                if (name == oidColumn)
                    _oidColumnIndex = i;

                if (_geomColumnIndex > -1 && _oidColumnIndex > -1)
                    break;
            }
            if (_geomColumnIndex > -1)
                _oidType = _internalReader.GetFieldType(_geomColumnIndex);
        }

        #region private helper

        protected Int32 recomputeIndex(Int32 index)
        {
            if (_geomColumnIndex > -1 && index >= _geomColumnIndex)
                return index + 1;

            return index;
        }

        #endregion

        public bool HasGeometry
        {
            get
            {
                return _geomColumnIndex > -1
                       && !_internalReader.IsDBNull(_geomColumnIndex);
            }
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        #region IFeatureDataReader Members

        public void Close()
        {
            Dispose();
        }

        public int Depth
        {
            get { return _internalReader.Depth; }
        }

        public DataTable GetSchemaTable()
        {
            if (_schema == null)
            {
                _schema = _internalReader.GetSchemaTable().Copy();
                if (_geomColumnIndex > -1) _schema.Rows.RemoveAt(_geomColumnIndex);
            }
            return _schema.Copy();
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
            _currentGeometry = null;
            return _internalReader.Read();
        }

        public int RecordsAffected
        {
            get { return _internalReader.RecordsAffected; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int FieldCount
        {
            get
            {
                return _geomColumnIndex > -1
                           ?
                               _internalReader.FieldCount - 1
                           :
                               _internalReader.FieldCount;
            }
        }

        public bool GetBoolean(int i)
        {
            return _internalReader.GetBoolean(recomputeIndex(i));
        }

        public byte GetByte(int i)
        {
            return _internalReader.GetByte(recomputeIndex(i));
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _internalReader.GetBytes(recomputeIndex(i), fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _internalReader.GetChar(recomputeIndex(i));
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _internalReader.GetChars(recomputeIndex(i), fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return _internalReader.GetData(recomputeIndex(i));
        }

        public string GetDataTypeName(int i)
        {
            return _internalReader.GetDataTypeName(recomputeIndex(i));
        }

        public DateTime GetDateTime(int i)
        {
            return _internalReader.GetDateTime(recomputeIndex(i));
        }

        public decimal GetDecimal(int i)
        {
            return _internalReader.GetDecimal(recomputeIndex(i));
        }

        public double GetDouble(int i)
        {
            return _internalReader.GetDouble(recomputeIndex(i));
        }

        public virtual Type GetFieldType(int i)
        {
            return _internalReader.GetFieldType(recomputeIndex(i));
        }

        public float GetFloat(int i)
        {
            return _internalReader.GetFloat(recomputeIndex(i));
        }

        public Guid GetGuid(int i)
        {
            return _internalReader.GetGuid(recomputeIndex(i));
        }

        public short GetInt16(int i)
        {
            return _internalReader.GetInt16(recomputeIndex(i));
        }

        public int GetInt32(int i)
        {
            return _internalReader.GetInt32(recomputeIndex(i));
        }

        public long GetInt64(int i)
        {
            return _internalReader.GetInt64(recomputeIndex(i));
        }

        public string GetName(int i)
        {
            return _internalReader.GetName(recomputeIndex(i));
        }

        public int GetOrdinal(string name)
        {
            return _internalReader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return _internalReader.GetString(recomputeIndex(i));
        }

        public object GetValue(int i)
        {
            return _internalReader.GetValue(recomputeIndex(i));
        }

        public int GetValues(object[] values)
        {
            int i;
            for (i = 0; i < FieldCount && i < values.Length; i++)
                values[i] = GetValue(i);

            return i;
        }

        public bool IsDBNull(int i)
        {
            return _internalReader.IsDBNull(recomputeIndex(i));
        }

        public object this[string name]
        {
            get
            {
                if (name == _geometryColumn)
                    throw new DataException("Cannot retrieve geometry column. Use Geometry property");
                return _internalReader[name];
            }
        }

        public object this[int i]
        {
            get { return _internalReader[recomputeIndex(i)]; }
        }

        public virtual IGeometry Geometry
        {
            get
            {
                if (HasGeometry)
                {
                    if (_currentGeometry == null)
                    {
                        IGeometry geom = _geomFactory.WkbReader.Read((byte[]) _internalReader[_geomColumnIndex]);

                        if (CoordinateTransformation == null)
                            _currentGeometry = geom;
                        else
                        {
                            if (_transFactory == null)
                            {
                                _transFactory = _geomFactory.Clone();
                                _transFactory.SpatialReference = CoordinateTransformation.Target;
                                _transFactory.Srid = SridMap.DefaultInstance.Process(_transFactory.SpatialReference, "");
                            }
                            _currentGeometry = CoordinateTransformation.Transform(geom, _transFactory);
                            //_currentGeometry.Centroid = CoordinateTransformation.Transform( geom.Centroid, _transFactory );
                        }
                    }
                }
                return _currentGeometry;
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
            get { return _oidType; }
        }

        public IEnumerator<IFeatureDataRecord> GetEnumerator()
        {
            while (Read())
            {
                yield return this;
            }
            //FObermaier:
            //needed for npgsql, because npgsql throws exceptions after a while
            //if the connection of the reader is not closed properly.
            //The Dispose Method got never called, possibly memory leakage?
            Dispose();
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IExtents Extents
        {
            get { return Geometry.Extents; }
        }

        public ICoordinateTransformation CoordinateTransformation { get; set; }

        #endregion

        ~SpatialDbFeatureDataReader()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _internalReader.Dispose();
            _disposed = true;
        }
    }
}