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
using System.Collections.ObjectModel;
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
using SharpMap.CoordinateSystems;

namespace SharpMap
{
	/// <summary>
	/// A map is a collection of <see cref="Layer">layers</see> 
    /// composed into a single frame of spatial reference.
	/// </summary>
	[DesignTimeVisible(false)]
    public class Map : INotifyPropertyChanged, IDisposable
	{
		static Map() { }

        #region Fields
		private readonly object _layersChangeSync = new object();
		private readonly object _activeToolSync = new object();
        private readonly object _spatialReferenceSync = new object();

        private readonly List<ILayer> _layers = new List<ILayer>();
		private readonly List<ILayer> _selectedLayers = new List<ILayer>();
		private BoundingBox _envelope = BoundingBox.Empty;
        private MapTool _activeTool = MapTool.None;
        private ICoordinateSystem _spatialReference;
        private bool _disposed;
        #endregion

        #region Object Creation / Disposal
        /// <summary>
		/// Initializes a new map.
		/// </summary>
		public Map()
		{
		}

        #region Dispose Pattern
        ~Map()
        {
            Dispose(false);
        }

        #region IDisposable Members
        /// <summary>
        /// Releases all resources deterministically.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                _disposed = true;
                GC.SuppressFinalize(this);

                EventHandler e = Disposed;
                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }
        }

        #endregion

		/// <summary>
		/// Disposes the map object and all layers.
		/// </summary>
		protected virtual void Dispose(bool disposing)
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

        /// <summary>
        /// Gets whether this layer is disposed, and no longer accessible.
        /// </summary>
        public bool IsDisposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// Event fired when the layer is disposed.
        /// </summary>
        public event EventHandler Disposed;
        #endregion
        #endregion

        #region Events

        /// <summary>
		/// Event fired when layers have been added to the map.
		/// </summary>
        public event EventHandler<LayersChangedEventArgs> LayersChanged;

		public event EventHandler SelectedLayersChanged;
		public event EventHandler SelectedToolChanged;
		#endregion

		#region Methods

		public void AddLayer(ILayer layer)
		{
			if (layer == null) throw new ArgumentNullException("layer");

            lock (_layersChangeSync)
            {
                _layers.Add(layer);
                OnLayersChanged(new ILayer[] { layer }, LayersChangeType.Added);
            }
		}

		public void AddLayers(IEnumerable<ILayer> layers)
		{
			if (layers == null) throw new ArgumentNullException("layers");

            lock (_layersChangeSync)
            {
                _layers.AddRange(layers);
                OnLayersChanged(layers, LayersChangeType.Added);
            }
		}

		public void RemoveLayer(ILayer layer)
		{
			if (layer != null)
			{
                lock (_layersChangeSync)
                {
                    _layers.Remove(layer);
                    OnLayersChanged(new ILayer[] { layer }, LayersChangeType.Removed);
                }
			}
		}

		public void RemoveLayer(string name)
		{
            lock (_layersChangeSync)
            {
			    ILayer layer = GetLayerByName(name);
			    RemoveLayer(layer);   
            }
		}

		public void SelectLayer(int index)
		{
			lock (_layersChangeSync)
			{
				SelectLayers(new int[] { index });
			}
		}

		public void SelectLayer(string name)
		{
			lock (_layersChangeSync)
			{
				SelectLayers(new string[] { name });
			}
		}

		public void SelectLayer(ILayer layer)
		{
			lock (_layersChangeSync)
			{
				SelectLayers(new ILayer[] { layer });
			}
		}

		public void SelectLayers(IEnumerable<int> indexes)
		{
			if (indexes == null) throw new ArgumentNullException("indexes");

			lock (_layersChangeSync)
			{
				Converter<IEnumerable<int>, IEnumerable<ILayer>> layerGenerator 
                    = new Converter<IEnumerable<int>, IEnumerable<ILayer>>(layersGenerator);
				selectLayersInternal(layerGenerator(indexes));
			}
		}

		public void SelectLayers(IEnumerable<string> layerNames)
		{
			if (layerNames == null) throw new ArgumentNullException("layerNames");

			lock (_layersChangeSync)
			{
				Converter<IEnumerable<string>, IEnumerable<ILayer>> layerGenerator 
                    = new Converter<IEnumerable<string>, IEnumerable<ILayer>>(layersGenerator);
				selectLayersInternal(layerGenerator(layerNames));
			}
		}

		public void SelectLayers(IEnumerable<ILayer> layers)
		{
			if (layers == null) throw new ArgumentNullException("layers");

			lock (_layersChangeSync)
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
			if (indexes == null) throw new ArgumentNullException("indexes");

			lock (_layersChangeSync)
			{
				Converter<IEnumerable<int>, IEnumerable<ILayer>> layerGenerator = layersGenerator;
				unselectLayersInternal(layerGenerator(indexes));
			}
		}

		public void UnselectLayers(IEnumerable<string> layerNames)
		{
			if (layerNames == null) throw new ArgumentNullException("layerNames");

			lock (_layersChangeSync)
			{
                unselectLayersInternal(layersGenerator(layerNames));
			}
		}

		public void UnselectLayers(IEnumerable<ILayer> layers)
		{
			if (layers == null) throw new ArgumentNullException("layers");

			lock (_layersChangeSync)
			{
				unselectLayersInternal(layers);
			}
		}

		public void SetLayerStyle(int index, Style style)
		{
			if (index < 0 || index >= Layers.Count) throw new ArgumentOutOfRangeException("index");

            lock (_layersChangeSync)
            {
                setLayerStyleInternal(Layers[index], style);
            }
		}

		public void SetLayerStyle(string name, Style style)
		{
			if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            lock (_layersChangeSync)
            {
                setLayerStyleInternal(GetLayerByName(name), style);
            }
		}

		public void SetLayerStyle(ILayer layer, Style style)
		{
			if (layer == null) throw new ArgumentNullException("layer");

            lock (_layersChangeSync)
            {
                setLayerStyleInternal(layer, style);
            }
		}

		public void EnableLayer(int index)
		{
			if (index < 0 || index >= Layers.Count) throw new ArgumentOutOfRangeException("index");

            lock (_layersChangeSync)
            {
                changeLayerEnabled(Layers[index], false);
            }
		}

		public void EnableLayer(string name)
		{
			if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            lock (_layersChangeSync)
            {
                changeLayerEnabled(GetLayerByName(name), true);
            }
		}

		public void EnableLayer(ILayer layer)
		{
			if (layer == null) throw new ArgumentNullException("layer");

            lock (_layersChangeSync)
            {
			    changeLayerEnabled(layer, true);
            }
		}

		public void DisableLayer(int index)
		{
			if (index < 0 || index >= Layers.Count) throw new ArgumentOutOfRangeException("index");

            lock (_layersChangeSync)
            {
                changeLayerEnabled(Layers[index], false);
            }
		}

		public void DisableLayer(string name)
		{
			if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            lock (_layersChangeSync)
            {
                changeLayerEnabled(GetLayerByName(name), false);
            }
		}

		public void DisableLayer(ILayer layer)
		{
			if (layer == null) throw new ArgumentNullException("layer");

            lock (_layersChangeSync)
            {
                changeLayerEnabled(layer, false);
            }
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
            lock (_layersChangeSync)
            {
                foreach (ILayer layer in Layers)
                {
                    if (layer.LayerName.Contains(layerName))
                    {
                        yield return layer;
                    }
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
            lock (_layersChangeSync)
            {
                return _layers.Find(delegate(ILayer layer)
                {
                    return String.Compare(layer.LayerName, name,
                        StringComparison.CurrentCultureIgnoreCase) == 0;
                });
            }
		}

		/// <summary>
		/// Gets the extents of the map based on the extents of all the layers 
		/// in the layers collection.
		/// </summary>
		/// <returns>Full map extents.</returns>
		public BoundingBox GetExtents()
		{
            BoundingBox extents = BoundingBox.Empty;

            foreach (ILayer layer in Layers)
            {
                extents.ExpandToInclude(layer.Envelope);
            }

            return extents;
		}

		#endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently active tool used to
        /// interact with the map.
        /// </summary>
        public MapTool ActiveTool
        {
            get
            {
                lock (_activeToolSync)
                {
                    return _activeTool;
                }
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                lock (_activeToolSync)
                {
                    _activeTool = value;
                    OnActiveToolChanged();
                }
            }
        }

        /// <summary>
        /// Gets center of map in world coordinates.
        /// </summary>
        public GeoPoint Center
        {
            get { return _envelope.GetCentroid(); }
        }

        /// <summary>
        /// Gets a collection of layers. 
        /// The first layer in the list is drawn first, the last one on top.
        /// </summary>
        public ReadOnlyCollection<ILayer> Layers
        {
            get
            {
                lock (_layersChangeSync)
                {
                    return _layers.AsReadOnly();
                }
            }
            private set
            {
                _layers.Clear();

                AddLayers(value);
            }
        }

        /// <summary>
        /// Gets or sets a list of layers which are
        /// selected.
        /// </summary>
        public ReadOnlyCollection<ILayer> SelectedLayers
		{
			get
			{
				lock (_layersChangeSync)
				{
					return _selectedLayers.AsReadOnly();
				}
			}
			set
            {
                if (value == null) throw new ArgumentNullException("value");

                foreach (ILayer layer in value)
                {
                    if (!Layers.Contains(layer))
                    {
                        throw new ArgumentException(
                            "The set of layers contains a layer {0} which is not " +
                            "currently part of the map. Please add the layer to " +
                            "the map before selecting it.");
                    }
                }

				lock (_layersChangeSync)
				{
					_selectedLayers.Clear();
					_selectedLayers.AddRange(value);
					OnSelectedLayersChanged();
				}
			}
		}

        /// <summary>
        /// Gets or sets the spatial reference for the entire map.
        /// </summary>
        public ICoordinateSystem SpatialReference
        {
            get { return _spatialReference; }
            set 
            {
                if (value == null) throw new ArgumentNullException("value");
                _spatialReference = value; 
            }
        }

        /// <summary>
        /// Gets the currently visible features in all the enabled layers in the map.
        /// </summary>
        public FeatureDataSet VisibleFeatures
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the current visible envelope of the map.
        /// </summary>
		public BoundingBox VisibleRegion
		{
			get { return _envelope; }
			set 
            {
                _envelope = value;

                foreach (ILayer layer in Layers)
                {
                    layer.VisibleRegion = value;
                }

                OnVisibleRegionChanged();
            }
		}

        private void OnVisibleRegionChanged()
        {
            throw new Exception("The method or operation is not implemented.");
        }
		#endregion

		#region Event Generators
		private void OnLayersChanged(IEnumerable<ILayer> layers, LayersChangeType action)
		{
			EventHandler<LayersChangedEventArgs> @event = null;

			switch (action)
			{
				case LayersChangeType.Added:
					{
                        BoundingBox newRegion = VisibleRegion;

						foreach (ILayer layer in layers)
						{
				            newRegion.ExpandToInclude(layer.Envelope);
						}

                        VisibleRegion = newRegion;
					}
					break;
                case LayersChangeType.Removed:
					{
						recomputeEnvelope();
					}
					break;
                case LayersChangeType.Enabled:
                case LayersChangeType.Disabled:
                default:
                    throw new NotImplementedException();
			}
		}

		private void OnActiveToolChanged()
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

			VisibleRegion = envelope;
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

			_selectedLayers.AddRange(layers);

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
