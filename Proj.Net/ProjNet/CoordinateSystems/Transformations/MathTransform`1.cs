// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of GeoAPI.Net.
// GeoAPI.Net is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// GeoAPI.Net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with GeoAPI.Net; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NPack;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Transformations
{
    /// <summary>
    /// Abstract class for creating multi-dimensional coordinate points transformations.
    /// </summary>
    /// <remarks>
    /// If a client application wishes to query the source and target coordinate 
    /// systems of a transformation, then it should keep hold of the 
    /// <see cref="ICoordinateTransformation{TCoordinate}"/> interface, and use the contained 
    /// math transform object whenever it wishes to perform a transform.
    /// </remarks>
    public abstract class MathTransform<TCoordinate> : MathTransform,
                                                       IMathTransform<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        protected MathTransform(IEnumerable<Parameter> parameters, 
                                ICoordinateFactory<TCoordinate> coordinateFactory,
                                Boolean isInverse) 
            : base(parameters, coordinateFactory, isInverse) { }

        protected TCoordinate CreateCoordinate(Double x, Double y)
        {
            ICoordinateFactory<TCoordinate> factory = CoordinateFactory;
            Debug.Assert(factory != null);
            return factory.Create(x, y);
        }

        protected TCoordinate CreateCoordinate(Double x, Double y, Double w)
        {
            ICoordinateFactory<TCoordinate> factory = CoordinateFactory;
            Debug.Assert(factory != null);
            return factory.Create(x, y, w);
        }

        protected TCoordinate CreateCoordinate3D(Double x, Double y, Double z)
        {
            ICoordinateFactory<TCoordinate> factory = CoordinateFactory;
            Debug.Assert(factory != null);
            return factory.Create3D(x, y, z);
        }

        protected TCoordinate CreateCoordinate3D(Double x, Double y, Double z, Double w)
        {
            ICoordinateFactory<TCoordinate> factory = CoordinateFactory;
            Debug.Assert(factory != null);
            return factory.Create3D(x, y, z, w);
        }

        protected new ICoordinateFactory<TCoordinate> CoordinateFactory
        {
            get { return base.CoordinateFactory as ICoordinateFactory<TCoordinate>; }
        }

        #region IMathTransform<TCoordinate> Members

        public abstract IMatrix<DoubleComponent> Derivative(TCoordinate point);

        /// <summary>
        /// Gets flags classifying domain points within a convex hull.
        /// </summary>
        /// <remarks>
        /// The supplied ordinates are interpreted as a sequence of points, which 
        /// generates a convex hull in the source space. Conceptually, each of the 
        /// (usually infinite) points inside the convex hull is then tested against
        /// the source domain. The flags of all these tests are then combined. In 
        /// practice, implementations of different transforms will use different 
        /// short-cuts to avoid doing an infinite number of tests.
        /// </remarks>
        /// <param name="points"></param>
        /// <returns></returns>
        public abstract DomainFlags GetDomainFlags(IEnumerable<TCoordinate> points);

        public abstract IEnumerable<TCoordinate> GetCodomainConvexHull(IEnumerable<TCoordinate> points);

        /// <summary>
        /// Creates the inverse of this transform.
        /// </summary>
        /// <remarks>
        /// This method may fail if the transform is not one to one. 
        /// However, all cartographic projections should succeed.
        /// </remarks>
        /// <returns>
        /// A <see cref="IMathTransform{TCoordinate}"/> that is the inverse of the transform.
        /// </returns>
        public new IMathTransform<TCoordinate> Inverse
        {
            get { return (IMathTransform<TCoordinate>)base.Inverse;  }
        }

        /// <summary>
        /// Transforms a coordinate point. The passed parameter point should not be modified.
        /// </summary>
        public abstract TCoordinate Transform(TCoordinate point);

        /// <summary>
        /// Transforms a list of coordinates.
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
        public abstract IEnumerable<TCoordinate> Transform(IEnumerable<TCoordinate> points);

        public abstract ICoordinateSequence<TCoordinate> Transform(ICoordinateSequence<TCoordinate> points);

        #endregion
    }
}