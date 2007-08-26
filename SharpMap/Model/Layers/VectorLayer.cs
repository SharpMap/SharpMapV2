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
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Styles;
using GeoPoint = SharpMap.Geometries.Point;

namespace SharpMap.Layers
{
	/// <summary>
	/// A map layer of vector geometries.
	/// </summary>
	/// <example>
	/// Adding a <see cref="VectorLayer"/> to a map:
	/// </example>
	public class VectorLayer : Layer, IFeatureLayer
	{
		#region Fields

		private readonly object _selectedFeaturesSync = new object();
		private readonly object _highlightedFeaturesSync = new object();
		private FeatureDataTable _cachedFeatures = new FeatureDataTable();
		private FeatureDataView _visibleFeatureView;
		private FeatureDataView _selectedFeatures;
		private FeatureDataView _highlightedFeatures;
		private BoundingBox _fullExtents;

		#endregion

		#region Object Construction / Disposal

		/// <summary>
		/// Initializes a new, empty vector layer.
		/// </summary>
		public VectorLayer(IVectorLayerProvider dataSource)
			: this(String.Empty, dataSource)
		{
		}

		/// <summary>
		/// Initializes a new layer with the given name and datasource.
		/// </summary>
		/// <param name="layername">Name of the layer.</param>
		/// <param name="dataSource">Data source.</param>
		public VectorLayer(string layername, IVectorLayerProvider dataSource)
			: this(layername, new VectorStyle(), dataSource)
		{
		}

		/// <summary>
		/// Initializes a new layer with the given name, style and datasource.
		/// </summary>
		/// <param name="layername">Name of the layer.</param>
		/// <param name="style">Style to apply to the layer.</param>
		/// <param name="dataSource">Data source.</param>
		public VectorLayer(string layername, VectorStyle style, IVectorLayerProvider dataSource)
			: base(dataSource)
		{
			LayerName = layername;
			Style = style;

			initFromDataSource();
		}

		#region IDisposable Members

		/// <summary>
		/// Disposes the object.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (DataSource != null)
			{
				DataSource.Dispose();
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region IFeatureLayer Members

		public event EventHandler SelectedFeaturesChanged;
		public event EventHandler HighlightedFeaturesChanged;
		public event EventHandler VisibleFeaturesChanged;

		public new IVectorLayerProvider DataSource
		{
			get { return base.DataSource as IVectorLayerProvider; }
		}

		public FeatureDataView HighlightedFeatures
		{
			get
			{
				lock (_highlightedFeaturesSync)
				{
					return _highlightedFeatures;
				}
			}
			set
			{
				lock (_highlightedFeaturesSync)
				{
					_highlightedFeatures = value;
					onHighlightedFeaturesChanged();
				}
			}
		}

		public FeatureDataView SelectedFeatures
		{
			get
			{
				lock (_selectedFeaturesSync)
				{
					return _selectedFeatures;
				}
			}
			set
			{
				lock (_selectedFeaturesSync)
				{
					_selectedFeatures = value;
					onSelectedFeaturesChanged();
				}
			}
		}

		public FeatureDataView VisibleFeatures
		{
			get { return _visibleFeatureView; }
		}

		public FeatureDataTable Features
		{
			get { return _cachedFeatures; }
		}

		#endregion

		#region ILayer Members

		/// <summary>
		/// Returns the full extents of all the features in the layer.
		/// </summary>
		/// <returns>
		/// Bounding box corresponding to the full extent 
		/// of the features in the layer.
		/// </returns>
		public override BoundingBox Envelope
		{
			get { return _fullExtents; }
		}

		/// <summary>
		/// Gets the <abbr name="spatial reference ID">SRID</abbr> of this VectorLayer's data source.
		/// </summary>
		public override int? Srid
		{
			get
			{
				if (DataSource == null)
				{
					throw new InvalidOperationException("DataSource property is null on layer '" + LayerName + "'");
				}

				return DataSource.Srid;
			}
		}

		#endregion

		public new VectorStyle Style
		{
			get { return base.Style as VectorStyle; }
			set { base.Style = value; }
		}

		#region Layer Overrides

		IStyle ILayer.Style
		{
			get { return Style; }
			set
			{
				if (!(value is VectorStyle))
				{
					throw new ArgumentException("Style value must be of type VectorStyle.", "value");
				}

				Style = value as VectorStyle;
			}
		}

		protected override void OnVisibleRegionChanging(BoundingBox value, ref bool cancel)
		{
			DataSource.ExecuteIntersectionQuery(value, _cachedFeatures);
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Clones the layer
		/// </summary>
		/// <returns>cloned object</returns>
		public override object Clone()
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Private helper methods

		private void initFromDataSource()
		{
			DataSource.Open();

			DataSource.SetTableSchema(_cachedFeatures);
			_fullExtents = DataSource.GetExtents();

			if (CoordinateTransformation != null)
			{
				_fullExtents = GeometryTransform.TransformBox(_fullExtents,
				                                              CoordinateTransformation.MathTransform);
			}

			DataSource.Close();
		}

		private void onSelectedFeaturesChanged()
		{
			EventHandler e = SelectedFeaturesChanged;

			if (e != null)
			{
				e(this, EventArgs.Empty);
			}
		}

		private void onHighlightedFeaturesChanged()
		{
			EventHandler e = HighlightedFeaturesChanged;

			if (e != null)
			{
				e(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}