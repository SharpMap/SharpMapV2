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

namespace SharpMap.Expressions
{
    public class LiteralExpression<TValue> : LiteralExpression
    {
        private readonly TValue _value;
        private readonly IEqualityComparer<TValue> _comparer;

        public LiteralExpression(TValue value)
            : this(value, EqualityComparer<TValue>.Default) { }

        public LiteralExpression(TValue value, IEqualityComparer<TValue> comparer)
        {
            _value = value;
            _comparer = comparer;
        }

        public TValue Value
        {
            get { return (TValue)base.Value; }
        }

        public IEqualityComparer<TValue> Comparer
        {
            get { return _comparer; }
        }

        public override Boolean Contains(Expression other)
        {
            return Equals(other);
        }

        public override Expression Clone()
        {
            return new LiteralExpression<TValue>(_value, _comparer);
        }

        public override Boolean Equals(Expression other)
        {
            LiteralExpression<TValue> valueExpression = other as LiteralExpression<TValue>;

            return valueExpression != null &&
                   Comparer.Equals(_value, valueExpression._value);
        }

        protected override Object GetValue()
        {
            return _value;
        }
    }
}
