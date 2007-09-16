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
using System.ComponentModel;

namespace SharpMap.Presentation
{
    public class AttributePresenter : BasePresenter<IAttributeView>
    {
        public AttributePresenter(Map map, IAttributeView view)
            : base(map, view)
        {
			Map.Layers.ListChanged += handleMapLayersChanged;
			View.FeaturesSelectionChangeRequested += handleViewFeatureSelectionChanged;
        }

		private void handleMapLayersChanged(object sender, ListChangedEventArgs e)
        {
            // When the map layers collection changes, update the attribute view 
            // to show attributes for each enabled IFeatureLayer

            if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
			    ILayer layer = Map.Layers[e.NewIndex];

                View.RemoveLayer(layer.LayerName);
			}
            else if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                IFeatureLayer layer = Map.Layers[e.NewIndex] as IFeatureLayer;

                if (layer.Enabled && layer != null)
                {
                    View.AddLayer(layer.LayerName, layer.SelectedFeatures);
                }
			}
		}

		private void handleViewFeatureSelectionChanged(object sender, FeatureSelectionChangeRequestEventArgs e)
        {
            // When the user selects features in the view, we need to highlight those features
			e.FeatureDataView.SelectedRows = e.SelectedFeatures;
			View.SelectFeatures(e.LayerName, e.SelectedFeatures);
        }
	}
}
