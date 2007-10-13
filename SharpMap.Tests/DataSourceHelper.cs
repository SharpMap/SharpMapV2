using System;
using System.Collections.ObjectModel;
using System.Data;
using SharpMap.Data;
using SharpMap.Data.Providers.FeatureProvider;
using SharpMap.Data.Providers.GeometryProvider;
using SharpMap.Geometries;
using SharpMap.Layers;

namespace SharpMap.Tests
{
    internal static class DataSourceHelper
    {
        internal static IFeatureLayerProvider CreateGeometryDatasource()
        {
            Collection<Geometry> geoms = new Collection<Geometry>();
            geoms.Add(Geometry.FromText("POINT EMPTY"));
            geoms.Add(Geometry.FromText("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))"));
            geoms.Add(Geometry.FromText("MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), ((5 5, 7 5, 7 7, 5 7, 5 5)))"));
            geoms.Add(Geometry.FromText("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)"));
            geoms.Add(
                Geometry.FromText("MULTILINESTRING ((10 10, 40 50), (20 20, 30 20), (20 20, 50 20, 50 60, 20 20))"));
            geoms.Add(Geometry.FromText(
                          "POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), (21 21, 29 21, 29 " +
                          "29, 21 29, 21 21), (23 23, 23 27, 27 27, 27 23, 23 23))"));
            geoms.Add(Geometry.FromText("POINT (58.813841159 84.7561198972)"));
            geoms.Add(Geometry.FromText("MULTIPOINT (20 100, 45 32, 100 54)"));
            geoms.Add(Geometry.FromText("MULTIPOLYGON EMPTY"));
            geoms.Add(Geometry.FromText("MULTILINESTRING EMPTY"));
            geoms.Add(Geometry.FromText("MULTIPOINT EMPTY"));
            geoms.Add(Geometry.FromText("LINESTRING EMPTY"));
            return new GeometryProvider(geoms);
        }

        internal static GeometryLayer CreateGeometryFeatureLayer()
        {
            GeometryLayer layer = new GeometryLayer("TestGeometries", CreateGeometryDatasource());
            return layer;
        }

        internal static GeometryLayer CreateFeatureFeatureLayer()
        {
            GeometryLayer layer = new GeometryLayer("TestFeatures", CreateFeatureDatasource());
            return layer;
        }

        internal static FeatureProvider CreateFeatureDatasource()
        {
            DataColumn nameColumn = new DataColumn("FeatureName", typeof (string));
            FeatureProvider provider = new FeatureProvider(nameColumn);

            FeatureDataTable<Guid> features = new FeatureDataTable<Guid>("Oid");
            features.Columns.Add("FeatureName", typeof (string));
            FeatureDataRow<Guid> row;

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty point";
            row.Geometry = Geometry.FromText("POINT EMPTY");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A geometry collection";
            row.Geometry =
                Geometry.FromText("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multipolygon";
            row.Geometry =
                Geometry.FromText("MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), ((5 5, 7 5, 7 7, 5 7, 5 5)))");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A linestring";
            row.Geometry = Geometry.FromText("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multilinestring";
            row.Geometry =
                Geometry.FromText("MULTILINESTRING ((10 10, 40 50), (20 20, 30 20), (20 20, 50 20, 50 60, 20 20))");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A polygon";
            row.Geometry = Geometry.FromText(
                "POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), (21 21, 29 21, 29 " +
                "29, 21 29, 21 21), (23 23, 23 27, 27 27, 27 23, 23 23))");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A point";
            row.Geometry = Geometry.FromText("POINT (58.813841159 84.7561198972)");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multipoint";
            row.Geometry = Geometry.FromText("MULTIPOINT (20 100, 45 32, 100 54)");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multipolygon";
            row.Geometry = Geometry.FromText("MULTIPOLYGON EMPTY");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multilinestring";
            row.Geometry = Geometry.FromText("MULTILINESTRING EMPTY");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multipoint";
            row.Geometry = Geometry.FromText("MULTIPOINT EMPTY");
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty linestring";
            row.Geometry = Geometry.FromText("LINESTRING EMPTY");
            features.AddRow(row);

            provider.Insert(features);

            return provider;
        }
    }
}