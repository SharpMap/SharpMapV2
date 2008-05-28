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
    /// Represents an expression used to select or project data.
    /// </summary>
    public abstract class Expression : IEquatable<Expression>
    {
        /// <summary>
        /// Determines if <see cref="Expression"/> <paramref name="a"/> matches
        /// <see cref="Expression"/> <paramref name="b"/>. "Matches" means that
        /// the two expressions produce the same result given a set of input. Matching
        /// in general is not transitive, so <c>a.Matches(b)</c> is not the same
        /// as <c>b.Matches(a)</c>.
        /// </summary>
        /// <param name="a">
        /// The <see cref="Expression"/> to determine results matching to <paramref name="b"/>.
        /// </param>
        /// <param name="b">The <see cref="Expression"/> to match.</param>
        /// <returns>
        /// <see langword="true"/> if <c>a.Matches(b)</c>; <see langword="false"/> otherwise.
        /// </returns>
        public static Boolean Matches(Expression a, Expression b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Matches(b);
        }

        /// <summary>
        /// Determines if the given <see cref="Expression"/> matches
        /// this expression. "Matches" means that
        /// the two expressions produce the same result given a set of input. Matching
        /// in general is not transitive, so <c>this.Matches(other)</c> is not the same
        /// as <c>other.Matches(this)</c>, unless the expressions are equivalant.
        /// </summary>
        public abstract Boolean Matches(Expression other);

        /// <summary>
        /// Makes a duplicate of the expression.
        /// </summary>
        /// <returns>An expression with the same structure and type as this one.</returns>
        public abstract Expression Clone();

        #region IEquatable<Expression> Members

        public abstract Boolean Equals(Expression other);
        #endregion
    }
}