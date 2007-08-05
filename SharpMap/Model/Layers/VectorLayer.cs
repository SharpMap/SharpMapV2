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
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Styles;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Rendering;

namespace SharpMap.Layers
{
    /// <summary>
    /// Class for vector layer properties.
    /// </summary>
    /// <example>
    /// Adding a <see cref="VectorLayer"/> to a map:
    /// </example>
    public class VectorLayer : Layer, IFeatureLayer
    {
        #region Fields
        private readonly object _selectedFeaturesSync = new object();
        private readonly object _highlightedFeaturesSync = new object();
        private FeatureDataTable _features = new FeatureDataTable();
        private FeatureDataView _visibleFeatureView;
        private List<FeatureDataRow> _selectedFeatures = new List<FeatureDataRow>();
        private List<FeatureDataRow> _highlightedFeatures = new List<FeatureDataRow>();
        private BoundingBox _extents;
        #endregion

        #region Object Construction / Disposal
        /// <summary>
		/// Initializes a new, empty vector layer.
		/// </summary>
        public VectorLayer(IProvider dataSource)
            : this(String.Empty, dataSource)
		{
		}

        /// <summary>
        /// Initializes a new layer with the given name and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="dataSource">Data source.</param>
        public VectorLayer(string layername, IProvider dataSource)
            : base(dataSource)
		{
			LayerName = layername;
			Style = new VectorStyle();

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

            if (DataSource is IDisposable)
            {
                (DataSource as IDisposable).Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
        #endregion

        #region IFeatureLayer Members
        public event EventHandler SelectedFeaturesChanged;
        public event EventHandler HighlightedFeaturesChanged;
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

        public IList<FeatureDataRow> SelectedFeatures
        {
            get
            {
                lock (_selectedFeaturesSync)
                {
                    return _selectedFeatures.AsReadOnly();
                }
            }
            set
            {
                lock (_selectedFeaturesSync)
                {
                    _selectedFeatures.Clear();
                    _selectedFeatures.AddRange(value);
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
            get { return _features; }
        }

		#endregion

		#region ILayer Members
		/// <summary>
        /// Returns the extent of the layer.
        /// </summary>
        /// <returns>
        /// Bounding box corresponding to the extent 
        /// of the features in the layer.
        /// </returns>
        public override BoundingBox Envelope
        {
            get
            {
                return _extents;
            }
        }

        /// <summary>
        /// Gets or sets the SRID of this VectorLayer's data source
        /// </summary>
        public override int Srid
        {
            get
            {
                if (DataSource == null)
                {
                    throw new InvalidOperationException("DataSource property is null on layer '" + LayerName + "'");
                }

                return DataSource.Srid;
            }
            set { DataSource.Srid = value; }
        }
        #endregion

        #region Layer Overrides

        public new VectorStyle Style
        {
            get { return base.Style as VectorStyle; }
            set { base.Style = value; }
        }

        protected override void OnVisibleRegionChanging(BoundingBox value, ref bool cancel)
        {
            DataSource.ExecuteIntersectionQuery(value, _features);
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
            DataSource.GetSchema(_features);
            _extents = DataSource.GetExtents();

            if (CoordinateTransformation != null)
            {
                _extents = GeometryTransform.TransformBox(_extents, CoordinateTransformation.MathTransform);
            }

            DataSource.Close();
        }

        private void onSelectedFeaturesChanged()
		{
			EventHandler e = SelectedFeaturesChanged;

			if (e != null)
			{
				e(null, EventArgs.Empty);
			}
        }
        #endregion
    }
}
