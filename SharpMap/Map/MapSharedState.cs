using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMap
{
    public static class MapSharedState
    {
        private static IEnumerable<FeatureDataRow> _selectedFeatures;
        private static IList<ILayer> _layers;
        private static ILayer _activeLayer;
        private static ToolSet _selectedTool;

        private static object _selectedFeaturesSync = new object();
        private static object _layersSync = new object();
        private static object _activeLayerSync = new object();
        private static object _selectedToolSync = new object();

        public static event EventHandler SelectedFeaturesChanged;

        public static IEnumerable<FeatureDataRow> SelectedFeatures
        {
            get 
            {
                lock (_selectedFeaturesSync)
                    return _selectedFeatures; 
            }
            set 
            {
                lock (_selectedFeaturesSync)
                {
                    _selectedFeatures = value;
                    OnSelectedFeaturesChanged();
                }
            }
        }

        public static event EventHandler LayersChanged;

        public static IList<ILayer> Layers
        {
            get 
            {
                lock (_layersSync)
                    return _layers; 
            }
            set 
            {
                lock (_layersSync)
                {
                    _layers = value;
                    OnLayersChanged();
                }
            }
        }

        public static event EventHandler ActiveLayerChanged;

        public static ILayer ActiveLayer
        {
            get 
            {
                lock (_activeLayerSync)
                    return _activeLayer; 
            }
            set 
            {
                lock (_activeLayerSync)
                {
                    _activeLayer = value;
                    OnActiveLayerChanged();
                }
            }
        }

        public static event EventHandler SelectedToolChanged;

        public static ToolSet SelectedTool
        {
            get 
            {
                lock (_selectedToolSync)
                    return _selectedTool; 
            }
            set
            {
                lock (_selectedToolSync)
                {
                    _selectedTool = value;
                    OnSelectedToolChanged();
                }
            }
        }

        private static void OnSelectedFeaturesChanged()
        {
            EventHandler e = SelectedFeaturesChanged;

            if (e != null)
                e(null, EventArgs.Empty);
        }

        private static void OnSelectedToolChanged()
        {
            EventHandler e = SelectedToolChanged;

            if (e != null)
                e(null, EventArgs.Empty);
        }

        private static void OnLayersChanged()
        {
            EventHandler e = LayersChanged;

            if (e != null)
                e(null, EventArgs.Empty);
        }

        private static void OnActiveLayerChanged()
        {
            EventHandler e = ActiveLayerChanged;

            if (e != null)
                e(null, EventArgs.Empty);
        }
    }
}
