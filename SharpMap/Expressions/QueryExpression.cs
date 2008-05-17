using System;

namespace SharpMap.Expressions
{
    public class QueryExpression : Expression
    {
        private readonly ProjectionExpression _projection;
        private readonly PredicateExpression _predicate;

        public QueryExpression(ProjectionExpression projection, PredicateExpression predicate)
        {
            _projection = projection;
            _predicate = predicate;
        }

        public ProjectionExpression Projection
        {
            get { return _projection; }
        }

        public PredicateExpression Predicate
        {
            get { return _predicate; }
        }

        public override bool Matches(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(Expression other)
        {
            throw new NotImplementedException();
        }
    }
}
