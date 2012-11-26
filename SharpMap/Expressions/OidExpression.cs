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
    /// Expression for object identifiers
    /// </summary>
    [Serializable]
    public class OidExpression : Expression, IEquatable<OidExpression>
    {
        /// <summary>
        /// Determines if the given <see cref="Expression"/> contains
        /// this expression. "Contains" means that
        /// the expression will provide at least the same result set when applied to a given 
        /// input set, and perhaps more. Containment in general is not transitive, 
        /// so <c>this.Contains(other)</c> is not the same as <c>other.Contains(this)</c>, 
        /// unless the expressions are equivalant.
        /// </summary>
        public override Boolean Contains(Expression other)
        {
            return Equals(other);
        }

        public override Expression Clone()
        {
            return new OidExpression();
        }

        public override Boolean Equals(Expression other)
        {
            return other is OidExpression;
        }

        #region IEquatable<OidExpression> Members

        public Boolean Equals(OidExpression other)
        {
            return other != null;
        }

        #endregion

        public override Boolean Equals(Object obj)
        {
            return Equals(obj as OidExpression);
        }

        public override Int32 GetHashCode()
        {
            return 7;
        }
    }
}
