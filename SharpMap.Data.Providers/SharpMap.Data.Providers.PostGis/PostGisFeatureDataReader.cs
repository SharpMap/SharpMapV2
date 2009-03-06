using System.Collections.Generic;
using SharpMap.Data.Providers.Db;
using System.Data;
using GeoAPI.Geometries;

namespace SharpMap.Data.Providers.PostGis
{
    public class PostGisFeatureDataReader : SpatialDbFeatureDataReader
    {
        protected internal PostGisFeatureDataReader(IGeometryFactory geomFactory, IDataReader internalReader,
                                                      string geometryColumn, string oidColumn)
            :base(geomFactory, internalReader,geometryColumn,oidColumn)
        {
        }

        public override IEnumerator<IFeatureDataRecord> GetEnumerator()
        {
            while (Read())
            {
                yield return this;
            }
            Dispose();
            yield break;
        }
    }
}
