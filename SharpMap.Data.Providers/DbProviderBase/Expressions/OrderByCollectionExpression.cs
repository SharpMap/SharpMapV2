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
            : this(Processor.Transform(names, delegate(string o) { return new OrderByExpression(o); }))
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
}
