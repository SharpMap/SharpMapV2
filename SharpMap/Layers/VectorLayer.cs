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
using System.ComponentModel;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Features;
using SharpMap.Geometries;
using SharpMap.Presentation;
using SharpMap.Styles;
using GeoPoint = SharpMap.Geometries.Point;
using System.Globalization;

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

        private readonly FeatureDataTable _cachedFeatures;
        private readonly FeatureDataView _visibleFeatureView;
        private readonly FeatureDataView _selectedFeatures;
        private readonly FeatureDataView _highlightedFeatures;
        private BoundingBox _fullExtents;
        private readonly BackgroundWorker _dataQueryWorker = new BackgroundWorker();
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

            _dataQueryWorker.RunWorkerCompleted += _dataQueryWorker_RunWorkerCompleted;
            _dataQueryWorker.DoWork += _dataQueryWorker_DoWork;

            _cachedFeatures = new FeatureDataTable();
            _visibleFeatureView = new FeatureDataView(_cachedFeatures);

            init();
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

        /// <summary>
        /// Gets the data source for this layer as a more 
        /// strongly-typed IVectorLayerProvider.
        /// </summary>
        public new IVectorLayerProvider DataSource
        {
            get { return base.DataSource as IVectorLayerProvider; }
        }

        /// <summary>
        /// Gets or sets a view of highlighted features, which
        /// an <see cref="IAttributeView"/> or a map view 
        /// can show as highlighted within the selected features shown.
        /// </summary>
        public FeatureDataView HighlightedFeatures
        {
            get
            {
                return _highlightedFeatures;
            }
        }

        public CultureInfo Locale
        {
            get { return DataSource.Locale; }
        }

        /// <summary>
        /// Gets or sets a view of selected features, which 
        /// an <see cref="IAttributeView"/>
        /// can bind to in order to show selected feature 
        /// attribute data, or for a map view to show the features
        /// as selected.
        /// </summary>
        public FeatureDataView SelectedFeatures
        {
            get
            {
                return _selectedFeatures;
            }
        }

        /// <summary>
        /// Gets a view of the features which are currently at least
        /// partially visible within the <see cref="Envelope"/>.
        /// </summary>
        public FeatureDataView VisibleFeatures
        {
            get { return _visibleFeatureView; }
        }

        /// <summary>
        /// Gets the loaded feautres for this layer.
        /// </summary>
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
            get
            {
                if (CoordinateTransformation != null)
                {
                    return GeometryTransform.TransformBox(_fullExtents, CoordinateTransformation.MathTransform);
                }
                else
                {
                    return _fullExtents;
                }
            }
        }

        /// <summary>
        /// Gets the <abbr name="spatial reference ID">SRID</abbr> 
        /// of this VectorLayer's data source.
        /// </summary>
        public override int? Srid
        {
            get
            {
                if (DataSource == null)
                {
                    throw new InvalidOperationException(
                        "DataSource property is null on layer '" + LayerName + "'");
                }

                return DataSource.Srid;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the layer style as a VectorStyle.
        /// </summary>
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
            // Ignore an empty visible region
            if (value == BoundingBox.Empty)
            {
                return;
            }

            if (!_cachedFeatures.Envelope.Contains(value))
            {
                // Since the visible region changed, and we don't have the data
                // which covers this new region, we have to query for it.
                //
                // We can do it asynchronously, with a BackgroundWorker instance,
                // or synchronously
                if (AsyncQuery)
                {
                    _dataQueryWorker.RunWorkerAsync();
                }
                else
                {
                    executeQuery(value);
                    OnLayerDataAvailable();
                }
            }
        }

        protected override void OnVisibleRegionChanged()
        {
            _visibleFeatureView.GeometryIntersectionFilter = VisibleRegion.ToGeometry();
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones the layer.
        /// </summary>
        /// <returns>A copy of the layer.</returns>
        public override object Clone()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Private helper methods

        private void init()
        {
            // We generally want spatial indexing on the feature table...
            _cachedFeatures.IsSpatiallyIndexed = true;

            // We need to get the schema of the feature table and the full extents for the
            // layer, so we can make decisions about visibility.
            DataSource.Open();

            DataSource.SetTableSchema(_cachedFeatures);
            _fullExtents = DataSource.GetExtents();

            DataSource.Close();
        }

        private void _dataQueryWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnLayerDataAvailable();
        }

        private void _dataQueryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is BoundingBox)
            {
                BoundingBox bounds = (BoundingBox)e.Argument;
                executeQuery(bounds);
            }
            else if (e.Argument is Geometry)
            {
                Geometry geometry = e.Argument as Geometry;
                executeQuery(geometry);
            }
        }

        private void executeQuery(BoundingBox bounds)
        {
            DataSource.ExecuteIntersectionQuery(bounds, _cachedFeatures);
        }

        private void executeQuery(Geometry geometry)
        {
            DataSource.ExecuteIntersectionQuery(geometry, _cachedFeatures);
        }
        #endregion
    }
}