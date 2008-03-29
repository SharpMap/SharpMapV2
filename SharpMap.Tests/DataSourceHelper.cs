using System;
using System.Collections.ObjectModel;
using System.Data;
using GeoAPI.Geometries;
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
            geoms.Add(geoFactory.WktReader.Read("POINT EMPTY"));
            geoms.Add(geoFactory.WktReader.Read("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))"));
            geoms.Add(geoFactory.WktReader.Read("MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), ((5 5, 7 5, 7 7, 5 7, 5 5)))"));
            geoms.Add(geoFactory.WktReader.Read("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)"));
            geoms.Add(
                geoFactory.WktReader.Read("MULTILINESTRING ((10 10, 40 50), (20 20, 30 20), (20 20, 50 20, 50 60, 20 20))"));
            geoms.Add(geoFactory.WktReader.Read(
                          "POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), (21 21, 29 21, 29 " +
                          "29, 21 29, 21 21), (23 23, 23 27, 27 27, 27 23, 23 23))"));
            geoms.Add(geoFactory.WktReader.Read("POINT (58.813841159 84.7561198972)"));
            geoms.Add(geoFactory.WktReader.Read("MULTIPOINT (20 100, 45 32, 120 54)"));
            geoms.Add(geoFactory.WktReader.Read("MULTIPOLYGON EMPTY"));
            geoms.Add(geoFactory.WktReader.Read("MULTILINESTRING EMPTY"));
            geoms.Add(geoFactory.WktReader.Read("MULTIPOINT EMPTY"));
            geoms.Add(geoFactory.WktReader.Read("LINESTRING EMPTY"));
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
            FeatureProvider provider = new FeatureProvider(geoFactory, nameColumn);

            FeatureDataTable<Guid> features = new FeatureDataTable<Guid>("Oid", geoFactory);
            features.Columns.Add("FeatureName", typeof (String));
            FeatureDataRow<Guid> row;

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty point";
            row.Geometry = geoFactory.WktReader.Read("POINT EMPTY");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A geometry collection";
            row.Geometry = geoFactory.WktReader.Read(
                "GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multipolygon";
            row.Geometry = geoFactory.WktReader.Read(
                "MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), ((5 5, 7 5, 7 7, 5 7, 5 5)))");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A linestring";
            row.Geometry = geoFactory.WktReader.Read(
                "LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multilinestring";
            row.Geometry = geoFactory.WktReader.Read(
                "MULTILINESTRING ((10 10, 40 50), (20 20, 30 20), (20 20, 50 20, 50 60, 20 20))");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A polygon";
            row.Geometry = geoFactory.WktReader.Read(
                "POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), (21 21, 29 21, 29 " +
                "29, 21 29, 21 21), (23 23, 23 27, 27 27, 27 23, 23 23))");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A point";
            row.Geometry = geoFactory.WktReader.Read(
                "POINT (58.813841159 84.7561198972)");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multipoint";
            row.Geometry = geoFactory.WktReader.Read(
                "MULTIPOINT (20 100, 45 32, 120 54)");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multipolygon";
            row.Geometry = geoFactory.WktReader.Read("MULTIPOLYGON EMPTY");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multilinestring";
            row.Geometry = geoFactory.WktReader.Read("MULTILINESTRING EMPTY");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multipoint";
            row.Geometry = geoFactory.WktReader.Read("MULTIPOINT EMPTY");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty linestring";
            row.Geometry = geoFactory.WktReader.Read("LINESTRING EMPTY");
            features.AddRow(row);

            provider.Insert(features);

            return provider;
        }
    }
}