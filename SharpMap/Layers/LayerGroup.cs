// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
	/// Class for holding a group of layers.
	/// </summary>
	/// <remarks>
	/// The Group layer is useful for grouping a set of layers,
	/// for instance a set of image tiles, and expose them as a single layer
	/// </remarks>
	public class LayerGroup : Layer, IDisposable
	{
        private List<Layer> _layers;

		/// <summary>
		/// Initializes a new group layer
		/// </summary>
		/// <param name="layername">Name of layer</param>
		public LayerGroup(string layername)
		{
			this.LayerName = layername;
			_layers = new List<Layer>();
		}

		/// <summary>
		/// Sublayers in the group
		/// </summary>
		public List<Layer> Layers
		{
			get { return _layers; }
			set { _layers = value; }
		}

		/// <summary>
		/// Returns a layer by its name
		/// </summary>
		/// <param name="name">Name of layer</param>
		/// <returns>Layer</returns>
		public Layer GetLayerByName(string name)
		{
            return _layers.Find(delegate(SharpMap.Layers.Layer layer) { return layer.LayerName.Equals(name); });
		}

		/// <summary>
		/// Returns the extent of the layer
		/// </summary>
		/// <returns>Bounding box corresponding to the extent of the features in the layer</returns>
		public override BoundingBox Envelope
		{
			get
            {
                BoundingBox bbox = BoundingBox.Empty;
				
                if (this.Layers.Count == 0)
					return bbox;

                Layers.ForEach(delegate(Layer layer) 
                { 
                    bbox.ExpandToInclude(layer.Envelope); 
                });

				return bbox;
			}
		}


		#region ICloneable Members

		/// <summary>
		/// Clones the layer
		/// </summary>
		/// <returns>cloned object</returns>
		public override object Clone()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes the object
		/// </summary>
		public void Dispose()
		{
			foreach (SharpMap.Layers.Layer layer in this.Layers)
				if (layer is IDisposable)
					((IDisposable)layer).Dispose();
			this.Layers.Clear();
		}

		#endregion
	}
}
