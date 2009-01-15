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
    public interface IFeaturesCache
    {
        Boolean IsSpatiallyIndexed { get; set; }
        void AddRecord(IFeatureDataRecord record);
        IFeatureDataRecord this[Int32 index] { get; }
        Int32 FeatureCount { get; }
        void Clear();
        IFeaturesCache Clone();
        void CloneTo(IFeaturesCache cache);
        IFeatureDataRecord Find(Object key);
        IFeaturesCache GetChanges();
        void ImportRow(IFeatureDataRecord feature);

        void Load(IFeatureDataReader reader,
                  LoadOption loadOption,
                  FillErrorEventHandler errorHandler);

        void Merge(IFeaturesCache features);
        void Merge(IFeaturesCache features, Boolean preserveChanges);

        void Merge(IFeaturesCache features,
                   Boolean preserveChanges,
                   SchemaMergeAction schemaMergeAction);

        void Merge(IFeatureDataRecord record,
                   IGeometryFactory factory,
                   SchemaMergeAction schemaMergeAction);
        void Merge(IEnumerable<IFeatureDataRecord> records, IGeometryFactory factory);
        void Merge(IEnumerable<IFeatureDataRecord> records,
                   ICoordinateTransformation transform,
                   IGeometryFactory factory);

        void Merge(IEnumerable<IFeatureDataRecord> records,
                   ICoordinateTransformation transform,
                   IGeometryFactory factory,
                   SchemaMergeAction schemaMergeAction);

        void MergeSchemaTo(IFeaturesCache target);
        void MergeSchemaTo(IFeaturesCache target, SchemaMergeAction schemaMergeAction);
        void MergeSchemaFrom(IFeaturesCache source);
        void MergeSchemaFrom(IFeaturesCache source, SchemaMergeAction schemaMergeAction);
        IFeatureDataRecord NewFeature();
        void RemoveFeature(IFeatureDataRecord row);
        IFeaturesView Select(IExtents bounds);
        IFeaturesView Select(IGeometry geometry);
        IFeaturesView Select(SpatialBinaryExpression query);
        IFeaturesView DefaultView { get; }
    }
}
