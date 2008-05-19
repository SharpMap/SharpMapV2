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
using GeoAPI.Geometries;
using SharpMap.Data;
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

        /// <summary>
        /// Initializes a new, empty features layer
        /// which handles <see cref="FeatureDataTable.FeaturesNotFound"/> 
        /// events from <see cref="Features"/>.
        /// </summary>
        protected FeatureLayer(IFeatureProvider dataSource)
            : this(String.Empty, dataSource) { }

        /// <summary>
        /// Initializes a new features layer with the given name and datasource
        /// and which handles <see cref="FeatureDataTable.FeaturesNotFound"/> 
        /// events from <see cref="Features"/>.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(String layername, IFeatureProvider dataSource)
            : this(layername, new VectorStyle(), dataSource) { }

        /// <summary>
        /// Initializes a new features layer with the given name, style and datasource
        /// and which handles <see cref="FeatureDataTable.FeaturesNotFound"/> 
        /// events from <see cref="Features"/>.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="style">Style to apply to the layer.</param>
        /// <param name="dataSource">Data source.</param>
        protected FeatureLayer(String layername, 
                               VectorStyle style,
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

            IGeometry empty = dataSource.GeometryFactory.CreatePoint();

            FeatureQueryExpression selectedExpression
                = new FeatureQueryExpression(empty, SpatialOperation.Intersects, this);

            _selectedFeatures = new FeatureDataView(_features,
                                                    selectedExpression,
                                                    "",
                                                    DataViewRowState.CurrentRows);

            FeatureQueryExpression highlightExpression
                = new FeatureQueryExpression(empty, SpatialOperation.Intersects, this);

            _highlightedFeatures = new FeatureDataView(_features,
                                                       highlightExpression,
                                                       "",
                                                       DataViewRowState.CurrentRows);
        }

        internal class AsyncFeatureProviderAdapter : AsyncProviderAdapter, IFeatureProvider
        {
            public AsyncFeatureProviderAdapter(IFeatureProvider provider) 
                : base(provider) { }

            IFeatureProvider FeatureProvider
            {
                get { return Provider as IFeatureProvider; }
            }

            #region IFeatureProvider Members

            public FeatureDataTable CreateNewTable()
            {
                return FeatureProvider.CreateNewTable();
            }

            public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query,
                                                          FeatureQueryExecutionOptions options)
            {
                return FeatureProvider.ExecuteFeatureQuery(query, options);
            }

            public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query)
            {
                return FeatureProvider.ExecuteFeatureQuery(query);
            }

            public IGeometryFactory GeometryFactory
            {
                get { return FeatureProvider.GeometryFactory; }
                set { FeatureProvider.GeometryFactory = value; }
            }

            public Int32 GetFeatureCount()
            {
                return FeatureProvider.GetFeatureCount();
            }

            public DataTable GetSchemaTable()
            {
                return FeatureProvider.GetSchemaTable();
            }

            public CultureInfo Locale
            {
                get { return FeatureProvider.Locale; }
            }

            public void SetTableSchema(FeatureDataTable table)
            {
                FeatureProvider.SetTableSchema(table);
            }

            #endregion
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


        public Boolean AreFeaturesSelectable
        {
            get
            {
                FeatureStyle fstyle = Style as FeatureStyle;
                return fstyle != null && fstyle.AreFeaturesSelectable;
            }
            set
            {
                FeatureStyle fstyle = Style as FeatureStyle;

                if (fstyle != null)
                {
                    fstyle.AreFeaturesSelectable = value;
                }
            }
        }

        #endregion

        #region Layer overrides

        public override IEnumerable Select(Expression query)
        {
            throw new NotImplementedException();
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
            _features.Merge(features, GeometryFactory);
        }

        protected override IAsyncProvider CreateAsyncProvider(IProvider dataSource)
        {
            IFeatureProvider featureProvider = dataSource as IFeatureProvider;

            if (featureProvider == null)
            {
                throw new ArgumentException(
                    "The data source must be an IFeatureProvider for a FeatureLayer.");
            }

            return new AsyncFeatureProviderAdapter(featureProvider);
        }
    }
}