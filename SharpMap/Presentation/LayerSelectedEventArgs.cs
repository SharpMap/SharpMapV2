using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;

namespace SharpMap.Presentation
{
    [Serializable]
    public class LayerActionEventArgs : EventArgs
    {
        private ILayer _layer;

        public LayerActionEventArgs(ILayer layer)
        {
            _layer = layer;
        }

        public ILayer Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }
    }
}
