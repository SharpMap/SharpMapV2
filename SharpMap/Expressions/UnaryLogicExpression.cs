using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", IncludeInSchema = false)]
    public enum ItemChoiceType3
    {
        And,
        Not,
        Or,
        PropertyIsBetween,
        PropertyIsEqualTo,
        PropertyIsGreaterThan,
        PropertyIsGreaterThanOrEqualTo,
        PropertyIsLessThan,
        PropertyIsLessThanOrEqualTo,
        PropertyIsLike,
        PropertyIsNotEqualTo,
        PropertyIsNull,
        comparisonOps,
        logicOps,
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "UnaryLogicOpType")]
    [XmlRoot("Not", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class UnaryLogicExpression
    {
        private Expression _expression;
        private ExpressionType _itemElementName;

        [XmlElement("And", typeof (BinaryLogicExpression))]
        [XmlElement("Not", typeof (UnaryLogicExpression))]
        [XmlElement("Or", typeof(BinaryLogicExpression))]
        [XmlElement("PropertyIsBetween", typeof (PropertyIsBetweenExpression))]
        [XmlElement("PropertyIsEqualTo", typeof (BinaryComparisonExpression))]
        [XmlElement("PropertyIsGreaterThan", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsGreaterThanOrEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLessThan", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLessThanOrEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLike", typeof (PropertyIsLikeExpression))]
        [XmlElement("PropertyIsNotEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsNull", typeof (PropertyIsNullExpression))]
        [XmlElement("comparisonOps", typeof (ComparisonExpression))]
        //[XmlElement("logicOps", typeof (LogicExpression))]
        [XmlChoiceIdentifier("ExpressionElementType")]
        public Expression Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        [XmlIgnore]
        [XmlElement("ExpressionElementType")]
        public ExpressionType ExpressionElementType
        {
            get { return _itemElementName; }
            set { _itemElementName = value; }
        }
    }
}