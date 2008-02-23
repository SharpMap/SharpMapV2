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
using SharpMap.Presentation.Presenters;
using SharpMap.Presentation.Views;
using SharpMap.Tools;

namespace SharpMap.Presentation.Presenters
{
    /// <summary>
    /// The presenter for managing a <see cref="MapTool">view</see> of <see cref="IToolsView"/> instances.
    /// </summary>
    public class ToolsPresenter : BasePresenter<IToolsView>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="ToolsPresenter"/> with the given model and view.
        /// </summary>
        /// <param name="map">The map model to present.</param>
        /// <param name="toolsView">The view to accept input from and keep synchronized with the model.</param>
        public ToolsPresenter(Map map, IToolsView toolsView)
            : base(map, toolsView)
        {
            View.ToolChangeRequested += handleToolChangeRequested;

            // TODO: tool configuration should come from a config file and / or reflection
            List<MapTool> mapTools = new List<MapTool>(
                new MapTool[]
                    {
                        StandardMapTools2D.Pan, 
                        StandardMapTools2D.Query, 
                        StandardMapTools2D.ZoomIn,
                        StandardMapTools2D.ZoomOut
                    });

            View.Tools = mapTools;
        }

        protected override void OnMapPropertyChanged(String propertyName)
        {
            if (propertyName == Map.SelectedLayersProperty.Name)
            {
                View.SelectedTool = Map.ActiveTool;
            }

            if (propertyName == Map.ActiveToolProperty.Name)
            {
                View.SelectedTool = Map.ActiveTool;
            }

            if (propertyName == Map.SpatialReferenceProperty.Name)
            {
            }
        }

        private void handleToolChangeRequested(Object sender, ToolChangeRequestedEventArgs e)
        {
            if(e.RequestedTool == Map.ActiveTool)
            {
                return;
            }

            Map.ActiveTool = e.RequestedTool;
        }
    }
}