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
using SharpMap;
using SharpMap.Layers;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Provides a presenter for the layers of a <see cref="Map"/>.
    /// </summary>
    public class LayersPresenter : BasePresenter<ILayersView>
    {
        private EventHandler<LayerActionEventArgs> _selectedLayersChangeRequestedDelegate;
        private EventHandler<LayerActionEventArgs> _visibleLayersChangeRequestedDelegate;

        /// <summary>
        /// Creates a new instance of a LayersPresenter with the given <see cref="Map"/>
        /// instance and the given concrete <see cref="ILayersView"/> implementation.
        /// </summary>
        /// <param name="map">Map to present.</param>
        /// <param name="view">View to present to.</param>
        public LayersPresenter(Map map, ILayersView view)
            : base(map, view)
        {
            _selectedLayersChangeRequestedDelegate = handleLayerSelectionChangedRequested;
            _visibleLayersChangeRequestedDelegate = handleVisibleLayersChangeRequested;

            Map.Layers.ListChanged += handleLayersCollectionChanged;

            View.LayersSelectionChangeRequested += _selectedLayersChangeRequestedDelegate;
            View.LayersEnabledChangeRequested += _visibleLayersChangeRequestedDelegate;
        }

        protected override void OnMapPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case Map.SelectedLayersPropertyName:
                    View.SelectedLayers = new List<string>(generateLayerNames(Map.SelectedLayers));
                    break;
                case Map.VisibleRegionPropertyName:
                    // TODO: Make layers appear unavailable if the visible region is outside
                    // the MinVisible or MaxVisible for the layer
                    break;
                case Map.ActiveToolPropertyName:
                case Map.SpatialReferencePropertyName:
                default:
                    throw new NotImplementedException();
            }
        }

        #region Private helper functions

        private IEnumerable<string> generateLayerNames(IEnumerable<ILayer> layers)
        {
            foreach (ILayer layer in layers)
            {
                yield return layer.LayerName;
            }
        }

        private void handleLayersCollectionChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if(e.PropertyDescriptor.Name == Layer.EnabledProperty.Name)
                    {
                        ILayer layer = Map.Layers[e.NewIndex];
                        if(layer.Enabled)
                        {
                            View.EnableLayer(layer.LayerName);
                        }
                        else 
                        {
                            View.DisableLayer(layer.LayerName);
                        }
                    }
                    break;
                // The following are taken care of by data binding:
                case ListChangedType.ItemMoved:
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemDeleted:
                default:
                    break;
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
                default:
                    break;
            }
        }

        private void handleLayerSelectionChangedRequested(object sender, LayerActionEventArgs e)
        {
            switch (e.LayerAction)
            {
                case LayerAction.Selected:
                    Map.SelectLayers(e.Layers);
                    break;
                case LayerAction.Deselected:
                    Map.DeselectLayers(e.Layers);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}