using System;

namespace SharpMap.Expressions
{
    public class OidExpression : Expression
    {
        public override Boolean Matches(Expression other)
        {
            return Equals(other);
        }

        public override Expression Clone()
        {
            return new OidExpression();
        }

        public override Boolean Equals(Expression other)
        {
            return other is OidExpression;
        }
    }
}
