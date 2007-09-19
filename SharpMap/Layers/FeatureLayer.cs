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
using SharpMap.Features;
using SharpMap.Geometries;
using SharpMap.Presentation;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// Represents a map layer of features.
    /// </summary>
    public abstract class FeatureLayer : Layer, IFeatureLayer
    {
        #region Instance fields

        private readonly FeatureDataTable _cachedFeatures;
        private readonly FeatureDataView _visibleFeatureView;
        private readonly FeatureDataView _selectedFeatures;
        private readonly FeatureDataView _highlightedFeatures;

        #endregion

        /// <summary>
        /// Initializes a new, empty features layer.
        /// </summary>
        protected FeatureLayer(IFeatureLayerProvider dataSource)
            : this(String.Empty, dataSource)
        {
        }

        /// <summary>
        /// Initializes a new features layer with the given name and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(string layername, IFeatureLayerProvider dataSource)
            : this(layername, new VectorStyle(), dataSource)
        {
        }

        /// <summary>
        /// Initializes a new features layer with the given name, style and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="style">Style to apply to the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(string layername, VectorStyle style, IFeatureLayerProvider dataSource)
            : base(layername, style, dataSource)
        {
            _cachedFeatures = new FeatureDataTable();
            _visibleFeatureView = new FeatureDataView(_cachedFeatures, Point.Empty, "", DataViewRowState.CurrentRows);
            _selectedFeatures = new FeatureDataView(_cachedFeatures, Point.Empty, "", DataViewRowState.CurrentRows);
            _highlightedFeatures = new FeatureDataView(_cachedFeatures, Point.Empty, "", DataViewRowState.CurrentRows);

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

        protected override void LoadLayerDataForRegion(BoundingBox region)
        {
            DataSource.ExecuteIntersectionQuery(region, _cachedFeatures);
            base.LoadLayerDataForRegion(region);
        }

        protected override void LoadLayerDataForRegion(Polygon region)
        {
            DataSource.ExecuteIntersectionQuery(region, _cachedFeatures);
            base.LoadLayerDataForRegion(region);
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

        #endregion
    }
}