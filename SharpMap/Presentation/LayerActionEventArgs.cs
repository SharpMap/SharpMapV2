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

namespace SharpMap.Presentation
{
    [Serializable]
    public class LayerActionEventArgs : EventArgs
    {
        private readonly Object _layers;
        private readonly LayerAction _layerAction;

        public LayerActionEventArgs(String layer, LayerAction layerAction)
        {
            if (layer == null) throw new ArgumentNullException("layer");
            _layers = layer;
            _layerAction = layerAction;
        }

        public LayerActionEventArgs(IEnumerable<String> layers, LayerAction layerAction)
        {
            if (layers == null) throw new ArgumentNullException("layers");
            _layers = layers;
            _layerAction = layerAction;
        }

        public IEnumerable<String> Layers
        {
            get 
            { 
                if(_layers is String)
                {
                    yield return _layers as String;
                }
                else
                {
                    IEnumerable<String> layers = _layers as IEnumerable<String>;
                    foreach(String layer in layers)
                    {
                        yield return layer;
                    }
                }
            }
        }

        public LayerAction LayerAction
        {
            get { return _layerAction; }
        }
    }

    public enum LayerAction
    {
        Enabled,
        Disabled,
        Selected,
        Deselected
    }
}
