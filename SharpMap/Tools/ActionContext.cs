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
using SharpMap.Data;

namespace SharpMap.Tools
{
    public class ActionContext
    {
        private SharpMap.Map _map;

        public ActionContext(SharpMap.Map map)
        {
            _map = map;
        }

        public ReadOnlyCollection<ILayer> SelectedLayers
        {
            get { return new ReadOnlyCollection<ILayer>(_map.SelectedLayers); }
        }
    }
}
