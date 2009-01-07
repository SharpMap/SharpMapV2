using System;

namespace SharpMap.Expressions
{
    public class QueryExpression : Expression
    {
        private readonly SelectExpression _projection;
        private readonly LogicExpression _predicate;

        public QueryExpression(SelectExpression projection, LogicExpression predicate)
        {
            _projection = projection;
            _predicate = predicate;
        }

        public SelectExpression Projection
        {
            get { return _projection; }
        }

        public LogicExpression Predicate
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
            return new QueryExpression((SelectExpression)_projection.Clone(), 
                                       (LogicExpression)_predicate.Clone());
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
