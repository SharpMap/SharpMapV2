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
