using System;
using GeoAPI.CoordinateSystems;

namespace ProjNet.Tests.V2
{
    public abstract class AbstractFixture
    {
        protected ICoordinateSystem CrsFor(int srid, ICoordinateSystemFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            switch (srid)
            {
                case 4326:
                    {
                        const string source =
                            "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]";
                        return factory.CreateFromWkt(source);
                    }

                case 3857:
                case 900913:
                    const string googleWktFromGeoserver = "PROJCS[\"WGS84 / Google Mercator\", GEOGCS[\"WGS 84\", DATUM[\"World Geodetic System 1984\", SPHEROID[\"WGS 84\", 6378137.0, 298.257223563, AUTHORITY[\"EPSG\",\"7030\"]], AUTHORITY[\"EPSG\",\"6326\"]], PRIMEM[\"Greenwich\", 0.0, AUTHORITY[\"EPSG\",\"8901\"]], UNIT[\"degree\", 0.017453292519943295], AXIS[\"Longitude\", EAST], AXIS[\"Latitude\", NORTH], AUTHORITY[\"EPSG\",\"4326\"]], PROJECTION[\"Mercator_1SP\"], PARAMETER[\"semi_minor\", 6378137.0], PARAMETER[\"latitude_of_origin\", 0.0], PARAMETER[\"central_meridian\", 0.0], PARAMETER[\"scale_factor\", 1.0], PARAMETER[\"false_easting\", 0.0], PARAMETER[\"false_northing\", 0.0], UNIT[\"m\", 1.0], AXIS[\"x\", EAST], AXIS[\"y\", NORTH], AUTHORITY[\"EPSG\",\"900913\"]]";
                    const string googleWktFromProjNetV2 = "PROJCS[\"Google Mercator\",GEOGCS[\"WGS 84\",DATUM[\"World Geodetic System 1984\",SPHEROID[\"WGS 84\",6378137.0,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0.0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.017453292519943295],AXIS[\"Geodetic latitude\",NORTH],AXIS[\"Geodetic longitude\",EAST],AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Mercator_1SP\"],PARAMETER[\"semi_minor\",6378137.0],PARAMETER[\"latitude_of_origin\",0.0],PARAMETER[\"central_meridian\",0.0],PARAMETER[\"scale_factor\",1.0],PARAMETER[\"false_easting\",0.0],PARAMETER[\"false_northing\",0.0],UNIT[\"m\",1.0],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH],AUTHORITY[\"EPSG\",\"900913\"]]";
                    return factory.CreateFromWkt(googleWktFromProjNetV2);

                default:
                    string format = String.Format("SRID unmanaged: {0}", srid);
                    throw new ArgumentOutOfRangeException("srid", format);
            }         
        }

        //private static ICoordinateSystem GetMercatorProjection(ICoordinateSystemFactory factory)
        //{
        //    var parameters = new List<ProjectionParameter> {
        //        new ProjectionParameter("semi_major", 6378137),
        //        new ProjectionParameter("semi_minor", 6378137),
        //        new ProjectionParameter("latitude_of_origin", 0.0),
        //        new ProjectionParameter("central_meridian", 0.0),
        //        new ProjectionParameter("scale_factor", 1.0),
        //        new ProjectionParameter("false_easting", 0.0),
        //        new ProjectionParameter("false_northing", 0.0)
        //    };
        //    var projection = factory.CreateProjection("Mercator", "mercator_1sp", parameters);
        //    var gcs = factory.CreateGeographicCoordinateSystem(
        //        "WGS 84",
        //        AngularUnit.Degrees,
        //        HorizontalDatum.WGS84,
        //        PrimeMeridian.Greenwich,
        //        new AxisInfo("north", AxisOrientationEnum.North),
        //        new AxisInfo("east", AxisOrientationEnum.East));
        //    var mercator = factory.CreateProjectedCoordinateSystem(
        //        "Mercator",
        //        gcs,
        //        projection,
        //        LinearUnit.Metre,
        //        new AxisInfo("East", AxisOrientationEnum.East),
        //        new AxisInfo("North", AxisOrientationEnum.North));
        //    return mercator;
        //}
    }
}