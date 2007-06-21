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
using SharpMap.Data.Providers;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Layers
{
	/// <summary>
	/// Label layer class
	/// </summary>
	/// <example>
	/// Creates a new label layer and sets the label text to the "Name" column in the FeatureDataTable of the datasource
	/// <code lang="C#">
	/// //Set up a label layer
	/// SharpMap.Layers.LabelLayer layLabel = new SharpMap.Layers.LabelLayer("Country labels");
	/// layLabel.DataSource = layCountries.DataSource;
	/// layLabel.Enabled = true;
	/// layLabel.LabelColumn = "Name";
	/// layLabel.Style = new SharpMap.Styles.LabelStyle();
	/// layLabel.Style.CollisionDetection = true;
	/// layLabel.Style.CollisionBuffer = new SizeF(20, 20);
	/// layLabel.Style.ForeColor = Color.White;
	/// layLabel.Style.Font = new Font(FontFamily.GenericSerif, 8);
	/// layLabel.MaxVisible = 90;
	/// layLabel.Style.HorizontalAlignment = SharpMap.Styles.LabelStyle.HorizontalAlignmentEnum.Center;
	/// </code>
	/// </example>
	public class LabelLayer<TLabel> : Layer, IFeatureLayer, IDisposable
    {
        private int _priority;
        private string _rotationColumn;
        private GenerateLabelTextDelegate _getLabelMethod;
        private string _labelColumn;
        private IProvider _dataSource;
		private LabelFilterDelegate _labelFilter;
        private MultipartGeometryBehaviour _multipartGeometryBehaviour;

		/// <summary>
		/// Delegate method for creating advanced label text.
		/// </summary>
        /// <param name="feature">The feature to label.</param>
		/// <returns>A string to display as the label for the feature.</returns>
		public delegate string GenerateLabelTextDelegate(FeatureDataRow feature);

		/// <summary>
		/// Creates a new instance of a LabelLayer with the given name.
		/// </summary>
        /// <param name="layername">Name of the layer.</param>
		public LabelLayer(string layerName)
		{
			LayerName = layerName;
			_multipartGeometryBehaviour = MultipartGeometryBehaviour.All;
			_labelFilter = new LabelFilterDelegate(LabelCollisionDetection2D.SimpleCollisionDetection);
		}

		/// <summary>
		/// Gets or sets labeling behavior on multipart geometries.
		/// </summary>
		/// <remarks>Default value is <see cref="MultipartGeometryBehaviourEnum.All"/>.</remarks>
		public MultipartGeometryBehaviour MultipartGeometryBehaviour
		{
			get { return _multipartGeometryBehaviour; }
			set { _multipartGeometryBehaviour = value; }
		}

		/// <summary>
		/// Delegate for performing filtering on labels.
		/// </summary>
		/// <remarks>
		/// Default method is <see cref="SharpMap.Rendering.LabelCollisionDetection.SimpleCollisionDetection"/>.
		/// </remarks>
        public LabelFilterDelegate LabelFilter
		{
			get { return _labelFilter; }
			set { _labelFilter = value; }
		}

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		public IProvider DataSource
		{
			get { return _dataSource; }
			set { _dataSource = value; }
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
		/// <para>If this method is not null, it will override the <see cref="LabelColumn"/> value.</para>
		/// <para>The label delegate must take a <see cref="FeatureDataRow"/> and return a string.</para>
		/// <example>
		/// Creating a label-text by combining attributes "ROADNAME" and "STATE" into one string, using
		/// an anonymous delegate:
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

        public IEnumerable<FeatureDataRow> GetFeatures(BoundingBox region)
        {
            FeatureDataSet dataSet = new FeatureDataSet();
            DataSource.Open();
            DataSource.ExecuteIntersectionQuery(region, dataSet);
            DataSource.Close();

			foreach (FeatureDataRow row in dataSet.Tables[0])
			{
				yield return row;
			}
        }

		public event EventHandler SelectedFeaturesChanged;

		public IList<FeatureDataRow> SelectedFeatures
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
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

			if (DataSource is IDisposable)
			{
				(DataSource as IDisposable).Dispose();
			}

			base.Dispose(disposing);
        }

        #endregion
	}

    /// <summary>
    /// Labelling behaviour for Multipart geometry collections
    /// </summary>
    public enum MultipartGeometryBehaviour
    {
        /// <summary>
        /// Place label on all parts (default)
        /// </summary>
        All,
        /// <summary>
        /// Place label on object which the greatest length or area.
        /// </summary>
        /// <remarks>
        /// Multipoint geometries will default to <see cref="First"/>
        /// </remarks>
        Largest,
        /// <summary>
        /// The center of the combined geometries
        /// </summary>
        CommonCenter,
        /// <summary>
        /// Center of the first geometry in the collection (fastest method)
        /// </summary>
        First
    }
}
