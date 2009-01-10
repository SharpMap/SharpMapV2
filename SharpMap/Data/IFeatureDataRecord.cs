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
using System.Data;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;

namespace SharpMap.Data
{
	/// <summary>
	/// Encapsulates both the <see cref="Geometry"/> and
    /// attribute values for a feature.
	/// </summary>
	public interface IFeatureDataRecord : IDataRecord, IEvaluable
	{
		/// <summary>
		/// Gets the geometry for the feature.
		/// </summary>
        IGeometry Geometry { get; }

        /// <summary>
        /// Gets the extents for the feature.
        /// </summary>
        IExtents Extents { get; }

        /// <summary>
        /// Gets the object ID for the feature.
        /// </summary>
        /// <returns>
        /// The object ID for the record, or <see langword="null"/> 
        /// if <see cref="HasOid"/> is <see langword="false"/>.
        /// </returns>
        Object GetOid();

	    /// <summary>
        /// Gets a value indicating if the feature record
        /// has an object Identifier (OID).
        /// </summary>
        Boolean HasOid { get; }

	    /// <summary>
	    /// Gets a value indicating whether this feature record
	    /// has been fully loaded from the data source.
	    /// </summary>
        // TODO: Reevaluate the IsFullyLoaded flag, since consecutive loads may 
        // eventually fully load a record, yet this won't be able to record it.
	    Boolean IsFullyLoaded { get; }

	    /// <summary>
        /// Gets the <see cref="Type"/> of the object ID.
        /// </summary>
        /// <remarks>
        /// OidType gets a <see cref="Type"/> which can be used
        /// to call GetOid with generic type parameters in order to avoid 
        /// boxing. If <see cref="HasOid"/> returns false, <see cref="OidType"/>
        /// returns <see langword="null"/>.
        /// </remarks>
        Type OidType { get; }

        /// <summary>
        /// Gets or sets an <see cref="ICoordinateTransformation"/> to apply to the 
        /// <see cref="Geometry"/> property of this feature.
        /// </summary>
        ICoordinateTransformation CoordinateTransformation { get; set; }

        /// <summary>
        /// Gets the <see cref="IFeatureAttribute"/> of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The <see cref="IFeatureAttribute"/> of the specified field.</returns>
	    IFeatureAttribute GetAttribute(Int32 i);

        DataRowState FeatureState { get; }
        DataRowVersion FeatureVersion { get; }
	}
}
