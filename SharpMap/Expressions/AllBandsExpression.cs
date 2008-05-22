namespace SharpMap.Expressions
{
    public class AllBandsExpression : ProjectionExpression
    {
        public override bool Matches(Expression other)
        {
            return Equals(other);
        }

        public override Expression Clone()
        {
            return new AllBandsExpression();
        }

        public override bool Equals(Expression other)
        {
            return other is AllBandsExpression;
        }
    }
}