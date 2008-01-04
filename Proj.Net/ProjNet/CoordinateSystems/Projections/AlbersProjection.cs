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
    ///		Implements the Albers projection.
    /// </summary>
    /// <remarks>
    /// 	<para>Implements the Albers projection. The Albers projection is most commonly
    /// 	used to project the United States of America. It gives the northern
    /// 	border with Canada a curved appearance.</para>
    /// 	
    ///		<para>The <a href="http://www.geog.mcgill.ca/courses/geo201/mapproj/naaeana.gif">Albers Equal Area</a>
    ///		projection has the property that the area bounded
    ///		by any pair of parallels and meridians is exactly reproduced between the 
    ///		image of those parallels and meridians in the projected domain, that is,
    ///		the projection preserves the correct area of the earth though distorts
    ///		direction, distance and shape somewhat.</para>
    /// </remarks>
    internal class AlbersProjection<TCoordinate> : MapProjection<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>,
            IConvertible
    {
        private readonly Double _falseEasting;
        private readonly Double _falseNorthing;
        private readonly Double _c; //constant c
        private readonly Double ro0;
        private readonly Double n;
        private readonly Double lon_center; //center longitude   

        #region Constructors

        /// <summary>
        /// Creates an instance of an Albers projection object.
        /// </summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        /// <remarks>
        /// <para>The parameters this projection expects are listed below.</para>
        /// <list type="table">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term>latitude_of_false_origin</term><description>The latitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
        /// <item><term>longitude_of_false_origin</term><description>The longitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
        /// <item><term>latitude_of_1st_standard_parallel</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is nearest the pole.  Scale is true along this parallel.</description></item>
        /// <item><term>latitude_of_2nd_standard_parallel</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is furthest from the pole.  Scale is true along this parallel.</description></item>
        /// <item><term>easting_at_false_origin</term><description>The easting value assigned to the false origin.</description></item>
        /// <item><term>northing_at_false_origin</term><description>The northing value assigned to the false origin.</description></item>
        /// </list>
        /// </remarks>
        public AlbersProjection(IEnumerable<ProjectionParameter> parameters, ICoordinateFactory<TCoordinate> coordinateFactory)
            : this(parameters, coordinateFactory, false) { }

        /// <summary>
        /// Creates an instance of an Albers projection object.
        /// </summary>
        /// <remarks>
        /// <para>The parameters this projection expects are listed below.</para>
        /// <list type="table">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term>latitude_of_center</term><description>The latitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
        /// <item><term>longitude_of_center</term><description>The longitude of the point which is not the natural origin and at which grid coordinate values false easting and false northing are defined.</description></item>
        /// <item><term>standard_parallel_1</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is nearest the pole.  Scale is true along this parallel.</description></item>
        /// <item><term>standard_parallel_2</term><description>For a conic projection with two standard parallels, this is the latitude of intersection of the cone with the ellipsoid that is furthest from the pole.  Scale is true along this parallel.</description></item>
        /// <item><term>false_easting</term><description>The easting value assigned to the false origin.</description></item>
        /// <item><term>false_northing</term><description>The northing value assigned to the false origin.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        /// <param name="isInverse">Indicates whether the projection forward (meters to degrees or degrees to meters).</param>
        public AlbersProjection(IEnumerable<ProjectionParameter> parameters, ICoordinateFactory<TCoordinate> coordinateFactory, Boolean isInverse)
            : base(parameters, coordinateFactory, isInverse)
        {
            Name = "Albers_Conic_Equal_Area";

            //Retrieve parameters
            ProjectionParameter longitude_of_center = GetParameter("longitude_of_center");
            ProjectionParameter latitude_of_center = GetParameter("latitude_of_center");
            ProjectionParameter standard_parallel_1 = GetParameter("standard_parallel_1");
            ProjectionParameter standard_parallel_2 = GetParameter("standard_parallel_2");
            ProjectionParameter false_easting = GetParameter("false_easting");
            ProjectionParameter false_northing = GetParameter("false_northing");

            //Check for missing parameters
            if (longitude_of_center == null)
            {
                longitude_of_center = GetParameter("central_meridian"); //Allow for altenative name
                if (longitude_of_center == null)
                {
                    throw new ArgumentException(
                        "Missing projection parameter 'longitude_of_center'");
                }
            }

            if (latitude_of_center == null)
            {
                latitude_of_center = GetParameter("latitude_of_origin"); //Allow for altenative name
                
                if (latitude_of_center == null)
                {
                    throw new ArgumentException(
                        "Missing projection parameter 'latitude_of_center'");
                }
            }

            if (standard_parallel_1 == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'standard_parallel_1'");
            }

            if (standard_parallel_2 == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'standard_parallel_2'");
            }

            if (false_easting == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'false_easting'");
            }

            if (false_northing == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'false_northing'");
            }

            lon_center = DegreesToRadians(longitude_of_center.Value);
            Double lat0 = DegreesToRadians(latitude_of_center.Value);
            Double lat1 = DegreesToRadians(standard_parallel_1.Value);
            Double lat2 = DegreesToRadians(standard_parallel_2.Value);
            _falseEasting = false_easting.Value * _metersPerUnit;
            _falseNorthing = false_northing.Value * _metersPerUnit;

            if (Math.Abs(lat1 + lat2) < Double.Epsilon)
            {
                throw new ArgumentException("Equal latitudes for standard parallels on opposite sides of Equator.");
            }

            Double alpha1 = alpha(lat1);
            Double alpha2 = alpha(lat2);

            Double m1 = Math.Cos(lat1) / Math.Sqrt(1 - E2 * Math.Pow(Math.Sin(lat1), 2));
            Double m2 = Math.Cos(lat2) / Math.Sqrt(1 - E2 * Math.Pow(Math.Sin(lat2), 2));

            n = (Math.Pow(m1, 2) - Math.Pow(m2, 2)) / (alpha2 - alpha1);
            _c = Math.Pow(m1, 2) + (n * alpha1);

            ro0 = computeRo(alpha(lat0));
            /*
            Double sin_p0 = Math.Sin(lat0);
            Double cos_p0 = Math.Cos(lat0);
            Double q0 = qsfnz(e, sin_p0, cos_p0);

            Double sin_p1 = Math.Sin(lat1);
            Double cos_p1 = Math.Cos(lat1);
            Double m1 = msfnz(e,sin_p1,cos_p1);
            Double q1 = qsfnz(e,sin_p1,cos_p1);


            Double sin_p2 = Math.Sin(lat2);
            Double cos_p2 = Math.Cos(lat2);
            Double m2 = msfnz(e,sin_p2,cos_p2);
            Double q2 = qsfnz(e,sin_p2,cos_p2);

            if (Math.Abs(lat1 - lat2) > EPSLN)
                ns0 = (m1 * m1 - m2 * m2)/ (q2 - q1);
            else
                ns0 = sin_p1;
            C = m1 * m1 + ns0 * q1;
            rh = this._semiMajor * Math.Sqrt(C - ns0 * q0)/ns0;
            */
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Converts coordinates in decimal degrees to projected meters.
        /// </summary>
        /// <param name="lonlat">The point in decimal degrees.</param>
        /// <returns>Point in projected meters</returns>
        public override TCoordinate DegreesToMeters(TCoordinate lonlat)
        {
            Double dLongitude = DegreesToRadians((Double)lonlat[0]);
            Double dLatitude = DegreesToRadians((Double)lonlat[1]);

            Double a = alpha(dLatitude);
            Double ro = computeRo(a);
            Double theta = n * (dLongitude - lon_center);
            dLongitude = _falseEasting + ro * Math.Sin(theta);
            dLatitude = _falseNorthing + ro0 - (ro * Math.Cos(theta));

            if (lonlat.ComponentCount == 2)
            {
                return CreateCoordinate(dLongitude / _metersPerUnit, dLatitude / _metersPerUnit);
            }
            else
            {
                return CreateCoordinate(dLongitude / _metersPerUnit, dLatitude / _metersPerUnit, (Double)lonlat[2]);
            }
        }

        /// <summary>
        /// Converts coordinates in projected meters to decimal degrees.
        /// </summary>
        /// <param name="p">Point in meters</param>
        /// <returns>Transformed point in decimal degrees</returns>
        public override TCoordinate MetersToDegrees(TCoordinate p)
        {
            Double theta = Math.Atan((p[0] * _metersPerUnit - _falseEasting) 
                / (ro0 - (p[1].Multiply(_metersPerUnit) - _falseNorthing)));
            
            Double ro = Math.Sqrt(Math.Pow(p[0] * _metersPerUnit - _falseEasting, 2)
                + Math.Pow(ro0 - (p[1].Multiply(_metersPerUnit) - _falseNorthing), 2));

            Double q = (_c - Math.Pow(ro, 2) * Math.Pow(n, 2) / Math.Pow(SemiMajor, 2)) / n;
            //Double b = Math.Sin(q / (1 - ((1 - e_sq) / (2 * e)) * Math.Log((1 - e) / (1 + e))));

            Double lat = Math.Asin(q * 0.5);
            Double preLat = Double.MaxValue;
            Int32 iterationCounter = 0;

            while (Math.Abs(lat - preLat) > 0.000001)
            {
                preLat = lat;
                Double sin = Math.Sin(lat);
                Double e2sin2 = E2 * Math.Pow(sin, 2);

                lat += (Math.Pow(1 - e2sin2, 2) / (2 * Math.Cos(lat))) *
                       ((q / (1 - E2)) - sin / (1 - e2sin2) + 1 / (2 * E) * 
                            Math.Log((1 - E * sin) / (1 + E * sin)));

                iterationCounter++;

                if (iterationCounter > 25)
                {
                    throw new ComputationConvergenceException(
                        "Transformation failed to converge in Albers "+
                        "backwards transformation.");
                }
            }

            Double lon = lon_center + (theta / n);

            if (p.ComponentCount == 2)
            {
                return CreateCoordinate(RadiansToDegrees(lon), RadiansToDegrees(lat));
            }
            else
            {
                return CreateCoordinate(RadiansToDegrees(lon), RadiansToDegrees(lat), (Double)p[2]);
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
                _inverse = new AlbersProjection<TCoordinate>(
                    Enumerable.Downcast<ProjectionParameter, Parameter>(Parameters), 
                    CoordinateFactory, !_isInverse);
            }

            return _inverse;
        }

        #endregion

        #region Math helper functions

        //private Double ToAuthalic(Double lat)
        //{
        //    return Math.Atan(Q(lat) / Q(Math.PI * 0.5));
        //}

        //private Double Q(Double angle)
        //{
        //    Double sin = Math.Sin(angle);
        //    Double esin = e * sin;
        //    return Math.Abs(sin / (1 - Math.Pow(esin, 2)) - 0.5 * e) * Math.Log((1 - esin) / (1 + esin)));
        //}

        private Double alpha(Double lat)
        {
            Double sin = Math.Sin(lat);
            Double sinsq = Math.Pow(sin, 2);
            return (1 - E2) * (((sin / (1 - E2 * sinsq)) - 1 
                / (2 * E) * Math.Log((1 - E * sin) / (1 + E * sin))));
        }

        private Double computeRo(Double a)
        {
            return SemiMajor * Math.Sqrt((_c - n * a)) / n;
        }

        #endregion
    }
}