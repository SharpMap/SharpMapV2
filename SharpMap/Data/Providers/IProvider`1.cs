using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers
{
	/// <summary>
	/// Interface for data providers with features having ID values.
	/// </summary>
	public interface IProvider<TOid> : IProvider
	{
		/// <summary>
		/// Returns all objects whose <see cref="SharpMap.Geometries.BoundingBox"/> intersects <paramref name="boundingBox"/>.
		/// </summary>
		/// <remarks>
		/// This method is usually much faster than the QueryFeatures method, 
		/// because intersection tests are performed on objects simplifed by 
		/// their <see cref="SharpMap.Geometries.BoundingBox"/>, often using
		/// spatial indexing to retrieve the id values.
		/// </remarks>
		/// <param name="boundingBox">BoundingBox that objects should intersect.</param>
		/// <returns>An enumeration of all intersecting objects' ids.</returns>
		IEnumerable<TOid> GetObjectIdsInView(BoundingBox boundingBox);

		/// <summary>
		/// Returns the geometry corresponding to the Object ID
		/// </summary>
		/// <param name="oid">Object ID.</param>
		/// <returns>The geometry corresponding to the <paramref name="oid"/>.</returns>
		Geometry GetGeometryById(TOid oid);

		/// <summary>
		/// Returns a <see cref="SharpMap.Data.FeatureDataRow"/> based on an OID.
		/// </summary>
		/// <param name="oid">The object id (OID) of the feature.</param>
		/// <returns>The feature corresponding to the <paramref name="oid"/>.</returns>
		FeatureDataRow<TOid> GetFeature(TOid oid);
	}
}
