// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of Proj.Net.
// Proj.Net is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Proj.Net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Proj.Net; if not, write to the Free Software
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
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Utilities;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Projections
{
    /// <summary>
    /// Summary description for MathTransform.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Universal (UTM) and Modified (MTM) Transverses Mercator projections. This
    /// is a cylindrical projection, in which the cylinder has been rotated 90°.
    /// Instead of being tangent to the equator (or to an other standard latitude),
    /// it is tangent to a central meridian. Deformation are more important as we
    /// are going futher from the central meridian. The Transverse Mercator
    /// projection is appropriate for region wich have a greater extent north-south
    /// than east-west.
    /// </para>
    /// 
    /// <para>Reference: John P. Snyder (Map Projections - A Working Manual,
    ///            U.S. Geological Survey Professional Paper 1395, 1987)</para>
    /// </remarks>
    internal class TransverseMercator<TCoordinate> : MapProjection<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<Double, TCoordinate>,
            IConvertible
    {
        private readonly Double _scale_factor;     /* scale factor				*/
        private readonly Double _central_meridian; /* Center longitude (projection center) */
        private readonly Double _lat_origin;       /* center latitude			*/
        private readonly Double _e0, _e1, _e2, _e3;   /* eccentricity constants	*/
        private readonly Double _esp;              /* eccentricity constants	*/
        private readonly Double _ml0;              /* small value m			*/
        private readonly Double _false_northing;   /* y offset in meters		*/
        private readonly Double _false_easting;    /* x offset in meters		*/
        //static Double ind;		               /* spherical flag			*/

        /// <summary>
        /// Creates an instance of an TransverseMercatorProjection projection 
        /// object.
        /// </summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        public TransverseMercator(IEnumerable<ProjectionParameter> parameters, ICoordinateFactory<TCoordinate> coordinateFactory)
            : this(parameters, coordinateFactory, false) { }

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
        public TransverseMercator(IEnumerable<ProjectionParameter> parameters, ICoordinateFactory<TCoordinate> coordinateFactory, Boolean inverse)
            : base(parameters, coordinateFactory, inverse)
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
                throw new ArgumentException(
                    "Missing projection parameter 'scale_factor'");
            }

            if (par_central_meridian == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'central_meridian'");
            }

            if (par_latitude_of_origin == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'latitude_of_origin'");
            }

            if (par_false_easting == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'false_easting'");
            }

            if (par_false_northing == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'false_northing'");
            }

            _scale_factor = par_scale_factor.Value;
            _central_meridian = DegreesToRadians(par_central_meridian.Value);
            _lat_origin = DegreesToRadians(par_latitude_of_origin.Value);
            _false_easting = par_false_easting.Value * _metersPerUnit;
            _false_northing = par_false_northing.Value * _metersPerUnit;

            Double semiMajor = SemiMajor;
            _e0 = ComputeE0(E2);
            _e1 = ComputeE1(E2);
            _e2 = ComputeE2(E2);
            _e3 = ComputeE3(E2);
            _ml0 = semiMajor * MeridianLength(_e0, _e1, _e2, _e3, _lat_origin);
            _esp = E2 / (1.0 - E2);
        }

        /// <summary>
        /// Converts coordinates in decimal degrees to projected meters.
        /// </summary>
        /// <param name="lonlat">The point in decimal degrees.</param>
        /// <returns>Point in projected meters</returns>
        public override TCoordinate DegreesToMeters(TCoordinate lonlat)
        {
            Double lon = DegreesToRadians((Double)lonlat[0]);
            Double lat = DegreesToRadians((Double)lonlat[1]);
            Double semiMajor = SemiMajor;

            Double delta_lon; /* Delta longitude (Given longitude - center 	*/
            Double sin_phi, cos_phi; /* sin and cos value				*/
            Double al, als; /* temporary values				*/
            Double c, t, tq; /* temporary values				*/
            Double con, n, ml; /* cone constant, small m			*/

            delta_lon = AdjustLongitude(lon - _central_meridian);
            SinCos(lat, out sin_phi, out cos_phi);

            al = cos_phi * delta_lon;
            als = Math.Pow(al, 2);
            c = _esp * Math.Pow(cos_phi, 2);
            tq = Math.Tan(lat);
            t = Math.Pow(tq, 2);
            con = 1.0 - E2 * Math.Pow(sin_phi, 2);
            n = semiMajor / Math.Sqrt(con);
            ml = semiMajor * MeridianLength(_e0, _e1, _e2, _e3, lat);

            Double x = _scale_factor * n * al * (1.0 + als / 6.0 * (1.0 - t + c + als / 20.0 *
                (5.0 - 18.0 * t + Math.Pow(t, 2) + 72.0 * c - 58.0 * _esp)))
                + _false_easting;

            Double y = _scale_factor * (ml - _ml0 + n * tq * (als * (0.5 + als / 24.0 *
                (5.0 - t + 9.0 * c + 4.0 * Math.Pow(c, 2) + als / 30.0 *
                (61.0 - 58.0 * t + Math.Pow(t, 2) + 600.0 * c - 330.0 * _esp)))))
                + _false_northing;

            if (lonlat.ComponentCount < 3)
            {
                return CreateCoordinate(x / _metersPerUnit, y / _metersPerUnit);
            }
            else
            {
                return CreateCoordinate(x / _metersPerUnit, y / _metersPerUnit, (Double)lonlat[2]);
            }
        }

        /// <summary>
        /// Converts coordinates in projected meters to decimal degrees.
        /// </summary>
        /// <param name="p">Point in meters</param>
        /// <returns>Transformed point in decimal degrees</returns>
        public override TCoordinate MetersToDegrees(TCoordinate p)
        {
            Double con, phi;    /* temporary angles	*/
            Int64 i;            /* counter variable	*/
            Double semiMajor = SemiMajor;

            Double x = ((Double)p[0]) * _metersPerUnit - _false_easting;
            Double y = ((Double)p[1]) * _metersPerUnit - _false_northing;

            con = (_ml0 + y / _scale_factor) / semiMajor;
            phi = con;

            for (i = 0; ; i++)
            {
                Double delta_phi; /* difference between longitudes		*/

                delta_phi = ((con + _e1 * Math.Sin(2.0 * phi)
                    - _e2 * Math.Sin(4.0 * phi) + _e3 * Math.Sin(6.0 * phi))
                             / _e0) - phi;

                phi += delta_phi;

                if (Math.Abs(delta_phi) <= Epsilon)
                {
                    break;
                }

                if (i >= MaxIterationCount)
                {
                    throw new ComputationConvergenceException(String.Format(
                        "Latitude failed to converge in transverse mercator projection after {0} iterations.",
                        MaxIterationCount));
                }
            }

            if (Math.Abs(phi) < HalfPI)
            {
                Double c, cs, t, ts, n, r, d, ds; /* temporary variable */

                Double sin_phi, cos_phi, tan_phi; /* sin cos and tangent values	*/
                SinCos(phi, out sin_phi, out cos_phi);
                tan_phi = Math.Tan(phi);
                c = _esp * Math.Pow(cos_phi, 2);
                cs = Math.Pow(c, 2);
                t = Math.Pow(tan_phi, 2);
                ts = Math.Pow(t, 2);
                con = 1.0 - E2 * Math.Pow(sin_phi, 2);
                n = semiMajor / Math.Sqrt(con);
                r = n * (1.0 - E2) / con;
                d = x / (n * _scale_factor);
                ds = Math.Pow(d, 2);

                Double lat = phi - (n * tan_phi * ds / r) 
                    * (0.5 - ds / 24.0 * (5.0 + 3.0 * t + 10.0 * c - 4.0 * cs - 9.0 * _esp - ds / 30.0 
                    * (61.0 + 90.0 * t + 298.0 * c + 45.0 * ts - 252.0 * _esp - 3.0 * cs)));

                Double lon = AdjustLongitude(_central_meridian 
                    + (d * (1.0 - ds / 6.0 * 
                        (1.0 + 2.0 * t + c -ds / 20.0 * 
                            (5.0 - 2.0 * c + 28.0 * t - 3.0 * cs + 8.0 * _esp + 24.0 * ts))) 
                        / cos_phi));

                if (p.ComponentCount < 3)
                {
                    return CreateCoordinate(RadiansToDegrees(lon), RadiansToDegrees(lat));
                }
                else
                {
                    return CreateCoordinate(RadiansToDegrees(lon), RadiansToDegrees(lat), (Double)p[2]);
                }
            }
            else
            {
                if (p.ComponentCount < 3)
                {
                    return CreateCoordinate(RadiansToDegrees(HalfPI * Sign(y)), RadiansToDegrees(_central_meridian));
                }
                else
                {
                    return CreateCoordinate(RadiansToDegrees(HalfPI * Sign(y)), RadiansToDegrees(_central_meridian), (Double)p[2]);
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
                _inverse = new TransverseMercator<TCoordinate>(
                    Enumerable.Downcast<ProjectionParameter, Parameter>(Parameters), 
                    CoordinateFactory, !_isInverse);
            }

            return _inverse;
        }
    }
}