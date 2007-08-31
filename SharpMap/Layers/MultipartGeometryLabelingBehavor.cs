using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Layers
{
    /// <summary>
    /// Labeling behaviour for Multipart geometry collections
    /// </summary>
    public enum MultipartGeometryLabelingBehaviour
    {
        /// <summary>
        /// Place label on all parts (default)
        /// </summary>
        All,

        /// <summary>
        /// Place label on object which the greatest length or area.
        /// </summary>
        /// <remarks>
        /// Multipoint geometries will default to <see cref="First"/>
        /// </remarks>
        Largest,

        /// <summary>
        /// The center of the combined geometries
        /// </summary>
        CommonCenter,

        /// <summary>
        /// Center of the first geometry in the collection (fastest method)
        /// </summary>
        First
    }
}
