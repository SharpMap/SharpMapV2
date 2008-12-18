using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "FilterType")]
    [XmlRoot("Filter", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class FilterExpression
    {
        private Expression[] _expressions;
        private ExpressionType[] _expressionElementTypes;

        [XmlElement("And", typeof (BinaryLogicExpression))]
        [XmlElement("Not", typeof (UnaryLogicExpression))]
        [XmlElement("Or", typeof (BinaryLogicExpression))]
        [XmlElement("PropertyIsBetween", typeof (PropertyIsBetweenExpression))]
        [XmlElement("PropertyIsEqualTo", typeof (BinaryComparisonExpression))]
        [XmlElement("PropertyIsGreaterThan", typeof (BinaryComparisonExpression))]
        [XmlElement("PropertyIsGreaterThanOrEqualTo", typeof (BinaryComparisonExpression))]
        [XmlElement("PropertyIsLessThan", typeof (BinaryComparisonExpression))]
        [XmlElement("PropertyIsLessThanOrEqualTo", typeof (BinaryComparisonExpression))]
        [XmlElement("PropertyIsLike", typeof (PropertyIsLikeExpression))]
        [XmlElement("PropertyIsNotEqualTo", typeof (BinaryComparisonExpression))]
        [XmlElement("PropertyIsNull", typeof(PropertyIsNullExpression))]
        [XmlElement("FeatureId", typeof(FeatureIdExpression))]
        [XmlElement("_Id", typeof (AbstractIdExpression))]
        [XmlElement("comparisonOps", typeof (ComparisonExpression))]
        //[XmlElement("logicOps", typeof (LogicExpression))]
        [XmlChoiceIdentifier("ExpressionElementTypes")]
        public Expression[] Expressions
        {
            get { return _expressions; }
            set { _expressions = value; }
        }

        [XmlElement("ExpressionElementTypes")]
        [XmlIgnore]
        public ExpressionType[] ExpressionElementTypes
        {
            get { return _expressionElementTypes; }
            set { _expressionElementTypes = value; }
        }
    }
}