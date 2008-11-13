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
using System.Collections;

#if DOTNET35
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else
using Enumerable = GeoAPI.DataStructures.Enumerable;
using Caster = GeoAPI.DataStructures.Caster;
#endif

namespace SharpMap.Expressions
{
    public class CollectionExpression : Expression, IEnumerable
    {
        private readonly IEnumerable _collection;

        public CollectionExpression(IEnumerable collection)
        {
            _collection = collection;
        }

        public IEnumerable Collection
        {
            get { return _collection; }
        }

        public override Boolean Contains(Expression other)
        {
            CollectionExpression ce = other as CollectionExpression;

            return ce != null &&
                   Enumerable.All(Caster.Cast<object>(_collection), delegate(Object item)
                   {
                       return Enumerable.Contains(Caster.Cast<object>(ce._collection), item);
                   });
        }

        public override Boolean Equals(Expression other)
        {
            CollectionExpression ce = other as CollectionExpression;

            return ce != null && Enumerable.SequenceEqual(Caster.Cast<object>(_collection), Caster.Cast<object>(ce._collection));
        }

        public override Expression Clone()
        {
            return new CollectionExpression(Enumerable.ToArray(Caster.Cast<object>(_collection)));
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion




    }
}
