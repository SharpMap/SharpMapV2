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
using System.Text;

using SharpMap.Layers;

namespace SharpMap.Presentation
{
    public class AttributePresenter : BasePresenter<IAttributeView>
    {
        public AttributePresenter(SharpMap.Map map, IAttributeView view)
            : base(map, view)
        {
			Map.LayersChanged += new EventHandler<LayersChangedEventArgs>(Map_LayersChanged);

			// load the initial layers into the view
			Map_LayersChanged(this, new LayersChangedEventArgs(LayersChangeType.Added, Map.Layers));

			View.FeaturesSelectionChangeRequested += new EventHandler<FeatureSelectionChangeRequestEventArgs>(View_FeatureSelectionChanged);
        }

    	/// <summary>
		/// When the map layers collection changes, update the tab control to show one page for each
		/// enabled IFeatureLayer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Map_LayersChanged(object sender, LayersChangedEventArgs e)
		{
			if(e.ChangeType == LayersChangeType.Removed || e.ChangeType == LayersChangeType.Disabled)
			{
				foreach(ILayer layer in e.LayersAffected)
				{
					View.RemoveLayer(layer.LayerName);
				}
			}
			else if(e.ChangeType == LayersChangeType.Added || e.ChangeType == LayersChangeType.Enabled)
			{
				foreach(ILayer layer in e.LayersAffected)
				{
					IFeatureLayer flayer = layer as IFeatureLayer;

					if(layer.Enabled && flayer != null)
					{
						View.AddLayer(layer.LayerName, flayer.SelectedFeatures);
					}
				}
			}
		}

		/// <summary>
		/// When the user selects features in the view, we need to highlight those features
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void View_FeatureSelectionChanged(object sender, FeatureSelectionChangeRequestEventArgs e)
		{
			e.FeatureDataView.SelectedRows = e.SelectedFeatures;

			View.SelectFeatures(e.LayerName, e.SelectedFeatures);
		}
	}
}
