using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Presentation
{
    public class LayerStylePresenter
    {
        private SharpMap.Map.Map _map;
        private ILayerStyleView _view;

        public LayerStylePresenter(SharpMap.Map.Map map, ILayerStyleView view)
        {
            _map = map;
            _view = view;
        }
    }
}
