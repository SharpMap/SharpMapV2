// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Data
{
    /// <summary>
    /// Specifies which data is retrieved from the <see cref="IFeatureLayerProvider"/>.
    /// </summary>
    [Flags]
    public enum QueryExecutionOptions
    {
        /// <summary>
        /// Specifies that the keys should be retrieved.
        /// </summary>
        Keys = 1,

        /// <summary>
        /// Specifies that the bounding boxes of feature geometries should be retrieved.
        /// </summary>
        BoundingBoxes = 2,

        /// <summary>
        /// Specifies that both the keys and the bounding boxes of 
        /// feature geometries should be retrieved.
        /// </summary>
        KeysAndBoundingBoxes = Keys | BoundingBoxes,
        
        /// <summary>
        /// Specifies that the feature geometries should be retrieved.
        /// </summary>
        Geometries = 4,

        /// <summary>
        /// Specifies that the feature attributes should be retrieved.
        /// </summary>
        Attributes = 8,

        /// <summary>
        /// Specifies that all feature data should be retrieved.
        /// </summary>
        FullFeature = Keys | Geometries | Attributes,
    }
}
