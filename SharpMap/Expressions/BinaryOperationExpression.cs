using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "BinaryOperatorType")]
    [XmlRoot("Add", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class BinaryOperationExpression : BinaryExpressionBase<ExpressionType>
    {
        private Expression[] _expressions;
        private ExpressionType[] _expressionElementTypes;

        public BinaryOperationExpression(Expression left, ExpressionType op, Expression right) : base(left, op, right) {}

        [XmlElement("Add", typeof (BinaryOperationExpression), Order = 0)]
        [XmlElement("Div", typeof (BinaryOperationExpression), Order = 0)]
        [XmlElement("Function", typeof (FunctionExpression), Order = 0)]
        [XmlElement("Literal", typeof (LiteralExpression), Order = 0)]
        [XmlElement("Mul", typeof (BinaryOperationExpression), Order = 0)]
        [XmlElement("PropertyName", typeof (PropertyNameExpression), Order = 0)]
        [XmlElement("Sub", typeof (BinaryOperationExpression), Order = 0)]
        [XmlChoiceIdentifier("ExpressionElementTypes")]
        public Expression[] Expressions
        {
            get { return _expressions; }
            set { _expressions = value; }
        }

        [XmlElement("ExpressionElementTypes", Order = 1)]
        [XmlIgnore]
        public ExpressionType[] ExpressionElementTypes
        {
            get { return _expressionElementTypes; }
            set { _expressionElementTypes = value; }
        }
    }
}