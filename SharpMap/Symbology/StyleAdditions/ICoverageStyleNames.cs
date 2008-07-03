using System;

namespace SharpMap.Symbology.StyleAdditions
{
    public interface ICoverageStyleNames : IStyleNames
    {
        String CoverageName { get; set; }
    }
}
