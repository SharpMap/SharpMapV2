// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
using NPack.Interfaces;
using ProjNet.IO.CoordinateSystems;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// Builds up complex objects from simpler objects or values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ICoordinateSystemFactory allows applications to make coordinate 
    /// systems that cannot be created by a 
    /// <see cref="ICoordinateSystemAuthorityFactory{TCoordinate}"/>. 
    /// This factory is very flexible, whereas the authority factory is easier 
    /// to use.
    /// </para>
    /// <para>
    /// So <see cref="ICoordinateSystemAuthorityFactory{TCoordinate}"/>can be 
    /// used to make 'standard' coordinate systems, and 
    /// <see cref="CoordinateSystemFactory{TCoordinate}"/> can be used to make
    /// 'special' coordinate systems.
    /// </para>
    /// <para>
    /// For example, the EPSG authority has codes for USA state plane 
    /// coordinate systems using the NAD83 datum, but these coordinate 
    /// systems always use meters. EPSG does not have codes for NAD83 state 
    /// plane coordinate systems that use feet units. This factory
    /// lets an application create such a hybrid coordinate system.
    /// </para>
    /// </remarks>
    public class CoordinateSystemFactory<TCoordinate> : ICoordinateSystemFactory<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>,
            IConvertible
    {
        private readonly CoordinateFactoryDelegate<TCoordinate> _coordFactory;

        public CoordinateSystemFactory(CoordinateFactoryDelegate<TCoordinate> coordinateFactory)
        {
            _coordFactory = coordinateFactory;
        }

        #region ICoordinateSystemFactory Members

        /// <summary>
        /// Creates a coordinate system object from an XML String.
        /// </summary>
        /// <param name="xml">
        /// XML representation for the spatial reference.
        /// </param>
        /// <returns>The resulting spatial reference object.</returns>
        public ICoordinateSystem<TCoordinate> CreateFromXml(String xml)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a spatial reference object given its Well-Known Text 
        /// representation. The output object may be either a 
        /// <see cref="IGeographicCoordinateSystem{TCoordinate}"/> or
        /// a <see cref="IProjectedCoordinateSystem{TCoordinate}"/>.
        /// </summary>
        /// <param name="Wkt">
        /// The Well-Known Text representation for the spatial reference.
        /// </param>
        /// <returns>
        /// The resulting spatial reference object.
        /// </returns>
        public ICoordinateSystem<TCoordinate> CreateFromWkt(String Wkt)
        {
            CoordinateSystemWktReader<TCoordinate> reader = new CoordinateSystemWktReader<TCoordinate>(_coordFactory);
            return reader.Parse(Wkt) as ICoordinateSystem<TCoordinate>;
        }

        /// <summary>
        /// Creates a <see cref="ICompoundCoordinateSystem{TCoordinate}"/> [NOT IMPLEMENTED].
        /// </summary>
        /// <param name="name">Name of compound coordinate system.</param>
        /// <param name="head">Head coordinate system.</param>
        /// <param name="tail">Tail coordinate system.</param>
        /// <returns>Compound coordinate system.</returns>
        public ICompoundCoordinateSystem<TCoordinate> CreateCompoundCoordinateSystem(
            String name, ICoordinateSystem<TCoordinate> head, ICoordinateSystem<TCoordinate> tail)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a <see cref="IFittedCoordinateSystem{TCoordinate}"/>.
        /// </summary>
        /// <remarks>
        /// The units of the axes in the fitted coordinate system will be 
        /// inferred from the units of the base coordinate system. If the affine map
        /// performs a rotation, then any mixed axes must have identical units. For
        /// example, a (lat_deg,lon_deg,height_feet) system can be rotated in the 
        /// (lat,lon) plane, since both affected axes are in degrees. But you 
        /// should not rotate this coordinate system in any other plane.
        /// </remarks>
        /// <param name="name">
        /// Name of coordinate system.
        /// </param>
        /// <param name="baseCoordinateSystem">Base coordinate system.</param>
        /// <returns>Fitted coordinate system.</returns>
        public IFittedCoordinateSystem<TCoordinate> CreateFittedCoordinateSystem(
            String name, ICoordinateSystem<TCoordinate> baseCoordinateSystem,
            String toBaseWkt, IEnumerable<AxisInfo> axes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a <see cref="ILocalCoordinateSystem{TCoordinate}">local coordinate system</see>.
        /// </summary>
        /// <remarks>
        /// The dimension of the local coordinate system is determined by the size of 
        /// the axis array. All the axes will have the same units. If you want to make 
        /// a coordinate system with mixed units, then you can make a compound 
        /// coordinate system from different local coordinate systems.
        /// </remarks>
        /// <param name="name">Name of local coordinate system.</param>
        /// <param name="datum">Local datum.</param>
        /// <param name="unit">Units.</param>
        /// <param name="axes">Axis info.</param>
        /// <returns>Local coordinate system.</returns>
        public ILocalCoordinateSystem<TCoordinate> CreateLocalCoordinateSystem(
            String name, ILocalDatum datum, IUnit unit, IEnumerable<AxisInfo> axes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an <see cref="Ellipsoid"/> from radius values.
        /// </summary>
        /// <seealso cref="CreateFlattenedSphere"/>
        /// <param name="name">Name of ellipsoid.</param>
        /// <returns>Ellipsoid.</returns>
        public IEllipsoid CreateEllipsoid(String name, Double semiMajorAxis, 
            Double semiMinorAxis, ILinearUnit linearUnit)
        {
            return new Ellipsoid(semiMajorAxis, semiMinorAxis, 1.0, false, 
                linearUnit, name, String.Empty, -1, String.Empty, String.Empty, 
                String.Empty);
        }

        /// <summary>
        /// Creates an <see cref="Ellipsoid"/> from an major radius, 
        /// and inverse flattening.
        /// </summary>
        /// <seealso cref="CreateEllipsoid"/>
        /// <param name="name">Name of ellipsoid.</param>
        /// <param name="semiMajorAxis">Semi major-axis.</param>
        /// <param name="inverseFlattening">Inverse flattening.</param>
        /// <param name="linearUnit">Linear unit.</param>
        /// <returns>Ellipsoid.</returns>
        public IEllipsoid CreateFlattenedSphere(String name, Double semiMajorAxis, 
            Double inverseFlattening, ILinearUnit linearUnit)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid name");
            }

            return new Ellipsoid(semiMajorAxis, -1, inverseFlattening, true, 
                linearUnit, name, String.Empty, -1, String.Empty, String.Empty, 
                String.Empty);
        }

        /// <summary>
        /// Creates a <see cref="ProjectedCoordinateSystem{TCoordinate}"/> using a 
        /// projection object.
        /// </summary>
        /// <param name="name">Name of projected coordinate system.</param>
        /// <param name="gcs">Geographic coordinate system.</param>
        /// <param name="projection">Projection.</param>
        /// <param name="linearUnit">Linear unit.</param>
        /// <param name="axis0">Primary axis.</param>
        /// <param name="axis1">Secondary axis.</param>
        /// <returns>Projected coordinate system.</returns>
        public IProjectedCoordinateSystem<TCoordinate> CreateProjectedCoordinateSystem(
            String name, IGeographicCoordinateSystem<TCoordinate> gcs, IProjection projection,
            ILinearUnit linearUnit, AxisInfo axis0, AxisInfo axis1)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid name.");
            }

            if (gcs == null)
            {
                throw new ArgumentNullException("gcs");
            }

            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }

            if (linearUnit == null)
            {
                throw new ArgumentNullException("linearUnit");
            }

            AxisInfo[] info = new AxisInfo[]
                {
                    axis0,
                    axis1
                };

            return new ProjectedCoordinateSystem<TCoordinate>(
                null, gcs, linearUnit, projection, info, name, String.Empty,
                -1, String.Empty, String.Empty, String.Empty);
        }

        /// <summary>
        /// Creates a <see cref="Projection"/>.
        /// </summary>
        /// <param name="name">Name of projection.</param>
        /// <param name="wktProjectionClass">Projection class.</param>
        /// <param name="parameters">Projection parameters.</param>
        /// <returns>Projection.</returns>
        public IProjection CreateProjection(String name, String wktProjectionClass,
                                            IEnumerable<ProjectionParameter> parameters)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid name");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            List<ProjectionParameter> paramList = new List<ProjectionParameter>(parameters);
            paramList.AddRange(parameters);

            if (paramList.Count == 0)
            {
                throw new ArgumentException("Invalid projection parameters.");
            }

            return new Projection(wktProjectionClass, paramList, name, 
                String.Empty, -1, String.Empty, String.Empty, String.Empty);
        }

        /// <summary>
        /// Creates <see cref="HorizontalDatum"/> from ellipsoid and Bursa-Wolf 
        /// parameters.
        /// </summary>
        /// <remarks>
        /// Since this method contains a set of Bursa-Wolf parameters, the created 
        /// datum will always have a relationship to WGS84. If you wish to create a
        /// horizontal datum that has no relationship with WGS84, then you can 
        /// either specify a <see cref="DatumType">horizontalDatumType</see> of 
        /// <see cref="DatumType.HorizontalOther"/>, or create it via Wkt.
        /// </remarks>
        /// <param name="name">Name of ellipsoid.</param>
        /// <param name="datumType">Type of datum.</param>
        /// <param name="ellipsoid">Ellipsoid.</param>
        /// <param name="toWgs84">Wgs84 conversion parameters.</param>
        /// <returns>Horizontal datum.</returns>
        public IHorizontalDatum CreateHorizontalDatum(String name, DatumType datumType, IEllipsoid ellipsoid,
                                                      Wgs84ConversionInfo toWgs84)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid name");
            }

            if (ellipsoid == null)
            {
                throw new ArgumentException("Ellipsoid was null");
            }

            return new HorizontalDatum(ellipsoid, toWgs84, datumType, name, 
                String.Empty, -1, String.Empty, String.Empty, String.Empty);
        }

        /// <summary>
        /// Creates a <see cref="PrimeMeridian"/>, relative to Greenwich.
        /// </summary>
        /// <param name="name">Name of prime meridian.</param>
        /// <param name="angularUnit">Angular unit.</param>
        /// <param name="longitude">Longitude.</param>
        /// <returns>Prime meridian.</returns>
        public IPrimeMeridian CreatePrimeMeridian(String name, IAngularUnit angularUnit, Double longitude)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid name");
            }

            return new PrimeMeridian(longitude, angularUnit, name, 
                String.Empty, -1, String.Empty, String.Empty, String.Empty);
        }

        /// <summary>
        /// Creates a <see cref="GeographicCoordinateSystem{TCoordinate}"/>, 
        /// which could be Lat / Lon or Lon / Lat.
        /// </summary>
        /// <param name="name">Name of geographical coordinate system.</param>
        /// <param name="angularUnit">Angular units.</param>
        /// <param name="datum">Horizontal datum.</param>
        /// <param name="primeMeridian">Prime meridian.</param>
        /// <param name="axis0">First axis.</param>
        /// <param name="axis1">Second axis.</param>
        /// <returns>Geographic coordinate system.</returns>
        public IGeographicCoordinateSystem<TCoordinate> CreateGeographicCoordinateSystem(
            String name, IAngularUnit angularUnit, IHorizontalDatum datum,
            IPrimeMeridian primeMeridian, AxisInfo axis0, AxisInfo axis1)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid name");
            }

            List<AxisInfo> info = new List<AxisInfo>(2);
            info.Add(axis0);
            info.Add(axis1);
            return new GeographicCoordinateSystem<TCoordinate>(
                angularUnit, datum, primeMeridian, info, name, String.Empty,
                -1, String.Empty, String.Empty, String.Empty);
        }

        /// <summary>
        /// Creates a <see cref="ILocalDatum"/>.
        /// </summary>
        /// <param name="name">Name of datum.</param>
        /// <param name="datumType">Datum type.</param>
        /// <returns></returns>
        public ILocalDatum CreateLocalDatum(String name, DatumType datumType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a <see cref="IVerticalDatum"/> from an enumerated type value.
        /// </summary>
        /// <param name="name">Name of datum.</param>
        /// <param name="datumType">Type of datum.</param>
        /// <returns>Vertical datum.</returns>	
        public IVerticalDatum CreateVerticalDatum(String name, DatumType datumType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a <see cref="IVerticalCoordinateSystem{TCoordinate}"/> from a 
        /// <see cref="IVerticalDatum">datum</see> and 
        /// <see cref="LinearUnit">linear units</see>.
        /// </summary>
        /// <param name="name">Name of vertical coordinate system.</param>
        /// <param name="datum">Vertical datum.</param>
        /// <param name="verticalUnit">Unit.</param>
        /// <param name="axis">Axis info.</param>
        /// <returns>Vertical coordinate system.</returns>
        public IVerticalCoordinateSystem<TCoordinate> CreateVerticalCoordinateSystem(
            String name, IVerticalDatum datum, ILinearUnit verticalUnit, AxisInfo axis)
        {
            throw new NotImplementedException();
        }

        public IGeocentricCoordinateSystem<TCoordinate> CreateWgs84CoordinateSystem()
        {
            return CreateGeocentricCoordinateSystem(
                "WGS84 Geocentric", HorizontalDatum.WGS84,
                LinearUnit.Metre, PrimeMeridian.Greenwich);
        }

        /// <summary>
        /// Creates a <see cref="CreateGeocentricCoordinateSystem"/> from a 
        /// <see cref="IHorizontalDatum">datum</see>, 
        /// <see cref="ILinearUnit">linear unit</see> and <see cref="IPrimeMeridian"/>.
        /// </summary>
        /// <param name="name">Name of geocentric coordinate system</param>
        /// <param name="datum">Horizontal datum</param>
        /// <param name="linearUnit">Linear unit</param>
        /// <param name="primeMeridian">Prime meridian</param>
        /// <returns>Geocentric Coordinate System</returns>
        public IGeocentricCoordinateSystem<TCoordinate> CreateGeocentricCoordinateSystem(
            String name, IHorizontalDatum datum, ILinearUnit linearUnit, IPrimeMeridian primeMeridian)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid name");
            }

            AxisInfo[] info = new AxisInfo[]
                {
                    new AxisInfo("X", AxisOrientation.Other),
                    new AxisInfo("Y", AxisOrientation.Other),
                    new AxisInfo("Z", AxisOrientation.Other)
                };

            return new GeocentricCoordinateSystem<TCoordinate>(datum, linearUnit, 
                    primeMeridian, info, name, String.Empty, -1, String.Empty, 
                    String.Empty, String.Empty);
        }

        #endregion
    }
}