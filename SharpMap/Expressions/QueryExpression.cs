using System;

namespace SharpMap.Expressions
{
    [Serializable]
    public class QueryExpression : Expression
    {
        private readonly ProjectionExpression _projection;
        private readonly PredicateExpression _predicate;
        private readonly SortExpressionCollectionExpression _sort;
        public QueryExpression(ProjectionExpression projection, PredicateExpression predicate)
            : this(projection, predicate, null)
        {
        }

        public QueryExpression(ProjectionExpression projection, PredicateExpression predicate, SortExpressionCollectionExpression sort)
        {
            _projection = projection;
            _predicate = predicate;
            _sort = sort;
        }

        public ProjectionExpression Projection
        {
            get { return _projection; }
        }

        public PredicateExpression Predicate
        {
            get { return _predicate; }
        }

        public SortExpressionCollectionExpression Sort
        {
            get
            {
                return _sort;
            }
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
