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

using System.Collections.Generic;
using System.Diagnostics;
using SharpMap.Features;
using SharpMap.Layers;
using System.ComponentModel;
using System.Data;

namespace SharpMap.Presentation
{
    public class AttributePresenter : BasePresenter<IAttributeView>
    {
        public AttributePresenter(Map map, IAttributeView view)
            : base(map, view)
        {
            Map.Layers.ListChanged += handleLayersChanged;
            View.FeaturesHighlightedChangeRequested += handleFeaturesHighlightedChangeRequested;
            View.Layers = Map.Layers;
        }

        private void handleLayersChanged(object sender, ListChangedEventArgs e)
        {
            if(e.ListChangedType == ListChangedType.ItemAdded)
            {
                if(Map.Layers[e.NewIndex] is IFeatureLayer)
                {
                    wireupFeatureLayer(Map.Layers[e.NewIndex] as IFeatureLayer);
                }
            }

            if(e.ListChangedType == ListChangedType.ItemDeleted)
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

            layer.HighlightedFeatures.SelectedFeatures = getFeaturesFromIndexes(layer, e.HighlightedFeatures);
        }

        private void handleHighlightedFeaturesChanged(object sender, ListChangedEventArgs e)
        {
            IFeatureLayer layer = sender as IFeatureLayer;

            Debug.Assert(layer != null);

            // When the user selects features in the view, 
            // we need to highlight those features
            IEnumerable<int> indexes = getSelectedFeatureIndexesFromHighlighedFeatures(
                layer.SelectedFeatures, layer.HighlightedFeatures);

            View.SetHighlightedFeatures(layer.LayerName, indexes);
        }

        private void wireupFeatureLayer(IFeatureLayer layer)
        {
            layer.HighlightedFeatures.ListChanged += handleHighlightedFeaturesChanged;
        }

        private void unwireFeatureLayer(IFeatureLayer layer)
        {
            layer.HighlightedFeatures.ListChanged -= handleHighlightedFeaturesChanged;
        }

        private static IEnumerable<FeatureDataRow> getFeaturesFromIndexes(IFeatureLayer layer, IEnumerable<int> indexes)
        {
            foreach (int index in indexes)
            {
                yield return layer.SelectedFeatures[index].Row as FeatureDataRow;
            }
        }

        private static IEnumerable<int> getSelectedFeatureIndexesFromHighlighedFeatures(
            FeatureDataView selectedFeatures, FeatureDataView highlightedFeatures)
        {
            foreach (FeatureDataRow feature in highlightedFeatures)
            {
                yield return selectedFeatures.IndexOfFeature(feature);
            }
        }
	}
}
