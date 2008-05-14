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
    public abstract class BinaryExpressionBase<TOperator> : Expression
    {
        private readonly Expression _left;
        private readonly TOperator _op;
        private readonly Expression _right;

        protected BinaryExpressionBase(Expression left, TOperator op, Expression right)
        {
            _left = left;
            _op = op;
            _right = right;
        }

        protected Expression Left
        {
            get { return _left; }
        }

        public TOperator Op
        {
            get { return _op; }
        }

        protected Expression Right
        {
            get { return _right; }
        }

        public override Boolean Matches(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Boolean Equals(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            Expression leftClone = (Left == null ? null : Left.Clone());
            Expression rightClone = (Right == null ? null : Right.Clone());
            Expression clone = Create(leftClone, Op, rightClone);

            return clone;
        }

        protected abstract BinaryExpressionBase<TOperator> Create(Expression left, 
                                                                  TOperator op,
                                                                  Expression right);
    }
}