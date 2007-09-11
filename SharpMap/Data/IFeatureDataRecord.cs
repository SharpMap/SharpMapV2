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

using System.Data;
using SharpMap.Geometries;
using System;

namespace SharpMap.Data
{
	/// <summary>
	/// Encapsulates both the <see cref="Geometry"/> and
    /// attribute values for a feature.
	/// </summary>
	public interface IFeatureDataRecord : IDataRecord
	{
		/// <summary>
		/// Gets the geometry for the current position in the reader.
		/// </summary>
        Geometry Geometry { get; }

        /// <summary>
        /// Gets the Object ID for the record.
        /// </summary>
        /// <returns>
        /// The Object ID for the record, or <see langword="null"/> 
        /// if <see cref="HasOid"/> is <see langword="false"/>.
        /// </returns>
        object GetOid();

        /// <summary>
        /// Gets a value indicating if the feature record
        /// has an Object Identifier (OID).
        /// </summary>
        bool HasOid { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the Object ID.
        /// </summary>
        /// <remarks>
        /// OidType gets a <see cref="Type"/> which can be used
        /// to call GetOid with generic type parameters in order to avoid 
        /// boxing. If <see cref="HasOid"/> returns false, <see cref="OidType"/>
        /// returns <see langword="null"/>.
        /// </remarks>
        Type OidType { get; }
	}
}
