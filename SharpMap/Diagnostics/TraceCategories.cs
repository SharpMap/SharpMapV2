
using System;

namespace SharpMap.Diagnostics
{
    /// <summary>
    /// Collection of trace categories
    /// </summary>
    public static class TraceCategories
    {
        /// <summary>
        /// Label for trace messages related to presentation
        /// </summary>
        public static readonly String Presentation = "Presentation";
        
        /// <summary>
        /// Label for trace messages related to rendering
        /// </summary>
        public static readonly String Rendering = "Rendering";
        
        /// <summary>
        /// Label for trace messages related to data access
        /// </summary>
        public static readonly String DataAccess = "DataAccess";

        /// <summary>
        /// Label for trace messages related to tool usage
        /// </summary>
        public static readonly String Tool = "Tool";
    }
}