// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.ComponentModel;
using System.Diagnostics;
using SharpMap.Data;
using GeoAPI.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Expressions;

namespace SharpMap.Presentation.Presenters
{
    /// <summary>
    /// Manages views of attribute data and coordinates layer attribute interaction.
    /// </summary>
    public class AttributePresenter : BasePresenter<IAttributeView>
    {
        private Boolean _highlightUpdating;

        /// <summary>
        /// Creates a new AttributePresenter with the given map and view.
        /// </summary>
        /// <param name="map">The map model to observe and control.</param>
        /// <param name="view">The view to use to display attribute data.</param>
        public AttributePresenter(Map map, IAttributeView view)
            : base(map, view)
        {
            Map.Layers.ListChanged += handleLayersChanged;
            View.FeaturesHighlightedChanged += handleViewFeaturesHighlightedChanged;
            View.Layers = Map.Layers;
        }

        private void handleLayersChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                if (Map.Layers[e.NewIndex] is IFeatureLayer)
                {
                    wireupFeatureLayer(Map.Layers[e.NewIndex] as IFeatureLayer);
                }
            }

            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                if (e.NewIndex < 0 && Map.Layers[e.OldIndex] is IFeatureLayer)
                {
                    unwireFeatureLayer(Map.Layers[e.OldIndex] as IFeatureLayer);
                }
            }
        }

        private void handleViewFeaturesHighlightedChanged(object sender, FeaturesHighlightedChangedEventArgs e)
        {
            if (_highlightUpdating)
            {
                return;
            }

            IFeatureLayer layer = Map.Layers[e.LayerName] as IFeatureLayer;

            Debug.Assert(layer != null);

            FeatureSpatialExpression query;

            if (layer.HighlightedFeatures.ViewDefinition.QueryRegion.IsEmpty())
            {
                query = new FeatureSpatialExpression(Point.Empty,
                                        SpatialExpressionType.Disjoint,
                                        getFeatureIdsFromIndexes(layer, e.HighlightedFeatures));
            }
            else
            {
                query = new FeatureSpatialExpression(layer.HighlightedFeatures.ViewDefinition.QueryRegion,
                                        SpatialExpressionType.Intersects,
                                        getFeatureIdsFromIndexes(layer, e.HighlightedFeatures));
            }

            _highlightUpdating = true;
            layer.HighlightedFeatures.ViewDefinition = query;
            _highlightUpdating = false;
        }

        private void handleHighlightedFeaturesChanged(object sender, ListChangedEventArgs e)
        {
            if(_highlightUpdating)
            {
                return;
            }

            IFeatureLayer featureLayer = null;

            foreach (ILayer layer in Map.Layers)
            { 
                featureLayer = layer as IFeatureLayer;

                if (featureLayer == null)
                {
                    continue;
                }

                if (ReferenceEquals(featureLayer.HighlightedFeatures, sender))
                {
                    break;
                }
            }
            
            Debug.Assert(featureLayer != null);

            // When the user selects features in the view, 
            // we need to highlight those features
            IEnumerable<Int32> indexes = getSelectedFeatureIndexesFromHighlighedFeatures(
                featureLayer.SelectedFeatures, featureLayer.HighlightedFeatures);

            _highlightUpdating = true;
            View.SetHighlightedFeatures(featureLayer.LayerName, indexes);
            _highlightUpdating = false;
        }

        private void wireupFeatureLayer(IFeatureLayer layer)
        {
            layer.HighlightedFeatures.ListChanged += handleHighlightedFeaturesChanged;
        }

        private void unwireFeatureLayer(IFeatureLayer layer)
        {
            layer.HighlightedFeatures.ListChanged -= handleHighlightedFeaturesChanged;
        }

        private static IEnumerable<object> getFeatureIdsFromIndexes(
            IFeatureLayer layer, IEnumerable<Int32> indexes)
        {
            foreach (Int32 index in indexes)
            {
                FeatureDataRow feature = layer.SelectedFeatures[index].Row as FeatureDataRow;
                Debug.Assert(feature != null);

                if (!feature.HasOid)
                {
                    throw new InvalidOperationException(
                        "Feature must have object identifier in order to highlight.");
                }

                yield return feature.GetOid();
            }
        }

        private static IEnumerable<Int32> getSelectedFeatureIndexesFromHighlighedFeatures(
            FeatureDataView selectedFeatures, FeatureDataView highlightedFeatures)
        {
            foreach (FeatureDataRow feature in highlightedFeatures)
            {
                yield return selectedFeatures.Find(feature);
            }
        }
    }
}