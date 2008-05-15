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

namespace SharpMap.Expressions
{
    public class AttributeBinaryExpression : BinaryExpression
    {
        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Expression right)
            : base(left, op, right) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, String value)
            : base(left, op, new StringExpression(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Int16 value)
            : base(left, op, new ValueExpression<Int16>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Int32 value)
            : base(left, op, new ValueExpression<Int32>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Int64 value)
            : base(left, op, new ValueExpression<Int64>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Single value)
            : base(left, op, new ValueExpression<Single>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Double value)
            : base(left, op, new ValueExpression<Double>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Decimal value)
            : base(left, op, new ValueExpression<Decimal>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Guid value)
            : base(left, op, new ValueExpression<Guid>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, UInt16 value)
            : base(left, op, new ValueExpression<UInt16>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, UInt32 value)
            : base(left, op, new ValueExpression<UInt32>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, UInt64 value)
            : base(left, op, new ValueExpression<UInt64>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Char value)
            : base(left, op, new ValueExpression<Char>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, Byte value)
            : base(left, op, new ValueExpression<Byte>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, SByte value)
            : base(left, op, new ValueExpression<SByte>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, DateTime value)
            : base(left, op, new ValueExpression<DateTime>(value)) { }

        public AttributeBinaryExpression(AttributeExpression left, BinaryOperator op, CollectionExpression right)
            : base(left, op, right) { }
    }
}
