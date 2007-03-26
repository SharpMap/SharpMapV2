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
using System.Collections.Generic;
using System.Text;

namespace SharpMap
{
    /// <summary>
    /// Map tools enumeration
    /// </summary>
    public enum ToolSet
    {
        /// <summary>
        /// No active tool
        /// </summary>
        None = 0,
        /// <summary>
        /// Pan
        /// </summary>
        Pan,
        /// <summary>
        /// Zoom in
        /// </summary>
        ZoomIn,
        /// <summary>
        /// Zoom out
        /// </summary>
        ZoomOut,
        /// <summary>
        /// Query tool
        /// </summary>
        Query,
        /// <summary>
        /// QueryAdd tool
        /// </summary>
        QueryAdd,
        /// <summary>
        /// QueryRemove tool
        /// </summary>
        QueryRemove,
        /// <summary>
        /// Add feature tool
        /// </summary>
        FeatureAdd,
        /// <summary>
        /// Remove feature tool
        /// </summary>
        FeatureRemove
    }
}
