using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;

namespace SharpMap.Tests
{
    internal static class DataSourceHelper
    {
        internal static IFeatureProvider CreateGeometryDatasource(IGeometryFactory geoFactory,
                                                                Boolean includeGeometryCollections)
        {
            Collection<IGeometry> geoms = new Collection<IGeometry>();
            geoms.Add(geoFactory.WktReader.Read(EmptyPoint));

            if (includeGeometryCollections)
            {
                geoms.Add(geoFactory.WktReader.Read(GeometryCollection));
            }

            geoms.Add(geoFactory.WktReader.Read(MultiPolygon));
            geoms.Add(geoFactory.WktReader.Read(LineString));
            geoms.Add(geoFactory.WktReader.Read(MultiLineString));
            geoms.Add(geoFactory.WktReader.Read(Polygon));
            geoms.Add(geoFactory.WktReader.Read(Point));
            geoms.Add(geoFactory.WktReader.Read(MultiPoint));
            geoms.Add(geoFactory.WktReader.Read(EmptyMultiPolygon));
            geoms.Add(geoFactory.WktReader.Read(EmptyMultiLineString));
            geoms.Add(geoFactory.WktReader.Read(EmptyMultiPoint));
            geoms.Add(geoFactory.WktReader.Read(EmptyLineString));
            return new GeometryProvider(geoms);
        }

        public static readonly String EmptyPoint = "POINT EMPTY";
        public static readonly String EmptyLineString = "LINESTRING EMPTY";
        public static readonly String EmptyMultiPoint = "MULTIPOINT EMPTY";
        public static readonly String EmptyMultiPolygon = "MULTIPOLYGON EMPTY";
        public static readonly String EmptyMultiLineString = "MULTILINESTRING EMPTY";

        public static readonly String GeometryCollection = "GEOMETRYCOLLECTION (" +
                                                            "POINT (10 10), " +
                                                            "POINT (30 30), " +
                                                            "LINESTRING (15 15, 20 20))";
        public static readonly String Point = "POINT (58.813841159 84.7561198972)";
        public static readonly String Polygon = "POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), " +
                                                "(21 21, 29 21, 29 29, 21 29, 21 21), " +
                                                "(23 23, 23 27, 27 27, 27 23, 23 23))";
        public static readonly String LineString = "LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)";
        public static readonly String MultiPoint = "MULTIPOINT (20 100, 45 32, 120 54)";
        public static readonly String MultiPolygon = "MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), " +
                                                                    "((5 5, 7 5, 7 7, 5 7, 5 5)))";
        public static readonly String MultiLineString = "MULTILINESTRING ((10 10, 40 50), " +
                                                                 "(20 20, 30 20), " +
                                                                 "(20 20, 50 20, 50 60, 20 20))";



        internal static GeometryLayer CreateGeometryFeatureLayer(IGeometryFactory geoFactory,
                                                                Boolean includeGeometryCollections)
        {
            GeometryLayer layer = new GeometryLayer("TestGeometries",
                                                    CreateGeometryDatasource(geoFactory, includeGeometryCollections));
            return layer;
        }

        internal static GeometryLayer CreateFeatureFeatureLayer(IGeometryFactory geoFactory,
                                                                Boolean includeGeometryCollections)
        {
            GeometryLayer layer = new GeometryLayer("TestFeatures",
                                                    CreateFeatureDatasource(geoFactory, includeGeometryCollections));
            return layer;
        }

        internal static FeatureProvider CreateFeatureDatasource(IGeometryFactory geoFactory,
                                                                Boolean includeGeometryCollections)
        {
            DataColumn nameColumn = new DataColumn("FeatureName", typeof(String));
            FeatureProvider provider = new FeatureProvider(geoFactory, nameColumn);

            FeatureDataTable<Guid> features = new FeatureDataTable<Guid>("Oid", geoFactory);
            features.Columns.Add("FeatureName", typeof(String));
            FeatureDataRow<Guid> row;

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty point";
            row.Geometry = geoFactory.WktReader.Read(EmptyPoint);
            features.AddRow(row);

            if (includeGeometryCollections)
            {
                row = features.NewRow(Guid.NewGuid());
                row["FeatureName"] = "A geometry collection";
                row.Geometry = geoFactory.WktReader.Read(GeometryCollection);
                features.AddRow(row);   
            }

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multipolygon";
            row.Geometry = geoFactory.WktReader.Read(MultiPolygon);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A linestring";
            row.Geometry = geoFactory.WktReader.Read(LineString);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multilinestring";
            row.Geometry = geoFactory.WktReader.Read(MultiLineString);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A polygon";
            row.Geometry = geoFactory.WktReader.Read(Polygon);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A point";
            row.Geometry = geoFactory.WktReader.Read(Point);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "A multipoint";
            row.Geometry = geoFactory.WktReader.Read(MultiPoint);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multipolygon";
            row.Geometry = geoFactory.WktReader.Read(EmptyMultiPolygon);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multilinestring";
            row.Geometry = geoFactory.WktReader.Read(EmptyMultiLineString);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty multipoint";
            row.Geometry = geoFactory.WktReader.Read(EmptyMultiPoint);
            features.AddRow(row);

            row = features.NewRow(Guid.NewGuid());
            row["FeatureName"] = "An empty linestring";
            row.Geometry = geoFactory.WktReader.Read(EmptyLineString);
            features.AddRow(row);

            provider.Insert((IEnumerable<FeatureDataRow<Guid>>) features);

            return provider;
        }

        internal static FeatureProvider CreateFeatureDatasource(IGeometryFactory geoFactory)
        {
            return CreateFeatureDatasource(geoFactory, true);
        }

        internal static IFeatureProvider CreateGeometryDatasource(IGeometryFactory geoFactory)
        {
            return CreateFeatureDatasource(geoFactory, true);
        }

        public static GeometryLayer CreateFeatureFeatureLayer(IGeometryFactory geoFactory)
        {
            return CreateFeatureFeatureLayer(geoFactory, true);
        }
    }
}