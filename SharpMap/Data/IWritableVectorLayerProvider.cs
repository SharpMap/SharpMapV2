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

using System.Collections.Generic;

namespace SharpMap.Data
{
    /// <summary>
    /// Provides an interface for writable data providers.
    /// </summary>
    /// <typeparam name="TOid">
    /// Type of the value used for feature object identifiers.
    /// </typeparam>
    public interface IWritableFeatureLayerProvider<TOid> : IFeatureLayerProvider<TOid>
    {
        /// <summary>
        /// Inserts a feature into the data source.
        /// </summary>
        /// <param name="feature">The feature to insert.</param>
        void Insert(FeatureDataRow<TOid> feature);

        /// <summary>
        /// Inserts a set of features into the data source.
        /// </summary>
        /// <param name="features">The features to insert.</param>
        void Insert(IEnumerable<FeatureDataRow<TOid>> features);

        /// <summary>
        /// Updates an existing feature in the data source.
        /// </summary>
        /// <param name="feature">The feature to update.</param>
        void Update(FeatureDataRow<TOid> feature);

        /// <summary>
        /// Updates a set of existing features in the data source.
        /// </summary>
        /// <param name="features">The features to update.</param>
        void Update(IEnumerable<FeatureDataRow<TOid>> features);

        /// <summary>
        /// Deletes a feature from the data source.
        /// </summary>
        /// <param name="feature">The feature to delete.</param>
        void Delete(FeatureDataRow<TOid> feature);

        /// <summary>
        /// Deletes a set of features from the data source.
        /// </summary>
        /// <param name="features">The features to delete.</param>
        void Delete(IEnumerable<FeatureDataRow<TOid>> features);
    }
}