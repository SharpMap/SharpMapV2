using System;
using System.Collections.Generic;
using SharpMap.Layers;

namespace SharpMap
{
    [Serializable]
    public class LayersChangedEventArgs : EventArgs
    {
        private readonly LayersChangeType _changeType;
        private readonly object _layersAffected;

        public LayersChangedEventArgs(LayersChangeType changeType, ILayer layerAffected)
        {
            _changeType = changeType;
            _layersAffected = layerAffected;
        }
        
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
            get 
            {
                if (_layersAffected is ILayer)
                {
                    yield return _layersAffected as ILayer;
                }
                else
                {
                    IEnumerable<ILayer> layers = _layersAffected as IEnumerable<ILayer>;
                    foreach (ILayer layer in layers)
                    {
                        yield return layer;
                    }
                }
            }
        }
    }

    public enum LayersChangeType
    {
        Added,
        Removed,
        Enabled,
        Disabled,
        Selected,
        Deselected
    }
}
