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
using GeoAPI.DataStructures;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Projections
{
    /// <summary>
    /// Implements the Mercator projection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This map projection introduced in 1569 by Gerardus Mercator. 
    /// It is often described as a cylindrical projection, but it must be derived
    /// mathematically. The meridians are equally spaced, parallel vertical lines, 
    /// and the parallels of latitude are parallel, horizontal straight lines, 
    /// spaced farther and farther apart as their distance from the Equator 
    /// increases. This projection is widely used for navigation charts, because 
    /// any straight line on a Mercator-projection map is a line of constant true 
    /// bearing that enables a navigator to plot a straight-line course. 
    /// It is less practical for world maps because the scale is distorted; 
    /// areas farther away from the equator appear disproportionately large. 
    /// On a Mercator projection, for example, the landmass of Greenland appears 
    /// to be greater than that of the continent of South America; in actual area, 
    /// Greenland is smaller than the Arabian Peninsula.
    /// </para>
    /// </remarks>
    internal class Mercator<TCoordinate> : MapProjection<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>, IComparable<TCoordinate>,
            IComputable<Double, TCoordinate>,
            IConvertible
    {
        private readonly Double _falseEasting;
        private readonly Double _falseNorthing;
        private readonly Double _lon_center; //Center longitude (projection center)
        private readonly Double _lat_origin; //center latitude
        private readonly Double _k0; //small value m

        /// <summary>
        /// Initializes the MercatorProjection object with the specified parameters to project points. 
        /// </summary>
        /// <param name="parameters">ParameterList with the required parameters.</param>
        /// <remarks>
        /// </remarks>
        public Mercator(IEnumerable<ProjectionParameter> parameters, ICoordinateFactory<TCoordinate> coordinateFactory)
            : this(parameters, coordinateFactory, false) {}


        /// <summary>
        ///   Initializes the MercatorProjection object with the specified parameters.
        ///</summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        /// <param name="isInverse">Indicates whether the projection forward (meters to degrees or degrees to meters).</param>
        /// <remarks>
        ///   <para>The parameters this projection expects are listed below.</para>
        ///   <list type="table">
        ///     <listheader>
        ///      <term>Items</term>
        ///      <description>Descriptions</description>
        ///    </listheader>
        ///     <item>
        ///      <term>central_meridian</term>
        ///      <description>
        ///         The longitude of the point from which the values of both the 
        ///         geographical coordinates on the ellipsoid and the grid coordinates 
        ///         on the projection are deemed to increment or decrement for computational purposes. 
        ///         Alternatively it may be considered as the longitude of the point which in the 
        ///         absence of application of false coordinates has grid coordinates of (0, 0).
        ///       </description>
        ///    </item>
        ///     <item>
        ///      <term>latitude_of_origin</term>
        ///      <description>
        ///         The latitude of the point from which the values of both the 
        ///         geographical coordinates on the ellipsoid and the grid coordinates 
        ///         on the projection are deemed to increment or decrement for computational purposes. 
        ///         Alternatively it may be considered as the latitude of the point which in the 
        ///         absence of application of false coordinates has grid coordinates of (0, 0). 
        ///      </description>
        ///    </item>
        ///     <item>
        ///      <term>scale_factor</term>
        ///      <description>
        ///         The factor by which the map grid is reduced or enlarged during the projection process, 
        ///         defined by its value at the natural origin.
        ///      </description>
        ///    </item>
        ///     <item>
        ///      <term>false_easting</term>
        ///      <description>
        ///         Since the natural origin may be at or near the centre of the projection and under 
        ///         normal coordinate circumstances would thus give rise to negative coordinates over 
        ///         parts of the mapped area, this origin is usually given false coordinates which are 
        ///         large enough to avoid this inconvenience. The False Easting, FE, is the easting 
        ///         value assigned to the abscissa (east).
        ///      </description>
        ///    </item>
        ///     <item>
        ///      <term>false_northing</term>
        ///      <description>
        ///         Since the natural origin may be at or near the centre of the projection and under 
        ///         normal coordinate circumstances would thus give rise to negative coordinates over 
        ///         parts of the mapped area, this origin is usually given false coordinates which are 
        ///         large enough to avoid this inconvenience. The False Northing, FN, is the northing 
        ///         value assigned to the ordinate.
        ///      </description>
        ///    </item>
        ///  </list>
        ///</remarks>
        public Mercator(IEnumerable<ProjectionParameter> parameters, ICoordinateFactory<TCoordinate> coordinateFactory,
                        Boolean isInverse)
            : base(parameters, coordinateFactory, isInverse)
        {
            Authority = "EPSG";

            ProjectionParameter central_meridian = GetParameter("central_meridian");
            ProjectionParameter latitude_of_origin = GetParameter("latitude_of_origin");
            ProjectionParameter scale_factor = GetParameter("scale_factor");
            ProjectionParameter false_easting = GetParameter("false_easting");
            ProjectionParameter false_northing = GetParameter("false_northing");

            //Check for missing parameters
            if (central_meridian == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'central_meridian'");
            }

            if (latitude_of_origin == null)
            {
                throw new ArgumentException(
                    "Missing projection parameter 'latitude_of_origin'");
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

            _lon_center = DegreesToRadians(central_meridian.Value);
            _lat_origin = DegreesToRadians(latitude_of_origin.Value);
            _falseEasting = false_easting.Value*_metersPerUnit;
            _falseNorthing = false_northing.Value*_metersPerUnit;

            if (scale_factor == null) //This is a two standard parallel Mercator projection (2SP)
            {
                _k0 = Math.Cos(_lat_origin)/Math.Sqrt(1.0 - E2*Math.Sin(_lat_origin)*Math.Sin(_lat_origin));
                AuthorityCode = 9805;
                Name = "Mercator_2SP";
            }
            else //This is a one standard parallel Mercator projection (1SP)
            {
                _k0 = scale_factor.Value;
                Name = "Mercator_1SP";
            }

            Authority = "EPSG";
        }

        /// <summary>
        /// Converts coordinates in decimal degrees to projected meters.
        /// </summary>
        /// <remarks>
        /// <para>The parameters this projection expects are listed below.</para>
        /// <list type="table">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term>longitude_of_natural_origin</term><description>The longitude of the point from which the values of both the geographical coordinates on the ellipsoid and the grid coordinates on the projection are deemed to increment or decrement for computational purposes. Alternatively it may be considered as the longitude of the point which in the absence of application of false coordinates has grid coordinates of (0,0).  Sometimes known as ""central meridian""."</description></item>
        /// <item><term>latitude_of_natural_origin</term><description>The latitude of the point from which the values of both the geographical coordinates on the ellipsoid and the grid coordinates on the projection are deemed to increment or decrement for computational purposes. Alternatively it may be considered as the latitude of the point which in the absence of application of false coordinates has grid coordinates of (0,0).</description></item>
        /// <item><term>scale_factor_at_natural_origin</term><description>The factor by which the map grid is reduced or enlarged during the projection process, defined by its value at the natural origin.</description></item>
        /// <item><term>false_easting</term><description>Since the natural origin may be at or near the centre of the projection and under normal coordinate circumstances would thus give rise to negative coordinates over parts of the mapped area, this origin is usually given false coordinates which are large enough to avoid this inconvenience. The False Easting, FE, is the easting value assigned to the abscissa (east).</description></item>
        /// <item><term>false_northing</term><description>Since the natural origin may be at or near the centre of the projection and under normal coordinate circumstances would thus give rise to negative coordinates over parts of the mapped area, this origin is usually given false coordinates which are large enough to avoid this inconvenience. The False Northing, FN, is the northing value assigned to the ordinate .</description></item>
        /// </list>
        /// </remarks>
        /// <param name="lonlat">The point in decimal degrees.</param>
        /// <returns>Point in projected meters</returns>
        public override TCoordinate DegreesToMeters(TCoordinate lonlat)
        {
            if (Double.IsNaN((Double) lonlat[0]) || Double.IsNaN((Double) lonlat[1]))
            {
                return CreateCoordinate(Double.NaN, Double.NaN);
            }

            Double dLongitude = DegreesToRadians((Double) lonlat[0]);
            Double dLatitude = DegreesToRadians((Double) lonlat[1]);

            /* Forward equations */
            if (Math.Abs(Math.Abs(dLatitude) - HalfPI) <= Epsilon)
            {
                throw new ComputationException(
                    "Transformation cannot be computed at the poles.");
            }
            else
            {
                Double esinphi = E*Math.Sin(dLatitude);
                Double semiMajor = SemiMajor;

                Double x = _falseEasting + semiMajor*_k0*(dLongitude - _lon_center);
                Double y = _falseNorthing +
                           semiMajor*_k0*
                           Math.Log(Math.Tan(PI*0.25 + dLatitude*0.5)
                                    *Math.Pow((1 - esinphi)/(1 + esinphi), E*0.5));

                if (lonlat.ComponentCount < 3)
                {
                    return CreateCoordinate(x/_metersPerUnit, y/_metersPerUnit);
                }
                else
                {
                    return CreateCoordinate(x/_metersPerUnit, y/_metersPerUnit, (Double) lonlat[2]);
                }
            }
        }

        /// <summary>
        /// Converts coordinates in projected meters to decimal degrees.
        /// </summary>
        /// <param name="p">Point in meters</param>
        /// <returns>Transformed point in decimal degrees</returns>
        public override TCoordinate MetersToDegrees(TCoordinate p)
        {
            Double dLongitude;
            Double dLatitude;
            Double semiMajor = SemiMajor;

            /* Inverse equations
              -----------------*/
            Double dX = p[Ordinates.X]*_metersPerUnit - _falseEasting;
            Double dY = p[Ordinates.Y]*_metersPerUnit - _falseNorthing;
            Double smallT = Math.Exp(-dY/(semiMajor*_k0)); // t

            Double chi = HalfPI - 2*Math.Atan(smallT);
            Double e4 = Math.Pow(E, 4);
            Double e6 = Math.Pow(E, 6);
            Double e8 = Math.Pow(E, 8);

            dLatitude = chi + (E2*0.5 + 5*e4/24 + e6/12 + 13*e8/360)*Math.Sin(2*chi)
                        + (7*e4/48 + 29*e6/240 + 811*e8/11520)*Math.Sin(4*chi) +
                        +(7*e6/120 + 81*e8/1120)*Math.Sin(6*chi) +
                        +(4279*e8/161280)*Math.Sin(8*chi);

            dLongitude = dX/(semiMajor*_k0) + _lon_center;

            if (p.ComponentCount < 3)
            {
                return CreateCoordinate(RadiansToDegrees(dLongitude), RadiansToDegrees(dLatitude));
            }
            else
            {
                return CreateCoordinate(RadiansToDegrees(dLongitude), RadiansToDegrees(dLatitude), (Double) p[2]);
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
                _inverse = new Mercator<TCoordinate>(
                    Enumerable.Downcast<ProjectionParameter, Parameter>(Parameters),
                    CoordinateFactory, !_isInverse);
            }

            return _inverse;
        }
    }
}