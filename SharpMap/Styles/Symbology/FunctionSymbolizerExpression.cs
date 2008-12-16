using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using SharpMap.Expressions;

namespace SharpMap.Styles.Symbology
{
    [XmlInclude(typeof(RecodeType))]
    [XmlInclude(typeof(InterpolateType))]
    [XmlInclude(typeof(CategorizeType))]
    [XmlInclude(typeof(StringLengthType))]
    [XmlInclude(typeof(StringPositionType))]
    [XmlInclude(typeof(TrimType))]
    [XmlInclude(typeof(ChangeCaseType))]
    [XmlInclude(typeof(ConcatenateType))]
    [XmlInclude(typeof(SubstringType))]
    [XmlInclude(typeof(FormatDateType))]
    [XmlInclude(typeof(FormatNumberType))]
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "FunctionType")]
    public abstract class FunctionSymbolizerExpression : Expression
    {
    }
}
