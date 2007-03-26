using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;
using SharpMap.Styles;

namespace SharpMap.Presentation
{
    public interface ILayersPresenter
    {
        IList<ILayer> Layers { get; }
        ILayer ActiveLayer { get; }

        void SetLayerStyle(int index, Style style);
        void SetLayerStyle(string name, Style style);
        void EnableLayer(int index);
        void EnableLayer(string name);
        void DisableLayer(int index);
        void DisableLayer(string name);
        ILayer GetLayerByName(string layerName);
    }
}
