using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;

namespace SharpMap.Map
{
    public class LayersChangedEventArgs : EventArgs
    {
        private IEnumerable<ILayer> _changedLayers;
        private LayersChangedType _changeType;

        internal LayersChangedEventArgs(ILayer changedLayer, LayersChangedType changeType)
        {
            List<ILayer> layers = new List<ILayer>();
            layers.Add(changedLayer);
            _changedLayers = layers;
            _changeType = changeType;
        }

        internal LayersChangedEventArgs(IEnumerable<ILayer> changedLayers, LayersChangedType changeType)
        {
            _changedLayers = changedLayers;
            _changeType = changeType;
        }

        public IEnumerable<ILayer> ChangedLayers
        {
            get { return _changedLayers; }
        }

        public LayersChangedType ChangeType
        {
            get { return _changeType; }
        }
    }
}
