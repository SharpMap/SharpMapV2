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

namespace SharpMap.Data
{
    /// <summary>
    /// Provides an interface for writable data providers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// NOTICE: The writeable interface is not stable yet.
    /// </para>
    /// <para>
    /// The current definition suffices for short term simple use cases, but
    /// leaves many details to the discretion of the provider.  Things that
    /// may be addressed in the future include but are not limited to:
    /// <list type="bullet">
    /// <item>some level of transaction control</item>
    /// <item>replace FeatureDataRow{TOid} argument types with IFeatureDataRecord</item>
    /// <item>add Delete overloads to accept a single OID, or enumeration of OIDs</item>
    /// <item>notification of provider capabilities</item>
    /// </list>
    /// </para>
    /// <para>
    /// The current philosophy of the provider interface is to provide  
    /// primitives operations for updating the data source.  Optimization
    /// of the process is left to the calling code based on the the current 
    /// use case.  If the provider is told to update a feature with a 
    /// particular OID, it may assume the feature exists and update it.
    /// <para>
    /// </para>
    /// Some providers may accept an Oid pre-populated in the Insert methods
    /// while others will always assign an Oid of their own choosing to the inserted
    /// features.  If an Oid is supplied by the provider, and with the current
    /// FeatureDataRow{TOid} implementation, it is good form to update the Oid 
    /// in the FeatureDataRow with the value actually used.
    /// </para>
    /// <para>
    /// Transaction control is currently left up to the provider, but
    /// obviously it is a good idea to leave the datasource in a consistent 
    /// and predictable state based on the success or failure of the operation.
    /// </para>
    /// </remarks>
    public interface IWritableFeatureProvider : IFeatureProvider
    {
        /// <summary>
        /// Inserts a feature into the data source.
        /// </summary>
        /// <param name="feature">The feature to insert.</param>
        void Insert(IFeatureDataRecord feature);

        /// <summary>
        /// Inserts a set of features into the data source.
        /// </summary>
        /// <param name="features">The features to insert.</param>
        void Insert(IEnumerable<IFeatureDataRecord> features);

        /// <summary>
        /// Updates an existing feature in the data source.
        /// </summary>
        /// <param name="feature">The feature to update.</param>
        void Update(IFeatureDataRecord feature);

        /// <summary>
        /// Updates a set of existing features in the data source.
        /// </summary>
        /// <param name="features">The features to update.</param>
        void Update(IEnumerable<IFeatureDataRecord> features);

        /// <summary>
        /// Deletes a feature from the data source.
        /// </summary>
        /// <param name="feature">The feature to delete.</param>
        void Delete(IFeatureDataRecord feature);

        /// <summary>
        /// Deletes a set of features from the data source.
        /// </summary>
        /// <param name="features">The features to delete.</param>
        void Delete(IEnumerable<IFeatureDataRecord> features);
    }
}