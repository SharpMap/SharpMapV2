using System;
using System.Collections.Generic;
using System.IO;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.IO.WellKnownText;
using GisSharpBlog.NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
#if Buffered
using Coordinate2D = NetTopologySuite.Coordinates.BufferedCoordinate;
using Coordinate2DFactory = NetTopologySuite.Coordinates.BufferedCoordinateFactory;
using Coordinate2DSequenceFactory = NetTopologySuite.Coordinates.BufferedCoordinateSequenceFactory;
#else
using Coordinate2D = NetTopologySuite.Coordinates.Simple.Coordinate;
using Coordinate2DFactory = NetTopologySuite.Coordinates.Simple.CoordinateFactory;
using Coordinate2DSequenceFactory = NetTopologySuite.Coordinates.Simple.CoordinateSequenceFactory;
#endif
namespace ProjNet.Tests
{
    internal class SridReader
    {
        private const String filename = @"..\..\SRID.csv";

        public struct WktString
        {
            /// <summary>
            /// Well-known ID
            /// </summary>
            public Int32 Wkid;

            /// <summary>
            /// Well-known Text
            /// </summary>
            public String Wkt;
        }

        /// <summary>
        /// Enumerates all SRID's in the SRID.csv file.
        /// </summary>
        /// <returns>Enumerator</returns>
        public static IEnumerable<WktString> GetSrids()
        {
            using (StreamReader sr = File.OpenText(filename))
            {
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    Int32 split = line.IndexOf(';');

                    if (split > -1)
                    {
                        WktString wkt = new WktString();
                        wkt.Wkid = Int32.Parse(line.Substring(0, split));
                        wkt.Wkt = line.Substring(split + 1);
                        yield return wkt;
                    }
                }
                sr.Close();
            }
        }

        /// <summary>
        /// Gets a coordinate system from the SRID.csv file
        /// </summary>
        /// <param name="id">EPSG ID</param>
        /// <returns>Coordinate system, or null if SRID was not found.</returns>
        public static ICoordinateSystem<Coordinate2D> GetCSbyID(Int32 id)
        {
            ICoordinateFactory<Coordinate2D> cf = new Coordinate2DFactory();
            IGeometryFactory<Coordinate2D> gf =
                new GeometryFactory<Coordinate2D>(new Coordinate2DSequenceFactory());
            CoordinateSystemFactory<Coordinate2D> fac = new CoordinateSystemFactory<Coordinate2D>(
                cf, gf);

            foreach (WktString wkt in GetSrids())
            {
                if (wkt.Wkid == id)
                {
                    return
                        WktReader<Coordinate2D>.ToCoordinateSystemInfo(wkt.Wkt, fac) as
                        ICoordinateSystem<Coordinate2D>;
                }
            }
            return null;
        }
    }
}