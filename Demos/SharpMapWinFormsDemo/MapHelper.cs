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
        internal static IEnumerable<ILayer> CreateLayers()
        {
            GeometryServices services = new GeometryServices();
            IGeometryFactory geoFactory = services.DefaultGeometryFactory;
            ICoordinateTransformationFactory ctFactory = services.CoordinateTransformationFactory;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["LocalConnectionString"];
            string connectionString = settings.ConnectionString;

            string[] layers = new[] { "poly_landmarks", "tiger_roads", "poi", };            
            foreach (string layer in layers)
            {
                MsSqlServer2008Provider<int> provider = new MsSqlServer2008Provider<int>(geoFactory, connectionString,
                    "dbo", layer, "UID", "geom") { CoordinateTransformationFactory = ctFactory };
                GeometryLayer item = new GeometryLayer(layer, provider);
                yield return item;
            }            
        }
    }
}
