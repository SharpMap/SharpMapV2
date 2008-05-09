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

using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Utilities;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Transformations
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Latitude, Longitude and ellipsoidal height in terms of a 3-dimensional geographic system
    /// may by expressed in terms of a geocentric (earth centered) Cartesian coordinate reference system
    /// X, Y, Z with the Z axis corresponding to the earth's rotation axis positive northwards, the X
    /// axis through the intersection of the prime meridian and equator, and the Y axis through
    /// the intersection of the equator with longitude 90 degrees east. The geographic and geocentric
    /// systems are based on the same geodetic datum.</para>
    /// <para>Geocentric coordinate reference systems are conventionally taken to be defined with the X
    /// axis through the intersection of the Greenwich meridian and equator. This requires that the equivalent
    /// geographic coordinate reference systems based on a non-Greenwich prime meridian should first be
    /// transformed to their Greenwich equivalent. Geocentric coordinates X, Y and Z take their units from
    /// the units of the ellipsoid axes (a and b). As it is conventional for X, Y and Z to be in metres,
    /// if the ellipsoid axis dimensions are given in another linear unit they should first be converted
    /// to metres.</para>
    /// </remarks>
    internal class GeocentricTransform<TCoordinate> : MathTransform<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        private const Double COS_67P5 = 0.38268343236508977; /* cosine of 67.5 degrees */
        private const Double AD_C = 1.0026000; /* Toms region 1 constant */

        private Double _ses; // Second eccentricity squared : (a^2 - b^2)/b^2
        //private Double ab; // Semi_major / semi_minor
        //private Double ba; // Semi_minor / semi_major

        protected MathTransform<TCoordinate> _inverse;

        /// <summary>
        /// Initializes a geocentric projection object
        /// </summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        /// <param name="isInverse">Indicates whether the projection forward (meters to degrees or degrees to meters).</param>
        public GeocentricTransform(IEnumerable<ProjectionParameter> parameters,
                                   ICoordinateFactory<TCoordinate> coordinateFactory, Boolean isInverse)
            : base(Enumerable.Upcast<Parameter, ProjectionParameter>(parameters), coordinateFactory, isInverse)
        {
            Double semiMinor = SemiMinor;
            _ses = (Math.Pow(SemiMajor, 2) - Math.Pow(semiMinor, 2))/Math.Pow(semiMinor, 2);
            //ba = _semiMinor / _semiMajor;
            //ab = _semiMajor / _semiMinor;
        }

        /// <summary>
        /// Initializes a geocentric projection object
        /// </summary>
        /// <param name="parameters">List of parameters to initialize the projection.</param>
        internal GeocentricTransform(IEnumerable<ProjectionParameter> parameters,
                                     ICoordinateFactory<TCoordinate> coordinateFactory)
            : this(parameters, coordinateFactory, false) {}


        /// <summary>
        /// Returns the inverse of this conversion.
        /// </summary>
        /// <returns>IMathTransform that is the reverse of the current conversion.</returns>
        public override IMathTransform<TCoordinate> Inverse()
        {
            if (_inverse == null)
            {
                _inverse = new GeocentricTransform<TCoordinate>(
                    Enumerable.Downcast<ProjectionParameter, Parameter>(Parameters),
                    CoordinateFactory, !_isInverse);
            }

            return _inverse;
        }

        /// <summary>
        /// Converts coordinates in decimal degrees to projected meters.
        /// </summary>
        /// <param name="lonlat">The point in decimal degrees.</param>
        /// <returns>Point in projected meters</returns>
        private TCoordinate DegreesToMeters(TCoordinate lonlat)
        {
            Double lon = DegreesToRadians((Double) lonlat[0]);
            Double lat = DegreesToRadians((Double) lonlat[1]);
            Double h = lonlat.ComponentCount < 3
                           ? 0
                           : ((Double) lonlat[2]).Equals(Double.NaN)
                                 ? 0
                                 : (Double) lonlat[2];

            Double v = SemiMajor/Math.Sqrt(1 - E2*Math.Pow(Math.Sin(lat), 2));
            Double x = (v + h)*Math.Cos(lat)*Math.Cos(lon);
            Double y = (v + h)*Math.Cos(lat)*Math.Sin(lon);
            Double z = ((1 - E2)*v + h)*Math.Sin(lat);
            return CreateCoordinate(x, y, z);
        }

        /// <summary>
        /// Converts coordinates in projected meters to decimal degrees.
        /// </summary>
        /// <param name="pnt">Point in meters</param>
        /// <returns>Transformed point in decimal degrees</returns>		
        private TCoordinate MetersToDegrees(TCoordinate pnt)
        {
            Boolean atPole = false; // indicates whether location is in polar region */
            Double z = pnt.ComponentCount < 3
                           ? 0
                           : ((Double) pnt[2]).Equals(Double.NaN)
                                 ? 0
                                 : (Double) pnt[2];

            Double lon;
            Double lat = 0;
            Double Height;

            if ((Double)pnt[0] != 0.0)
            {
                lon = Math.Atan2((Double) pnt[1], (Double) pnt[0]);
            }
            else
            {
                if ((Double) pnt[1] > 0)
                {
                    lon = Math.PI/2;
                }
                else if ((Double) pnt[1] < 0)
                {
                    lon = -Math.PI*0.5;
                }
                else
                {
                    atPole = true;
                    lon = 0.0;
                    if (z > 0.0)
                    {
                        /* north pole */
                        lat = Math.PI*0.5;
                    }
                    else if (z < 0.0)
                    {
                        /* south pole */
                        lat = -Math.PI*0.5;
                    }
                    else
                    {
                        /* center of earth */
                        return CreateCoordinate(RadiansToDegrees(lon), RadiansToDegrees(Math.PI*0.5), -SemiMajor);
                    }
                }
            }

            Double semiMajor = SemiMajor;

            Double W2 = (Double)pnt[0].Multiply(pnt[0]) + (Double) pnt[1]*(Double) pnt[1]; // Square of distance from Z axis
            Double W = Math.Sqrt(W2); // distance from Z axis
            Double T0 = z*AD_C; // initial estimate of vertical component
            Double S0 = Math.Sqrt(T0*T0 + W2); //initial estimate of horizontal component
            Double Sin_B0 = T0/S0; //sin(B0), B0 is estimate of Bowring aux variable
            Double Cos_B0 = W/S0; //cos(B0)
            Double Sin3_B0 = Math.Pow(Sin_B0, 3);
            Double T1 = z + semiMajor*_ses*Sin3_B0; //corrected estimate of vertical component
            Double Sum = W - semiMajor*E2*Cos_B0*Cos_B0*Cos_B0; //numerator of cos(phi1)
            Double S1 = Math.Sqrt(T1*T1 + Sum*Sum); //corrected estimate of horizontal component
            Double Sin_p1 = T1/S1; //sin(phi1), phi1 is estimated latitude
            Double Cos_p1 = Sum/S1; //cos(phi1)
            Double Rn = semiMajor/Math.Sqrt(1.0 - E2*Sin_p1*Sin_p1); //Earth radius at location

            if (Cos_p1 >= COS_67P5)
            {
                Height = W/Cos_p1 - Rn;
            }
            else if (Cos_p1 <= -COS_67P5)
            {
                Height = W/-Cos_p1 - Rn;
            }
            else
            {
                Height = z/Sin_p1 + Rn*(E2 - 1.0);
            }

            if (!atPole)
            {
                lat = Math.Atan(Sin_p1/Cos_p1);
            }

            return CreateCoordinate(RadiansToDegrees(lon), RadiansToDegrees(lat), Height);
        }

        /// <summary>
        /// Transforms a coordinate point. The passed parameter point should not be modified.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override TCoordinate Transform(TCoordinate point)
        {
            if (!_isInverse)
            {
                return DegreesToMeters(point);
            }
            else
            {
                return MetersToDegrees(point);
            }
        }

        /// <summary>
        /// Transforms a list of coordinate point ordinal values.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method is provided for efficiently transforming many points. The supplied array
        /// of ordinal values will contain packed ordinal values. For example, if the source
        /// dimension is 3, then the ordinals will be packed in this order (x0,y0,z0,x1,y1,z1 ...).
        /// The size of the passed array must be an integer multiple of DimSource. The returned
        /// ordinal values are packed in a similar way. In some DCPs. the ordinals may be
        /// transformed in-place, and the returned array may be the same as the passed array.
        /// So any client code should not attempt to reuse the passed ordinal values (although
        /// they can certainly reuse the passed array). If there is any problem then the server
        /// implementation will throw an exception. If this happens then the client should not
        /// make any assumptions about the state of the ordinal values.
        /// </remarks>
        public override IEnumerable<TCoordinate> Transform(IEnumerable<TCoordinate> points)
        {
            foreach (TCoordinate point in points)
            {
                yield return Transform(point);
            }
        }

        /// <summary>
        /// Gets a Well-Known Text representation of this object.
        /// </summary>
        /// <value></value>
        public override String Wkt
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets an XML representation of this object.
        /// </summary>
        /// <value></value>
        public override String Xml
        {
            get { throw new NotImplementedException(); }
        }
    }
}