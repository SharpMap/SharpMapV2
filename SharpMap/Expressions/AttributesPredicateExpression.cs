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
    public class AttributesPredicateExpression : BinaryLogicExpression
    {
        #region Overrides of Expression

        public AttributesPredicateExpression(AttributeBinaryExpression left,
                                             BinaryLogicOperator op,
                                             AttributeBinaryExpression right)
            : base(left, op, right) {}

        public override Boolean Contains(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            return new AttributesPredicateExpression(Left.Clone() as AttributeBinaryExpression,
                                                     Op,
                                                     Right.Clone() as AttributeBinaryExpression);
        }

        protected override BinaryLogicExpressionBase<BinaryLogicOperator> Create(Expression left,
                                                                            BinaryLogicOperator op,
                                                                            Expression right)
        {
            if (!(left is AttributeBinaryExpression && right is AttributeBinaryExpression))
            {
                throw new ArgumentException("Both parameter 'left' and parameter 'right' " +
                                            "must be a AttributeBinaryExpression instance.");
            }

            return new AttributesPredicateExpression(left as AttributeBinaryExpression,
                                                     op,
                                                     right as AttributeBinaryExpression);
        }

        public override Boolean Equals(Expression other)
        {
            AttributesPredicateExpression predicate = other as AttributesPredicateExpression;

            if (predicate == null)
            {
                return false;
            }

            return predicate.Left.Equals(Left) &&
                   predicate.Op == Op &&
                   predicate.Right.Equals(Right);
        }

        #endregion
    }
}