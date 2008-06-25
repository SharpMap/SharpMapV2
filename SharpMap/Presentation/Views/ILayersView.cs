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
using SharpMap.Layers;
using SharpMap.Presentation.Presenters;
using SharpMap.Presentation.Views;

namespace SharpMap.Presentation.Views
{
    /// <summary>
    /// Provides the interface for a view to display the 
    /// <see cref="LayerCollection"/> in a <see cref="Map"/>.
    /// </summary>
    public interface ILayersView : IView
    {
        /// <summary>
        /// Sets the layers to display attributes for.
        /// </summary>
        IBindingList Layers { set; }

		/// <summary>
		/// Disables the layer identified by <paramref name="layer"/>.
		/// </summary>
		/// <param name="layer">The name of the <see cref="Layer"/> to disable.</param>
		void DisableLayer(String layer);

		/// <summary>
		/// Enables the layer identified by <paramref name="layer"/>.
		/// </summary>
		/// <param name="layer">The name of the <see cref="Layer"/> to enable.</param>
		void EnableLayer(String layer);

		/// <summary>
		/// Disables the layer identified by <paramref name="layer"/>.
		/// </summary>
		/// <param name="layer">The name of the <see cref="Layer"/> to disable.</param>
		void DisableChildLayers(String layer);

		/// <summary>
		/// Enables the layer identified by <paramref name="layer"/>.
		/// </summary>
		/// <param name="layer">The name of the <see cref="Layer"/> to enable.</param>
		void EnableChildLayers(String layer);

		/// <summary>
		/// Make the layer identified by <paramref name="layer"/> (un)selectable.
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="selectable"></param>
    	void SetFeaturesSelectable(String layer, Boolean selectable);

        /// <summary>
        /// Gets or sets a list of layers to show as selected.
        /// </summary>
        IList<String> SelectedLayers { get; set; }

        /// <summary>
        /// Event triggered when the layers selection is requested to be changed
        /// by the associated <see cref="LayersPresenter"/>.
        /// </summary>
        event EventHandler<LayerActionEventArgs> LayersSelectionChangeRequested;

		/// <summary>
		/// Event triggered when a layer is requested to be enabled or disabled
		/// by the associated <see cref="LayersPresenter"/>.
		/// </summary>
		event EventHandler<LayerActionEventArgs> LayersEnabledChangeRequested;

		/// <summary>
		/// Event triggered when a layer is requested to have its children be
		/// enabled or disabled by the associated <see cref="LayersPresenter"/>.
		/// </summary>
		event EventHandler<LayerActionEventArgs> LayerChildrenVisibilityChangeRequested;

		/// <summary>
		/// Event triggered when a layer is requested to have its selectability be
		/// enabled or disabled by the associated <see cref="LayersPresenter"/>.
		/// </summary>
		event EventHandler<LayerActionEventArgs> LayerSelectabilityChangeRequested;
	}
}