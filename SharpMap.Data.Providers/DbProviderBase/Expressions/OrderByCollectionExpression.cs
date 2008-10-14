using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using GeoAPI.DataStructures;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.Db.Expressions
{
    public class OrderByCollectionExpression
        : ProviderPropertyExpression<CollectionExpression<OrderByExpression>>
    {
        public OrderByCollectionExpression(IEnumerable<string> names)
            : this(Processor.Transform(names, o => new OrderByExpression(o)))
        { }

        public OrderByCollectionExpression(string name, SortOrder direction)
            : this(new OrderByExpression(name, direction))
        { }

        public OrderByCollectionExpression(OrderByExpression orderByExpression)
            : this(new[] { orderByExpression })
        { }

        public OrderByCollectionExpression(IEnumerable<OrderByExpression> orderByExpressions)
            : this(new CollectionExpression<OrderByExpression>(orderByExpressions))
        { }

        public OrderByCollectionExpression(CollectionExpression<OrderByExpression> orderByCollectionExpression)
            : base("OrderByCollection", orderByCollectionExpression)
        { }

        public override Expression Clone()
        {
            return new OrderByCollectionExpression((CollectionExpression<OrderByExpression>)PropertyValueExpression.Clone());
        }
    }

    public class OrderByExpression : Expression
    {
        public OrderByExpression(string name)
            : this(name, SortOrder.Ascending)
        { }

        public OrderByExpression(string name, SortOrder direction)
            : this(new PropertyNameExpression(name), direction)
        { }

        public OrderByExpression(PropertyNameExpression nameExpression, SortOrder direction)
        {
            PropertyNameExpression = nameExpression;
            Direction = direction;
        }

        public PropertyNameExpression PropertyNameExpression
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
            throw new NotImplementedException();
        }

        public override bool Equals(Expression other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return PropertyNameExpression.PropertyName + (Direction == SortOrder.Descending ? " DESC " : " ASC ");
        }

        public string ToString(string formatString)
        {
            return string.Format(formatString, PropertyNameExpression.PropertyName) + 
                (Direction == SortOrder.Descending ? " DESC " : " ASC ");
        }
    }
}
