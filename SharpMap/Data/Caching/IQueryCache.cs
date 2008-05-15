using System;
using System.Collections;
using SharpMap.Expressions;

namespace SharpMap.Data.Caching
{
    public interface IQueryCache : IEnumerable
    {
        void AddExpressionResult(Expression expression, Object result);
        void AddExpressionResults(Expression expression, IEnumerable result);
        Boolean Contains(Expression expression);
        Boolean Contains(IEnumerable items);
        Boolean Contains(Object item);
        void Expire(Expression expression);
        void Expire(Object item);
        void Expire(IEnumerable items);
        IEnumerable Retrieve(Expression expression);
    }
}
