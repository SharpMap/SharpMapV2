// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
// Portions copyright 2008: Ron Emmert (justsome.handle@gmail.com)
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
using System.Collections;
using System.Collections.Generic;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    /// <summary>
    /// Represents a set of methods and properties used to fill and refresh a FeatureDataTable
    /// </summary>
    /// <remarks>
    /// Currently a place to dump all the extraneous FeatureDataTable and FeatureDataSet related 
    /// methods from IFeatureLayerProvider, but we have better uses planned for it.
    /// </remarks>
    public interface IFeatureAdapter
    {
        /// <summary>
        /// Begins to retrieve the features which match the specified 
        /// <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Feature spatial query to execute.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteFeatureQuery(FeatureSpatialExpression query,
            FeatureDataSet dataSet, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the features which match the specified 
        /// <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Feature spatial query to execute.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteFeatureQuery(FeatureSpatialExpression query,
            FeatureDataTable table, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds,
            FeatureDataSet dataSet, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet,
            QueryExecutionOptions options, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table,
            AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Begins to retrieve the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <param name="callback">
        /// <see cref="AsyncCallback"/> delegate to invoke when the operation completes.
        /// </param>
        /// <param name="asyncState">
        /// Custom state to pass to the <paramref name="callback"/>.
        /// </param>
        IAsyncResult BeginExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table,
            QueryExecutionOptions options, AsyncCallback callback, Object asyncState);

        IAsyncResult BeginGetFeatures(IEnumerable oids, AsyncCallback callback, Object asyncState);

        /// <summary>
        /// Ends a retrieval of the features, waiting on the 
        /// <see cref="IAsyncResult.AsyncWaitHandle"/> if the operation is not complete.
        /// </summary>
        void EndExecuteFeatureQuery(IAsyncResult asyncResult);

        IEnumerable<IFeatureDataRecord> EndGetFeatures(IAsyncResult asyncResult);
        /// <summary>
        /// Retrieves features into a <see cref="FeatureDataSet"/> that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataSet dataSet);

        /// <summary>
        /// Retrieves features into a <see cref="FeatureDataTable"/> that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        void ExecuteFeatureQuery(FeatureSpatialExpression query, FeatureDataTable table);

        /// <summary>
        /// Gets the geometries within the specified 
        /// <see cref="GeoAPI.Geometries.IExtents"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <returns>
        /// An enumeration of features within the specified 
        /// <see cref="GeoAPI.Geometries.IExtents"/>.
        /// </returns>
        IEnumerable<IGeometry> ExecuteGeometryIntersectionQuery(IExtents bounds);

        /// <summary>
        /// Gets the geometries within the specified 
        /// <see cref="GeoAPI.Geometries.IGeometry"/>.
        /// </summary>
        /// <param name="geometry">
        /// <see cref="IGeometry"/> to intersect with.
        /// </param>
        /// <returns>
        /// An enumeration of features within the specified 
        /// <see cref="GeoAPI.Geometries.IExtents"/>.
        /// </returns>
        IEnumerable<IGeometry> ExecuteGeometryIntersectionQuery(IGeometry geometry);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        void ExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataSet dataSet);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(IExtents bounds, FeatureDataSet dataSet, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="dataSet"><see cref="FeatureDataSet"/> to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataSet dataSet, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataTable table);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(IExtents bounds, FeatureDataTable table, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="table"><see cref="FeatureDataTable"/> to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(IGeometry geometry, FeatureDataTable table, QueryExecutionOptions options);

        /// <summary>
        /// Returns a <see cref="IFeatureDataReader"/> for obtaining features
        /// from a set of feature object identifiers (oids).
        /// </summary>
        /// <param name="oids">A set of object ids (OIDs) of the features.</param>
        /// <returns>
        /// A set of features corresponding one-to-one to the given <paramref name="oids"/>.
        /// </returns>
        IEnumerable<IFeatureDataRecord> GetFeatures(IEnumerable oids);
    }
}
