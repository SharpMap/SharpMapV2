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
using System.ComponentModel;

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
            _visibleLayersChangeRequestedDelegate = handleVisibileLayersChangeRequested;

            Map.PropertyChanged += handleMapPropertyChanged;
            Map.LayersChanged += handleMapLayersCollectionChanged;

            View.LayersSelectionChangeRequested += _selectedLayersChangeRequestedDelegate;
            View.LayersEnabledChangeRequested += _visibleLayersChangeRequestedDelegate;
        }

        #region Helper Functions

        private void handleMapLayersCollectionChanged(object sender, LayersChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void handleMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void handleVisibileLayersChangeRequested(object sender, LayerActionEventArgs e)
        {
        }

        private void handleLayerSelectionChangedRequested(object sender, LayerActionEventArgs e)
        {
            Map.SelectLayers(e.Layers);
        }

        #endregion
    }
}