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

using System.Collections.Generic;
using GeoAPI.Geometries;

namespace SharpMap.Data
{
	/// <summary>
	/// Interface for data providers with features having ID values.
	/// </summary>
    public interface IFeatureProvider<TOid> : IFeatureProvider
	{
        /// <summary>
        /// Returns all objects whose <see cref="IExtents"/> 
        /// intersects <paramref name="extents"/>.
        /// </summary>
        /// <remarks>
        /// This method is usually much faster than the ExecuteIntersectionQuery method, 
        /// because intersection tests are performed on objects simplifed by 
        /// their <see cref="IExtents"/>, often using
        /// spatial indexing to retrieve the id values.
        /// </remarks>
        /// <param name="extents">BoundingBox that objects should intersect.</param>
        /// <returns>An enumeration of all intersecting objects' ids.</returns>
	    IEnumerable<TOid> GetIntersectingObjectIds(IExtents extents);

		/// <summary>
		/// Returns the geometry corresponding to the object ID.
		/// </summary>
		/// <param name="oid">Object ID.</param>
		/// <returns>The geometry corresponding to the <paramref name="oid"/>.</returns>
		IGeometry GetGeometryById(TOid oid);

		/// <summary>
        /// Returns a <see cref="FeatureDataRow"/> based on an object id (OID).
		/// </summary>
		/// <param name="oid">The object id (OID) of the feature.</param>
		/// <returns>The feature corresponding to the <paramref name="oid"/>.</returns>
        IFeatureDataRecord GetFeature(TOid oid);

        /// <summary>
        /// Returns a <see cref="IFeatureDataReader"/> for obtaining features
        /// from a set of feature object identifiers (oids).
        /// </summary>
        /// <param name="oids">A set of object ids (OIDs) of the features.</param>
        /// <returns>
        /// A set of features corresponding one-to-one to the given <paramref name="oids"/>.
        /// </returns>
        IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable<TOid> oids);

        /// <summary>
        /// Configures a <see cref="FeatureDataTable{TOid}"/> with the schema 
        /// present in the IProvider with the given connection.
        /// </summary>
        /// <param name="table">The FeatureDataTable to configure the schema of.</param>
        void SetTableSchema(FeatureDataTable<TOid> table);

		/// <summary>
		/// Configures a <see cref="FeatureDataTable{TOid}"/> with the schema 
		/// present in the IProvider with the given connection.
		/// </summary>
		/// <param name="table">The FeatureDataTable to configure the schema of.</param>
		/// <param name="schemaAction">Indicates how to merge schema information.</param>
		void SetTableSchema(FeatureDataTable<TOid> table, SchemaMergeAction schemaAction);
	}
}
