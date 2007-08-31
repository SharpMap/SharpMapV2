using System;
using System.Collections.Generic;
using SharpMap.Layers;

namespace SharpMap
{
    [Serializable]
    public class LayersChangedEventArgs : EventArgs
    {
        private readonly LayersChangeType _changeType;
        private readonly IEnumerable<ILayer> _layersAffected;
	
        
        public LayersChangedEventArgs(LayersChangeType changeType, IEnumerable<ILayer> layersAffected)
        {
            _changeType = changeType;
            _layersAffected = layersAffected;
        }

        public LayersChangeType ChangeType
        {
            get { return _changeType; }
        }

        public IEnumerable<ILayer> LayersAffected
        {
            get { return _layersAffected; }
        }
    }

    public enum LayersChangeType
    {
        Added,
        Removed,
        Enabled,
        Disabled
    }
}
