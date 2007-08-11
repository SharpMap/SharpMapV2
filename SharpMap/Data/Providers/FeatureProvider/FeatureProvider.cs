using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Geometries;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.CoordinateSystems;

namespace SharpMap.Data.Providers.FeatureProvider
{
    public class FeatureProvider : IWritableVectorLayerProvider<Guid>
    {
        #region IWritableVectorLayerProvider<Guid> Members

        public void Insert(FeatureDataRow<Guid> feature)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<FeatureDataRow<Guid>> features)
        {
            throw new NotImplementedException();
        }

        public void Update(FeatureDataRow<Guid> feature)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<FeatureDataRow<Guid>> features)
        {
            throw new NotImplementedException();
        }

        public void Delete(FeatureDataRow<Guid> feature)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<FeatureDataRow<Guid>> features)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVectorLayerProvider<Guid> Members

        public IEnumerable<Guid> GetObjectIdsInView(BoundingBox boundingBox)
        {
            throw new NotImplementedException();
        }

        public Geometry GetGeometryById(Guid oid)
        {
            throw new NotImplementedException();
        }

        public FeatureDataRow<Guid> GetFeature(Guid oid)
        {
            throw new NotImplementedException();
        }

        public void SetTableSchema(FeatureDataTable<Guid> table)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVectorLayerProvider Members

        public IEnumerable<Geometry> GetGeometriesInView(BoundingBox boundingBox)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(Geometry geom, FeatureDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(Geometry geom, FeatureDataTable table)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteIntersectionQuery(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(BoundingBox box, FeatureDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public void ExecuteIntersectionQuery(BoundingBox box, FeatureDataTable table)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataReader ExecuteIntersectionQuery(BoundingBox box)
        {
            throw new NotImplementedException();
        }

        public int GetFeatureCount()
        {
            throw new NotImplementedException();
        }

        public void SetTableSchema(FeatureDataTable table)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ILayerProvider Members

        public ICoordinateTransformation CoordinateTransformation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ICoordinateSystem SpatialReference
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsOpen
        {
            get { throw new NotImplementedException(); }
        }

        public int? Srid
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public BoundingBox GetExtents()
        {
            throw new NotImplementedException();
        }

        public string ConnectionId
        {
            get { throw new NotImplementedException(); }
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
