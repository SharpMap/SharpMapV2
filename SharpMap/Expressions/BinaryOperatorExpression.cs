using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", IncludeInSchema = false, TypeName = "BinaryOperationType")]
    public enum BinaryOperation
    {
        Add,
        Div,
        Mul,
        Sub,
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "BinaryOperatorType")]
    [XmlRoot("Add", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class BinaryOperatorExpression : Expression
    {
        private Expression _left;
        private readonly BinaryOperation _op;
        private Expression _right;

        public BinaryOperatorExpression(Expression left, BinaryOperation op, Expression right)
        {

            _left = left;
            _op = op;
            _right = right;
        }

        [XmlIgnore]
        public Expression Left
        {
            get { return _left; }
            protected set { _left = value; }
        }

        [XmlIgnore]
        public BinaryOperation Op
        {
            get { return _op; }
        }

        [XmlIgnore]
        public Expression Right
        {
            get { return _right; }
            protected set { _right = value; }
        }

        public override Boolean Contains(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Boolean Equals(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            Expression leftClone = (Left == null ? null : Left.Clone());
            Expression rightClone = (Right == null ? null : Right.Clone());
            BinaryOperatorExpression clone = new BinaryOperatorExpression(leftClone, Op, rightClone);

            return clone;
        }

        [XmlIgnore]
        public override ExpressionType ExpressionType
        {
            get
            {
                switch (Op)
                {
                    case BinaryOperation.Add:
                        return SharpMap.Expressions.ExpressionType.Add;
                    case BinaryOperation.Div:
                        return SharpMap.Expressions.ExpressionType.Div;
                    case BinaryOperation.Mul:
                        return SharpMap.Expressions.ExpressionType.Mul;
                    case BinaryOperation.Sub:
                        return SharpMap.Expressions.ExpressionType.Sub;
                    default:
                        throw new InvalidOperationException("Unknown binary operation: " + Op);
                }
            }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlElement("Add", typeof(BinaryOperatorExpression), Order = 0)]
        [XmlElement("Div", typeof(BinaryOperatorExpression), Order = 0)]
        [XmlElement("Function", typeof(FunctionExpression), Order = 0)]
        [XmlElement("Literal", typeof(LiteralExpression), Order = 0)]
        [XmlElement("Mul", typeof(BinaryOperatorExpression), Order = 0)]
        [XmlElement("PropertyName", typeof(PropertyNameExpression), Order = 0)]
        [XmlElement("Sub", typeof(BinaryOperatorExpression), Order = 0)]
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
            set { }
        }
    }
}