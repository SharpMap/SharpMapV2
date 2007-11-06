// Copyright 2006 - Morten Nielsen (www.iter.dk)
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

using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Utilities;
using NPack;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Transformations
{
    /// <summary>
    /// Transformation for applying 
    /// </summary>
    internal class DatumTransform<TCoordinate> : MathTransform<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>,
            IConvertible
    {
        protected IMathTransform<TCoordinate> _inverse;
        private readonly Wgs84ConversionInfo _toWgs84;
        private readonly ITransformMatrix<DoubleComponent> _transform;
        private readonly ITransformMatrix<DoubleComponent> _inverseTransform;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatumTransform{TCoordinate}"/> class.
        /// </summary>
        public DatumTransform(Wgs84ConversionInfo towgs84, CoordinateFactoryDelegate<TCoordinate> coordinateFactory)
            : this(towgs84, coordinateFactory, false) { }

        private DatumTransform(Wgs84ConversionInfo towgs84, CoordinateFactoryDelegate<TCoordinate> coordinateFactory, Boolean isInverse)
            : base(null, coordinateFactory, isInverse)
        {
            _toWgs84 = towgs84;
            _transform = _toWgs84.GetAffineTransform();
            _inverseTransform = _transform.Inverse as ITransformMatrix<DoubleComponent>;
            _isInverse = isInverse;
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

        /// <summary>
        /// Creates the inverse transform of this object.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method may fail if the transform is not one to one. However, all cartographic projections should succeed.</remarks>
        public override IMathTransform<TCoordinate> Inverse()
        {
            if (_inverse == null)
            {
                _inverse = new DatumTransform<TCoordinate>(_toWgs84,
                    CoordinateFactory, !_isInverse);
            }

            return _inverse;
        }

        private TCoordinate applyTransformToPoint(TCoordinate p)
        {
            return (TCoordinate)_transform.TransformVector(p);
            //return _coordinateFactory(
            //        _transform[0]*p[0] - _transform[3]*p[1] + _transform[2]*p[2] + _transform[4],
            //        _transform[3]*p[0] + _transform[0]*p[1] - _transform[1]*p[2] + _transform[5],
            //        -_transform[2]*p[0] + _transform[1]*p[1] + _transform[0]*p[2] + _transform[6],
            //    );
        }

        private TCoordinate applyInvertedTransformToPoint(TCoordinate p)
        {
            return (TCoordinate)_inverseTransform.TransformVector(p);
            //return _coordinateFactory(
            //        _transform[0]*p[0] + _transform[3]*p[1] - _transform[2]*p[2] - _transform[4],
            //        -_transform[3]*p[0] + _transform[0]*p[1] + _transform[1]*p[2] - _transform[5],
            //        _transform[2]*p[0] - _transform[1]*p[1] + _transform[0]*p[2] - _transform[6],
            //    );
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
                return applyTransformToPoint(point);
            }
            else
            {
                return applyInvertedTransformToPoint(point);
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
        public override IEnumerable<TCoordinate> Transform(IEnumerable<TCoordinate> points)
        {
            return EnumerableConverter.Downcast<TCoordinate, IVector<DoubleComponent>>(
                _transform.TransformVectors(EnumerableConverter.Upcast<IVector<DoubleComponent>, TCoordinate>(points)));
        }
    }
}