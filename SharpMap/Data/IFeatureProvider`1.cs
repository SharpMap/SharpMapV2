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
using SharpMap.Expressions;

namespace SharpMap.Data
{
	/// <summary>
	/// Interface for data providers with features having ID values.
	/// </summary>
    public interface IFeatureProvider<TOid> : IFeatureProvider
	{
        /// <summary>
        /// Returns all object ids which match the given query.
        /// </summary>
        /// <param name="query">An expression which will match features in the data source.</param>
        /// <returns>An enumeration of all matching features' object ids.</returns>
	    IEnumerable<TOid> ExecuteOidQuery(SpatialBinaryExpression query);

	    IExtents GetExtentsByOid(TOid oid);

		/// <summary>
		/// Returns the geometry corresponding to the object ID.
		/// </summary>
		/// <param name="oid">Object ID.</param>
		/// <returns>The geometry corresponding to the <paramref name="oid"/>.</returns>
		IGeometry GetGeometryByOid(TOid oid);

        /// <summary>
        /// Gets a feature row from the data source with the specified id.
        /// </summary>
        /// <param name="oid">Id of the feautre to return.</param>
        /// <returns>
        /// The feature corresponding to <paramref name="oid" />, or null if no feature is found.
        /// </returns>
        IFeatureDataRecord GetFeatureByOid(TOid oid);

        ///// <summary>
        ///// Returns a <see cref="IFeatureDataReader"/> for obtaining features
        ///// from a set of feature object identifiers (oids).
        ///// </summary>
        ///// <param name="oids">A set of object ids (OIDs) of the features.</param>
        ///// <returns>
        ///// A set of features corresponding one-to-one to the given <paramref name="oids"/>.
        ///// </returns>
        //IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable<TOid> oids);

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
