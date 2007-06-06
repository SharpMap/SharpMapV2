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

using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;
using SharpMap.Rendering;
using SharpMap.Styles;

namespace SharpMap.Layers
{
	/// <summary>
	/// Interface for map layers.
	/// </summary>
	public interface ILayer
    {
		/// <summary>
		/// Name of layer.
		/// </summary>
		string LayerName { get; set; }

		/// <summary>
		/// Gets the boundingbox of the entire layer.
		/// </summary>
		BoundingBox Envelope { get; }
	
		/// <summary>
		/// The spatial reference ID (CRS).
		/// </summary>
        int Srid { get; set; }

        /// <summary>
        /// Gets or sets a value representing the visibility of the layer.
        /// </summary>
        /// <remarks>
        /// Should be the same value as <see cref="Style.Enabled"/>.
        /// </remarks>
        bool Enabled { get; set; }

        /// <summary>
        /// The style for the layer.
        /// </summary>
        IStyle Style { get; set; }
	}
}
