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
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NPack;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Transformations
{
    public abstract class MathTransform : IMathTransform
    {
        protected Boolean _isInverse;
        private readonly List<Parameter> _parameters;
        private readonly ICoordinateFactory _coordinateFactory;
        private IMathTransform _inverse;

        protected MathTransform(IEnumerable<Parameter> parameters, 
                                ICoordinateFactory coordinateFactory,
                                Boolean isInverse)
        {
            _isInverse = isInverse;
            _parameters = new List<Parameter>(parameters ?? new Parameter[0]);
            _coordinateFactory = coordinateFactory;
        }

        public Int32 ParameterCount
        {
            get { return _parameters.Count; }
        }

        protected ICoordinate CreateCoordinate(Double x, Double y)
        {
            return _coordinateFactory.Create(x, y);
        }

        protected ICoordinate CreateCoordinate(Double x, Double y, Double w)
        {
            return _coordinateFactory.Create(x, y, w);
        }

        protected ICoordinate CreateCoordinate3D(Double x, Double y, Double z)
        {
            return _coordinateFactory.Create3D(x, y, z);
        }

        protected ICoordinate CreateCoordinate3D(Double x, Double y, Double z, Double w)
        {
            return _coordinateFactory.Create3D(x, y, z, w);
        }

        protected ICoordinateFactory CoordinateFactory
        {
            get { return _coordinateFactory; }
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

        /// <summary>
        /// Gets the parameter at the given index.
        /// </summary>
        /// <param name="index">Index of parameter.</param>
        /// <returns>The parameter at the given index.</returns>
        protected Parameter GetParameterInternal(Int32 index)
        {
            if (index < 0 || index >= _parameters.Count)
            {
                throw new ArgumentOutOfRangeException("index", 
                                                      index,
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

        /// <summary>
        /// Returns the inverse of the concrete transformation.
        /// </summary>
        /// <returns>
        /// The <see cref="IMathTransform"/> that is the inverse of the transformation, if one exists.
        /// </returns>
        protected abstract IMathTransform GetInverseInternal();

        #region IMathTransform Members

        /// <summary>
        /// Gets the dimension of input points.
        /// </summary>
        public abstract Int32 SourceDimension { get; }

        /// <summary>
        /// Gets the dimension of output points.
        /// </summary>
        public abstract Int32 TargetDimension { get; }

        /// <summary>
        /// Tests whether this transform does not move any points.
        /// </summary>
        /// <returns></returns>
        public abstract Boolean IsIdentity { get; }

        /// <summary>
        /// Gets a Well-Known Text representation of this object.
        /// </summary>
        public abstract String Wkt { get; }

        /// <summary>
        /// Gets an XML representation of this object.
        /// </summary>
        public abstract String Xml { get; }

        public abstract IMatrix<DoubleComponent> Derivative(ICoordinate point);

        public abstract IEnumerable<ICoordinate> GetCodomainConvexHull(IEnumerable<ICoordinate> points);

        public abstract DomainFlags GetDomainFlags(IEnumerable<ICoordinate> points);

        public IMathTransform Inverse
        {
            get
            {
                if (_inverse == null)
                {
                    _inverse = GetInverseInternal();
                }

                return _inverse;
            }
        }

        public abstract ICoordinate Transform(ICoordinate coordinate);

        public abstract IEnumerable<ICoordinate> Transform(IEnumerable<ICoordinate> points);

        public abstract ICoordinateSequence Transform(ICoordinateSequence points);

        /// <summary>
        /// Reverses the transformation
        /// </summary>
        public virtual void Invert()
        {
            _isInverse = !_isInverse;
        }

        #endregion
    }
}