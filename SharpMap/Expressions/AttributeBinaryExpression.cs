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
using GeoAPI.Diagnostics;

namespace SharpMap.Expressions
{
    public class AttributeBinaryExpression : BinaryExpression
    {
        private TypeCode _valueTypeCode = TypeCode.Empty;

        protected AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Expression right)
            : base(left, op, right) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, String value)
            : base(left, op, new StringExpression(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Int16 value)
            : base(left, op, new LiteralExpression<Int16>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Int32 value)
            : base(left, op, new LiteralExpression<Int32>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Int64 value)
            : base(left, op, new LiteralExpression<Int64>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Single value)
            : base(left, op, new LiteralExpression<Single>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Double value)
            : base(left, op, new LiteralExpression<Double>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Decimal value)
            : base(left, op, new LiteralExpression<Decimal>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Guid value)
            : base(left, op, new LiteralExpression<Guid>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, UInt16 value)
            : base(left, op, new LiteralExpression<UInt16>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, UInt32 value)
            : base(left, op, new LiteralExpression<UInt32>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, UInt64 value)
            : base(left, op, new LiteralExpression<UInt64>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Char value)
            : base(left, op, new LiteralExpression<Char>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, Byte value)
            : base(left, op, new LiteralExpression<Byte>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, SByte value)
            : base(left, op, new LiteralExpression<SByte>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, DateTime value)
            : base(left, op, new LiteralExpression<DateTime>(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<String> value)
            : base(left, op, new StringCollectionExpression(value)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Int16> values)
            : base(left, op, new CollectionExpression<Int16>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Int32> values)
            : base(left, op, new CollectionExpression<Int32>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Int64> values)
            : base(left, op, new CollectionExpression<Int64>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Single> values)
            : base(left, op, new CollectionExpression<Single>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Double> values)
            : base(left, op, new CollectionExpression<Double>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Decimal> values)
            : base(left, op, new CollectionExpression<Decimal>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Guid> values)
            : base(left, op, new CollectionExpression<Guid>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<UInt16> values)
            : base(left, op, new CollectionExpression<UInt16>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<UInt32> values)
            : base(left, op, new CollectionExpression<UInt32>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<UInt64> values)
            : base(left, op, new CollectionExpression<UInt64>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Char> values)
            : base(left, op, new CollectionExpression<Char>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<Byte> values)
            : base(left, op, new CollectionExpression<Byte>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<SByte> values)
            : base(left, op, new CollectionExpression<SByte>(values)) { }

        public AttributeBinaryExpression(PropertyNameExpression left, BinaryLogicOperator op, IEnumerable<DateTime> values)
            : base(left, op, new CollectionExpression<DateTime>(values)) { }
        
        public Boolean HasSingleValueExpression
        {
            get { return Right is LiteralExpression; }
        }

        public Boolean HasCollectionValueExpression
        {
            get { return Right is CollectionExpression; }
        }

        public TypeCode ValueExpressionType
        {
            get
            {
                if (_valueTypeCode != TypeCode.Empty)
                {
                    return _valueTypeCode;
                }

                LiteralExpression valueExpression = Right as LiteralExpression;

                if (valueExpression != null)
                {
                    _valueTypeCode = Type.GetTypeCode(valueExpression.Value.GetType());
                }

                CollectionExpression collectionExpression = Right as CollectionExpression;

                if (collectionExpression != null)
                {
                    _valueTypeCode = Type.GetTypeCode(collectionExpression.GetType().GetGenericArguments()[0]);
                }

                Assert.IsNotEquals(_valueTypeCode, TypeCode.Empty);

                return _valueTypeCode;
            }
        }

        public PropertyNameExpression Attribute
        {
            get { return Left as PropertyNameExpression; }
        }

        public LiteralExpression Value
        {
            get { return Right as LiteralExpression; }
        }

        public CollectionExpression Values
        {
            get { return Right as CollectionExpression; }
        }

        public LiteralExpression<TValue> GetValue<TValue>()
        {
            return Right as LiteralExpression<TValue>;
        }

        public CollectionExpression<TValue> GetValues<TValue>()
        {
            return Right as CollectionExpression<TValue>;
        }
    }
}
