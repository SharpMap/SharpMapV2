using System;

namespace SharpMap.Symbology.StyleAdditions
{
    /// <summary>
    /// Defines additional name for
    /// </summary>
    public interface IFeatureStyleNames : IStyleNames
    {
        String FeatureTypeName { get; set; }
    }
}
