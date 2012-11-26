using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using System.Data;

namespace SharpMap.Expressions
{

    [Serializable]
    public class SortExpressionCollectionExpression : CollectionExpression<SortExpression>
    {
        public SortExpressionCollectionExpression(IEnumerable<SortExpression> sortExpressions)
            : base(sortExpressions ?? new SortExpression[] { })
        {
        }
    }

    public class SortExpression : Expression
    {
        private readonly SortOrder _direction;
        private readonly Expression _expression;

        public SortExpression(Expression expr, SortOrder direction)
        {
            _expression = expr;
            _direction = direction;
        }

        public SortExpression(Expression expr)
            : this(expr, SortOrder.Ascending)
        {
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public SortOrder Direction
        {
            get { return _direction; }
        }

        public override bool Contains(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            return new SortExpression(Expression.Clone(), Direction);
        }

        public override bool Equals(Expression other)
        {
            SortExpression osort = other as SortExpression;
            if (osort == null)
                return false;

            return Equals(Expression, osort.Expression) && Equals(Direction, osort.Direction);
        }
    }
}