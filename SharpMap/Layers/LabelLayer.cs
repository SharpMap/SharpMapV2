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
using System.Globalization;
using System.ComponentModel;

using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// A map layer of feature labels.
    /// </summary>
    /// <example>
    /// Creates a new label layer and sets the label text to the 
    /// "Name" column in the FeatureDataTable of the datasource
    /// <code lang="C#">
    /// //Set up a label layer
    /// </code>
    /// </example>
    public class LabelLayer : FeatureLayer
    {
        #region Nested Classes

        /// <summary>
        /// Delegate method for creating advanced label text.
        /// </summary>
        /// <param name="feature">The feature to label.</param>
        /// <returns>A String to display as the label for the feature.</returns>
        public delegate String GenerateLabelTextDelegate(FeatureDataRow feature);

        #endregion

        #region Fields

        private Int32 _priority;
        private String _rotationColumn;
        private GenerateLabelTextDelegate _getLabelMethod;
        private String _labelColumn;
        private LabelFilterDelegate _labelFilter;
        private MultipartGeometryLabelingBehavior _multipartGeometryBehaviour;
		private IFeatureLayer _master;

        #endregion

        #region Object Construction / Disposal

		/// <summary>
		/// Creates a new instance of a LabelLayer with the given name.
		/// </summary>
		/// <param name="layerName">Name of the layer.</param>
		/// <param name="dataSource">Data source provider for the layer.</param>
		public LabelLayer(String layerName, IFeatureLayerProvider dataSource)
			: base(layerName, dataSource)
		{
			_multipartGeometryBehaviour = MultipartGeometryLabelingBehavior.Default;
			_labelFilter = LabelCollisionDetection2D.SimpleCollisionDetection;
			_master = this;
		}

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        protected override void Dispose(Boolean disposing)
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

        /// <summary>
        /// Gets or sets labeling behavior on multipart geometries.
        /// </summary>
        /// <remarks>
        /// Default value is <see cref="MultipartGeometryLabelingBehavior.All"/>.
        /// </remarks>
        public MultipartGeometryLabelingBehavior MultipartGeometryLabelingBehaviour
        {
            get { return _multipartGeometryBehaviour; }
            set { _multipartGeometryBehaviour = value; }
        }

        /// <summary>
        /// Delegate for performing filtering on labels.
        /// </summary>
        /// <remarks>
        /// Default method is 
        /// <see cref="LabelCollisionDetection2D.SimpleCollisionDetection"/>.
        /// </remarks>
        public LabelFilterDelegate LabelFilter
        {
            get { return _labelFilter; }
            set { _labelFilter = value; }
        }

        /// <summary>
        /// Data column or expression where label text is extracted from.
        /// </summary>
        /// <remarks>
        /// This property is overriden by the 
        /// <see cref="GenerateLabelTextDelegate"/>.
        /// </remarks>
        public String LabelColumn
        {
            get { return _labelColumn; }
            set { _labelColumn = value; }
        }

        /// <summary>
        /// Gets or sets the method for creating a custom label String 
        /// based on a feature.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this method is not null, it will override the 
        /// <see cref="LabelColumn"/> value.
        /// </para>
        /// <para>
        /// The label delegate must take a <see cref="FeatureDataRow"/> 
        /// and return a String.
        /// </para>
        /// <example>
        /// Creating a label-text by combining attributes "ROADNAME" 
        /// and "STATE" into one String, using an anonymous delegate:
        /// <code lang="C#">
        /// myLabelLayer.LabelStringDelegate = delegate(FeatureDataRow fdr)
        ///				{ return fdr["ROADNAME"].ToString() + ", " + fdr["STATE"].ToString(); };
        /// </code>
        /// </example>
        /// </remarks>
        public GenerateLabelTextDelegate LabelTextDelegate
        {
            get { return _getLabelMethod; }
            set { _getLabelMethod = value; }
        }

        /// <summary>
        /// Data column from where the label rotation is derived.
        /// If this is empty, rotation will be zero, or aligned to a linestring.
        /// Rotation are in degrees (positive = clockwise).
        /// </summary>
        public String RotationColumn
        {
            get { return _rotationColumn; }
            set { _rotationColumn = value; }
        }

        /// <summary>
        /// A value indication the priority of the label in cases of label-collision detection.
        /// </summary>
        public Int32 Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

		/// <summary>
		/// Generates the label text for a given feature.
		/// </summary>
		/// <param name="feature">The feature to label.</param>
		/// <returns>The text of the label.</returns>
		public String GetLabelText(FeatureDataRow feature)
		{
			if (_getLabelMethod != null)
			{
				return _getLabelMethod(feature);
			}
			else
			{
				try
				{
					if (feature.IsNull(LabelColumn))
					{
						return String.Empty;
					}
					else
					{
						return feature[LabelColumn].ToString();
					}
				}
				catch(Exception e)
				{
					return e.Message;
				}
			}
		}

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override IStyle CreateStyle()
        {
            return new LabelStyle();
		}

#if false

		#region ILayer methods
		/// <summary>
		/// Gets or sets a value indicating that data is obtained asynchronously.
		/// </summary>
		Boolean AsyncQuery 
		{
			get
			{
				if (_master == this)
				{
					return base.AsyncQuery;
				}

				return _master.AsyncQuery;
			}
			set
			{
				if (_master == this)
				{
					base.AsyncQuery = value;
				}
				else
				{
					_master.AsyncQuery = value;
				}
			}
		}

		/// <summary>
		/// The dataum, projection and coordinate system used for this layer.
		/// </summary>
		ICoordinateSystem CoordinateSystem 
		{
			get
			{
				if (_master == this)
				{
					return base.CoordinateSystem;
				}
				return _master.CoordinateSystem;
			}
		}

		/// <summary>
		/// Applies a coordinate transformation to the geometries in this layer.
		/// </summary>
		ICoordinateTransformation CoordinateTransformation 
		{
			get
			{
				if (_master == this)
				{
					return base.CoordinateTransformation;
				}
				return _master.CoordinateTransformation;
			}
			set
			{
				if (_master == this)
				{
					base.CoordinateTransformation = value;
				}
				else
				{
					_master.CoordinateTransformation = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value representing the visibility of the layer.
		/// </summary>
		/// <remarks>
		/// Should be the same value as <see cref="Style"/>'s 
		/// <see cref="IStyle.Enabled"/> value.
		/// </remarks>
		Boolean Enabled 
		{
			get
			{
				if (_master == this)
				{
					return base.Enabled;
				}
				return _master.Enabled;
			}
			set
			{
				if (_master == this)
				{
					base.Enabled = value;
				}
				else
				{
					_master.Enabled = value;
				}
			}
		}

		/// <summary>
		/// Gets the boundingbox of the entire layer.
		/// </summary>
		BoundingBox Extents 
		{
			get
			{
				if (_master == this)
				{
					return base.Extents;
				}
				return _master.Extents;
			}
		}

		/// <summary>
		/// Name of layer.
		/// </summary>
		String LayerName 
		{
			get
			{
				if (_master == this)
				{
					return base.LayerName;
				}
				return _master.LayerName;
			}
			set
			{
				if (_master == this)
				{
					base.LayerName = value;
				}
				else
				{
					_master.LayerName = value;
				}
			}
		}

		/// <summary>
		/// The spatial reference ID of the layer data source.
		/// </summary>
		Int32? Srid 
		{
			get
			{
				if (_master == this)
				{
					return base.Srid;
				}
				return _master.Srid;
			}
		}

		/// <summary>
		/// The style for the layer.
		/// </summary>
		IStyle Style 
		{
			get
			{
				if (_master == this)
				{
					return base.Style;
				}
				return _master.Style;
			}
			set
			{
				if (_master == this)
				{
					base.Style = value;
				}
				else
				{
					_master.Style = value;
				}
			}
		}

		public Boolean IsVisibleWhen(Predicate<ILayer> condition)
		{
			if (_master == this)
			{
				return base.IsVisibleWhen(condition);
			}
			return _master.IsVisibleWhen(condition);
		}

		#endregion

		#region IFeatureLayer methods
		/// <summary>
		/// Gets the data source for this layer as a more 
		/// strongly-typed IFeatureLayerProvider.
		/// </summary>
		public new IFeatureLayerProvider DataSource
		{
			get 
			{
				if (_master == this)
				{
					return base.DataSource as IFeatureLayerProvider;
				}
				else
				{
					return _master.DataSource as IFeatureLayerProvider;
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="FeatureDataTable"/> of cached features for the layer.
		/// </summary>
		FeatureDataTable Features 
		{
			get
			{
				if (_master == this)
				{
					return base.Features;
				}
				return _master.Features;
			}
		}

		/// <summary>
		/// Gets a <see cref="FeatureDataView"/> of features which have been 
		/// highlighted.
		/// </summary>
		FeatureDataView HighlightedFeatures 
		{
			get
			{
				if (_master == this)
				{
					return base.HighlightedFeatures;
				}
				return _master.HighlightedFeatures;
			}
		}

		public void LoadFeaturesByOids(IEnumerable oids)
		{
			if (_master == this)
			{
				base.LoadFeaturesByOids(oids);
			}
			else
			{
				_master.LoadFeaturesByOids(oids);
			}
		}

		/// <summary>
		/// Gets the <see cref="CultureInfo"/> used to encode text
		/// and format numbers for this layer.
		/// </summary>
		CultureInfo Locale 
		{
			get
			{
				if (_master == this)
				{
					return base.Locale;
				}
				return _master.Locale;
			}
		}

		/// <summary>
		/// Gets a <see cref="FeatureDataView"/> of features which have been 
		/// selected.
		/// </summary>
		FeatureDataView SelectedFeatures 
		{
			get
			{
				if (_master == this)
				{
					return base.SelectedFeatures;
				}
				return _master.SelectedFeatures;
			}
		}
		#endregion
#endif
	}
}