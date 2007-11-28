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
    public abstract class MathTransform<TCoordinate> : IMathTransform<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>,
            IConvertible
	{
        private readonly ICoordinateFactory<TCoordinate> _coordinateFactory;
        protected Boolean _isInverse = false;
        private readonly Double _e;
        private readonly Double _e2;
        private readonly Double _semiMajor;
        private readonly Double _semiMinor;
        private readonly List<Parameter> _parameters;

        protected MathTransform(IEnumerable<Parameter> parameters, ICoordinateFactory<TCoordinate> coordinateFactory, Boolean isInverse)
        {
            _isInverse = isInverse;
            _parameters = new List<Parameter>(parameters ?? new Parameter[0]);
            _coordinateFactory = coordinateFactory;

            Parameter semiMajorParam = null, semiMinorParam = null;

            foreach (Parameter p in _parameters)
            {
                String name = p.Name;

                if (name.Equals("semi_major", StringComparison.OrdinalIgnoreCase))
                {
                    semiMajorParam = p;
                }

                if (name.Equals("semi_minor", StringComparison.OrdinalIgnoreCase))
                {
                    semiMinorParam = p;
                }
            }

            if (ReferenceEquals(semiMajorParam, null))
            {
                throw new ArgumentException(
                    "Missing projection parameter 'semi_major'.");
            }

            if (ReferenceEquals(semiMinorParam, null))
            {
                throw new ArgumentException(
                    "Missing projection parameter 'semi_minor'.");
            }

            _semiMajor = semiMajorParam.Value;
            _semiMinor = semiMinorParam.Value;

            _e2 = 1.0 - Math.Pow(_semiMinor / _semiMajor, 2);
            _e = Math.Sqrt(_e2);
        }

        public Int32 ParameterCount
        {
            get { return _parameters.Count; }
        }

        /// <summary>
        /// Gets the parameter at the given index.
        /// </summary>
        /// <param name="index">Index of parameter.</param>
        /// <returns>The parameter at the given index.</returns>
        protected Parameter GetParameterInternal(Int32 index)
        {
            if (index < 0 || index >= _parameters.Count)
            {
                throw new ArgumentOutOfRangeException("index", index, 
                    "Parameter index out of range.");
            }

            return _parameters[index];
        }

        /// <summary>
        /// Gets an named parameter of the projection.
        /// </summary>
        /// <remarks>The parameter name is case insensitive</remarks>
        /// <param name="name">Name of parameter</param>
        /// <returns>parameter or null if not found</returns>
        protected Parameter GetParameterInternal(String name)
        {
            foreach (Parameter parameter in Parameters)
            {
                if (parameter.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return parameter;
                }
            }

            return null;
        }

        public Double SemiMajor
        {
            get { return _semiMajor; }
        }

        public Double SemiMinor
        {
            get { return _semiMinor; }
        }

        protected Double E
        {
            get { return _e; }
        }

        protected Double E2
        {
            get { return _e2; }
        }

        protected TCoordinate CreateCoordinate(Double x, Double y)
        {
            return _coordinateFactory.Create(x, y);
        }

        protected TCoordinate CreateCoordinate(Double x, Double y, Double z)
        {
            return _coordinateFactory.Create3D(x, y, z);
        }

	    protected IEnumerable<Parameter> Parameters
	    {
	        get
	        {
	            foreach (Parameter parameter in _parameters)
	            {
                    yield return parameter;
	            }
	        }
	    }

        protected ICoordinateFactory<TCoordinate> CoordinateFactory
        {
            get { return _coordinateFactory; }
        }

		#region IMathTransform Members

		/// <summary>
		/// Gets the dimension of input points.
		/// </summary>
		public virtual Int32 SourceDimension
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the dimension of output points.
		/// </summary>
		public virtual Int32 TargetDimension
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Tests whether this transform does not move any points.
		/// </summary>
		/// <returns></returns>
		public virtual Boolean IsIdentity
		{
            get
            {
                throw new NotImplementedException();
            }
		}

		/// <summary>
		/// Gets a Well-Known Text representation of this object.
		/// </summary>
		public abstract String Wkt { get; }

		/// <summary>
		/// Gets an XML representation of this object.
		/// </summary>
		public abstract String Xml { get; }

		public virtual IMatrix<DoubleComponent> Derivative(TCoordinate point)
		{
			throw new NotImplementedException();
		}

        public virtual IEnumerable<TCoordinate> GetCodomainConvexHull(IEnumerable<TCoordinate> points)
		{
			throw new NotImplementedException();
		}

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
        public virtual DomainFlags GetDomainFlags(IEnumerable<TCoordinate> points)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates the inverse transform of this object.
		/// </summary>
		/// <remarks>
		/// This method may fail if the transform is not one to one. 
		/// However, all cartographic projections should succeed.
		/// </remarks>
		/// <returns></returns>
		public abstract IMathTransform<TCoordinate> Inverse();

		/// <summary>
		/// Transforms a coordinate point. The passed parameter point should not be modified.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
        public abstract TCoordinate Transform(TCoordinate point);

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
        public abstract IEnumerable<TCoordinate> Transform(IEnumerable<TCoordinate> points);

		/// <summary>
		/// Reverses the transformation
		/// </summary>
		public virtual void Invert()
		{
		    _isInverse = !_isInverse;
		}

		/// <summary>
		/// Number of degrees per radian.
		/// </summary>
		protected const Double DegreesPerRadian = 180 / Math.PI;

		/// <summary>
		/// Number of radians per degree.
		/// </summary>
		protected const Double RadiansPerDegree = Math.PI / 180;

		/// <summary>
		/// Converts an angular measure in degrees into an equivilant measure
		/// in radians.
		/// </summary>
		/// <param name="degrees">The measure in degrees to convert.</param>
		/// <returns>
		/// The number of radians for the given <paramref name="degrees"/>
		/// measure.
		/// </returns>
		protected static Double DegreesToRadians(Double degrees)
		{
			return RadiansPerDegree * degrees;

		}

		/// <summary>
		/// Converts an angular measure in radians into an equivilant measure
		/// in degrees.
		/// </summary>
		/// <param name="radians">The measure in radians to convert.</param>
		/// <returns>
		/// The number of radians for the given <paramref name="radians"/>
		/// measure.
		/// </returns>
		protected static Double RadiansToDegrees(Double radians)
		{
			return DegreesPerRadian * radians;
		}

		#endregion
	}
}
