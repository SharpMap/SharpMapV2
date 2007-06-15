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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Diagnostics;
using System.Globalization;

using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
using SharpMap.Styles;
using SharpMap.Tools;
using SharpMap.Utilities;

namespace SharpMap
{
	/// <summary>
	/// A map is a collection of <see cref="Layer">layers</see> composed into a single spatial frame of reference.
	/// </summary>
	/// <example>
	/// Creating a new map instance, adding layers and rendering the map:
	/// </code>
	/// </example>
	[DesignTimeVisible(false)]
	public class Map : MarshalByValueComponent, IModelObject, IDisposable
	{
		/// <summary>
		/// Used for converting numbers to/from strings
		/// </summary>
		internal readonly static NumberFormatInfo NumberFormat_EnUS = new CultureInfo("en-US", false).NumberFormat;

		static Map() { }

		private readonly object _layersSync = new object();
		private readonly object _selectedLayersSync = new object();
		private readonly object _selectedToolSync = new object();
		private readonly LayersCollection _layers = new LayersCollection();
		private readonly List<ILayer> _selectedLayers = new List<ILayer>();
		private BoundingBox _envelope = BoundingBox.Empty;
		private MapTool _selectedTool = MapTool.None;

		/// <summary>
		/// Initializes a new map.
		/// </summary>
		public Map()
		{
			_layers.CollectionChanged += new EventHandler<ModelCollectionChangedEventArgs<ILayer>>(HandleLayersChanged);
		}

		/// <summary>
		/// Disposes the map object and all layers.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
            if (disposing)
            {
                foreach (ILayer layer in Layers)
                {
                    if (layer is IDisposable && layer != null)
                    {
                        ((IDisposable)layer).Dispose();
                    }
                }

                _layers.Clear();
            }
		}

		#region Events

		/// <summary>
		/// Event fired when layers have been added to the map.
		/// </summary>
        public event EventHandler<ModelCollectionChangedEventArgs<ILayer>> LayersCollectionChanged;

		public event EventHandler SelectedLayersChanged;
		public event EventHandler SelectedToolChanged;
		#endregion

		#region Methods

		public void AddLayer(ILayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}

			_layers.Add(layer);
		}

		public void AddLayers(IEnumerable<ILayer> layers)
		{
			if (layers == null)
			{
				throw new ArgumentNullException("layers");
			}

			_layers.AddLayers(layers);
		}

		public void RemoveLayer(ILayer layer)
		{
			if (layer != null)
			{
				_layers.Remove(layer);
			}
		}

		public void RemoveLayer(string name)
		{
			ILayer layer = GetLayerByName(name);
			RemoveLayer(layer);
		}

		public void SelectLayer(int index)
		{
			lock (_selectedLayersSync)
			{
				SelectLayers(new int[] { index });
			}
		}

		public void SelectLayer(string name)
		{
			lock (_selectedLayersSync)
			{
				SelectLayers(new string[] { name });
			}
		}

		public void SelectLayer(ILayer layer)
		{
			lock (_selectedLayersSync)
			{
				SelectLayers(new ILayer[] { layer });
			}
		}

		public void SelectLayers(IEnumerable<int> indexes)
		{
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}

			lock (_selectedLayersSync)
			{
				Converter<IEnumerable<int>, IEnumerable<ILayer>> layerGenerator = new Converter<IEnumerable<int>, IEnumerable<ILayer>>(layersGenerator);
				selectLayersInternal(layerGenerator(indexes));
			}
		}

		public void SelectLayers(IEnumerable<string> layerNames)
		{
			if (layerNames == null)
			{
				throw new ArgumentNullException("layerNames");
			}

			lock (_selectedLayersSync)
			{
				Converter<IEnumerable<string>, IEnumerable<ILayer>> layerGenerator = new Converter<IEnumerable<string>, IEnumerable<ILayer>>(layersGenerator);
				selectLayersInternal(layerGenerator(layerNames));
			}
		}

		public void SelectLayers(IEnumerable<ILayer> layers)
		{
			if (layers == null)
			{
				throw new ArgumentNullException("layers");
			}

			lock (_selectedLayersSync)
			{
				selectLayersInternal(layers);
			}
		}

		public void UnselectLayer(int index)
		{
			UnselectLayers(new int[] { index });
		}

		public void UnselectLayer(string name)
		{
			UnselectLayers(new string[] { name });
		}

		public void UnselectLayer(ILayer layer)
		{
			UnselectLayers(new ILayer[] { layer });
		}

		public void UnselectLayers(IEnumerable<int> indexes)
		{
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}

			lock (_selectedLayersSync)
			{
				Converter<IEnumerable<int>, IEnumerable<ILayer>> layerGenerator = new Converter<IEnumerable<int>, IEnumerable<ILayer>>(layersGenerator);
				unselectLayersInternal(layerGenerator(indexes));
			}
		}

		public void UnselectLayers(IEnumerable<string> layerNames)
		{
			if (layerNames == null)
			{
				throw new ArgumentNullException("layerNames");
			}

			lock (_selectedLayersSync)
			{
				Converter<IEnumerable<string>, IEnumerable<ILayer>> layerGenerator = new Converter<IEnumerable<string>, IEnumerable<ILayer>>(layersGenerator);
				unselectLayersInternal(layerGenerator(layerNames));
			}
		}

		public void UnselectLayers(IEnumerable<ILayer> layers)
		{
			if (layers == null)
			{
				throw new ArgumentNullException("layers");
			}

			lock (_selectedLayersSync)
			{
				unselectLayersInternal(layers);
			}
		}

		public void SetLayerStyle(int index, Style style)
		{
			if (index < 0 || index >= Layers.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			setLayerStyleInternal(Layers[index], style);
		}

		public void SetLayerStyle(string name, Style style)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			setLayerStyleInternal(GetLayerByName(name), style);
		}

		public void SetLayerStyle(ILayer layer, Style style)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}

			setLayerStyleInternal(layer, style);
		}

		public void EnableLayer(int index)
		{
			if (index < 0 || index >= Layers.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			changeLayerEnabled(Layers[index], false);
		}

		public void EnableLayer(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			changeLayerEnabled(GetLayerByName(name), true);
		}

		public void EnableLayer(ILayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}

			changeLayerEnabled(layer, true);
		}

		public void DisableLayer(int index)
		{
			if (index < 0 || index >= Layers.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			changeLayerEnabled(Layers[index], false);
		}

		public void DisableLayer(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			changeLayerEnabled(GetLayerByName(name), false);
		}

		public void DisableLayer(ILayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}

			changeLayerEnabled(layer, false);
		}

		/// <summary>
		/// Returns an enumerable set of all layers containing the string <paramref name="layerName"/> 
		/// in the <see cref="ILayer.LayerName"/> property.
		/// </summary>
		/// <param name="layerName">String to search for.</param>
		/// <returns>IEnumerable{ILayer} of all layers with <see cref="ILayer.LayerName"/> 
		/// containing <paramref name="layerName"/>.</returns>
		public IEnumerable<ILayer> FindLayers(string layerName)
		{
			foreach (ILayer layer in Layers)
			{
				if (layer.LayerName.Contains(layerName))
				{
					yield return layer;
				}
			}
		}

		/// <summary>
		/// Returns a layer by its name.
		/// </summary>
		/// <remarks>
		/// Performs culture-sensitive, case-insensitive search.
		/// </remarks>
		/// <param name="name">Name of layer.</param>
		/// <returns>Layer with <see cref="ILayer.LayerName"/> of <paramref name="name"/>.</returns>
		public ILayer GetLayerByName(string name)
		{
			return _layers.Find(delegate(ILayer layer)
			{
				return String.Compare(layer.LayerName, name, StringComparison.CurrentCultureIgnoreCase) == 0;
			});
		}

		/// <summary>
		/// Gets the extents of the map based on the extents of all the layers 
		/// in the layers collection.
		/// </summary>
		/// <returns>Full map extents.</returns>
		public BoundingBox GetExtents()
		{
			return Envelope;
		}

		#endregion

		#region Properties

		public IList<ILayer> SelectedLayers
		{
			get
			{
				lock (_selectedLayersSync)
				{
					return _selectedLayers.AsReadOnly();
				}
			}
			set
			{
				lock (_selectedLayersSync)
				{
					_selectedLayers.Clear();
					_selectedLayers.AddRange(value);
					OnSelectedLayersChanged();
				}
			}
		}

		public MapTool SelectedTool
		{
			get
			{
				lock (_selectedToolSync)
				{
					return _selectedTool;
				}
			}
			set
			{
				lock (_selectedToolSync)
				{
					_selectedTool = value;
					OnSelectedToolChanged();
				}
			}
		}

		public BoundingBox Envelope
		{
			get { return _envelope; }
			private set { _envelope = value; }
		}

		/// <summary>
		/// Gets a collection of layers. The first layer in the list is drawn first, the last one on top.
		/// </summary>
		public LayersCollection Layers
		{
			get
			{
				return _layers;
			}
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_layers.Clear();

				AddLayers(value);
			}
		}

		/// <summary>
		/// Gets or sets center of map in world coordinates.
		/// </summary>
		public GeoPoint Center
		{
			get { return _envelope.GetCentroid(); }
		}
		#endregion

		#region Event Generators
		private void OnLayersChanged(IEnumerable<ILayer> layers, CollectionChangeAction action)
		{
			EventHandler<ModelCollectionChangedEventArgs<ILayer>> @event = null;

			switch (action)
			{
				case CollectionChangeAction.Add:
					{
						foreach (ILayer layer in layers)
						{
							Envelope = Envelope.Join(layer.Envelope);
						}
					}
					break;
				case CollectionChangeAction.Remove:
					{
						recomputeEnvelope();
					}
					break;
				case CollectionChangeAction.Refresh:
                default:

                    @event = LayersCollectionChanged;

                    if (@event != null)
                    {
                        @event(this, new ModelCollectionChangedEventArgs<ILayer>(layers, action));
                    }

					break;
			}
		}

		private void OnSelectedToolChanged()
		{
			EventHandler e = SelectedToolChanged;

			if (e != null)
			{
				e(null, EventArgs.Empty);
			}
		}

		private void OnSelectedLayersChanged()
		{
			EventHandler e = SelectedLayersChanged;

			if (e != null)
			{
				e(null, EventArgs.Empty);
			}
		}
		#endregion

		#region Event Handlers
        private void HandleLayersChanged(object sender, ModelCollectionChangedEventArgs<ILayer> e)
		{
			OnLayersChanged(e.Elements, e.Action);
		}
		#endregion

		#region Private helper methods
		private void recomputeEnvelope()
		{
			BoundingBox envelope = BoundingBox.Empty;

			foreach (ILayer layer in Layers)
			{
				if (layer.Enabled)
				{
					envelope.ExpandToInclude(layer.Envelope);
				}
			}

			Envelope = envelope;
		}

		private void changeLayerEnabled(ILayer layer, bool enabled)
		{
			layer.Style.Enabled = enabled;
		}

		private void setLayerStyleInternal(ILayer layer, Style style)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}

			if (style == null)
			{
				throw new ArgumentNullException("style");
			}

			layer.Style = style;
		}

		private void selectLayersInternal(IEnumerable<ILayer> layers)
		{
			checkLayersExist();

			_layers.AddLayers(layers);

			OnSelectedLayersChanged();
		}

		private void unselectLayersInternal(IEnumerable<ILayer> layers)
		{
			checkLayersExist();

			List<ILayer> removeLayers = layers is List<ILayer> ? layers as List<ILayer> : new List<ILayer>(layers);
			_selectedLayers.RemoveAll(delegate(ILayer match) { return removeLayers.Contains(match); });

			OnSelectedLayersChanged();
		}

		private IEnumerable<ILayer> layersGenerator(IEnumerable<int> layerIndexes)
		{
			foreach (int index in layerIndexes)
			{
				if (index < 0 || index >= _layers.Count)
				{
					throw new ArgumentOutOfRangeException("index", index, String.Format("Layer index must be between 0 and {0}", _layers.Count));
				}

				yield return _layers[index];
			}
		}

		private IEnumerable<ILayer> layersGenerator(IEnumerable<string> layerNames)
		{
			foreach (string name in layerNames)
			{
				if (String.IsNullOrEmpty(name))
				{
					throw new ArgumentException("Layer name must not be null or empty.", "layerNames");
				}

				yield return GetLayerByName(name);
			}
		}

		private void checkLayersExist()
		{
			if (_layers.Count == 0)
			{
				throw new InvalidOperationException("No layers are present in the map, so layer operation cannot be performed");
			}
		}
		#endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
