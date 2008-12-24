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
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    /// <summary>
    /// Represents an expression used to select, project, transform, render or style geographical data.
    /// </summary>
    [XmlInclude(typeof(LiteralExpression))]
    [XmlInclude(typeof(FunctionExpression))]
    [XmlInclude(typeof(PropertyNameExpression))]
    [XmlInclude(typeof(BinaryOperationExpression))]
    [XmlInclude(typeof(Expression))]
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "ExpressionType")]
    public abstract class Expression : IEquatable<Expression>
    {
        /// <summary>
        /// Determines if <see cref="Expression"/> <paramref name="a"/> contains
        /// <see cref="Expression"/> <paramref name="b"/>. "Contains" means that
        /// the two expressions produce the same result given a set of input. Containment
        /// in general is not transitive, so <c>a.Contains(b)</c> is not the same
        /// as <c>b.Contains(a)</c>.
        /// </summary>
        /// <param name="a">
        /// The <see cref="Expression"/> to determine containment of <paramref name="b"/>.
        /// </param>
        /// <param name="b">
        /// The <see cref="Expression"/> to test containment of by <paramref name="a"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <c>a.Contains(b)</c>; <see langword="false"/> otherwise.
        /// </returns>
        public static Boolean Contains(Expression a, Expression b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Contains(b);
        }

        /// <summary>
        /// Determines if the given <see cref="Expression"/> contains
        /// this expression. "Contains" means that
        /// the expression will provide at least the same result set when applied to a given 
        /// input set, and perhaps more. Containment in general is not transitive, 
        /// so <c>this.Contains(other)</c> is not the same as <c>other.Contains(this)</c>, 
        /// unless the expressions are equivalant.
        /// </summary>
        public abstract Boolean Contains(Expression other);

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