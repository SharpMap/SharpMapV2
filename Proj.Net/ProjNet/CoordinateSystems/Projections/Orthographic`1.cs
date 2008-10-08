// Copyright 2008: Rory Plaire (codekaizen@gmail.com)
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

using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.DataStructures;
using GeoAPI.Units;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Projections
{
    internal class Orthographic<TCoordinate> : MapProjection<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        private readonly Radians _lon_center;
        private readonly Radians _lat_origin;
        private readonly Double _falseEasting;
        private readonly Double _falseNorthing;
        private readonly Double _radius;
        private readonly Double _cosLatOrigin;
        private readonly Double _sinLatOrigin;
        private readonly TCoordinate _zero;

        /// <summary>
        /// Initializes an <see cref="Orthographic{TCoordinate}"/> projection
        /// with the specified parameters to project points. 
        /// </summary>
        /// <param name="parameters">Parameters of the projection.</param>
        /// <param name="coordinateFactory">Coordinate factory to use.</param>
        /// <remarks>
        ///   <para>The parameters this projection expects are listed below.</para>
        ///   <list type="table">
        ///     <listheader>
        ///      <term>Parameter</term>
        ///      <description>Description</description>
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
        ///      <term>false_easting</term>
        ///      <description>
        ///         Since the natural origin may be at or near the center of the projection and under 
        ///         normal coordinate circumstances would thus give rise to negative coordinates over 
        ///         parts of the mapped area, this origin is usually given false coordinates which are 
        ///         large enough to avoid this inconvenience. The False Easting, FE, is the easting 
        ///         value assigned to the abscissa (east).
        ///      </description>
        ///    </item>
        ///     <item>
        ///      <term>false_northing</term>
        ///      <description>
        ///         Since the natural origin may be at or near the center of the projection and under 
        ///         normal coordinate circumstances would thus give rise to negative coordinates over 
        ///         parts of the mapped area, this origin is usually given false coordinates which are 
        ///         large enough to avoid this inconvenience. The False Northing, FN, is the northing 
        ///         value assigned to the ordinate.
        ///      </description>
        ///    </item>
        ///  </list>
        /// </remarks>
        public Orthographic(IEnumerable<ProjectionParameter> parameters,
                            ICoordinateFactory<TCoordinate> coordinateFactory)
            : this(parameters, coordinateFactory, false) { }

        /// <summary>
        /// Initializes the MercatorProjection object with the specified parameters.
        /// </summary>
        /// <param name="parameters">Parameters of the projection.</param>
        /// <param name="coordinateFactory">Coordinate factory to use.</param>
        /// <param name="isInverse">
        /// Indicates whether the projection is inverse (meters to degrees vs. degrees to meters).
        /// </param>
        /// <remarks>
        ///   <para>The parameters this projection expects are listed below.</para>
        ///   <list type="table">
        ///     <listheader>
        ///      <term>Parameter</term>
        ///      <description>Description</description>
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
        ///      <term>false_easting</term>
        ///      <description>
        ///         Since the natural origin may be at or near the center of the projection and under 
        ///         normal coordinate circumstances would thus give rise to negative coordinates over 
        ///         parts of the mapped area, this origin is usually given false coordinates which are 
        ///         large enough to avoid this inconvenience. The False Easting, FE, is the easting 
        ///         value assigned to the abscissa (east).
        ///      </description>
        ///    </item>
        ///     <item>
        ///      <term>false_northing</term>
        ///      <description>
        ///         Since the natural origin may be at or near the center of the projection and under 
        ///         normal coordinate circumstances would thus give rise to negative coordinates over 
        ///         parts of the mapped area, this origin is usually given false coordinates which are 
        ///         large enough to avoid this inconvenience. The False Northing, FN, is the northing 
        ///         value assigned to the ordinate.
        ///      </description>
        ///    </item>
        ///    <item>
        ///         <term>radius</term>
        ///         <description>The radius of the sphere to use to compute the projection.</description>
        ///    </item>
        ///  </list>
        ///</remarks>
        public Orthographic(IEnumerable<ProjectionParameter> parameters,
                            ICoordinateFactory<TCoordinate> coordinateFactory,
                            Boolean isInverse)
            : base(parameters, coordinateFactory, isInverse)
        {
            ProjectionParameter central_meridian = GetParameter("central_meridian");
            ProjectionParameter latitude_of_origin = GetParameter("latitude_of_origin");
            ProjectionParameter false_easting = GetParameter("false_easting");
            ProjectionParameter false_northing = GetParameter("false_northing");
            ProjectionParameter radius = GetParameter("radius");

            //Check for missing parameters
            if (central_meridian == null)
            {
                throw new ArgumentException("Missing projection parameter 'central_meridian'");
            }

            if (latitude_of_origin == null)
            {
                throw new ArgumentException("Missing projection parameter 'latitude_of_origin'");
            }

            if (false_easting == null)
            {
                throw new ArgumentException("Missing projection parameter 'false_easting'");
            }

            if (false_northing == null)
            {
                throw new ArgumentException("Missing projection parameter 'false_northing'");
            }

            if (radius == null)
            {
                throw new ArgumentException("Missing projection parameter 'radius'");
            }

            _lon_center = (Radians)new Degrees(central_meridian.Value);
            _lat_origin = (Radians)new Degrees(latitude_of_origin.Value);
            _falseEasting = false_easting.Value * MetersPerUnit;
            _falseNorthing = false_northing.Value * MetersPerUnit;
            _radius = radius.Value * MetersPerUnit;

            _cosLatOrigin = Math.Cos((Double)_lat_origin);
            _sinLatOrigin = Math.Sin((Double)_lat_origin);

            _zero = coordinateFactory.Create(0, 0);

            Name = "Orthographic";
        }

        public override TCoordinate MetersToDegrees(TCoordinate coordinate)
        {
            Double lat = coordinate[Ordinates.X];
            Double lon = coordinate[Ordinates.Y];

            Double sinLat = Math.Sin(lat);
            Double cosLat = Math.Cos(lat);

            Double sinDeltaLon = Math.Sin(lon - (Double)_lon_center);
            Double cosDeltaLon = Math.Cos(lon - (Double)_lon_center);

            Double x = _radius * cosLat * sinDeltaLon;
            Double y = _radius * (_cosLatOrigin * sinLat - _sinLatOrigin * cosLat * cosDeltaLon);

            Double distanceCheck = _sinLatOrigin * sinLat + _cosLatOrigin + cosLat * cosDeltaLon;

            // NOTE: not sure if returning +Inf, +Inf makes sense for points not on the projection
            return distanceCheck < 0
                       ? CoordinateFactory.Create(Double.PositiveInfinity, Double.PositiveInfinity)
                       : CoordinateFactory.Create(x, y);
        }

        public override TCoordinate DegreesToMeters(TCoordinate coordinate)
        {
            Double rho = coordinate.Distance(_zero);
            Double c = Math.Asin(rho / _radius);

            Double x = (Double)(Radians)new Degrees(coordinate[Ordinates.X]);
            Double y = (Double)(Radians)new Degrees(coordinate[Ordinates.Y]);

            Double cosC = Math.Cos(c);
            Double sinC = Math.Sin(c);

            Double lat = Math.Asin(cosC * _sinLatOrigin + (y * sinC * _cosLatOrigin) / rho);
            Double lon;

            if (y == HalfPI)
            {
                lon = x + Math.Atan2(x, -y);
            }
            else if (y == -HalfPI)
            {
                lon = x + Math.Atan2(x, y);
            }
            else
            {
                lon = (Double)_lon_center +
                      Math.Atan2(x * sinC, (rho * _cosLatOrigin * cosC) - (y * _sinLatOrigin * sinC));
            }

            return CoordinateFactory.Create(lon, lat);
        }

        protected override IMathTransform GetInverseInternal()
        {
            IEnumerable<ProjectionParameter> parameters =
                Caster.Downcast<ProjectionParameter, Parameter>(Parameters);

            return new Mercator<TCoordinate>(parameters, CoordinateFactory, !_isInverse);
        }
    }
}
