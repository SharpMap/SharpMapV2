using System;
using System.Collections.Generic;

namespace SharpMap.Symbology.StyleAdditions
{
    public interface IStyleNames
    {
        String Name { get; set; }
        Description Description { get; set; }
        ICollection<String> SemanticTypeIdentifier { get; set; }
    }
}
