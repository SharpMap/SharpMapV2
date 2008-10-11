// Copyright 2006 - Morten Nielsen (www.iter.dk)
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
using GeoAPI.CoordinateSystems.Transformations;
using NPack;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Transformations
{
    internal class ConcatenatedTransform<TCoordinate> : MathTransform<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        private IMathTransform<TCoordinate> _inverse;
        private readonly List<ICoordinateTransformation<TCoordinate>> _transforms;

        public ConcatenatedTransform(ICoordinateFactory<TCoordinate> coordinateFactory)
            : this(new ICoordinateTransformation<TCoordinate>[0], coordinateFactory) {}

        public ConcatenatedTransform(IEnumerable<ICoordinateTransformation<TCoordinate>> transforms,
                                     ICoordinateFactory<TCoordinate> coordinateFactory)
            : base(null, coordinateFactory, false)
        {
            _transforms = new List<ICoordinateTransformation<TCoordinate>>(transforms);
        }

        public IEnumerable<ICoordinateTransformation<TCoordinate>> Transforms
        {
            get { return _transforms; }
            set
            {
                _transforms.Clear();
                _transforms.AddRange(value);
                _inverse = null;
            }
        }

        public override IMatrix<DoubleComponent> Derivative(TCoordinate point)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<TCoordinate> GetCodomainConvexHull(IEnumerable<TCoordinate> points)
        {
            throw new System.NotImplementedException();
        }

        public override DomainFlags GetDomainFlags(IEnumerable<TCoordinate> points)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Transforms a coordinate.
        /// </summary>
        public override TCoordinate Transform(TCoordinate point)
        {
            foreach (ICoordinateTransformation<TCoordinate> ct in _transforms)
            {
                point = ct.MathTransform.Transform(point);
            }

            return point;
        }

        /// <summary>
        /// Transforms a set of coordinates.
        /// </summary>
        public override IEnumerable<TCoordinate> Transform(IEnumerable<TCoordinate> points)
        {
            IEnumerable<TCoordinate> transformed = points;

            foreach (ICoordinateTransformation<TCoordinate> ct in _transforms)
            {
                transformed = ct.MathTransform.Transform(transformed);
            }

            return transformed;
        }

        public override ICoordinateSequence<TCoordinate> Transform(ICoordinateSequence<TCoordinate> points)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns the inverse of this conversion.
        /// </summary>
        /// <returns>
        /// <see cref="IMathTransform"/> that is the reverse of the current conversion.
        /// </returns>
        protected override IMathTransform GetInverseInternal()
        {
            IMathTransform inverse = new ConcatenatedTransform<TCoordinate>(_transforms, 
                                                                            CoordinateFactory);
            inverse.Invert();

            return inverse;
        }

        public override int SourceDimension
        {
            get { throw new System.NotImplementedException(); }
        }

        public override int TargetDimension
        {
            get { throw new System.NotImplementedException(); }
        }

        public override bool IsIdentity
        {
            get { throw new System.NotImplementedException(); }
        }

        public override ICoordinate Transform(ICoordinate coordinate)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<ICoordinate> Transform(IEnumerable<ICoordinate> points)
        {
            throw new System.NotImplementedException();
        }

        public override ICoordinateSequence Transform(ICoordinateSequence points)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Reverses the transformation
        /// </summary>
        public override void Invert()
        {
            _transforms.Reverse();

            foreach (ICoordinateTransformation<TCoordinate> transformation in _transforms)
            {
                transformation.MathTransform.Invert();
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

        public override IMatrix<DoubleComponent> Derivative(ICoordinate point)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<ICoordinate> GetCodomainConvexHull(IEnumerable<ICoordinate> points)
        {
            throw new System.NotImplementedException();
        }

        public override DomainFlags GetDomainFlags(IEnumerable<ICoordinate> points)
        {
            throw new System.NotImplementedException();
        }
    }
}