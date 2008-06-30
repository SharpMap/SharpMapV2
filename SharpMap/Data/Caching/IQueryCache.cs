using System;
using System.Collections;
using SharpMap.Expressions;

namespace SharpMap.Data.Caching
{
    /// <summary>
    /// Interface to represent a cache of data which has been retrieved from an 
    /// <see cref="IProvider"/> instance via a query <see cref="Expression"/>.
    /// </summary>
    public interface IQueryCache : IEnumerable
    {
        /// <summary>
        /// Adds the <paramref name="result"/> of executing the query <paramref name="expression"/>
        /// to the cache.
        /// </summary>
        /// <param name="expression">
        /// The <see cref="Expression"/> which was executed on the <see cref="IProvider"/>.
        /// </param>
        /// <param name="result">The result of executing the <paramref name="expression"/>.</param>
        // DESIGN_NOTE: should parameter 'expression' be type 'QueryExpression'?
        void AddExpressionResult(Expression expression, Object result);

        /// <summary>
        /// Adds the set of <paramref name="results"/> of executing the query 
        /// <paramref name="expression"/> to the cache.
        /// </summary>
        /// <param name="expression">
        /// The <see cref="Expression"/> which was executed on the <see cref="IProvider"/>.
        /// </param>
        /// <param name="results">The result of executing the <paramref name="expression"/>.</param>
        // DESIGN_NOTE: should parameter 'expression' be type 'QueryExpression'?
        void AddExpressionResults(Expression expression, IEnumerable results);

        /// <summary>
        /// Computes whether a query <paramref name="expression"/> can be satisfied by 
        /// data contained in the cache.
        /// </summary>
        /// <param name="expression">
        /// The <see cref="Expression"/> to use to check for cached data coverage.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IQueryCache"/> can satisfy the 
        /// <paramref name="expression"/> from cached data, <see langword="false"/>
        /// otherwise.
        /// </returns>
        Boolean Contains(Expression expression);

        /// <summary>
        /// Computes whether a set of <paramref name="items"/> is contained in the cache.
        /// </summary>
        /// <param name="items">The set of items to check cache containment of.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IQueryCache"/> contains each item in 
        /// <paramref name="items"/>, <see langword="false"/> otherwise.
        /// </returns>
        Boolean Contains(IEnumerable items);

        /// <summary>
        /// Computes whether an <paramref name="item"/> is contained in the cache.
        /// </summary>
        /// <param name="item">The item to check cache containment of.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IQueryCache"/> contains the item, 
        /// <see langword="false"/> otherwise.
        /// </returns>
        Boolean Contains(Object item);

        /// <summary>
        /// Forces the expiration of all items which are matched by the 
        /// <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">
        /// The <see cref="Expression"/> to use to match items in the cache.
        /// </param>
        void Expire(Expression expression);

        /// <summary>
        /// Forces the expiration of the specified item from the cache.
        /// </summary>
        /// <param name="item">
        /// The item to expire from the cache.
        /// </param>
        void Expire(Object item);

        /// <summary>
        /// Forces the expiration of the specified items from the cache.
        /// </summary>
        /// <param name="items">
        /// The set of items to expire from the cache.
        /// </param>
        void Expire(IEnumerable items);

        /// <summary>
        /// Retrieves the items in the cache which match the given <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">
        /// The <see cref="Expression"/> to match cached items to retrieve.
        /// </param>
        /// <returns></returns>
        IEnumerable Retrieve(Expression expression);

        /// <summary>
        /// Processes the given <paramref name="query"/> to optimize it for caching purposes. 
        /// </summary>
        /// <param name="query">
        /// The query to process to optimize based on data stored in the cache.
        /// </param>
        /// <returns>
        /// A (possibly) modified query based on the given <paramref name="query"/> and the data in 
        /// the cache which will keep the cache optimized.
        /// </returns>
        /// <remarks>
        /// The behavior of this method is highly dependent on the implementation of
        /// <see cref="IQueryCache"/>. Various cache implementations may have very different
        /// strategies to maintain a cache of data, and this will be reflected in the queries
        /// which are optimized by this method.
        /// </remarks>
        QueryExpression FilterQuery(QueryExpression query);
    }
}
