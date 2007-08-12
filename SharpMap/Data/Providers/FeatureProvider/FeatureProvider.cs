using System;
using System.Collections.Generic;
using System.Data;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers.FeatureProvider
{
	public class FeatureProvider : IWritableVectorLayerProvider<Guid>
	{
		private FeatureDataTable<Guid> _features = new FeatureDataTable<Guid>("Oid");
		private ICoordinateTransformation _transform = null;

		public FeatureProvider(params DataColumn[] columns)
		{
			_features.Columns.AddRange(columns);
		}

		#region IWritableVectorLayerProvider<Guid> Members

		public void Insert(FeatureDataRow<Guid> feature)
		{
			_features.ImportRow(feature);
		}

		public void Insert(IEnumerable<FeatureDataRow<Guid>> features)
		{
			foreach (FeatureDataRow<Guid> feature in features)
			{
				Insert(feature);
			}
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

		public DataTable GetSchemaTable()
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
			IFeatureDataReader reader = ExecuteIntersectionQuery(box);
			table.Load(reader);
		}

		public IFeatureDataReader ExecuteIntersectionQuery(BoundingBox box)
		{
			FeatureDataReader reader = new FeatureDataReader(_features, box);
			return reader;
		}

		public int GetFeatureCount()
		{
			return _features.FeatureCount;
		}

		public void SetTableSchema(FeatureDataTable table)
		{
			_features.CopyTableSchema(table);
		}

		#endregion

		#region ILayerProvider Members

		public ICoordinateTransformation CoordinateTransformation
		{
			get { return _transform; }
			set { _transform = value; }
		}

		public ICoordinateSystem SpatialReference
		{
			get { return GeographicCoordinateSystem.WGS84;  }
		}

		public bool IsOpen
		{
			get { return true; }
		}

		public int? Srid
		{
			get { return null; }
			set {  }
		}

		public BoundingBox GetExtents()
		{
			return _features.Envelope;
		}

		public string ConnectionId
		{
			get { return String.Empty; }
		}

		public void Open()
		{
			// Do nothing...
		}

		public void Close()
		{
			// Do nothing...
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if(_features != null)
			{
				_features.Dispose();
				_features = null;
			}
		}

		#endregion
	}
}