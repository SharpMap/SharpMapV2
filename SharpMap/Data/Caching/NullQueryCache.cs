#region Namespace Imports

using System;
using System.Collections;
using SharpMap.Expressions;

#endregion

namespace SharpMap.Data.Caching
{
    public class NullQueryCache : IQueryCache
    {
        #region IQueryCache Members

        public void AddExpressionResult(Expression expression, Object result) { }

        public void AddExpressionResults(Expression expression, IEnumerable result) { }

        public Boolean Contains(Expression expression)
        {
            return false;
        }

        public Boolean Contains(IEnumerable items)
        {
            return false;
        }

        public Boolean Contains(object item)
        {
            return false;
        }

        public IEnumerable Retrieve(Expression expression) { yield break; }

        public void Expire(Expression expression) { }

        public void Expire(Object item) { }

        public void Expire(IEnumerable items) { }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            yield break;
        }

        #endregion
    }
}