using System;
using System.Collections.Generic;

namespace SharpMap.Symbology.StyleAdditions
{
    /// <summary>
    /// Symbology endcoding names
    /// </summary>
    public interface IStyleNames
    {
        /// <summary>
        /// Gets or sets a value indicating the symbology encoding name
        /// </summary>
        String Name { get; set; }
        /// <summary>
        /// Gets or sets a <see cref="Description"/> of the style
        /// </summary>
        Description Description { get; set; }
        /// <summary>
        /// Gets or sets a list of semantic type identifiers
        /// </summary>
        ICollection<String> SemanticTypeIdentifier { get; set; }
    }
}
