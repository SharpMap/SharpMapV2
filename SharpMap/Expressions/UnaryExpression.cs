using System;

namespace SharpMap.Expressions
{
    public class UnaryExpression : Expression
    {
        private readonly UnaryOperator _op;
        private readonly Expression _operand;

        public UnaryExpression(UnaryOperator op, Expression operand)
        {
            _op = op;
            _operand = operand;
        }

        public UnaryOperator Op
        {
            get { return _op; }
        }

        public Expression Operand
        {
            get { return _operand; }
        }

        public override Boolean Matches(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Boolean Equals(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            throw new NotImplementedException();
        }
    }
}
