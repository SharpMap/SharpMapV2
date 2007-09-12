using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using SharpMap.Data;
using SharpMap.Features;
using SharpMap.Geometries;
using SharpMap.Presentation;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    public abstract class FeatureLayer : Layer, IFeatureLayer
    {
        #region Instance fields
        private readonly FeatureDataTable _cachedFeatures;
        private readonly FeatureDataView _visibleFeatureView;
        private readonly FeatureDataView _selectedFeatures;
        private readonly FeatureDataView _highlightedFeatures;
        private readonly BackgroundWorker _dataQueryWorker = new BackgroundWorker();
        #endregion

        /// <summary>
        /// Initializes a new, empty vector layer.
        /// </summary>
        protected FeatureLayer(IFeatureLayerProvider dataSource)
            : this(String.Empty, dataSource)
        {
        }

        /// <summary>
        /// Initializes a new layer with the given name and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(string layername, IFeatureLayerProvider dataSource)
            : this(layername, new VectorStyle(), dataSource)
        {
        }

        /// <summary>
        /// Initializes a new layer with the given name, style and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="style">Style to apply to the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(string layername, VectorStyle style, IFeatureLayerProvider dataSource)
            : base(layername, style, dataSource)
        {
            _dataQueryWorker.RunWorkerCompleted += _dataQueryWorker_RunWorkerCompleted;
            _dataQueryWorker.DoWork += _dataQueryWorker_DoWork;

            _cachedFeatures = new FeatureDataTable();
            _visibleFeatureView = new FeatureDataView(_cachedFeatures);

            init();
        }

        #region IFeatureLayer Members

        /// <summary>
        /// Gets the data source for this layer as a more 
        /// strongly-typed IFeatureLayerProvider.
        /// </summary>
        public new IFeatureLayerProvider DataSource
        {
            get { return base.DataSource as IFeatureLayerProvider; }
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

        #region Layer overrides
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

        
        #region Private helper methods
        private void init()
        {
            // We generally want spatial indexing on the feature table...
            _cachedFeatures.IsSpatiallyIndexed = true;

            // We need to get the schema of the feature table.
            DataSource.Open();

            DataSource.SetTableSchema(_cachedFeatures);

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
