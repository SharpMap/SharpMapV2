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
using SharpMap.Layers;

namespace SharpMap.Presentation
{
    public class AttributePresenter : BasePresenter<IAttributeView>
    {
        public AttributePresenter(Map map, IAttributeView view)
            : base(map, view)
        {
			Map.LayersChanged += handleMapLayersChanged;
			View.FeaturesSelectionChangeRequested += handleViewFeatureSelectionChanged;
        }

		private void handleMapLayersChanged(object sender, LayersChangedEventArgs e)
        {
            // When the map layers collection changes, update the attribute view 
            // to show attributes for each enabled IFeatureLayer

			if(e.ChangeType == LayersChangeType.Removed || e.ChangeType == LayersChangeType.Disabled)
			{
			    IEnumerable<ILayer> layers = e.LayersAffected;

			    removeLayersFromView(layers);
			}
			else if(e.ChangeType == LayersChangeType.Added || e.ChangeType == LayersChangeType.Enabled)
			{
			    IEnumerable<ILayer> layers = e.LayersAffected;

			    addLayersToView(layers);
			}
		}

		private void handleViewFeatureSelectionChanged(object sender, FeatureSelectionChangeRequestEventArgs e)
        {
            // When the user selects features in the view, we need to highlight those features
			e.FeatureDataView.SelectedRows = e.SelectedFeatures;
			View.SelectFeatures(e.LayerName, e.SelectedFeatures);
        }

        private void removeLayersFromView(IEnumerable<ILayer> layers)
        {
            foreach (ILayer layer in layers)
            {
                View.RemoveLayer(layer.LayerName);
            }
        }

        private void addLayersToView(IEnumerable<ILayer> layers)
        {
            foreach (ILayer layer in layers)
            {
                IFeatureLayer featureLayer = layer as IFeatureLayer;

                if (layer.Enabled && featureLayer != null)
                {
                    View.AddLayer(layer.LayerName, featureLayer.SelectedFeatures);
                }
            }
        }
	}
}
