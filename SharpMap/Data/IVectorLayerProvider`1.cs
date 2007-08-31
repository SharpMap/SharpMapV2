using System.Collections.Generic;
using SharpMap.Features;
using SharpMap.Geometries;

namespace SharpMap.Data
{
	/// <summary>
	/// Interface for data providers with features having ID values.
	/// </summary>
    public interface IVectorLayerProvider<TOid> : IVectorLayerProvider
	{
		/// <summary>
		/// Returns all objects whose <see cref="SharpMap.Geometries.BoundingBox"/> 
        /// intersects <paramref name="boundingBox"/>.
		/// </summary>
		/// <remarks>
		/// This method is usually much faster than the ExecuteIntersectionQuery method, 
		/// because intersection tests are performed on objects simplifed by 
		/// their <see cref="SharpMap.Geometries.BoundingBox"/>, often using
		/// spatial indexing to retrieve the id values.
		/// </remarks>
		/// <param name="boundingBox">BoundingBox that objects should intersect.</param>
		/// <returns>An enumeration of all intersecting objects' ids.</returns>
		IEnumerable<TOid> GetObjectIdsInView(BoundingBox boundingBox);

		/// <summary>
		/// Returns the geometry corresponding to the object ID.
		/// </summary>
		/// <param name="oid">Object ID.</param>
		/// <returns>The geometry corresponding to the <paramref name="oid"/>.</returns>
		Geometry GetGeometryById(TOid oid);

		/// <summary>
        /// Returns a <see cref="FeatureDataRow"/> based on an object id (OID).
		/// </summary>
		/// <param name="oid">The object id (OID) of the feature.</param>
		/// <returns>The feature corresponding to the <paramref name="oid"/>.</returns>
		FeatureDataRow<TOid> GetFeature(TOid oid);

        /// <summary>
        /// Configures a <see cref="FeatureDataTable{TOid}"/> with the schema 
        /// present in the IProvider with the given connection.
        /// </summary>
        /// <param name="table">The FeatureDataTable to configure the schema of.</param>
        void SetTableSchema(FeatureDataTable<TOid> table);
	}
}
