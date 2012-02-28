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
    /// <summary>
    /// An abstract expression which represents a binary logical combination that can be used in a
    /// compound expression or an expression tree.
    /// </summary>
    /// <typeparam name="TOperator">The type of the operator</typeparam>
    public class CollectionBinaryExpression : BinaryExpressionBase<CollectionOperator>
    {
        public CollectionBinaryExpression(Expression left, CollectionOperator op, CollectionExpression right)
            : base(left, op, right) { }

        public new Expression Left
        {
            get { return base.Left; }
        }

        public new CollectionExpression Right
        {
            get { return base.Right as CollectionExpression; }
        }

        protected override BinaryExpressionBase<CollectionOperator> Create(Expression left,
                                                                           CollectionOperator op,
                                                                           Expression right)
        {
            if (right == null) throw new ArgumentNullException("right");

            CollectionExpression ce = right as CollectionExpression;

            if (ce == null)
            {
                throw new ArgumentException("Parameter must be a CollectionExpression instance.",
                                            "right");
            }

            return new CollectionBinaryExpression(left, op, ce);
        }
    }
}