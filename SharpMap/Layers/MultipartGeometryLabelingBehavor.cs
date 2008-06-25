// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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

using GeoAPI.Geometries;

namespace SharpMap.Layers
{
    /// <summary>
    /// Labeling behaviour for multipart geometry collections.
    /// </summary>
    public enum MultipartGeometryLabelingBehavior
    {
        /// <summary>
        /// Default labeling behavior: <see cref="MultipartGeometryLabelingBehavior.All"/>.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Place label on all parts (default).
        /// </summary>
        All = Default,

        /// <summary>
        /// Place label on object which the greatest length or area.
        /// </summary>
        /// <remarks>
        /// <see cref="IMultiPoint"/> geometries, since they are zero-dimensional,
        /// will default to <see cref="First"/>.
        /// </remarks>
        Largest,

        /// <summary>
        /// The center of the combined geometries.
        /// </summary>
        CommonCenter,

        /// <summary>
        /// Center of the first geometry in the collection (fastest method).
        /// </summary>
        First
    }
}
