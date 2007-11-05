// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace NPack.Interfaces
{
    /// <summary>
    /// This interface will be moved into the linear algebra library.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TVertexComponent"></typeparam>
    public interface IVertexStream<TVertex, TVertexComponent>
        where TVertexComponent : struct, IEquatable<TVertexComponent>, IComparable<TVertexComponent>,
            IComputable<TVertexComponent>, IConvertible
        where TVertex : IVector<TVertexComponent>
    {
        /// <summary>
        /// Gets an enumeration of the vertices in the object.
        /// </summary>
        /// <returns>An enumeration of vertex data contained by the object.</returns>
        IEnumerable<TVertex> GetVertices();
    }
}