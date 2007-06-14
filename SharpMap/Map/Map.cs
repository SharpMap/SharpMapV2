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

namespace SharpMap.Map
{
	/// <summary>
	/// Map class
	/// </summary>
	/// <example>
	/// Creating a new map instance, adding layers and rendering the map:
	/// <code lang="C#">
	/// SharpMap.Map myMap = new SharpMap.Map(picMap.Size);
	/// myMap.MinimumZoom = 100;
	/// myMap.BackgroundColor = Color.White;
	/// 
	/// SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("My layer");
	///	string ConnStr = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=password;Database=myGisDb;";
	/// myLayer.DataSource = new SharpMap.Data.Providers.PostGIS(ConnStr, "myTable", "the_geom", 32632);
	/// myLayer.FillStyle = new SolidBrush(Color.FromArgb(240,240,240)); //Applies to polygon types only
	///	myLayer.OutlineStyle = new Pen(Color.Blue, 1); //Applies to polygon and linetypes only
	/// //Setup linestyle (applies to line types only)
	///	myLayer.Style.Line.Width = 2;
	///	myLayer.Style.Line.Color = Color.Black;
	///	myLayer.Style.Line.EndCap = System.Drawing.Drawing2D.LineCap.Round; //Round end
	///	myLayer.Style.Line.StartCap = layRailroad.LineStyle.EndCap; //Round start
	///	myLayer.Style.Line.DashPattern = new float[] { 4.0f, 2.0f }; //Dashed linestyle
	///	myLayer.Style.EnableOutline = true;
	///	myLayer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //Render smooth lines
	///	myLayer.MaxVisible = 40000;
	/// 
	/// myMap.Layers.Add(myLayer);
	/// // [add more layers...]
	/// 
	/// myMap.Center = new SharpMap.Geometries.Point(725000, 6180000); //Set center of map
	///	myMap.Zoom = 1200; //Set zoom level
	/// myMap.Size = new System.Drawing.Size(300,200); //Set output size
	/// 
	/// System.Drawing.Image imgMap = myMap.GetMap(); //Renders the map
	/// </code>
	/// </example>
	[DesignTimeVisible(false)]
	public class Map : IDisposable
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
			_layers.LayersChanged += new EventHandler<LayersChangedEventArgs>(HandleLayersChanged);
		}

		/// <summary>
		/// Disposes the map object
		/// </summary>
		public void Dispose()
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

		#region Events

		/// <summary>
		/// Event fired when layers have been added to the map.
		/// </summary>
		public event EventHandler<LayersChangedEventArgs> LayersAdded;

		/// <summary>
		/// Event fired when layers have been removed from the map.
		/// </summary>
		public event EventHandler<LayersChangedEventArgs> LayersRemoved;

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
		private void OnLayersChanged(IEnumerable<ILayer> layers, LayersChangedType changeType)
		{
			EventHandler<LayersChangedEventArgs> @event = null;

			switch (changeType)
			{
				case LayersChangedType.Added:
					{
						foreach (ILayer layer in layers)
						{
							Envelope = Envelope.Join(layer.Envelope);
						}

						@event = LayersAdded;

						if (@event != null)
						{
							@event(this, new LayersChangedEventArgs(layers, changeType));
						}
					}
					break;
				case LayersChangedType.Removed:
					{
						recomputeEnvelope();

						@event = LayersRemoved;

						if (@event != null)
						{
							@event(this, new LayersChangedEventArgs(layers, changeType));
						}
					}
					break;
				case LayersChangedType.Unknown:
				default:
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
		private void HandleLayersChanged(object sender, LayersChangedEventArgs e)
		{
			OnLayersChanged(e.ChangedLayers, e.ChangeType);
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
	}
}
