// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Data;
using SharpMap.Geometries;
using System.Globalization;

namespace SharpMap.Data
{
    /// <summary>
    /// Interface for feature data providers.
    /// </summary>
    public interface IFeatureLayerProvider : ILayerProvider
    {
        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are related to <paramref name="geometry"/> by <paramref name="queryType"/>.
        /// </summary>
        /// <param name="geometry">Geometry to query with.</param>
        /// <param name="queryType">Type of spatial query to execute.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteFeatureQuery(Geometry geometry, SpatialQueryType queryType);

        /// <summary>
        /// Retrieves the features related to <paramref name="geometry"/> by <paramref name="queryType"/>.
        /// </summary>
        /// <param name="geometry">Geometry to query with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        /// <param name="queryType">Type of spatial query to execute.</param>
        void ExecuteFeatureQuery(Geometry geometry, FeatureDataSet dataSet, SpatialQueryType queryType);

        /// <summary>
        /// Retrieves the features related to <paramref name="geometry"/> by <paramref name="queryType"/>.
        /// </summary>
        /// <param name="geometry">Geometry to query with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        /// <param name="queryType">Type of spatial query to execute.</param>
        void ExecuteFeatureQuery(Geometry geometry, FeatureDataTable table, SpatialQueryType queryType);

        /// <summary>
        /// Gets the geometries within the specified <see cref="SharpMap.Geometries.BoundingBox"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <returns>
        /// An enumeration of features within the specified <see cref="SharpMap.Geometries.BoundingBox"/>.
        /// </returns>
        IEnumerable<Geometry> ExecuteGeometryIntersectionQuery(BoundingBox bounds);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(BoundingBox bounds);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(BoundingBox bounds, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        void ExecuteIntersectionQuery(BoundingBox bounds, FeatureDataSet dataSet);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="dataSet">FeatureDataSet to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(BoundingBox bounds, FeatureDataSet dataSet, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        void ExecuteIntersectionQuery(BoundingBox bounds, FeatureDataTable table);

        /// <summary>
        /// Retrieves the data associated with all the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">BoundingBox to intersect with.</param>
        /// <param name="table">FeatureDataTable to fill data into.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        void ExecuteIntersectionQuery(BoundingBox bounds, FeatureDataTable table, QueryExecutionOptions options);

        /// <summary>
        /// Returns the number of features in the entire data source.
        /// </summary>
        /// <returns>Count of the features in the entire data source.</returns>
        int GetFeatureCount();

        /// <summary>
        /// Returns a <see cref="DataTable"/> with rows describing the columns in the schema
        /// for the configured provider. Provides the same result as 
        /// <see cref="IDataReader.GetSchemaTable"/>.
        /// </summary>
        /// <seealso cref="IDataReader.GetSchemaTable"/>
        /// <returns>A DataTable that describes the column metadata.</returns>
        DataTable GetSchemaTable();

        /// <summary>
        /// Gets the locale of the data as a CultureInfo.
        /// </summary>
        CultureInfo Locale { get; }

        /// <summary>
        /// Configures a <see cref="FeatureDataTable{TOid}"/> with the schema 
        /// present in the IProvider with the given connection.
        /// </summary>
        /// <param name="table">The FeatureDataTable to configure the schema of.</param>
        void SetTableSchema(FeatureDataTable table);
    }
}
