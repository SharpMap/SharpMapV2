using System;
using SharpMap.Symbology.Serialization;
using SharpMap.Symbology.StyleAdditions;

namespace SharpMap.Symbology
{
    public static class OgcSymbologyDeserialize
    {
        public static LabelStyle LabelStyle(String xml)
        {
            var rawStyle = FeatureTypeStyleSerializer.Deserialize(xml);
            var labelStyle = new LabelStyle();

            if (!String.IsNullOrEmpty(rawStyle.Name))
            {
                labelStyle.Name = rawStyle.Name;
            }

            if (rawStyle.Description != null)
            {
                labelStyle.Description = new Description(rawStyle.Description.Title,
                                                         rawStyle.Description.Abstract);
            }

            return labelStyle;
        }
    }
}
