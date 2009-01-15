// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    public class DefaultFeatureCache : IFeaturesCache
    {
        private readonly DefaultFeatureSchema _schema;
        private readonly List<IFeatureDataRecord> _records = new List<IFeatureDataRecord>();

        public DefaultFeatureCache(DefaultFeatureSchema schema)
        {
            _schema = schema;
        }

        #region IFeaturesCache Members

        public bool IsSpatiallyIndexed
        {
            get { return false; }
            set { throw new NotSupportedException(); }
        }

        public void AddRecord(IFeatureDataRecord record)
        {
            if (!_schema.Equals(record.Schema))
            {
                throw new ArgumentException("Record doesn't match cache schema.");
            }

            _records.Add(record);
        }

        public IFeatureDataRecord this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        public int FeatureCount
        {
            get { throw new NotImplementedException(); }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IFeaturesCache Clone()
        {
            throw new NotImplementedException();
        }

        public void CloneTo(IFeaturesCache cache)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataRecord Find(object key)
        {
            throw new NotImplementedException();
        }

        public IFeaturesCache GetChanges()
        {
            throw new NotImplementedException();
        }

        public void ImportRow(IFeatureDataRecord feature)
        {
            throw new NotImplementedException();
        }

        public void Load(IFeatureDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler)
        {
            throw new NotImplementedException();
        }

        public void Merge(IFeaturesCache features)
        {
            throw new NotImplementedException();
        }

        public void Merge(IFeaturesCache features, bool preserveChanges)
        {
            throw new NotImplementedException();
        }

        public void Merge(IFeaturesCache features, bool preserveChanges, SchemaMergeAction schemaMergeAction)
        {
            throw new NotImplementedException();
        }

        public void Merge(IFeatureDataRecord record, IGeometryFactory factory, SchemaMergeAction schemaMergeAction)
        {
            throw new NotImplementedException();
        }

        public void Merge(IEnumerable<IFeatureDataRecord> records, IGeometryFactory factory)
        {
            throw new NotImplementedException();
        }

        public void Merge(IEnumerable<IFeatureDataRecord> records,
                          ICoordinateTransformation transform,
                          IGeometryFactory factory)
        {
            throw new NotImplementedException();
        }

        public void Merge(IEnumerable<IFeatureDataRecord> records,
                          ICoordinateTransformation transform,
                          IGeometryFactory factory,
                          SchemaMergeAction schemaMergeAction)
        {
            throw new NotImplementedException();
        }

        public void MergeSchemaTo(IFeaturesCache target)
        {
            throw new NotImplementedException();
        }

        public void MergeSchemaTo(IFeaturesCache target, SchemaMergeAction schemaMergeAction)
        {
            throw new NotImplementedException();
        }

        public void MergeSchemaFrom(IFeaturesCache source)
        {
            throw new NotImplementedException();
        }

        public void MergeSchemaFrom(IFeaturesCache source, SchemaMergeAction schemaMergeAction)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataRecord NewFeature()
        {
            throw new NotImplementedException();
        }

        public void RemoveFeature(IFeatureDataRecord row)
        {
            throw new NotImplementedException();
        }

        public IFeaturesView Select(IExtents bounds)
        {
            throw new NotImplementedException();
        }

        public IFeaturesView Select(IGeometry geometry)
        {
            throw new NotImplementedException();
        }

        public IFeaturesView Select(SpatialBinaryExpression query)
        {
            throw new NotImplementedException();
        }

        public IFeaturesView DefaultView
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}