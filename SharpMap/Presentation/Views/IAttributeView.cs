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
using SharpMap.Presentation.Views;

namespace SharpMap.Presentation.Views
{
    /// <summary>
    /// Defines a view for attribute data.
    /// </summary>
    public interface IAttributeView : IView
    {
        /// <summary>
        /// Sets the layers to display attributes for.
        /// </summary>
        IBindingList Layers { set; }

        /// <summary>
        /// Sets a layer's attribute source.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="attributes"></param>
        void SetLayerAttributes(String layer, IBindingList attributes);

        /// <summary>
        /// Raises an event to request that the controlling presenter
        /// change the list of selected features shown in this view.
        /// </summary>
        event EventHandler<FeaturesHighlightedChangedEventArgs> FeaturesHighlightedChanged;

        /// <summary>
        /// Update the view to show the given features on the given layer as selected.
        /// </summary>
        /// <param name="layerName">
        /// Layer to set selected features on.
        /// </param>
        /// <param name="featureIndexes">
        /// Set of indexes of features to set to be selected.
        /// </param>
        void SetHighlightedFeatures(String layerName, IEnumerable<Int32> featureIndexes);
    }
}