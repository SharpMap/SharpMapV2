using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", IncludeInSchema = false)]
    public enum BinaryOperationType
    {
        Add,
        Div,
        Mul,
        Sub,
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "BinaryOperatorType")]
    [XmlRoot("Add", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class BinaryOperationExpression : BinaryExpressionBase<BinaryOperationType>
    {
        public BinaryOperationExpression(Expression left, BinaryOperationType op, Expression right) : base(left, op, right) { }
        
        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
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
            get { return new Expression[] { Left, Right }; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Length != 2)
                {
                    throw new ArgumentException("Value must be an array with 2 expressions.");
                }

                Left = value[0];
                Right = value[1];
            }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlElement("ExpressionElementTypes", Order = 1)]
        [XmlIgnore]
        public ExpressionType[] ExpressionElementTypes
        {
            get { return new ExpressionType[] { Left.ExpressionType, Right.ExpressionType }; }
            set {  }
        }

        #region Overrides of BinaryExpressionBase<ExpressionType>

        protected override BinaryExpressionBase<BinaryOperationType> Create(Expression left, BinaryOperationType op, Expression right)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}