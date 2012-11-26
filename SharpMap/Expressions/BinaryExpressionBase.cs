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
    /// Abstract base class for binary expressions
    /// </summary>
    /// <typeparam name="TOperator">The type of the operator</typeparam>
    [Serializable]
    public abstract class BinaryExpressionBase<TOperator> : PredicateExpression
    {
        private readonly Expression _left;
        private readonly TOperator _op;
        private readonly Expression _right;

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="left">The left-hand-side of the expression</param>
        /// <param name="op">The operator</param>
        /// <param name="right">The right-hand-side of the expression</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the argumens are <value>null</value>. This only applies to <paramref name="op"/> if <typeparamref name="TOperator"/> is a class.</exception>
        protected BinaryExpressionBase(Expression left, TOperator op, Expression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            if (typeof (TOperator).IsClass && op==null)
                throw new ArgumentNullException("op");

            _left = left;
            _op = op;
            _right = right;
        }

        /// <summary>
        /// Gets the left-hand side of the binary espression
        /// </summary>
        protected Expression Left
        {
            get { return _left; }
        }

        /// <summary>
        /// Gets the operator used to combine <see cref="Left"/> and <see cref="Right"/> espressions
        /// </summary>
        public TOperator Op
        {
            get { return _op; }
        }

        /// <summary>
        /// Gets the right-hand side of the binary espression
        /// </summary>
        protected Expression Right
        {
            get { return _right; }
        }


        /// <summary>
        /// Determines if the given <see cref="Expression"/> contains
        /// this expression. "Contains" means that
        /// the expression will provide at least the same result set when applied to a given 
        /// input set, and perhaps more. Containment in general is not transitive, 
        /// so <c>this.Contains(other)</c> is not the same as <c>other.Contains(this)</c>, 
        /// unless the expressions are equivalant.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when called.</exception>
        public override Boolean Contains(Expression other)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override Boolean Equals(Expression other)
        {
            var beOther = other as BinaryExpressionBase<TOperator>;
            if (beOther == null) return false;

            if (!_left.Equals(beOther._left))
                return false;
            if (_op.Equals(beOther._op))
                return false;
            if (!_right.Equals(beOther._right))
                return false;
            return true;
        }

        public override Expression Clone()
        {
            var leftClone = (Left == null ? null : Left.Clone());
            var rightClone = (Right == null ? null : Right.Clone());
            var clone = Create(leftClone, Op, rightClone);

            return clone;
        }

        /// <summary>
        /// Factory method to create a binary expression of a specific type.
        /// </summary>
        /// <param name="left">The left-hand-side of the expression</param>
        /// <param name="op">The operator</param>
        /// <param name="right">The right-hand-side of the expression</param>
        /// <returns>The created expression</returns>
        protected abstract BinaryExpressionBase<TOperator> Create(Expression left, 
                                                                  TOperator op,
                                                                  Expression right);
    }
}