using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Symbology.Serialization;
using SharpMap.Symbology.StyleAdditions;

namespace SharpMap.Symbology
{
    public static class OgcSymbologyDeserialize
    {
        public static LabelStyle LabelStyle(String xml)
        {
            FeatureTypeStyleType rawStyle = FeatureTypeStyleSerializer.Deserialize(xml);
            LabelStyle labelStyle = new LabelStyle();

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
