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
using System.Data;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Geometries;
using System.Globalization;

namespace SharpMap.Data.Providers.FeatureProvider
{
	public class FeatureProvider : IWritableFeatureLayerProvider<Guid>
	{
        internal readonly static string OidColumnName = "Oid";
		private FeatureDataTable<Guid> _features = new FeatureDataTable<Guid>(OidColumnName);
		private ICoordinateTransformation _transform = null;

		public FeatureProvider(params DataColumn[] columns)
		{
            foreach (DataColumn column in columns)
            {
                string keyColumnName = _features.PrimaryKey[0].ColumnName;
                if (String.Compare(keyColumnName, column.ColumnName) != 0)
                {
                    _features.Columns.Add(column);
                }
            }
		}

		#region IWritableFeatureLayerProvider<Guid> Members

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

		#region IFeatureLayerProvider<Guid> Members

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

        public void SetTableSchema(FeatureDataTable<Guid> table, SchemaMergeAction schemaMergeAction)
        {
            _features.MergeSchema(table, schemaMergeAction);
        }

		#endregion

        #region IFeatureLayerProvider Members

        public CultureInfo Locale
        {
            get { throw new NotImplementedException(); }
        }

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
			_features.MergeSchema(table);
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