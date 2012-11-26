
using System;
using System.Collections.Generic;
#if DOTNET35
using Enumerable = System.Linq.Enumerable;
#else
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Enumerable = GeoAPI.DataStructures.Enumerable;
#endif

namespace SharpMap.Expressions
{
    /// <summary>
    /// A collection of <typeparamref name="TValue"/> expressions
    /// </summary>
    /// <typeparam name="TValue">The type of expressions to collect</typeparam>
    [Serializable]
    public class CollectionExpression<TValue> : CollectionExpression, IEnumerable<TValue>
        where TValue : Expression
    {
        private readonly IEqualityComparer<TValue> _comparer = EqualityComparer<TValue>.Default;

        public CollectionExpression(IEnumerable<TValue> collection) 
            : this(collection, EqualityComparer<TValue>.Default) { }
        
        public CollectionExpression(IEnumerable<TValue> collection, IEqualityComparer<TValue> comparer) 
            : base(collection)
        {
            _comparer = comparer;
        }

        protected CollectionExpression(SerializationInfo info, StreamingContext context)
            //:base(info, context)
        {
            var count = info.GetInt32("count");
            var items = new List<TValue>(count);

            for (var i = 0; i < count; i++)
                items.Add((TValue)info.GetValue(string.Format("i{0}", i), typeof(TValue)));

            base.Collection = items;
        }

        public new IEnumerable<TValue> Collection
        {
            get { return base.Collection as IEnumerable<TValue>; }
        }

        public IEqualityComparer<TValue> Comparer
        {
            get { return _comparer; }
        }

        public override Boolean Contains(Expression other)
        {
            CollectionExpression<TValue> ce = other as CollectionExpression<TValue>;

            if (ce == null)
            {
                return false;
            }

            IEnumerable<TValue> collection = ce.Collection;
            
            return Enumerable.All<TValue>(collection, delegate(TValue item)
                   {
                       return Enumerable.Contains<TValue>(collection, item, _comparer);
                   });
        }

        public override Boolean Equals(Expression other)
        {
            CollectionExpression<TValue> ce = other as CollectionExpression<TValue>;

            if (ce == null)
            {
                return false;
            }

            IEnumerable<TValue> otherCollection = ce.Collection;

            return Enumerable.SequenceEqual(Collection, otherCollection, _comparer);
        }

        public override Expression Clone()
        {
            IEnumerable<TValue> values = Collection;
            return new CollectionExpression(Enumerable.ToArray(values));
        }

        #region IEnumerable<TValue> Members

        public new IEnumerator<TValue> GetEnumerator()
        {
            if (Collection == null)
                return null;

            return Collection.GetEnumerator();
        }

        #endregion

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var count = 0;
            foreach (var item in Collection)
                count++;

            info.AddValue("count", count);
            if (count == 0) return;

            count = 0;
            foreach (var item in Collection)
            {
                info.AddValue(string.Format("i{0}", count++), item);
            }
        }
    }
}
