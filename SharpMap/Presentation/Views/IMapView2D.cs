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
using System.IO;
using System.Text;

using SharpMap.Geometries;
using SharpMap.Rendering;
using SharpMap.Styles;

namespace SharpMap.Presentation
{
    public interface IMapView2D
    {
        ViewSize2D ViewSize { get; set; }
        void ShowRenderedObject(ViewPoint2D location, object renderedObject);
        event EventHandler<MapActionEventArgs> Hover;
        event EventHandler<MapActionEventArgs> BeginAction;
        event EventHandler<MapActionEventArgs> MoveTo;
        event EventHandler<MapActionEventArgs> EndAction;
        event EventHandler ViewSizeChanged;
    }

    public class MapActionEventArgs : EventArgs
    {
        private ViewPoint2D _actionPoint;

        public MapActionEventArgs(ViewPoint2D actionPoint)
        {
            _actionPoint = actionPoint;
        }

        public ViewPoint2D ActionPoint
        {
            get { return _actionPoint; }
            protected set { _actionPoint = value; }
        }
    }
}
