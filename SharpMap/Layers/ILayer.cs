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
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Caching;
using SharpMap.Expressions;
using SharpMap.Styles;
using SharpMap.Symbology;

namespace SharpMap.Layers
{
    /// <summary>
    /// Interface for map layers.
    /// </summary>
    public interface ILayer : IHasDynamicProperties, IDisposable
	{
		/// <summary>
		/// Gets or sets a value indicating that data is obtained asynchronously.
		/// </summary>
		Boolean AsyncQuery { get; set; }

		/// <summary>
		/// Applies a coordinate transformation to the geometries in this layer.
		/// </summary>
		ICoordinateTransformation CoordinateTransformation { get; set; }

		/// <summary>
		/// Gets the data source used to create this layer.
		/// </summary>
		IProvider DataSource { get; }

		/// <summary>
		/// Gets or sets a value representing the visibility of the layer.
		/// </summary>
		/// <remarks>
		/// Should be the same value as <see cref="Style"/>'s 
		/// <see cref="IStyle.Enabled"/> value.
		/// </remarks>
		Boolean Enabled { get; set; }

		/// <summary>
		/// Gets the boundingbox of the entire layer.
		/// </summary>
		IExtents Extents { get; }

		/// <summary>
		/// Name of layer.
		/// </summary>
        String LayerName { get; set; }

        /// <summary>
        /// The dataum, projection and coordinate system used for this layer.
        /// </summary>
        ICoordinateSystem SpatialReference { get; }

		/// <summary>
		/// The spatial reference ID of the layer data source.
		/// </summary>
		String Srid { get; }

		/// <summary>
		/// The style for the layer.
		/// </summary>
		Style Style { get; set; }

        /// <summary>
        /// Gets an <see cref="IGeometry"/> instance describing the region which has been
        /// loaded from the layer <see cref="DataSource"/>.
        /// </summary>
        IGeometry LoadedRegion { get; }

        /// <summary>
        /// Gets a value indicating if the layer is loading data from its <see cref="DataSource"/>.
        /// </summary>
        Boolean IsLoadingData { get; }

        /// <summary>
        /// Computes the visibility of the layer based on a given <paramref name="condition"/>.
        /// </summary>
        /// <param name="condition">The predicate to use to compute layer visibility.</param>
        /// <returns>
        /// <see langword="true"/> if the layer is visible given <paramref name="condition"/>.
        /// </returns>
        Boolean IsVisibleWhen(Predicate<ILayer> condition);

        /// <summary>
        /// Event fired when layer data is loaded.
        /// </summary>
        event EventHandler<LayerDataLoadedEventArgs> DataLoaded;

        /// <summary>
        /// Loads the data contained in the layer where it intersects <paramref name="region"/>.
        /// </summary>
        /// <param name="region">The region to load the data in.</param>
        void LoadIntersectingLayerData(IExtents region);

        /// <summary>
        /// Loads the data contained in the layer where it intersects <paramref name="region"/>.
        /// </summary>
        /// <param name="region">The region to load the data in.</param>
        void LoadIntersectingLayerData(IGeometry region);

        /// <summary>
        /// Loads the data contained in the layer where satisfies
        /// the <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The expression to match layer data to.
        /// </param>
        /// <remarks>
        /// Loads the data either synchronously or asynchronously depending
        /// on <see cref="AsyncQuery"/>.
        /// </remarks>
        void LoadLayerData(QueryExpression query);

        /// <summary>
        /// Loads the data contained in the layer where satisfies
        /// the <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The expression to match layer data to.
        /// </param>
        /// <remarks>
        /// Loads the data asynchronously.
        /// </remarks>
        void LoadLayerDataAsync(QueryExpression query);

        /// <summary>
        /// Evaluates the given <paramref name="query"/> and retrieves data in the 
        /// layer which satisfies it.
        /// </summary>
        /// <param name="query">The query to evaluate against the layer.</param>
        /// <returns>
        /// Any data which the layer contains which satisfies the <paramref name="query"/>.
        /// </returns>
        IEnumerable Select(Expression query);

        /// <summary>
        /// Gets or sets an <see cref="IQueryCache"/> instance for the layer.
        /// </summary>
        /// <remarks>
        /// The <see cref="IQueryCache"/> is used to cache data on a layer-by-layer basis.
        /// </remarks>
        IQueryCache QueryCache { get; set; }
	}
}
