using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology.Capabilities
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/ogc", TypeName = "FID")]
    [XmlRoot(Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class Eid {}
}