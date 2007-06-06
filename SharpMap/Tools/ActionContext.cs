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
        private SharpMap.Map.Map _map;

        public ActionContext(SharpMap.Map.Map map)
        {
            _map = map;
        }

        public ReadOnlyCollection<ILayer> SelectedLayers
        {
            get { return new ReadOnlyCollection<ILayer>(_map.SelectedLayers); }
        }

        public ReadOnlyCollection<FeatureDataRow> SelectedFeatures
        {
            get { return new ReadOnlyCollection<FeatureDataRow>(_map.SelectedFeatures); }
        }
    }
}
