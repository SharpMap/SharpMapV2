// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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

// SOURCECODE IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Projections
{
    /// <summary>
    /// Summary description for MathTransform.
    /// </summary>
    /// <remarks>
    /// <para>Universal (UTM) and Modified (MTM) Transverses Mercator projections. This
    /// is a cylindrical projection, in which the cylinder has been rotated 90°.
    /// Instead of being tangent to the equator (or to an other standard latitude),
    /// it is tangent to a central meridian. Deformation are more important as we
    /// are going futher from the central meridian. The Transverse Mercator
    /// projection is appropriate for region wich have a greater extent north-south
    /// than east-west.</para>
    /// 
    /// <para>Reference: John P. Snyder (Map Projections - A Working Manual,
    ///            U.S. Geological Survey Professional Paper 1395, 1987)</para>
    /// </remarks>
    internal class TransverseMercator<TCoordinate> : MapProjection<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>,
            IConvertible
    {
        private double scale_factor; /* scale factor				*/
        private double central_meridian; /* Center longitude (projection center) */
        private double lat_origin; /* center latitude			*/
        private double e0, e1, e2, e3; /* eccentricity constants		*/
        private double e, es, esp; /* eccentricity constants		*/
        private double ml0; /* small value m			*/
        private double false_northing; /* y offset in meters			*/
        private double false_easting; /* x offset in meters			*/
        //static double ind;		/* spherical flag			*/

        /// <summary>
        /// Creates an instance of an TransverseMercatorProjection projection object.
        /// </summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        public TransverseMercator(List<ProjectionParameter> parameters)
            : this(parameters, false) { }

        /// <summary>
        /// Creates an instance of an TransverseMercatorProjection projection object.
        /// </summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        /// <param name="inverse">Flag indicating wether is a forward/projection (false) or an inverse projection (true).</param>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term>semi_major</term><description>Semi major radius</description></item>
        /// <item><term>semi_minor</term><description>Semi minor radius</description></item>
        /// <item><term>scale_factor</term><description></description></item>
        /// <item><term>central meridian</term><description></description></item>
        /// <item><term>latitude_origin</term><description></description></item>
        /// <item><term>false_easting</term><description></description></item>
        /// <item><term>false_northing</term><description></description></item>
        /// </list>
        /// </remarks>
        public TransverseMercator(List<ProjectionParameter> parameters, bool inverse)
            : base(parameters, inverse)
        {
            Name = "Transverse_Mercator";
            Authority = "EPSG";
            AuthorityCode = 9807;
            ProjectionParameter par_scale_factor = GetParameter("scale_factor");
            ProjectionParameter par_central_meridian = GetParameter("central_meridian");
            ProjectionParameter par_latitude_of_origin = GetParameter("latitude_of_origin");
            ProjectionParameter par_false_easting = GetParameter("false_easting");
            ProjectionParameter par_false_northing = GetParameter("false_northing");
            //Check for missing parameters
            if (par_scale_factor == null)
            {
                throw new ArgumentException("Missing projection parameter 'scale_factor'");
            }
            if (par_central_meridian == null)
            {
                throw new ArgumentException("Missing projection parameter 'central_meridian'");
            }
            if (par_latitude_of_origin == null)
            {
                throw new ArgumentException("Missing projection parameter 'latitude_of_origin'");
            }
            if (par_false_easting == null)
            {
                throw new ArgumentException("Missing projection parameter 'false_easting'");
            }
            if (par_false_northing == null)
            {
                throw new ArgumentException("Missing projection parameter 'false_northing'");
            }

            scale_factor = par_scale_factor.Value;
            central_meridian = DegreesToRadians(par_central_meridian.Value);
            lat_origin = DegreesToRadians(par_latitude_of_origin.Value);
            false_easting = par_false_easting.Value * _metersPerUnit;
            false_northing = par_false_northing.Value * _metersPerUnit;

            es = 1.0 - Math.Pow(_semiMinor / _semiMajor, 2);
            e = Math.Sqrt(es);
            e0 = ComputeE0(es);
            e1 = ComputeE1(es);
            e2 = ComputeE2(es);
            e3 = ComputeE3(es);
            ml0 = _semiMajor * MeridianLength(e0, e1, e2, e3, lat_origin);
            esp = es / (1.0 - es);
        }

        /// <summary>
        /// Converts coordinates in decimal degrees to projected meters.
        /// </summary>
        /// <param name="lonlat">The point in decimal degrees.</param>
        /// <returns>Point in projected meters</returns>
        public override TCoordinate DegreesToMeters(TCoordinate lonlat)
        {
            double lon = DegreesToRadians(lonlat[0]);
            double lat = DegreesToRadians(lonlat[1]);

            double delta_lon; /* Delta longitude (Given longitude - center 	*/
            double sin_phi, cos_phi; /* sin and cos value				*/
            double al, als; /* temporary values				*/
            double c, t, tq; /* temporary values				*/
            double con, n, ml; /* cone constant, small m			*/

            delta_lon = AdjustLongitude(lon - central_meridian);
            SinCos(lat, out sin_phi, out cos_phi);

            al = cos_phi * delta_lon;
            als = Math.Pow(al, 2);
            c = esp * Math.Pow(cos_phi, 2);
            tq = Math.Tan(lat);
            t = Math.Pow(tq, 2);
            con = 1.0 - es * Math.Pow(sin_phi, 2);
            n = _semiMajor / Math.Sqrt(con);
            ml = _semiMajor * MeridianLength(e0, e1, e2, e3, lat);

            double x =
                scale_factor * n * al * (1.0 + als / 6.0 * (1.0 - t + c + als / 20.0 *
                                                                (5.0 - 18.0 * t + Math.Pow(t, 2) + 72.0 * c - 58.0 * esp))) +
                false_easting;
            double y = scale_factor * (ml - ml0 + n * tq * (als * (0.5 + als / 24.0 *
                                                                 (5.0 - t + 9.0 * c + 4.0 * Math.Pow(c, 2) +
                                                                  als / 30.0 * (61.0 - 58.0 * t
                                                                            + Math.Pow(t, 2) + 600.0 * c - 330.0 * esp))))) +
                       false_northing;
            if (lonlat.ComponentCount < 3)
            {
                return new double[] { x / _metersPerUnit, y / _metersPerUnit };
            }
            else
            {
                return new double[] { x / _metersPerUnit, y / _metersPerUnit, lonlat[2] };
            }
        }

        /// <summary>
        /// Converts coordinates in projected meters to decimal degrees.
        /// </summary>
        /// <param name="p">Point in meters</param>
        /// <returns>Transformed point in decimal degrees</returns>
        public override TCoordinate MetersToDegrees(TCoordinate p)
        {
            double con, phi; /* temporary angles				*/
            double delta_phi; /* difference between longitudes		*/
            long i; /* counter variable				*/
            double sin_phi, cos_phi, tan_phi; /* sin cos and tangent values	*/
            double c, cs, t, ts, n, r, d, ds; /* temporary variables		*/
            long max_iter = 6; /* maximun number of iterations	*/


            double x = p[0] * _metersPerUnit - false_easting;
            double y = p[1] * _metersPerUnit - false_northing;

            con = (ml0 + y / scale_factor) / _semiMajor;
            phi = con;
            for (i = 0; ; i++)
            {
                delta_phi = ((con + e1 * Math.Sin(2.0 * phi) - e2 * Math.Sin(4.0 * phi) + e3 * Math.Sin(6.0 * phi))
                             / e0) - phi;
                phi += delta_phi;
                if (Math.Abs(delta_phi) <= Epsilon)
                {
                    break;
                }
                if (i >= max_iter)
                {
                    throw new ApplicationException("Latitude failed to converge");
                }
            }
            if (Math.Abs(phi) < HalfPI)
            {
                SinCos(phi, out sin_phi, out cos_phi);
                tan_phi = Math.Tan(phi);
                c = esp * Math.Pow(cos_phi, 2);
                cs = Math.Pow(c, 2);
                t = Math.Pow(tan_phi, 2);
                ts = Math.Pow(t, 2);
                con = 1.0 - es * Math.Pow(sin_phi, 2);
                n = _semiMajor / Math.Sqrt(con);
                r = n * (1.0 - es) / con;
                d = x / (n * scale_factor);
                ds = Math.Pow(d, 2);

                double lat = phi - (n * tan_phi * ds / r) * (0.5 - ds / 24.0 * (5.0 + 3.0 * t +
                                                                    10.0 * c - 4.0 * cs - 9.0 * esp - ds / 30.0 * (61.0 + 90.0 * t +
                                                                                                         298.0 * c +
                                                                                                         45.0 * ts -
                                                                                                         252.0 * esp -
                                                                                                         3.0 * cs)));
                double lon = AdjustLongitude(central_meridian + (d * (1.0 - ds / 6.0 * (1.0 + 2.0 * t +
                                                                                  c -
                                                                                  ds / 20.0 *
                                                                                  (5.0 - 2.0 * c + 28.0 * t - 3.0 * cs +
                                                                                   8.0 * esp +
                                                                                   24.0 * ts))) / cos_phi));

                if (p.ComponentCount < 3)
                {
                    return new double[] { RadiansToDegrees(lon), RadiansToDegrees(lat) };
                }
                else
                {
                    return new double[] { RadiansToDegrees(lon), RadiansToDegrees(lat), p[2] };
                }
            }
            else
            {
                if (p.ComponentCount < 3)
                {
                    return new double[] { RadiansToDegrees(HalfPI * Sign(y)), RadiansToDegrees(central_meridian) };
                }
                else
                {
                    return new double[] { RadiansToDegrees(HalfPI * Sign(y)), RadiansToDegrees(central_meridian), p[2] };
                }
            }
        }


        /// <summary>
        /// Returns the inverse of this projection.
        /// </summary>
        /// <returns>IMathTransform that is the reverse of the current projection.</returns>
        public override IMathTransform<TCoordinate> Inverse()
        {
            if (_inverse == null)
            {
                _inverse = new TransverseMercator<TCoordinate>(_parameters, !_isInverse);
            }

            return _inverse;
        }
    }
}