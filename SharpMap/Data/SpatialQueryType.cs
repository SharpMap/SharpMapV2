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

namespace SharpMap.Data
{
    /// <summary>
    /// Indicates the type of spatial query to execute.
    /// </summary>
    /// <remarks>
    /// See section 6.1.15 Relational operators in Open Geospatial Consortium's Simple Features Access 
    /// (reference number: OGC 06-103r3) for more careful definitions of these terms.
    /// </remarks>
    public enum SpatialQueryType
    {
        /// <summary>
        /// No spatial relation. Returns an empty set.
        /// </summary>
        None = 0,

        /// <summary>
        /// A query for geometries which are spatially contained in the query geometry.
        /// </summary>
        Contains,

        /// <summary>
        /// A query for geometries which spatially cross the query geometry.
        /// </summary>
        Crosses,

        /// <summary>
        /// A query for geometries which are spatially disjoint from the query geometry.
        /// </summary>
        Disjoint,

        /// <summary>
        /// A query for geometries which are spatially equal to the query geometry.
        /// </summary>
        Equals,

        /// <summary>
        /// A query for geometries which spatially intersect the query geometry.
        /// </summary>
        Intersects,

        /// <summary>
        /// A query for geometries which spatially overlap the query geometry.
        /// </summary>
        Overlaps,

        /// <summary>
        /// A query for geometries which spatially touch the query geometry.
        /// </summary>
        Touches,

        /// <summary>
        /// A query for geometries which are spatially within the query geometry.
        /// </summary>
        Within,
    }
}
