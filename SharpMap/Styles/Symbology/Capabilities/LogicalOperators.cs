#region Namespace imports

using System;
using System.Xml.Serialization;

#endregion

namespace SharpMap.Styles.Symbology.Capabilities
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot(Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class LogicalOperators {}
}