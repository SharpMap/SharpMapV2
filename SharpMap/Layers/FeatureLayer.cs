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
using System.Data;
using System.Globalization;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Presentation.Views;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// Represents a map layer of features.
    /// </summary>
    public abstract class FeatureLayer : Layer, IFeatureLayer
    {
        #region Instance fields
        private readonly FeatureDataTable _features;
        private readonly FeatureDataView _selectedFeatures;
        private readonly FeatureDataView _highlightedFeatures;
        #endregion

        /// <summary>
        /// Initializes a new, empty features layer
        /// which handles <see cref="FeatureDataTable.FeaturesRequested"/> 
        /// events from <see cref="Features"/>.
        /// </summary>
        protected FeatureLayer(IFeatureLayerProvider dataSource)
            : this(String.Empty, dataSource)
        {
        }

        /// <summary>
        /// Initializes a new features layer with the given name and datasource
        /// and which handles <see cref="FeatureDataTable.FeaturesRequested"/> 
        /// events from <see cref="Features"/>.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(string layername, IFeatureLayerProvider dataSource)
            : this(layername, new VectorStyle(), dataSource, true)
        {
        }

        /// <summary>
        /// Initializes a new features layer with the given name and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="handleFeatureDataRequest">
        /// Value to indicate the layer should handle 
        /// <see cref="FeatureDataTable.FeaturesRequested"/> events from the <see cref="Features"/>
        /// table.
        /// </param>
        protected FeatureLayer(string layername, IFeatureLayerProvider dataSource, bool handleFeatureDataRequest)
            : this(layername, new VectorStyle(), dataSource, handleFeatureDataRequest)
        {
        }
        
        /// <summary>
        /// Initializes a new features layer with the given name, style and datasource
        /// and which handles <see cref="FeatureDataTable.FeaturesRequested"/> 
        /// events from <see cref="Features"/>.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="style">Style to apply to the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(string layername, VectorStyle style, IFeatureLayerProvider dataSource)
            : this(layername, style, dataSource, true) {}

        /// <summary>
        /// Initializes a new features layer with the given name, style and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="style">Style to apply to the layer.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="handleFeatureDataRequest">
        /// Value to indicate the layer should handle 
        /// <see cref="FeatureDataTable.FeaturesRequested"/> events from the <see cref="Features"/>
        /// table.
        /// </param>
        protected FeatureLayer(string layername, VectorStyle style, IFeatureLayerProvider dataSource, bool handleFeatureDataRequest)
            : base(layername, style, dataSource)
        {
            ShouldHandleDataCacheMissEvent = handleFeatureDataRequest;

            _features = new FeatureDataTable();
            _selectedFeatures = new FeatureDataView(_features, Point.Empty, "", DataViewRowState.CurrentRows);
            _highlightedFeatures = new FeatureDataView(_features, Point.Empty, "", DataViewRowState.CurrentRows);

            if (ShouldHandleDataCacheMissEvent)
            {
                _features.FeaturesRequested += handleFeaturesRequested;
            }

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
            get { return _highlightedFeatures; }
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
            get { return _selectedFeatures; }
        }

        /// <summary>
        /// Gets the loaded feautres for this layer.
        /// </summary>
        public FeatureDataTable Features
        {
            get { return _features; }
        }

        #endregion

        #region Layer overrides

        protected override void LoadLayerDataForRegion(BoundingBox region)
        {
            if (!AsyncQuery)
            {
                DataSource.ExecuteIntersectionQuery(region, _features);
            }
            else
            {
                FeatureDataTable featureCache = new FeatureDataTable();
                DataSource.SetTableSchema(featureCache);
                DataSource.BeginExecuteIntersectionQuery(region, featureCache, 
                    queryCallback, featureCache);
            }

            base.LoadLayerDataForRegion(region);
        }

        protected override void LoadLayerDataForRegion(Geometry region)
        {
            if (!AsyncQuery)
            {
                DataSource.ExecuteFeatureQuery(region, _features, SpatialQueryType.Intersects);
            }
            else
            {
                FeatureDataTable featureCache = new FeatureDataTable();
                DataSource.SetTableSchema(featureCache);
                DataSource.BeginExecuteFeatureQuery(region, featureCache,
                    SpatialQueryType.Intersects, queryCallback, featureCache);
            }

            base.LoadLayerDataForRegion(region);
        }
        #endregion

        #region Private helper methods

        private void init()
        {
            // We generally want spatial indexing on the feature table...
            _features.IsSpatiallyIndexed = true;

            // We need to get the schema of the feature table.
            DataSource.Open();
            DataSource.SetTableSchema(_features);
            DataSource.Close();
        }

        private void handleFeaturesRequested(object sender, FeaturesRequestedEventArgs e)
        {
            if (e.RequestedRegion != null)
            {
                LoadLayerDataForRegion(e.RequestedRegion);
            }
        }

        private void queryCallback(IAsyncResult result)
        {
            FeatureDataTable features = result.AsyncState as FeatureDataTable;

            if (features != null)
            {
                Features.MergeFeatures(features);
            }

            DataSource.EndExecuteFeatureQuery(result);
        }
        #endregion
    }
}