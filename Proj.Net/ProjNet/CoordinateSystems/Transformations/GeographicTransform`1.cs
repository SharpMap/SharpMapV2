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
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Transformations
{
    /// <summary>
    /// The <see cref="GeographicTransform{TCoordinate}"/> class is 
    /// implemented on geographic transformation objects and implements 
    /// datum transformations between geographic coordinate systems.
    /// </summary>
    public class GeographicTransform<TCoordinate> : MathTransform<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        internal GeographicTransform(IGeographicCoordinateSystem<TCoordinate> source,
                                     IGeographicCoordinateSystem<TCoordinate> target,
                                     ICoordinateFactory<TCoordinate> coordinateFactory)
            : base(null, coordinateFactory, false)
        {
            _source = source;
            _target = target;
        }

        #region IGeographicTransform Members

        private IGeographicCoordinateSystem<TCoordinate> _source;

        /// <summary>
        /// Gets or sets the source geographic coordinate system for the transformation.
        /// </summary>
        public IGeographicCoordinateSystem<TCoordinate> Source
        {
            get { return _source; }
            set { _source = value; }
        }

        private IGeographicCoordinateSystem<TCoordinate> _target;

        /// <summary>
        /// Gets or sets the target geographic coordinate system for the transformation.
        /// </summary>
        public IGeographicCoordinateSystem<TCoordinate> Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// Returns the Well-Known Text for this object
        /// as defined in the simple features specification. [NOT IMPLEMENTED].
        /// </summary>
        public override String Wkt
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets an XML representation of this object [NOT IMPLEMENTED].
        /// </summary>
        public override String Xml
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        /// <summary>
        /// Creates the inverse transform of this object.
        /// </summary>
        /// <remarks>This method may fail if the transform is not one to one. However, all cartographic projections should succeed.</remarks>
        /// <returns></returns>
        public override IMathTransform<TCoordinate> Inverse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transforms a coordinate point. 
        /// The passed parameter point should not be modified.
        /// </summary>
        public override TCoordinate Transform(TCoordinate point)
        {
            Double value = (Double)point[0];

            value /= Source.AngularUnit.RadiansPerUnit;
            value -= Source.PrimeMeridian.Longitude/Source.PrimeMeridian.AngularUnit.RadiansPerUnit;
            value += Target.PrimeMeridian.Longitude/Target.PrimeMeridian.AngularUnit.RadiansPerUnit;
            value *= Source.AngularUnit.RadiansPerUnit;

            Int32 componentCount = point.ComponentCount;
            Double[] coordValues = new Double[componentCount];
            coordValues[0] = value;

            if (point is ICoordinate3D)
            {
                return CreateCoordinate(point[Ordinates.X], point[Ordinates.Y], point[Ordinates.Z]);
            }
            else
            {
                return CreateCoordinate(point[Ordinates.X], point[Ordinates.Y]);
            }
        }

        /// <summary>
        /// Transforms a list of coordinate point ordinal values.
        /// </summary>
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
        /// <param name="points"></param>
        /// <returns></returns>
        public override IEnumerable<TCoordinate> Transform(IEnumerable<TCoordinate> points)
        {
            foreach (TCoordinate point in points)
            {
                yield return Transform(point);
            }
        }

        /// <summary>
        /// Reverses the transformation
        /// </summary>
        public override void Invert()
        {
            throw new NotImplementedException();
        }
    }
}