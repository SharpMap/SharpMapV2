using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("refreshMode", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public enum refreshModeEnumType
    {
        /// <remarks/>
        onChange,

        /// <remarks/>
        onInterval,

        /// <remarks/>
        onExpire,
    }
}