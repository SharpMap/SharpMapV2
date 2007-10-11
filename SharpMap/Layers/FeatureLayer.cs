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
using System.Data;
using System.Globalization;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Expressions;
using SharpMap.Styles;
using System.Collections.Generic;

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
        /// which handles <see cref="FeatureDataTable.FeaturesNotFound"/> 
        /// events from <see cref="Features"/>.
        /// </summary>
        protected FeatureLayer(IFeatureLayerProvider dataSource)
            : this(String.Empty, dataSource)
        {
        }

        /// <summary>
        /// Initializes a new features layer with the given name and datasource
        /// and which handles <see cref="FeatureDataTable.FeaturesNotFound"/> 
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
        /// <see cref="FeatureDataTable.FeaturesNotFound"/> events from the <see cref="Features"/>
        /// table.
        /// </param>
        protected FeatureLayer(string layername, IFeatureLayerProvider dataSource, bool handleFeatureDataRequest)
            : this(layername, new VectorStyle(), dataSource, handleFeatureDataRequest)
        {
        }
        
        /// <summary>
        /// Initializes a new features layer with the given name, style and datasource
        /// and which handles <see cref="FeatureDataTable.FeaturesNotFound"/> 
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
        /// <see cref="FeatureDataTable.FeaturesNotFound"/> events from the <see cref="Features"/>
        /// table.
        /// </param>
        protected FeatureLayer(string layername, VectorStyle style, IFeatureLayerProvider dataSource, bool handleFeatureDataRequest)
            : base(layername, style, dataSource)
        {
            ShouldHandleFeaturesNotFoundEvent = handleFeatureDataRequest;

            // We need to get the schema of the feature table.
            DataSource.Open();
            _features = DataSource.CreateNewTable() ?? new FeatureDataTable();
            DataSource.Close();

            // We generally want spatial indexing on the feature table...
            _features.IsSpatiallyIndexed = true;

            _selectedFeatures = new FeatureDataView(_features, Point.Empty, "", DataViewRowState.CurrentRows, true);
            _highlightedFeatures = new FeatureDataView(_features, Point.Empty, "", DataViewRowState.CurrentRows, true);

            if (ShouldHandleFeaturesNotFoundEvent)
            {
                _features.FeaturesNotFound += handleFeaturesRequested;
            }
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
        /// Gets a <see cref="FeatureDataTable"/> of cached features for the layer.
        /// </summary>
        public FeatureDataTable Features
        {
            get { return _features; }
        }

        /// <summary>
        /// Gets a <see cref="FeatureDataView"/> of features which have been 
        /// highlighted.
        /// </summary>
        public FeatureDataView HighlightedFeatures
        {
            get { return _highlightedFeatures; }
        }

        public void LoadFeaturesByOids(IEnumerable oids)
        {
            if (!AsyncQuery)
            {
                IEnumerable<IFeatureDataRecord> features = DataSource.GetFeatures(oids);
                MergeFeatures(features);
            }
            else
            {
                DataSource.BeginGetFeatures(oids, getFeaturesCallback, null);
            }
        }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> used to encode text
        /// and format numbers for this layer.
        /// </summary>
        public CultureInfo Locale
        {
            get { return DataSource.Locale; }
        }

        /// <summary>
        /// Gets a <see cref="FeatureDataView"/> of features which have been 
        /// selected.
        /// </summary>
        public FeatureDataView SelectedFeatures
        {
            get { return _selectedFeatures; }
        }

        #endregion

        #region Layer overrides

        public override Geometry LoadedRegion
        {
            get
            {
                return _features.Envelope;
            }
            protected set
            {
                _features.Envelope = value;
            }
        }

        /// <summary>
        /// Loads data from the <see cref="DataSource"/> which satisfy
        /// the given <see cref="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query used to match data on the data source.
        /// </param>
        public override void LoadLayerData(SpatialExpression query)
        {
            if (query == null) throw new ArgumentNullException("query");

            FeatureSpatialExpression featureQuery =
                query as FeatureSpatialExpression
                ?? new FeatureSpatialExpression(query.QueryRegion, query.QueryType, null);

            _features.SuspendIndexEvents();

            if (!AsyncQuery)
            {
                DataSource.ExecuteFeatureQuery(featureQuery, _features);
            }
            else
            {
                FeatureDataTable featureCache = new FeatureDataTable();
                DataSource.SetTableSchema(featureCache);
                DataSource.BeginExecuteFeatureQuery(featureQuery, featureCache,
                    queryCallback, featureCache);
            }

            base.LoadLayerData(query);
            _features.RestoreIndexEvents(true);
        }
        #endregion

        protected void MergeFeatures(IEnumerable<IFeatureDataRecord> features)
        {
            _features.MergeFeatures(features);
        }

        #region Private helper methods

        private void handleFeaturesRequested(object sender, FeaturesNotFoundEventArgs e)
        {
            Geometry available = Extents.ToGeometry().Intersection(e.MissingForQuery.QueryRegion);

            bool hasIntersectionWithLayerData = !(available.IsEmpty() || Features.Envelope.Contains(available)) &&
                                                e.MissingForQuery.QueryType != SpatialExpressionType.Disjoint;

            if(hasIntersectionWithLayerData || e.MissingForQuery.Oids != null)
            {
                LoadLayerData(e.MissingForQuery);   
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

        private void getFeaturesCallback(IAsyncResult result)
        {
            IEnumerable<IFeatureDataRecord> features = DataSource.EndGetFeatures(result);
            MergeFeatures(features);
        }

        #endregion
    }
}