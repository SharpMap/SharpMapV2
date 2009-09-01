using System;
using System.Data.SqlClient;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.Db.Expressions
{
    public class OrderByExpression : Expression
    {
        public OrderByExpression(string name)
            : this(name, SortOrder.Ascending)
        { }

        public OrderByExpression(string name, SortOrder direction)
            : this(new PropertyNameExpression(name), direction)
        { }

        public OrderByExpression(Expression nameExpression, SortOrder direction)
        {
            Expression = nameExpression;
            Direction = direction;
        }

        public Expression Expression
        {
            get;
            protected set;
        }

        public SortOrder Direction { get; protected set; }

        public override bool Contains(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            return new OrderByExpression(Expression.Clone(), Direction);
        }

        public override bool Equals(Expression other)
        {
            OrderByExpression order = other as OrderByExpression;
            if (order == null)
                return false;
            return order.Expression.Equals(Expression) && order.Direction == Direction; 
        }

        public override string ToString()
        {
            return Expression.ToString() + (Direction == SortOrder.Descending ? " DESC " : " ASC ");
        }

        public string ToString(string formatString)
        {
            return string.Format(formatString, Expression) + 
                   (Direction == SortOrder.Descending ? " DESC " : " ASC ");
        }
    }
}