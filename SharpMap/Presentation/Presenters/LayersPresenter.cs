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
using SharpMap.Layers;

namespace SharpMap.Presentation
{
    public class LayersPresenter : BasePresenter<ILayersView>
    {
        private EventHandler<LayerActionEventArgs> _selectedLayersChangeRequestedDelegate;
        private EventHandler<LayerActionEventArgs> _visibleLayersChangeRequestedDelegate;

        public LayersPresenter(Map map, ILayersView view)
            : base(map, view)
        {
            _selectedLayersChangeRequestedDelegate = handleLayerSelectionChangedRequested;
            _visibleLayersChangeRequestedDelegate = handleVisibleLayersChangeRequested;

            Map.Layers.ListChanged += handleMapLayersCollectionChanged;

            View.LayersSelectionChangeRequested += _selectedLayersChangeRequestedDelegate;
            View.LayersEnabledChangeRequested += _visibleLayersChangeRequestedDelegate;
        }

        protected override void OnMapPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case Map.ActiveToolPropertyName:
                    break;
                case Map.SelectedLayersPropertyName:
                    break;
                case Map.SpatialReferencePropertyName:
                    break;
                case Map.VisibleRegionPropertyName:
                    // TODO: Make layers appear unavailable if the visible region is outside
                    // the MinVisible or MaxVisible for the layer
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #region Helper Functions

        private void handleMapLayersCollectionChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if(e.PropertyDescriptor.Name == Layer.EnabledProperty.Name)
                    {
                        
                    }
                    break;
                case ListChangedType.ItemMoved:
                    throw new NotImplementedException("Need to implement layer reordering.");
                
                // The following are taken care of by data binding:
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemDeleted:
                default:
                    break;
            }
        }

        private static IEnumerable<string> getLayerNames(IEnumerable<ILayer> layers)
        {
            foreach (ILayer layer in layers)
            {
                yield return layer.LayerName;
            }
        }

        private void handleVisibleLayersChangeRequested(object sender, LayerActionEventArgs e)
        {
            switch (e.LayerAction)
            {
                case LayerAction.Enabled:
                    foreach (string layerName in e.Layers)
                    {
                        Map.EnableLayer(layerName);
                    }
                    break;
                case LayerAction.Disabled:
                    foreach (string layerName in e.Layers)
                    {
                        Map.DisableLayer(layerName);
                    }
                    break;
                case LayerAction.Selected:
                    break;
                case LayerAction.Deselected:
                    break;
                default:
                    break;
            }
        }

        private void handleLayerSelectionChangedRequested(object sender, LayerActionEventArgs e)
        {
            Map.SelectLayers(e.Layers);
        }

        #endregion
    }
}