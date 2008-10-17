// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.ComponentModel;
using System.Data;
using System.Globalization;
using GeoAPI.Diagnostics;
using SharpMap.Data;
using SharpMap.Data.Providers;
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
        private static readonly PropertyDescriptorCollection _featureLayerTypeProperties;

        static FeatureLayer()
        {
            _featureLayerTypeProperties = TypeDescriptor.GetProperties(typeof(FeatureLayer));
        }

        protected static PropertyDescriptorCollection FeatureLayerTypeProperties
        {
            get { return _featureLayerTypeProperties; }
        }

        public static PropertyDescriptor AreFeaturesSelectableProperty
        {
            get { return FeatureLayerTypeProperties.Find("AreFeaturesSelectable", false); }
        }

        #region Instance fields
        private readonly FeatureDataTable _features;
        private readonly FeatureDataView _selectedFeatures;
        private readonly FeatureDataView _highlightedFeatures;
        #endregion

        ///// <summary>
        ///// Initializes a new, empty features layer
        ///// which handles <see cref="FeatureDataTable.SelectRequested"/> 
        ///// events from <see cref="Features"/>.
        ///// </summary>
        //protected FeatureLayer(IFeatureProvider dataSource)
        //    : this(String.Empty, dataSource) { }

        ///// <summary>
        ///// Initializes a new features layer with the given name and datasource
        ///// and which handles <see cref="FeatureDataTable.SelectRequested"/> 
        ///// events from <see cref="Features"/>.
        ///// </summary>
        ///// <param name="layername">Name of the layer.</param>
        ///// <param name="dataSource">Data source.</param>
        //protected FeatureLayer(String layername, IFeatureProvider dataSource)
        //    : this(layername, new FeatureStyle(), dataSource) { }

        /// <summary>
        /// Initializes a new features layer with the given name, style and datasource
        /// and which handles <see cref="FeatureDataTable.SelectRequested"/> 
        /// events from <see cref="Features"/>.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="style">Style to apply to the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(String layername,
                               FeatureStyle style,
                               IFeatureProvider dataSource)
            : base(layername, style, dataSource)
        {
            //ShouldHandleFeaturesNotFoundEvent = handleFeatureDataRequest;

            // We need to get the schema of the feature table.
            DataSource.Open();
            _features = DataSource.CreateNewTable()
                        ?? new FeatureDataTable(dataSource.GeometryFactory);
            GeometryFactory = dataSource.GeometryFactory;
            DataSource.Close();

            // We generally want spatial indexing on the feature table...
            _features.IsSpatiallyIndexed = true;

            // handle the request on the feature data table for features
            _features.SelectRequested += handleFeaturesSelectRequested;

            // setup selected and highlighted views
            _selectedFeatures = new FeatureDataView(_features,
                                                    (FeatureQueryExpression)null,
                                                    "",
                                                    DataViewRowState.CurrentRows);
            _selectedFeatures.IsViewDefinitionExclusive = true;

            _highlightedFeatures = new FeatureDataView(_features,
                                                       (FeatureQueryExpression)null,
                                                       "",
                                                       DataViewRowState.CurrentRows);
            _highlightedFeatures.IsViewDefinitionExclusive = true;
        }

        #region IFeatureLayer Members

        /// <summary>
        /// Gets the data source for this layer as a more 
        /// strongly-typed IFeatureProvider.
        /// </summary>
        public new IFeatureProvider DataSource
        {
            get { return base.DataSource as IFeatureProvider; }
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

        public FeatureQueryExpression HighlightedFilter
        {
            get { return _highlightedFeatures.ViewDefinition; }
            set
            {
                _highlightedFeatures.AutoIndexingEnabled = false;

                if (value == null)
                {
                    _highlightedFeatures.IsViewDefinitionExclusive = true;
                    _highlightedFeatures.ViewDefinition = null;
                }
                else
                {
                    _highlightedFeatures.IsViewDefinitionExclusive = false;
                    _highlightedFeatures.ViewDefinition = value;
                }

                _highlightedFeatures.AutoIndexingEnabled = true;
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

        public FeatureQueryExpression SelectedFilter
        {
            get { return _selectedFeatures.ViewDefinition; }
            set
            {
                _selectedFeatures.AutoIndexingEnabled = false;

                if (value == null)
                {
                    _selectedFeatures.IsViewDefinitionExclusive = true;
                    _selectedFeatures.ViewDefinition = null;

                    HighlightedFilter = null;
                }
                else
                {
                    _selectedFeatures.IsViewDefinitionExclusive = false;
                    _selectedFeatures.ViewDefinition = value;
                }

                _selectedFeatures.AutoIndexingEnabled = true;
            }
        }

        /// <summary>
        /// Gets or sets the layer style as a <see cref="FeatureStyle"/>.
        /// </summary>
        public new FeatureStyle Style
        {
            get { return base.Style as FeatureStyle; }
            set { base.Style = value; }
        }

        /// <summary>
        /// Gets or sets a value which allows features in the layer to be selected
        /// or not.
        /// </summary>
        public Boolean AreFeaturesSelectable
        {
            get
            {
                FeatureStyle style = Style;
                return style != null && style.AreFeaturesSelectable;
            }
            set
            {
                if (value == AreFeaturesSelectable)
                {
                    return;
                }

                FeatureStyle style = Style;

                if (style != null)
                {
                    style.AreFeaturesSelectable = value;
                }
            }
        }

        public IEnumerable<FeatureDataRow> Select(FeatureQueryExpression query)
        {
            if (query == null) throw new ArgumentNullException("query");

            return Features.Select(query.SpatialPredicate);
        }

        #endregion

        #region Layer overrides

        public override IEnumerable Select(Expression query)
        {
            throw new System.NotImplementedException();
        }

        protected override QueryExpression GetQueryFromSpatialBinaryExpression(SpatialBinaryExpression exp)
        {
            return new FeatureQueryExpression(exp);
        }

        protected override void ProcessLoadResults(Object results)
        {
            _features.SuspendIndexEvents();

            IEnumerable<IFeatureDataRecord> features = results as IEnumerable<IFeatureDataRecord>;
            MergeFeatures(features);

            _features.RestoreIndexEvents(true);
        }
        #endregion

        protected void MergeFeatures(IEnumerable<IFeatureDataRecord> features)
        {
            //if (CoordinateTransformation != null)
            //{
            //    features = transformFeatures(features);
            //}

            _features.Merge(features, GeometryFactory);
        }

        //private IEnumerable<IFeatureDataRecord> transformFeatures(IEnumerable<IFeatureDataRecord> features)
        //{
        //    foreach (IFeatureDataRecord feature in features)
        //    {
        //        // TODO: fix this assumption of an IFeatureDataRecord being a FeatureDataRow
        //        FeatureDataRow row = feature as FeatureDataRow;
        //        Assert.IsNotNull(feature);
               
        //        if (row.Geometry.SpatialReference != CoordinateTransformation.Target)
        //        {
        //            row.Geometry = CoordinateTransformation.Transform(feature.Geometry, GeometryFactory);
        //        }

        //        yield return row;
        //    }
        //}

        protected override IAsyncProvider CreateAsyncProvider(IProvider dataSource)
        {
            IFeatureProvider featureProvider = dataSource as IFeatureProvider;

            if (featureProvider == null)
            {
                throw new ArgumentException("The data source must be an " +
                                            "IFeatureProvider for a FeatureLayer.");
            }

            return new AsyncFeatureProviderAdapter(featureProvider);
        }

        #region Event handlers

        void handleFeaturesSelectRequested(Object sender, SelectRequestedEventArgs e)
        {
            if (IsLoadingData)
            {
                return;
            }

            QueryExpression query = e.Query;

            if (!QueryCache.Contains(query))
            {
                query = QueryCache.FilterQuery(query);
                LoadLayerData(query);
            }
        }
        #endregion
    }
}