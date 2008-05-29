// Copyright 2007 - Rory Plaire (codekaizen@gmail.com)
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
using GeoAPI.Geometries;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Layers;

namespace DemoWinForm
{
    public class ShapeFileLayerFactory : ILayerFactory
    {
        private readonly IGeometryFactory _geometryFactory;

        public ShapeFileLayerFactory(IGeometryFactory geometryFactory)
        {
            _geometryFactory = geometryFactory;    
        }

        #region ILayerFactory Members

        public ILayer Create(String layerName, String connectionInfo)
        {
            ShapeFileProvider shapeFileData = new ShapeFileProvider(connectionInfo, _geometryFactory);
            GeometryLayer shapeFileLayer = new GeometryLayer(layerName, shapeFileData);
            return shapeFileLayer;
        }

        #endregion
    }
}