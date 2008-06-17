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

        public override Boolean Contains(Expression other)
        {
            QueryExpression otherQuery = other as QueryExpression;

            return otherQuery != null &&
                   Contains(otherQuery.Predicate, Predicate) &&
                   Contains(otherQuery.Projection, Projection);
        }

        public override Expression Clone()
        {
            return new QueryExpression((ProjectionExpression)_projection.Clone(), 
                                       (PredicateExpression)_predicate.Clone());
        }

        public override Boolean Equals(Expression other)
        {
            QueryExpression otherQuery = other as QueryExpression;

            return otherQuery != null &&
                   Equals(otherQuery.Predicate, Predicate) &&
                   Equals(otherQuery.Projection, Projection);
        }
    }
}
