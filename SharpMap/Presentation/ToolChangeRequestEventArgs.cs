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
using SharpMap.Presentation.Views;
using SharpMap.Tools;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Encapsulates arguments for the <see cref="IToolsView.ToolChangeRequested"/>
    /// event.
    /// </summary>
    public sealed class ToolChangeRequestedEventArgs : EventArgs
    {
        private readonly MapTool _requestedTool;

        /// <summary>
        /// Creates a new instance of a <see cref="ToolChangeRequestedEventArgs"/>
        /// with the given <see cref="MapTool"/>.
        /// </summary>
        /// <param name="requestedTool">
        /// The <see cref="MapTool"/> to request change to.
        /// </param>
        public ToolChangeRequestedEventArgs(MapTool requestedTool)
        {
            _requestedTool = requestedTool;
        }

        /// <summary>
        /// Gets the <see cref="MapTool"/> which the request is to change
        /// to.
        /// </summary>
        public MapTool RequestedTool
        {
            get { return _requestedTool; }
        }
    }
}