using System;
using System.IO;
using System.Xml.Serialization;
using SharpMap.Styles;

namespace SharpMap.Symbology.Serialization
{
    /// <summary>
    /// SLD serializer
    /// </summary>
    public class FeatureTypeStyleSerializer
    {
        public static FeatureTypeStyleType Deserialize(String xml)
        {
            var serializer = new XmlSerializer(typeof(FeatureTypeStyleType));
            return serializer.Deserialize(new StringReader(xml)) as FeatureTypeStyleType;
        }

        public static String Serialize(FeatureTypeStyleType featureTypeStyle)
        {
            var serializer = new XmlSerializer(typeof (FeatureTypeStyleType));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, featureTypeStyle);
                return writer.ToString();
            }
        }
    }
}
