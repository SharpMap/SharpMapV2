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
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Spatial query to execute.</param>
        /// <returns>An IFeatureDataReader to iterate over the results.</returns>
        IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query);

        /// <summary>
        /// Retrieves a <see cref="IFeatureDataReader"/> for the features that 
        /// match the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query used to retrieve features.</param>
        /// <param name="options">Options indicating which data to retrieve.</param>
        IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query, FeatureQueryExecutionOptions options);

        /// <summary>
        /// Gets or sets the <see cref="IGeometryFactory"/> used
        /// to construct <see cref="IGeometry"/> instances.
        /// </summary>
        IGeometryFactory GeometryFactory { get; set; }

        /// <summary>
        /// Returns the number of features in the entire data source.
        /// </summary>
        /// <returns>Count of the features in the entire data source.</returns>
        Int32 GetFeatureCount();

        IFeaturesSchema GetSchema();

        /// <summary>
        /// Gets the locale of the data as a <see cref="CultureInfo"/>.
        /// </summary>
        CultureInfo Locale { get; }

        /// <summary>
        /// Configures a <see cref="IFeaturesCache"/> with the schema 
        /// present in the <see cref="IProvider"/> with the given connection.
        /// </summary>
        /// <param name="cache">The <see cref="IFeaturesCache"/> to configure the schema of.</param>
        void SetCacheSchema(IFeaturesCache cache);

        /// <summary>
        /// Configures a <see cref="IFeaturesCache"/> with the schema 
        /// present in the <see cref="IProvider"/> with the given connection.
        /// </summary>
        /// <param name="cache">The <see cref="IFeaturesCache"/> to configure the schema of.</param>
        /// <param name="schemaAction">Indicates how to merge schema information.</param>
        void SetCacheSchema(IFeaturesCache cache, SchemaMergeAction schemaAction);
    }
}
