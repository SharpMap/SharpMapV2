using System;
using System.Collections.Generic;
using System.Text;

using System;
using IBM.Data.DB2;
using IBM.Data.DB2Types;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;

namespace SharpMap.Data.Providers.DB2_SpatialExtender
{
    public class DB2_SpatialExtender_FeatureDataReader : SpatialDbFeatureDataReader
    {
        protected internal DB2_SpatialExtender_FeatureDataReader(
            IGeometryFactory geomFactory, DB2DataReader internalReader,
            string geometryColumn, string oidColumn) :
            base(geomFactory, internalReader, geometryColumn, oidColumn)
        {

        }
        /// <summary>
        /// Gets the Geometry from DataReader
        /// </summary>
        public override IGeometry Geometry
        {
            get
            {
                if (HasGeometry)
                {
                    if (_currentGeometry == null)
                    {
                        DB2Blob blob = ((DB2DataReader)_internalReader).GetDB2Blob(_geomColumnIndex);
                        _currentGeometry = _geomFactory.WkbReader.Read(blob.Value);
                    }
                }
                return _currentGeometry;
            }
        }
        /// <summary>
        /// Gets DB2Blob from DataReader
        /// </summary>
        /// <param name="i">Colum index of DB2Blob Value</param>
        /// <returns>DB2Blob</returns>
        public DB2Blob GetDB2Blob(int i)
        {
            return ((DB2DataReader)_internalReader).GetDB2Blob(i);
        }
        /// <summary>
        /// Gets DB2Clob from DataReader
        /// </summary>
        /// <param name="i">Colum index of DB2Clob Value</param>
        /// <returns>DB2Clob</returns>
        public DB2Clob GetDB2Clob(int i)
        {
            return ((DB2DataReader)_internalReader).GetDB2Clob(i);
        }
        /// <summary>
        /// Gets DB2Binary from DataReader
        /// </summary>
        /// <param name="i">Colum index of DB2Binary Value</param>
        /// <returns>DB2Binary</returns>
        public DB2Binary GetDB2Binary(int i)
        {
            return ((DB2DataReader)_internalReader).GetDB2Binary(i);
        }
        public DB2Date GetDB2Date(int i)
        {
            return ((DB2DataReader)_internalReader).GetDB2Date(i);
        }
        public DB2Decimal GetDB2Decimal(int i)
        {
            return ((DB2DataReader)_internalReader).GetDB2Decimal(i);
        }
        public DB2DecimalFloat GetDB2DecimalFloat(int i)
        {
            return ((DB2DataReader)_internalReader).GetDB2DecimalFloat(i);
        }
        public DB2Double GetDB2Double(int i)
        {
            return ((DB2DataReader)_internalReader).GetDB2Double(i);
        }
    }
}
