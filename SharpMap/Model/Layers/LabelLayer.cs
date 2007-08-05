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
using System.Globalization;
using System.Text;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Geometries;
using SharpMap.Data;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using SharpMap.Data.Providers;

namespace SharpMap.Layers
{
	/// <summary>
	/// A layer to generate labels from feature data.
	/// </summary>
	/// <example>
	/// Creates a new label layer and sets the label text to the 
    /// "Name" column in the FeatureDataTable of the datasource
	/// <code lang="C#">
	/// //Set up a label layer
	/// SharpMap.Layers.LabelLayer labelLayer = new LabelLayer("Country labels");
	/// labelLayer.DataSource = layCountries.DataSource;
	/// labelLayer.Enabled = true;
	/// labelLayer.LabelColumn = "Name";
	/// labelLayer.Style = new SharpMap.Styles.LabelStyle();
	/// labelLayer.Style.CollisionDetection = true;
	/// labelLayer.Style.CollisionBuffer = new Size2D(20, 20);
	/// labelLayer.Style.ForeColor = Color.White;
	/// labelLayer.Style.Font = new Font(FontFamily.GenericSerif, 8);
	/// labelLayer.MaxVisible = 90;
	/// labelLayer.Style.HorizontalAlignment = SharpMap.Styles.LabelStyle.HorizontalAlignmentEnum.Center;
	/// </code>
	/// </example>
	public class LabelLayer<TLabel> : Layer, IFeatureLayer, IDisposable
    {
        #region Nested Classes
        /// <summary>
		/// Delegate method for creating advanced label text.
		/// </summary>
        /// <param name="feature">The feature to label.</param>
		/// <returns>A string to display as the label for the feature.</returns>
		public delegate string GenerateLabelTextDelegate(FeatureDataRow feature);
        #endregion

        #region Fields
        private int _priority;
        private string _rotationColumn;
        private GenerateLabelTextDelegate _getLabelMethod;
        private string _labelColumn;
		private LabelFilterDelegate _labelFilter;
        private MultipartGeometryLabelingBehaviour _multipartGeometryBehaviour;
        #endregion

        #region Object Construction / Disposal
        /// <summary>
		/// Creates a new instance of a LabelLayer with the given name.
		/// </summary>
        /// <param name="layername">Name of the layer.</param>
		public LabelLayer(string layerName, IProvider dataSource)
            : base(dataSource)
		{
			LayerName = layerName;
            _multipartGeometryBehaviour = MultipartGeometryLabelingBehaviour.All;
			_labelFilter = LabelCollisionDetection2D.SimpleCollisionDetection;
		}

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        protected override void  Dispose(bool disposing)
        {
			if (IsDisposed)
			{
				return;
			}

			if (DataSource is IDisposable && DataSource != null)
			{
				(DataSource as IDisposable).Dispose();
			}

			base.Dispose(disposing);
        }

        #endregion
        #endregion

        /// <summary>
		/// Gets or sets labeling behavior on multipart geometries.
		/// </summary>
		/// <remarks>
        /// Default value is <see cref="MultipartGeometryBehaviourEnum.All"/>.
        /// </remarks>
        public MultipartGeometryLabelingBehaviour MultipartGeometryLabelingBehaviour
		{
			get { return _multipartGeometryBehaviour; }
			set { _multipartGeometryBehaviour = value; }
		}

		/// <summary>
		/// Delegate for performing filtering on labels.
		/// </summary>
		/// <remarks>
        /// Default method is <see cref="LabelCollisionDetection2D.SimpleCollisionDetection"/>.
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
		/// This property is overriden by the <see cref="LabelStringDelegate"/>.
		/// </remarks>
		public string LabelColumn
		{
			get { return _labelColumn; }
			set { _labelColumn = value; }
		}

		/// <summary>
		/// Gets or sets the method for creating a custom label string based on a feature.
		/// </summary>
		/// <remarks>
		/// <para>
        /// If this method is not null, it will override the <see cref="LabelColumn"/> value.
        /// </para>
		/// <para>
        /// The label delegate must take a <see cref="FeatureDataRow"/> and return a string.
        /// </para>
		/// <example>
		/// Creating a label-text by combining attributes "ROADNAME" and "STATE" 
        /// into one string, using an anonymous delegate:
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
		public string RotationColumn
		{
			get { return _rotationColumn; }
			set { _rotationColumn = value; }
		}

		/// <summary>
		/// A value indication the priority of the label in cases of label-collision detection.
		/// </summary>
		public int Priority
		{
			get { return _priority; }
			set { _priority = value; }
        }

        #region IFeatureLayer Members

        public FeatureDataTable Features
        {
            get { throw new NotImplementedException(); }
        }

        public FeatureDataView VisibleFeatures
        {
            get
            {
                // Execute query if necessary and return table...
                throw new NotImplementedException();
            }
        }

        public event EventHandler VisibleFeaturesChanged;

        public IList<FeatureDataRow> HighlightedFeatures
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler HighlightedFeaturesChanged;

		public event EventHandler SelectedFeaturesChanged;

		public IList<FeatureDataRow> SelectedFeatures
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
        }

        protected override void OnVisibleRegionChanged()
        {
            throw new NotImplementedException();
        }

        protected override void OnVisibleRegionChanging(BoundingBox value, ref bool cancel)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Gets the bounding box of the entire layer.
        /// </summary>
        public override BoundingBox Envelope
        {
            get
            {
                if (this.DataSource == null)
                {
                    throw (new InvalidOperationException("DataSource property not set on layer '" + this.LayerName + "'"));
                }

                bool wasOpen = DataSource.IsOpen;

                if (!wasOpen)
                {
                    DataSource.Open();
                }

                BoundingBox box = DataSource.GetExtents();

                if (!wasOpen) //Restore state
                {
                    this.DataSource.Close();
                }

                return box;
            }
        }

        /// <summary>
        /// Gets or sets the SRID of this layer's data source.
        /// </summary>
        public override int Srid
        {
            get
            {
                if (DataSource == null)
                {
                    throw new InvalidOperationException(String.Format("DataSource property not set on layer '{0}'", LayerName));
                }

                return DataSource.Srid;
            }
            set 
            { 
                DataSource.Srid = value; 
            }
        }

        public string GetLabelText(FeatureDataRow feature)
        {
            if (_getLabelMethod != null)
            {
                return _getLabelMethod(feature);
            }
            else
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
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
