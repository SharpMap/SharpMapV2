// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Data;
using GeoAPI.Geometries;
using System.Globalization;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    /// <summary>
    /// Interface for feature data providers.
    /// </summary>
    public interface IFeatureProvider : IProvider
    {

        /// <summary>
        /// Creates a new <see cref="FeatureDataTable"/> from the data source's 
        /// schema.
        /// </summary>
        /// <returns>
        /// A <see cref="FeatureDataTable"/> which is configured for the 
        /// data source's schema.
        /// </returns>
        FeatureDataTable CreateNewTable();


        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteFeatureQuery(FeatureSpatialExpression query);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(IExtents bounds);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <returns>An <see cref="IFeatureDataReader"/> to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(IGeometry geometry);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"><see cref="IExtents" /> to intersect with.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(IExtents bounds, QueryExecutionOptions options);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// are intersected by <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="IGeometry"/> to intersect with.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        /// <returns>An <see cref="IFeatureDataReader"/> to iterate over the results.</returns>
        IFeatureDataReader ExecuteIntersectionQuery(IGeometry geometry, QueryExecutionOptions options);

        IGeometryFactory GeometryFactory { get; set; }

        /// <summary>
        /// Returns the number of features in the entire data source.
        /// </summary>
        /// <returns>Count of the features in the entire data source.</returns>
        Int32 GetFeatureCount();

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
