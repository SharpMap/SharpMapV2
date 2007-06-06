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
using System.Collections.ObjectModel;
using System.Text;

using SharpMap.Layers;
using SharpMap.Styles;

namespace SharpMap.Presentation
{
    public abstract class LayersPresenter
    {
        private SharpMap.Map.Map _map;
        private List<ILayersView> _views = new List<ILayersView>();
        private EventHandler<LayerActionEventArgs> _selectedLayersChangedDelegate;
        private EventHandler<LayerActionEventArgs> _viewLayerStylesRequestedDelegate;

        protected LayersPresenter(SharpMap.Map.Map map, ILayersView view)
            : this(map, new ILayersView[] { view }) { }

        protected LayersPresenter(SharpMap.Map.Map map, IEnumerable<ILayersView> views)
        {
            _map = map;

            _selectedLayersChangedDelegate = new EventHandler<LayerActionEventArgs>(handleLayerSelectionChanged);
            _viewLayerStylesRequestedDelegate = new EventHandler<LayerActionEventArgs>(handleViewLayerStyleRequested);

            addAndConfigureViews(views);
        }

        public SharpMap.Map.Map Map
        {
            get { return _map; }
        }

        protected ReadOnlyCollection<ILayersView> Views
        {
            get { return _views.AsReadOnly(); }
            set
            {
                removeViews();
                addAndConfigureViews(value);
            }
        }

        #region Helper Functions

        private void addAndConfigureViews(IEnumerable<ILayersView> views)
        {
            foreach (ILayersView view in views)
            {
                view.LayersSelectionChanged += _selectedLayersChangedDelegate;
                view.ViewLayerStylesRequested += _viewLayerStylesRequestedDelegate;
                _views.Add(view);
            }
        }

        private void removeViews()
        {
            foreach (ILayersView view in _views)
            {
                view.LayersSelectionChanged -= _selectedLayersChangedDelegate;
                view.ViewLayerStylesRequested -= _viewLayerStylesRequestedDelegate;
            }

            _views.Clear();
        }

        private void handleViewLayerStyleRequested(object sender, LayerActionEventArgs e)
        {

        }

        private void handleLayerSelectionChanged(object sender, LayerActionEventArgs e)
        {
            Map.SelectLayers(e.Layers);
        }
        #endregion
    }
}
