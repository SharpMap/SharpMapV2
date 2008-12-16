using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "VersionType")]
    public enum OgcSymbologyEncodingVersion
    {
        [XmlEnum(Name = "1.1.0")]
        Version110,
    }

}
