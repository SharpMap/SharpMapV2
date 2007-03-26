using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;

namespace SharpMap.Presentation
{
    public interface ILayersView
    {
        void AddLayer(ILayer layer);
        void AddLayers(IEnumerable<ILayer> layers);
        void RemoveLayer(ILayer layer);
        void ClearLayers();
        void DisableLayer(ILayer layer);
        void EnableLayer(ILayer layer);
        ILayer SelectedLayer { get; set; }
        ILayersPresenter Presenter { set; }
        event EventHandler<LayerActionEventArgs> LayerSelectionChanged;
        event EventHandler<LayerActionEventArgs> ViewLayerStyleRequested;
        event EventHandler<LayerActionEventArgs> LayerEnabledChanged;
    }
}
