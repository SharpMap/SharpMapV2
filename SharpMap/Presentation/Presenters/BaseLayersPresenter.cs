using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;
using SharpMap.Styles;

namespace SharpMap.Presentation
{
    public abstract class BaseLayersPresenter : ILayersPresenter
    {
        private object _activeLayerSetSync = new object();
        private List<ILayersView> _views = new List<ILayersView>();
        private List<ILayer> _layers = new List<ILayer>();
        private int _selectedLayerIndex = -1;
        private EventHandler<LayerActionEventArgs> activeLayerChangedDelegate;
        private EventHandler<LayerActionEventArgs> viewLayerStyleRequestedDelegate;

        protected BaseLayersPresenter(ILayersView view)
            : this(new ILayersView[] { view }) { }

        protected BaseLayersPresenter(IEnumerable<ILayersView> views)
        {
            activeLayerChangedDelegate = new EventHandler<LayerActionEventArgs>(HandleLayerSelectionChanged);
            viewLayerStyleRequestedDelegate = new EventHandler<LayerActionEventArgs>(HandleViewLayerStyleRequested);

            addAndConfigureViews(views);
        }

        private void addAndConfigureViews(IEnumerable<ILayersView> views)
        {
            foreach (ILayersView view in views)
            {
                view.LayerSelectionChanged += activeLayerChangedDelegate;
                view.ViewLayerStyleRequested += viewLayerStyleRequestedDelegate;
                _views.Add(view);
            }
        }

        private void HandleViewLayerStyleRequested(object sender, LayerActionEventArgs e)
        {

        }

        private void HandleLayerSelectionChanged(object sender, LayerActionEventArgs e)
        {
            ActiveLayer = e.Layer;
        }

        protected object ActiveLayerSetSync
        {
            get { return _activeLayerSetSync; }
        }

        protected int ActiveLayerIndex
        {
            get { return _selectedLayerIndex; }
            set { changeActiveLayerInternal(value); }
        }

        protected IList<ILayersView> Views
        {
            get { return _views; }
            set
            {
                foreach (ILayersView view in _views)
                {
                    view.LayerSelectionChanged -= activeLayerChangedDelegate;
                    view.ViewLayerStyleRequested -= viewLayerStyleRequestedDelegate;
                }

                _views.Clear();

                addAndConfigureViews(value);
            }
        }

        #region ILayersPresenter Members

        public IList<ILayer> Layers
        {
            get { return _layers; }
            protected set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                Layers.Clear();

                foreach (ILayer layer in value)
                    Layers.Add(layer);

                foreach (ILayersView view in Views)
                {
                    view.ClearLayers();
                    view.AddLayers(value);
                }
            }
        }

        public ILayer ActiveLayer
        {
            get
            {
                if (_selectedLayerIndex < 0 || _selectedLayerIndex >= _layers.Count)
                    return null;

                return Layers[ActiveLayerIndex];
            }
            protected set
            {
                if (!Layers.Contains(value))
                    throw new ArgumentException("Layer not found in Layers collection");

                changeActiveLayerInternal(Layers.IndexOf(value));
            }
        }

        public void SetLayerStyle(int index, Style style)
        {
            if (index < 0 || index >= Layers.Count)
                throw new ArgumentOutOfRangeException("index");

            setLayerStyleInternal(Layers[index], style);
        }

        public void SetLayerStyle(string name, Style style)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            setLayerStyleInternal(GetLayerByName(name), style);
        }

        public void EnableLayer(int index)
        {
            if (index < 0 || index >= Layers.Count)
                throw new ArgumentOutOfRangeException("index");

            changeLayerEnabled(Layers[index], false);
        }

        public void EnableLayer(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            changeLayerEnabled(GetLayerByName(name), true);
        }

        public void DisableLayer(int index)
        {
            if (index < 0 || index >= Layers.Count)
                throw new ArgumentOutOfRangeException("index");

            changeLayerEnabled(Layers[index], false);
        }

        public void DisableLayer(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            changeLayerEnabled(GetLayerByName(name), false);
        }

        public ILayer GetLayerByName(string name)
        {
            return _layers.Find(delegate(ILayer layer)
            {
                if (layer == null)
                    return false;

                return String.Compare(layer.LayerName, name, StringComparison.CurrentCultureIgnoreCase) == 0;
            });
        }
        #endregion

        #region Helper Functions

        private void changeLayerEnabled(ILayer layer, bool enabled)
        {
            layer.Style.Enabled = enabled;
        }

        private void setLayerStyleInternal(ILayer layer, Style style)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            if (style == null)
                throw new ArgumentNullException("style");

            layer.Style = style;
        }

        private void changeActiveLayerInternal(int layerIndex)
        {
            lock (ActiveLayerSetSync)
            {
                _selectedLayerIndex = layerIndex;

                foreach (ILayersView layerView in Views)
                {
                    layerView.SelectedLayer = Layers[_selectedLayerIndex];
                }
            }
        }
        #endregion
    }
}
