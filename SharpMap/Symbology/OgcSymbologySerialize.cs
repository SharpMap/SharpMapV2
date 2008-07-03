using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Symbology.Serialization;
using SharpMap.Symbology.StyleAdditions;

namespace SharpMap.Symbology
{
    public static class OgcSymbologySerialize
    {
        public static String LabelStyle(LabelStyle style)
        {
            FeatureTypeStyleType rawStyle = new FeatureTypeStyleType();

            if (!String.IsNullOrEmpty(style.Name))
            {
                rawStyle.Name = style.Name;
            }

            if (style.Description != null)
            {
                rawStyle.Description = new DescriptionType();
                rawStyle.Description.Title = style.Description.Title;
                rawStyle.Description.Abstract= style.Description.Abstract;
            }

            return FeatureTypeStyleSerializer.Serialize(rawStyle);
        }
    }
}
