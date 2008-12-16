#region Namespace imports

using System;
using System.Xml.Serialization;

#endregion

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc")]
    public enum ComparisonOperator
    {
        Like,
        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanEqualTo,
        LessThan,
        LessThanEqualTo,
        Between,
        NullCheck,
    }
}