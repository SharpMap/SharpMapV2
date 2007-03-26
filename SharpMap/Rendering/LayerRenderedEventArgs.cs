using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;

namespace SharpMap.Rendering
{
    public class LayerRenderedEventArgs : EventArgs
    {
        private ILayer _renderedLayer;

        public LayerRenderedEventArgs(ILayer layer)
        {
            _renderedLayer = layer;
        }

        public ILayer Layer
        {
            get { return _renderedLayer; }
        }
    }
}
