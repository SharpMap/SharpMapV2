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
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Query;

namespace SharpMap.Presentation.Presenters
{
    /// <summary>
    /// Manages views of attribute data and coordinates layer attribute interaction.
    /// </summary>
    public class AttributePresenter : BasePresenter<IAttributeView>
    {
        /// <summary>
        /// Creates a new AttributePresenter with the given map and view.
        /// </summary>
        /// <param name="map">The map model to observe and control.</param>
        /// <param name="view">The view to use to display attribute data.</param>
        public AttributePresenter(Map map, IAttributeView view)
            : base(map, view)
        {
            Map.Layers.ListChanged += handleLayersChanged;
            View.FeaturesHighlightedChangeRequested += handleFeaturesHighlightedChangeRequested;
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

        private void handleFeaturesHighlightedChangeRequested(object sender, FeaturesHighlightedChangeRequestEventArgs e)
        {
            IFeatureLayer layer = Map.Layers[e.LayerName] as IFeatureLayer;

            Debug.Assert(layer != null);

            FeatureSpatialQuery query;

            if (layer.HighlightedFeatures.ViewDefinition.QueryRegion.IsEmpty() &&
                layer.HighlightedFeatures.ViewDefinition.QueryType == SpatialQueryType.Disjoint)
            {
                query = new FeatureSpatialQuery(layer.Extents.ToGeometry(),
                                        SpatialQueryType.Intersects,
                                        getFeatureIdsFromIndexes(layer, e.HighlightedFeatures));
            }
            else
            {
                query = new FeatureSpatialQuery(Point.Empty,
                                        SpatialQueryType.Intersects,
                                        getFeatureIdsFromIndexes(layer, e.HighlightedFeatures));
            }

            layer.HighlightedFeatures.ViewDefinition = query;
        }

        private void handleHighlightedFeaturesChanged(object sender, ListChangedEventArgs e)
        {
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
            IEnumerable<int> indexes = getSelectedFeatureIndexesFromHighlighedFeatures(
                featureLayer.SelectedFeatures, featureLayer.HighlightedFeatures);

            View.SetHighlightedFeatures(featureLayer.LayerName, indexes);
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
            IFeatureLayer layer, IEnumerable<int> indexes)
        {
            foreach (int index in indexes)
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

        private static IEnumerable<int> getSelectedFeatureIndexesFromHighlighedFeatures(
            FeatureDataView selectedFeatures, FeatureDataView highlightedFeatures)
        {
            foreach (FeatureDataRow feature in highlightedFeatures)
            {
                yield return selectedFeatures.Find(feature);
            }
        }
    }
}