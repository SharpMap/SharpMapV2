using System;
using System.Collections.ObjectModel;
using System.Data;
using GeoAPI.Geometries;
using GeoAPI.IO.WellKnownText;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Data.Providers.GeometryProvider;
using SharpMap.Layers;

namespace SharpMap.Tests
{
    internal static class DataSourceHelper
    {
        internal static IFeatureLayerProvider CreateGeometryDatasource(IGeometryFactory geoFactory)
        {
            Collection<IGeometry> geoms = new Collection<IGeometry>();
            geoms.Add(GeometryFromWkt.Parse("POINT EMPTY", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), ((5 5, 7 5, 7 7, 5 7, 5 5)))", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)", geoFactory));
            geoms.Add(
                GeometryFromWkt.Parse("MULTILINESTRING ((10 10, 40 50), (20 20, 30 20), (20 20, 50 20, 50 60, 20 20))", geoFactory));
            geoms.Add(GeometryFromWkt.Parse(
                          "POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), (21 21, 29 21, 29 " +
                          "29, 21 29, 21 21), (23 23, 23 27, 27 27, 27 23, 23 23))", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("POINT (58.813841159 84.7561198972)", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("MULTIPOINT (20 100, 45 32, 120 54)", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("MULTIPOLYGON EMPTY", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("MULTILINESTRING EMPTY", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("MULTIPOINT EMPTY", geoFactory));
            geoms.Add(GeometryFromWkt.Parse("LINESTRING EMPTY", geoFactory));
            return new GeometryProvider(geoms);
        }

        internal static GeometryLayer CreateGeometryFeatureLayer(IGeometryFactory geoFactory)
        {
            GeometryLayer layer = new GeometryLayer("TestGeometries", CreateGeometryDatasource(geoFactory));
            return layer;
        }

        internal static GeometryLayer CreateFeatureFeatureLayer(IGeometryFactory geoFactory)
        {
            GeometryLayer layer = new GeometryLayer("TestFeatures", CreateFeatureDatasource(geoFactory));
            return layer;
        }

        internal static FeatureProvider CreateFeatureDatasource(IGeometryFactory geoFactory)
        {
            DataColumn nameColumn = new DataColumn("FeatureName", typeof (String));
            FeatureProvider provider = new FeatureProvider(nameColumn);

            FeatureDataTable<Guid> features = new FeatureDataTable<Guid>("Oid");
            features.Columns.Add("FeatureName", typeof (String));
            FeatureDataRow<Guid> row;

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty point";
            row.Geometry = GeometryFromWkt.Parse("POINT EMPTY", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A geometry collection";
            row.Geometry =
                GeometryFromWkt.Parse("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multipolygon";
            row.Geometry =
                GeometryFromWkt.Parse("MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), ((5 5, 7 5, 7 7, 5 7, 5 5)))", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A linestring";
            row.Geometry = GeometryFromWkt.Parse("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multilinestring";
            row.Geometry =
                GeometryFromWkt.Parse("MULTILINESTRING ((10 10, 40 50), (20 20, 30 20), (20 20, 50 20, 50 60, 20 20))", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A polygon";
            row.Geometry = GeometryFromWkt.Parse(
                "POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), (21 21, 29 21, 29 " +
                "29, 21 29, 21 21), (23 23, 23 27, 27 27, 27 23, 23 23))", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A point";
            row.Geometry = GeometryFromWkt.Parse("POINT (58.813841159 84.7561198972)", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multipoint";
            row.Geometry = GeometryFromWkt.Parse("MULTIPOINT (20 100, 45 32, 120 54)", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multipolygon";
            row.Geometry = GeometryFromWkt.Parse("MULTIPOLYGON EMPTY", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multilinestring";
            row.Geometry = GeometryFromWkt.Parse("MULTILINESTRING EMPTY", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multipoint";
            row.Geometry = GeometryFromWkt.Parse("MULTIPOINT EMPTY", geoFactory);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty linestring";
            row.Geometry = GeometryFromWkt.Parse("LINESTRING EMPTY", geoFactory);
            features.AddRow(row);

            provider.Insert(features);

            return provider;
        }
    }
}