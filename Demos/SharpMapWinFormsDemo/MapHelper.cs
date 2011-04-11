namespace MapViewer
{
    using System.Collections.Generic;
    using System.Configuration;
    using GeoAPI.CoordinateSystems.Transformations;
    using GeoAPI.Geometries;
    using SharpMap.Data.Providers;
    using SharpMap.Layers;
    using SharpMap.Utilities;

    internal static class MapHelper
    {
        private static readonly string[] layers = new[] { "poly_landmarks", "tiger_roads", "poi", };
        private static readonly GeometryServices services = new GeometryServices();

        internal static IEnumerable<IFeatureLayer> CreateSqlLayers()
        {
            IGeometryFactory geoFactory = services.DefaultGeometryFactory;
            ICoordinateTransformationFactory ctFactory = services.CoordinateTransformationFactory;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["LocalSqlConnectionString"];
            string connectionString = settings.ConnectionString;

            foreach (string layer in layers)
            {
                MsSqlServer2008Provider<int> provider = new MsSqlServer2008Provider<int>(geoFactory, connectionString,
                    "dbo", layer, "UID", "geom") { CoordinateTransformationFactory = ctFactory };
                yield return new GeometryLayer(layer, provider);
            }            
        }

        internal static IEnumerable<IFeatureLayer> CreatePgisLayers()
        {
            IGeometryFactory geoFactory = services.DefaultGeometryFactory;
            ICoordinateTransformationFactory ctFactory = services.CoordinateTransformationFactory;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["LocalPgisConnectionString"];
            string connectionString = settings.ConnectionString;

            foreach (string layer in layers)
            {
                PostGisProvider<int> provider = new PostGisProvider<int>(geoFactory, connectionString,
                    "public", layer, "gid", "the_geom") { CoordinateTransformationFactory = ctFactory };
                GeometryLayer item = new GeometryLayer(layer, provider);
                yield return item;
            }
        }
    }
}
