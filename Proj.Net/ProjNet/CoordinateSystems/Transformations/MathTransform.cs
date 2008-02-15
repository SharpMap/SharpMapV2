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
        protected Boolean _isInverse = false;
        private readonly Double _e;
        private readonly Double _e2;
        private readonly Double _semiMajor;
        private readonly Double _semiMinor;
        private readonly List<Parameter> _parameters;
        private readonly ICoordinateFactory _coordinateFactory;

        protected MathTransform(IEnumerable<Parameter> parameters, ICoordinateFactory coordinateFactory,
                                Boolean isInverse)
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

        protected ICoordinate CreateCoordinate(Double x, Double y)
        {
            return _coordinateFactory.Create(x, y);
        }

        protected ICoordinate CreateCoordinate(Double x, Double y, Double z)
        {
            return _coordinateFactory.Create3D(x, y, z);
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
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a Well-Known Text representation of this object.
        /// </summary>
        public abstract String Wkt { get; }

        /// <summary>
        /// Gets an XML representation of this object.
        /// </summary>
        public abstract String Xml { get; }

        public IMatrix<DoubleComponent> Derivative(ICoordinate point)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICoordinate> GetCodomainConvexHull(IEnumerable<ICoordinate> points)
        {
            throw new NotImplementedException();
        }

        public DomainFlags GetDomainFlags(IEnumerable<ICoordinate> points)
        {
            throw new NotImplementedException();
        }

        public IMathTransform Inverse()
        {
            throw new NotImplementedException();
        }

        public ICoordinate Transform(ICoordinate coordinate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICoordinate> Transform(IEnumerable<ICoordinate> points)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reverses the transformation
        /// </summary>
        public virtual void Invert()
        {
            _isInverse = !_isInverse;
        }

        #endregion

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
    }
}