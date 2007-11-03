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
using System.Globalization;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NPack.Interfaces;
using ProjNet.CoordinateSystems.Projections;

namespace ProjNet.CoordinateSystems.Transformations
{
    /// <summary>
    /// Creates coordinate transformations.
    /// </summary>
    public class CoordinateTransformationFactory<TCoordinate> : ICoordinateTransformationFactory<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>, IConvertible
    {
        #region ICoordinateTransformationFactory Members

        /// <summary>
        /// Creates a transformation between two coordinate systems.
        /// </summary>
        /// <remarks>
        /// This method will examine the coordinate systems in order to construct
        /// a transformation between them. This method may fail if no path between 
        /// the coordinate systems is found, using the normal failing behavior of 
        /// the DCP (e.g. throwing an exception).</remarks>
        /// <param name="source">Source coordinate system</param>
        /// <param name="target">Target coordinate system</param>
        /// <returns></returns>		
        public ICoordinateTransformation<TCoordinate> CreateFromCoordinateSystems(ICoordinateSystem<TCoordinate> source, ICoordinateSystem<TCoordinate> target)
        {
            if (source is IProjectedCoordinateSystem<TCoordinate> && target is IGeographicCoordinateSystem<TCoordinate>) //Projected -> Geographic
            {
                return projectedToGeographic((IProjectedCoordinateSystem<TCoordinate>)source, (IGeographicCoordinateSystem<TCoordinate>)target);
            }
            else if (source is IGeographicCoordinateSystem<TCoordinate> && target is IProjectedCoordinateSystem<TCoordinate>) //Geographic -> Projected
            {
                return geographicToProjected((IGeographicCoordinateSystem<TCoordinate>)source, (IProjectedCoordinateSystem<TCoordinate>)target);
            }
            else if (source is IGeographicCoordinateSystem<TCoordinate> && target is IGeocentricCoordinateSystem<TCoordinate>) //Geocentric -> Geographic
            {
                return geographicToGeocentric((IGeographicCoordinateSystem<TCoordinate>)source, (IGeocentricCoordinateSystem<TCoordinate>)target);
            }
            else if (source is IGeocentricCoordinateSystem<TCoordinate> && target is IGeographicCoordinateSystem<TCoordinate>) //Geocentric -> Geographic
            {
                return geocentricToGeographic((IGeocentricCoordinateSystem<TCoordinate>)source, (IGeographicCoordinateSystem<TCoordinate>)target);
            }
            else if (source is IProjectedCoordinateSystem<TCoordinate> && target is IProjectedCoordinateSystem<TCoordinate>) //Projected -> Projected
            {
                return projectedToProjected((source as IProjectedCoordinateSystem<TCoordinate>), (target as IProjectedCoordinateSystem<TCoordinate>));
            }
            else if (source is IGeocentricCoordinateSystem<TCoordinate> && target is IGeocentricCoordinateSystem<TCoordinate>) //Geocentric -> Geocentric
            {
                return createGeocentricToGeocentric((IGeocentricCoordinateSystem<TCoordinate>)source, (IGeocentricCoordinateSystem<TCoordinate>)target);
            }
            else if (source is IGeographicCoordinateSystem<TCoordinate> && target is IGeographicCoordinateSystem<TCoordinate>) //Geographic -> Geographic
            {
                return createGeographicToGeographic(source as IGeographicCoordinateSystem<TCoordinate>, target as IGeographicCoordinateSystem<TCoordinate>);
            }

            throw new NotSupportedException("No support for transforming between the two specified coordinate systems");
        }
        #endregion

        #region Methods for converting between specific systems

        private static ICoordinateTransformation<TCoordinate> geographicToGeocentric(IGeographicCoordinateSystem<TCoordinate> source, IGeocentricCoordinateSystem<TCoordinate> target)
        {
            IMathTransform<TCoordinate> geocMathTransform = createCoordinateOperation(target);
            return new CoordinateTransformation<TCoordinate>(source, target, TransformType.Conversion, geocMathTransform, String.Empty, String.Empty, -1, String.Empty, String.Empty);
        }

        private static ICoordinateTransformation<TCoordinate> geocentricToGeographic(IGeocentricCoordinateSystem<TCoordinate> source, IGeographicCoordinateSystem<TCoordinate> target)
        {
            IMathTransform<TCoordinate> geocMathTransform = createCoordinateOperation(source).Inverse();
            return new CoordinateTransformation<TCoordinate>(source, target, TransformType.Conversion, 
                geocMathTransform, String.Empty, String.Empty, -1, String.Empty, String.Empty);
        }

        private static ICoordinateTransformation<TCoordinate> projectedToProjected(IProjectedCoordinateSystem<TCoordinate> source, IProjectedCoordinateSystem<TCoordinate> target)
        {
            CoordinateTransformationFactory<TCoordinate> ctFac = new CoordinateTransformationFactory<TCoordinate>();

            ICoordinateTransformation<TCoordinate>[] transforms = new ICoordinateTransformation<TCoordinate>[]
                {
                    //First transform from projection to geographic
                    ctFac.CreateFromCoordinateSystems(source, source.GeographicCoordinateSystem),

                    //Transform geographic to geographic:
                    ctFac.CreateFromCoordinateSystems(source.GeographicCoordinateSystem, target.GeographicCoordinateSystem),

                    //Transform to new projection
                    ctFac.CreateFromCoordinateSystems(target.GeographicCoordinateSystem, target) 
                };

            ConcatenatedTransform<TCoordinate> ct = new ConcatenatedTransform<TCoordinate>(transforms);

            return new CoordinateTransformation<TCoordinate>(source,
                target, TransformType.Transformation, ct,
                String.Empty, String.Empty, -1, String.Empty, String.Empty);
        }

        private static ICoordinateTransformation<TCoordinate> geographicToProjected(IGeographicCoordinateSystem<TCoordinate> source, IProjectedCoordinateSystem<TCoordinate> target)
        {
            if (source.EqualParams(target.GeographicCoordinateSystem))
            {
                IMathTransform<TCoordinate> mathTransform = createCoordinateOperation(
                    target.Projection, target.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid, target.LinearUnit);

                return new CoordinateTransformation<TCoordinate>(source, target, TransformType.Transformation,
                    mathTransform, String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
            else
            {
                // Geographic coordinate systems differ - create concatenated transform
                CoordinateTransformationFactory<TCoordinate> ctFac = new CoordinateTransformationFactory<TCoordinate>();

                ICoordinateTransformation<TCoordinate>[] transforms = new ICoordinateTransformation<TCoordinate>[]
                    {
                        ctFac.CreateFromCoordinateSystems(source, target.GeographicCoordinateSystem),
                        ctFac.CreateFromCoordinateSystems(target.GeographicCoordinateSystem, target)
                    };

                ConcatenatedTransform<TCoordinate> ct = new ConcatenatedTransform<TCoordinate>(transforms);

                return new CoordinateTransformation<TCoordinate>(source,
                    target, TransformType.Transformation, ct,
                    String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
        }

        private static ICoordinateTransformation<TCoordinate> projectedToGeographic(IProjectedCoordinateSystem<TCoordinate> source, IGeographicCoordinateSystem<TCoordinate> target)
        {
            if (source.GeographicCoordinateSystem.EqualParams(target))
            {
                IMathTransform<TCoordinate> mathTransform = createCoordinateOperation(source.Projection, source.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid, source.LinearUnit).Inverse();
                return new CoordinateTransformation<TCoordinate>(source, target, TransformType.Transformation, mathTransform,
                    String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
            else
            {	
                // Geographic coordinate systems differ - create concatenated transform
                CoordinateTransformationFactory<TCoordinate> ctFac = new CoordinateTransformationFactory<TCoordinate>();

                ICoordinateTransformation<TCoordinate>[] transforms = new ICoordinateTransformation<TCoordinate>[]
                    {
                        ctFac.CreateFromCoordinateSystems(source, source.GeographicCoordinateSystem),
                        ctFac.CreateFromCoordinateSystems(source.GeographicCoordinateSystem, target)
                    };

                ConcatenatedTransform<TCoordinate> ct = new ConcatenatedTransform<TCoordinate>(transforms);

                return new CoordinateTransformation<TCoordinate>(source,
                    target, TransformType.Transformation, ct,
                    String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Geographic to geographic transformation
        /// </summary>
        /// <remarks>Adds a datum shift if nessesary</remarks>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static ICoordinateTransformation<TCoordinate> createGeographicToGeographic(IGeographicCoordinateSystem<TCoordinate> source, IGeographicCoordinateSystem<TCoordinate> target)
        {
            if (source.HorizontalDatum.EqualParams(target.HorizontalDatum))
            {
                // No datum shift needed
                return new CoordinateTransformation<TCoordinate>(source,
                    target, TransformType.Conversion, new GeographicTransform<TCoordinate>(source, target),
                    String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
            else
            {
                // Create datum shift
                // Convert to geocentric, perform shift and return to geographic
                CoordinateTransformationFactory<TCoordinate> ctFac = new CoordinateTransformationFactory<TCoordinate>();
                CoordinateSystemFactory<TCoordinate> cFac = new CoordinateSystemFactory<TCoordinate>();
                IGeocentricCoordinateSystem<TCoordinate> sourceCentric = cFac.CreateGeocentricCoordinateSystem(
                    source.HorizontalDatum.Name + " Geocentric", source.HorizontalDatum, LinearUnit.Metre, source.PrimeMeridian);
                IGeocentricCoordinateSystem<TCoordinate> targetCentric = cFac.CreateGeocentricCoordinateSystem(
                    target.HorizontalDatum.Name + " Geocentric", target.HorizontalDatum, LinearUnit.Metre, source.PrimeMeridian);

                ICoordinateTransformation<TCoordinate>[] transforms = new ICoordinateTransformation<TCoordinate>[]
                    {
                        ctFac.CreateFromCoordinateSystems(source, sourceCentric),
                        ctFac.CreateFromCoordinateSystems(sourceCentric, targetCentric),
                        ctFac.CreateFromCoordinateSystems(targetCentric, target)
                    };

                ConcatenatedTransform<TCoordinate> ct = new ConcatenatedTransform<TCoordinate>(transforms);

                return new CoordinateTransformation<TCoordinate>(source,
                    target, TransformType.Transformation, ct,
                    String.Empty, String.Empty, -1, String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// Geocentric to Geocentric transformation
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static ICoordinateTransformation<TCoordinate> createGeocentricToGeocentric(IGeocentricCoordinateSystem<TCoordinate> source, IGeocentricCoordinateSystem<TCoordinate> target)
        {
            List<ICoordinateTransformation<TCoordinate>> transforms = new List<ICoordinateTransformation<TCoordinate>>();

            //Does source has a datum different from WGS84 and is there a shift specified?
            if (source.HorizontalDatum.Wgs84Parameters != null && !source.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly)
            {
                IMathTransform<TCoordinate> dataumTransform =
                    new DatumTransform<TCoordinate>(source.HorizontalDatum.Wgs84Parameters);

                target = target.HorizontalDatum.Wgs84Parameters == null ||
                  target.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly
                     ? target
                     : GeocentricCoordinateSystem<TCoordinate>.Wgs84;

                transforms.Add(new CoordinateTransformation<TCoordinate>(target, source,
                    TransformType.Transformation, dataumTransform, "", "", -1, "", ""));
            }

            //Does target has a datum different from WGS84 and is there a shift specified?
            if (target.HorizontalDatum.Wgs84Parameters != null && !target.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly)
            {
                IMathTransform<TCoordinate> datumTransform =
                    new DatumTransform<TCoordinate>(target.HorizontalDatum.Wgs84Parameters).Inverse();

                source = source.HorizontalDatum.Wgs84Parameters == null ||
                           source.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly
                              ? source
                              : GeocentricCoordinateSystem<TCoordinate>.Wgs84;
                transforms.Add(new CoordinateTransformation<TCoordinate>(source, target,
                    TransformType.Transformation, datumTransform, "", "", -1, "", ""));
            }

            if (transforms.Count == 1) // Since we only have one shift, lets just return the datumshift from/to wgs84
            {
                return new CoordinateTransformation<TCoordinate>(source, target,
                    TransformType.ConversionAndTransformation, transforms[0].MathTransform, "", "", -1, "", "");
            }
            else
            {
                ConcatenatedTransform<TCoordinate> ct = new ConcatenatedTransform<TCoordinate>(transforms);

                return new CoordinateTransformation<TCoordinate>(source, target,
                    TransformType.ConversionAndTransformation, ct, "", "", -1, "", "");
            }
        }
        #endregion

        private static IMathTransform<TCoordinate> createCoordinateOperation(IGeocentricCoordinateSystem<TCoordinate> geo)
        {
            ProjectionParameter[] parameters = new ProjectionParameter[]
                {
                    new ProjectionParameter("semi_major", geo.HorizontalDatum.Ellipsoid.SemiMajorAxis),
                    new ProjectionParameter("semi_minor", geo.HorizontalDatum.Ellipsoid.SemiMinorAxis)
                };

            return new GeocentricTransform<TCoordinate>(parameters);
        }

        private static IMathTransform<TCoordinate> createCoordinateOperation(IProjection projection, IEllipsoid ellipsoid, ILinearUnit unit)
        {
            List<ProjectionParameter> parameterList = new List<ProjectionParameter>(projection);

            parameterList.Add(new ProjectionParameter("semi_major", ellipsoid.SemiMajorAxis));
            parameterList.Add(new ProjectionParameter("semi_minor", ellipsoid.SemiMinorAxis));
            parameterList.Add(new ProjectionParameter("unit", unit.MetersPerUnit));

            IMathTransform<TCoordinate> transform;

            switch (projection.ClassName.ToLower(CultureInfo.InvariantCulture).Replace(' ', '_'))
            {
                case "mercator":
                case "mercator_1sp":
                case "mercator_2sp":
                    //1SP
                    transform = new Mercator<TCoordinate>(parameterList);
                    break;
                case "transverse_mercator":
                    transform = new TransverseMercator<TCoordinate>(parameterList);
                    break;
                case "albers":
                case "albers_conic_equal_area":
                    transform = new AlbersProjection<TCoordinate>(parameterList);
                    break;
                case "lambert_conformal_conic":
                case "lambert_conformal_conic_2sp":
                case "lambert_conic_conformal_(2sp)":
                    transform = new LambertConformalConic2SP<TCoordinate>(parameterList);
                    break;
                default:
                    throw new NotSupportedException(String.Format("Projection {0} is not supported.", projection.ClassName));
            }

            return transform;
        }

    }
}

