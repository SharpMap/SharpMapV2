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
using System.ComponentModel;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// Interface for map layers.
    /// </summary>
	public interface ILayer : INotifyPropertyChanged, IDisposable
	{
		/// <summary>
		/// Gets or sets a value indicating that data is obtained asynchronously.
		/// </summary>
		Boolean AsyncQuery { get; set; }

		/// <summary>
		/// The dataum, projection and coordinate system used for this layer.
		/// </summary>
		ICoordinateSystem CoordinateSystem { get; }

		/// <summary>
		/// Applies a coordinate transformation to the geometries in this layer.
		/// </summary>
		ICoordinateTransformation CoordinateTransformation { get; set; }

		/// <summary>
		/// Gets the data source used to create this layer.
		/// </summary>
		ILayerProvider DataSource { get; }

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
		/// The spatial reference ID of the layer data source.
		/// </summary>
		Int32? Srid { get; }

		/// <summary>
		/// The style for the layer.
		/// </summary>
		IStyle Style { get; set; }

		Boolean ShowChildren { get; set; }

		Boolean AreFeaturesSelectable { get; set; }

		Boolean IsVisibleWhen(Predicate<ILayer> condition);
	}
}