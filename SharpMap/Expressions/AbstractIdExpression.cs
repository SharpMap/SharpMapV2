using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlInclude(typeof(FeatureIdExpression))]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "AbstractIdType")]
    public abstract class AbstractIdExpression
    {
    }
}
