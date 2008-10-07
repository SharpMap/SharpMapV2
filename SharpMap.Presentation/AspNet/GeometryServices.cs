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
using ProjNet.CoordinateSystems;

namespace SharpMap.Presentation.AspNet
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

        public IGeometryFactory this[int? srid]
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

            private readonly Dictionary<int, IGeometryFactory> _sridAwareGeometryFactories =
                new Dictionary<int, IGeometryFactory>();

            internal InternalFactoryService()
            {
                _coordinateFactory = new BufferedCoordinateFactory();
                _coordinateSequenceFactory =
                    new BufferedCoordinateSequenceFactory((BufferedCoordinateFactory)_coordinateFactory);
                _geometryFactory =
                    new GeometryFactory<BufferedCoordinate>(
                        (BufferedCoordinateSequenceFactory)_coordinateSequenceFactory);
                _coordinateSystemFactory =
                    new CoordinateSystemFactory<BufferedCoordinate>((BufferedCoordinateFactory)_coordinateFactory,
                                                                      (GeometryFactory<BufferedCoordinate>)
                                                                      _geometryFactory);

                ///TODO: Projection is not yet ready. Fill out when it is
                _coordinateTransformationFactory = null;
                /*
                new CoordinateTransformationFactory<BufferedCoordinate2D>(
                    (BufferedCoordinate2DFactory) _coordinateFactory,
                    (GeometryFactory<BufferedCoordinate2D>) _geometryFactory, null);*/
            }

            public ICoordinateSequenceFactory CoordinateSequenceFactory
            {
                get { return _coordinateSequenceFactory; }
            }

            #region IGeometryServices Members

            public IGeometryFactory this[int? srid]
            {
                get
                {
                    if (!srid.HasValue)
                        return DefaultGeometryFactory;

                    lock (_sridAwareGeometryFactories)
                        if (!_sridAwareGeometryFactories.ContainsKey(srid.Value))
                        {
                            _sridAwareGeometryFactories.Add(srid.Value,
                                                            new GeometryFactory<BufferedCoordinate>(srid.Value,
                                                                (BufferedCoordinateSequenceFactory)
                                                                _coordinateSequenceFactory));
                        }

                    return _sridAwareGeometryFactories[srid.Value];
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