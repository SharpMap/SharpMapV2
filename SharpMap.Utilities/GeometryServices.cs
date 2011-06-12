/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Coordinates;
using NetTopologySuite.Coordinates.Simple;
using NPack;
using NPack.Interfaces;
using NPack.Matrix;
using GisSharpBlog.NetTopologySuite.CoordinateSystems;
using GisSharpBlog.NetTopologySuite.CoordinateSystems.Transformations;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Utilities
{
    public class GeometryServices
        : IGeometryServices
    {
        private static readonly InternalFactoryService _internalFactoryService = new InternalFactoryService();

        public ICoordinateSequenceFactory CoordinateSequenceFactory
        {
            get { return _internalFactoryService.CoordinateSequenceFactory; }
        }

        #region IGeometryServices Members

        public IGeometryFactory this[string srid]
        {
            get { return _internalFactoryService[srid]; }
        }

        public ICoordinateFactory CoordinateFactory
        {
            get { return _internalFactoryService.CoordinateFactory; }
        }

        public ICoordinateTransformationFactory CoordinateTransformationFactory
        {
            get { return _internalFactoryService.CoordinateTransformationFactory; }
        }

        public IGeometryFactory DefaultGeometryFactory
        {
            get { return _internalFactoryService.DefaultGeometryFactory; }
        }

        public ICoordinateSystemFactory CoordinateSystemFactory
        {
            get { return _internalFactoryService.CoordinateSystemFactory; }
        }

        #endregion

        #region Nested type: InternalFactoryService

        private class InternalFactoryService : IGeometryServices
        {
            private readonly ICoordinateFactory _coordinateFactory;

            private readonly ICoordinateSequenceFactory _coordinateSequenceFactory;

            private readonly ICoordinateSystemFactory _coordinateSystemFactory;
            private readonly ICoordinateTransformationFactory _coordinateTransformationFactory;

            private readonly IGeometryFactory _geometryFactory;

            private readonly Dictionary<string, IGeometryFactory> _sridAwareGeometryFactories =
                new Dictionary<string, IGeometryFactory>();

            internal InternalFactoryService()
            {
                _coordinateFactory = new CoordinateFactory();
                _coordinateSequenceFactory =
                    new CoordinateSequenceFactory((CoordinateFactory)_coordinateFactory);
                _geometryFactory =
                    new GeometryFactory<Coordinate>(
                        (CoordinateSequenceFactory)_coordinateSequenceFactory);
                _coordinateSystemFactory =
                    new CoordinateSystemFactory<Coordinate>((CoordinateFactory)_coordinateFactory,
                                                                    (GeometryFactory<Coordinate>)
                                                                    _geometryFactory);

                _coordinateTransformationFactory = new CoordinateTransformationFactory<Coordinate>(
                      (CoordinateFactory)_coordinateFactory,
                      (GeometryFactory<Coordinate>)_geometryFactory, new LinearFactory<DoubleComponent>());

            }

            public ICoordinateSequenceFactory CoordinateSequenceFactory
            {
                get { return _coordinateSequenceFactory; }
            }

            #region IGeometryServices Members

            public IGeometryFactory this[string srid]
            {
                get
                {
                    if (string.IsNullOrEmpty(srid))
                        return DefaultGeometryFactory;

                    IGeometryFactory factory;



                    if (!_sridAwareGeometryFactories.TryGetValue(srid, out factory))
                    {
                        factory = new GeometryFactory<Coordinate>(srid,
                            (CoordinateSequenceFactory)_coordinateSequenceFactory);
                        factory.Srid = srid;
                        factory.SpatialReference = SridMap.DefaultInstance.Process(factory.Srid,
                                                                                   (ICoordinateSystem)null);
                        lock (_sridAwareGeometryFactories)
                            if (!_sridAwareGeometryFactories.ContainsKey(srid))
                                _sridAwareGeometryFactories.Add(srid, factory);
                            else
                                _sridAwareGeometryFactories.TryGetValue(srid, out factory);

                    }

                    return factory;
                }
            }

            public ICoordinateFactory CoordinateFactory
            {
                get { return _coordinateFactory; }
            }

            public ICoordinateTransformationFactory CoordinateTransformationFactory
            {
                get { return _coordinateTransformationFactory; }
            }

            public IGeometryFactory DefaultGeometryFactory
            {
                get { return _geometryFactory; }
            }

            public ICoordinateSystemFactory CoordinateSystemFactory
            {
                get { return _coordinateSystemFactory; }
            }

            #endregion
        }

        #endregion
    }
}