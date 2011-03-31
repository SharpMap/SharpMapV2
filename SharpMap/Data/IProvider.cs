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
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    /// <summary>
    /// Interface for layer data providers.
    /// </summary>
    public interface IProvider : IHasDynamicProperties, IDisposable
    {
        /// <summary>
        /// Applies a coordinate transformation to the geometries in 
        /// this provider.
        /// </summary>
        ICoordinateTransformation CoordinateTransformation { get; set; }

        /// <summary>
        /// Applies an inverse coordinate transformation to the geometries in 
        /// this provider.
        /// </summary>
        ICoordinateTransformation InverseCoordinateTransformation { get; set; }

        /// <summary>
        /// The dataum, projection and coordinate system which the data appears in
        /// when queried.
        /// </summary>
        ICoordinateSystem SpatialReference { get; }

        /// <summary>
        /// The dataum, projection and coordinate system which the underlying data 
        /// is stored as. This may be different from <see cref="SpatialReference"/> 
        /// if <see cref="CoordinateTransformation"/> is set.
        /// </summary>
        ICoordinateSystem OriginalSpatialReference { get; }

        /// <summary>
        /// Returns true if the datasource is currently open.
        /// </summary>
        Boolean IsOpen { get; }

        /// <summary>
        /// Gets the spatial reference ID for the provider's <see cref="SpatialReference"/>,
        /// if one exists; <see langword="null"/> otherwise.
        /// </summary>
        String Srid { get; }

        /// <summary>
        /// Gets the spatial reference ID for the underlying data's 
        /// <see cref="OriginalSpatialReference"/>, if one exists; 
        /// <see langword="null"/> otherwise.
        /// </summary>
        String OriginalSrid { get; }

        /// <summary>
        /// Geometric extent of the entire dataset.
        /// </summary>
        /// <returns>The extents of the dataset as a BoundingBox.</returns>
        IExtents GetExtents();

        /// <summary>
        /// Gets the connection ID of the datasource.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The ConnectionId should be unique to the datasource 
        /// (for instance the filename or the connectionstring), and is meant 
        /// to be used for connection pooling.
        /// </para>
        /// <para>
        /// If connection pooling doesn't apply to this datasource, 
        /// the ConnectionId should return String.Empty.
        /// </para>
        /// </remarks>
        String ConnectionId { get; }

        /// <summary>
        /// Opens the datasource.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the datasource.
        /// </summary>
        void Close();

        Object ExecuteQuery(Expression query);
    }
}
