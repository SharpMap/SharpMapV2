using System;

namespace SharpMap.Expressions
{
    public class AttributeBinaryStringExpression : BinaryExpressionBase<BinaryStringOperator>
    {
        protected AttributeBinaryStringExpression(PropertyNameExpression left, BinaryStringOperator op,
                                                  StringExpression right)
            : base(left, op, right)
        {
        }

        public AttributeBinaryStringExpression(PropertyNameExpression left, BinaryStringOperator op, string value)
            : this(left, op, new StringExpression(value))
        {
        }

        public AttributeBinaryStringExpression(string propertyName, BinaryStringOperator op, string value)
            : this(new PropertyNameExpression(propertyName), op, new StringExpression(value))
        {
        }

        public AttributeBinaryStringExpression(string propertyName, BinaryStringOperator op, string value, StringComparison comparison)
            : this(new PropertyNameExpression(propertyName), op, new StringExpression(value, comparison))
        {
        }

        public AttributeBinaryStringExpression(PropertyNameExpression left, BinaryStringOperator op, string value, StringComparison comparison)
            : this(left, op, new StringExpression(value, comparison)) { }


        protected override BinaryExpressionBase<BinaryStringOperator> Create(Expression left, BinaryStringOperator op,
                                                                             Expression right)
        {
            return new AttributeBinaryStringExpression((PropertyNameExpression)left, op, (StringExpression)right);
        }

        public new PropertyNameExpression Left
        {
            get { return (PropertyNameExpression)base.Left; }
        }

        public new StringExpression Right
        {
            get { return (StringExpression)base.Right; }
        }
    }
}