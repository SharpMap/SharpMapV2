using System;

namespace SharpMap.Symbology.StyleAdditions
{
    public interface IFeatureStyleNames : IStyleNames
    {
        String FeatureTypeName { get; set; }
    }
}
