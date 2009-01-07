// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using GeoAPI.Coordinates;
using NPack.Interfaces;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Symbology;

namespace SharpMap.Presentation.Presenters
{
	/// <summary>
	/// Provides a presenter for the layers of a <see cref="Map{TCoordinate}"/>.
	/// </summary>
    public class LayersPresenter<TCoordinate> : MapLayersListenerPresenter<TCoordinate, ILayersView>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
	{
		private readonly EventHandler<LayerActionEventArgs> _layersChildrenVisibleChangeRequestedDelegate;
        private readonly EventHandler<LayerActionEventArgs> _selectedLayersChangeRequestedDelegate;
        private readonly EventHandler<LayerActionEventArgs> _visibleLayersChangeRequestedDelegate;
        private readonly EventHandler<LayerActionEventArgs> _layerSelectabilityChangeRequestedDelegate;

		/// <summary>
        /// Creates a new instance of a <see cref="LayersPresenter{TCoordinate}"/> with the given <see cref="Map{TCoordinate}"/>
		/// instance and the given concrete <see cref="ILayersView"/> implementation.
		/// </summary>
		/// <param name="map">Map to present.</param>
		/// <param name="view">View to present to.</param>
		public LayersPresenter(Map<TCoordinate> map, ILayersView view)
			: base(map, view)
		{
			_selectedLayersChangeRequestedDelegate = handleLayerSelectionChangedRequested;
			_visibleLayersChangeRequestedDelegate = handleVisibleLayersChangeRequested;
			_layersChildrenVisibleChangeRequestedDelegate = handleLayerChildrenVisibleChangeRequested;
			_layerSelectabilityChangeRequestedDelegate = handleLayerSelectabilityChangeRequested;

			View.LayersSelectionChangeRequested += _selectedLayersChangeRequestedDelegate;
			View.LayersEnabledChangeRequested += _visibleLayersChangeRequestedDelegate;
			View.LayerChildrenVisibilityChangeRequested += _layersChildrenVisibleChangeRequestedDelegate;
			View.LayerSelectabilityChangeRequested += _layerSelectabilityChangeRequestedDelegate;
		}

		protected override void OnMapPropertyChanged(String propertyName)
		{
			if(propertyName == Map<TCoordinate>.SelectedLayersProperty.Name)
			{
				View.SelectedLayers = new List<String>(generateLayerNames(Map.SelectedLayers));
			}

			if(propertyName == Map<TCoordinate>.ActiveToolProperty.Name)
			{
			}

			if(propertyName == Map<TCoordinate>.SpatialReferenceProperty.Name)
			{
			}
		}

		#region Private helper functions

		private static IEnumerable<String> generateLayerNames(IEnumerable<ILayer> layers)
		{
			foreach(ILayer layer in layers)
			{
				yield return layer.LayerName;
			}
		}

        protected override void NotifyLayersChanged(ListChangedType listChangedType, int oldIndex, int newIndex, PropertyDescriptor propertyDescriptor)
        {
            switch (listChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (propertyDescriptor.Name == Layer.EnabledProperty.Name)
                    {
                        ILayer layer = Map.Layers[newIndex];
                        IEnumerable<ILayer> layers = layer as IEnumerable<ILayer>;

                        if (layers != null)
                        {
                            foreach (ILayer child in layers)
                            {
                                if (child.Enabled)
                                {
                                    View.EnableLayer(child.LayerName);
                                }
                                else
                                {
                                    View.DisableLayer(child.LayerName);
                                }
                            }
                        }
                        else if (layer.Enabled)
                        {
                            View.EnableLayer(layer.LayerName);
                        }
                        else
                        {
                            View.DisableLayer(layer.LayerName);
                        }
                    }
                    else if (propertyDescriptor.Name == LayerGroup.ShowChildrenProperty.Name)
                    {
                        ILayer layer = Map.Layers[newIndex];

                        if ((Boolean)layer.GetPropertyValue(LayerGroup.ShowChildrenProperty))
                        {
                            View.EnableChildLayers(layer.LayerName);
                        }
                        else
                        {
                            View.DisableChildLayers(layer.LayerName);
                        }
                    }
                    else if (propertyDescriptor.Name == FeatureLayer.AreFeaturesSelectableProperty.Name)
                    {
                        ILayer layer = Map.Layers[newIndex];
                        FeatureStyle style = layer.Style as FeatureStyle;

                        if (style != null && style.AreFeaturesSelectable)
                        {
                            View.SetFeaturesSelectable(layer.LayerName, true);
                        }
                        else
                        {
                            View.SetFeaturesSelectable(layer.LayerName, false);
                        }
                    }
                    break;
                // The following are taken care of by data binding:
                //case ListChangedType.ItemMoved:
                //case ListChangedType.ItemAdded:
                //case ListChangedType.ItemDeleted:
                default:
                    break;
            }

            base.NotifyLayersChanged(listChangedType, oldIndex, newIndex, propertyDescriptor);
        }

		private void handleVisibleLayersChangeRequested(Object sender, LayerActionEventArgs e)
		{
			switch(e.LayerAction)
			{
				case LayerAction.Enabled:
					foreach(String layerName in e.Layers)
					{
						Map.EnableLayer(layerName);
					}
					break;
				case LayerAction.Disabled:
					foreach(String layerName in e.Layers)
					{
						Map.DisableLayer(layerName);
					}
					break;
				default:
					break;
			}
		}

		private void handleLayerSelectabilityChangeRequested(Object sender, LayerActionEventArgs e)
		{
			// do nothing at this point
		}

		private void handleLayerChildrenVisibleChangeRequested(Object sender, LayerActionEventArgs e)
		{
			switch(e.LayerAction)
			{
				case LayerAction.Enabled:
				case LayerAction.Disabled:
					foreach(String layerName in e.Layers)
					{
                        LayerGroup group = Map.Layers[layerName] as LayerGroup;

						if(group != null)
						{
							foreach(ILayer layer in group)
							{
								if(layer == group.MasterLayer)
								{
									continue;
								}
								if(e.LayerAction == LayerAction.Enabled)
								{
									Map.EnableLayer(layer);
								}
								else
								{
									Map.DisableLayer(layer);
								}
							}
						}
					}
					break;
				default:
					break;
			}
		}

		private void handleLayerSelectionChangedRequested(Object sender, LayerActionEventArgs e)
		{
			switch(e.LayerAction)
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
