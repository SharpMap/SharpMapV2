using System;

namespace SharpMap.Expressions
{
    public class NullExpression : Expression
    {
        public override bool Contains(Expression other)
        {
            return false;
        }

        public override Expression Clone()
        {
            return new NullExpression();
        }

        public override bool Equals(Expression other)
        {
            return Equals(this, other);
        }
    }
}