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
using SharpMap.Tools;

namespace SharpMap.Presentation.Views
{
    /// <summary>
    /// Provides the interface for a view to display the 
    /// <see cref="MapTool"/> instances available to act on the <see cref="Map"/>.
    /// </summary>
    public interface IToolsView : IView
    {
        /// <summary>
        /// Gets or sets the currently selected tool.
        /// </summary>
        MapTool SelectedTool { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="MapTool"/> objects to display.
        /// </summary>
        IList<MapTool> Tools { get; set; }

        /// <summary>
        /// Event which requests the <see cref="ToolsPresenter"/>
        /// to change the <see cref="Map.ActiveTool"/>.
        /// </summary>
        event EventHandler<ToolChangeRequestedEventArgs> ToolChangeRequested;
    }
}