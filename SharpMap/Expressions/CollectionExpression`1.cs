// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
#if DOTNET35
using Enumerable = System.Linq.Enumerable;
#else
using Enumerable = GeoAPI.DataStructures.Enumerable;
#endif

namespace SharpMap.Expressions
{
    public class CollectionExpression<TValue> : CollectionExpression, IEnumerable<TValue>
    {
        private readonly IEqualityComparer<TValue> _comparer;

        public CollectionExpression(IEnumerable<TValue> collection) 
            : this(collection, EqualityComparer<TValue>.Default) { }
        
        public CollectionExpression(IEnumerable<TValue> collection, IEqualityComparer<TValue> comparer) 
            : base(collection)
        {
            _comparer = comparer;
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
            return Collection.GetEnumerator();
        }

        #endregion
    }
}
