using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using MapViewer.Controls;
using SharpMap;
using SharpMap.Layers;
using SharpMap.Presentation;
using SharpMap.Presentation.Presenters;
using SharpMap.Presentation.Views;

namespace MapViewer.Views
{
    public partial class LayersView : UserControl, ILayersView
    {
        public ContextMenuStrip LayersContextMenu { get; set; }

        private LayersPresenter _presenter;

        public LayersView()
        {
            InitializeComponent();
            layersTree1.RequestLayerEnabledChange += layersTree1_RequestLayerEnabledChange;
            layersTree1.LayerNodeClick += layersTree1_LayerNodeClick;
        }

        public Map Map
        {
            set
            {
                _presenter = new LayersPresenter(value, this);
                Layers = value.Layers;
            }
        }

        #region ILayersView Members

        /// <summary>
        /// Sets the layers to display attributes for.
        /// </summary>
        public IBindingList Layers
        {
            set { if (layersTree1 != null) layersTree1.Layers = value as BindingList<ILayer>; }
        }

        /// <summary>
        /// Disables the layer identified by <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The name of the <see cref="Layer"/> to disable.</param>
        public void DisableLayer(string layer)
        {
            if (LayersEnabledChangeRequested != null)
                LayersEnabledChangeRequested(this, new LayerActionEventArgs(layer, LayerAction.Disabled));
        }

        /// <summary>
        /// Enables the layer identified by <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The name of the <see cref="Layer"/> to enable.</param>
        public void EnableLayer(string layer)
        {
            if (LayersEnabledChangeRequested != null)
                LayersEnabledChangeRequested(this, new LayerActionEventArgs(layer, LayerAction.Enabled));
        }

        /// <summary>
        /// Disables the layer identified by <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The name of the <see cref="Layer"/> to disable.</param>
        public void DisableChildLayers(string layer)
        {
            if (LayerChildrenVisibilityChangeRequested != null)
                LayerChildrenVisibilityChangeRequested(this, new LayerActionEventArgs(layer, LayerAction.Disabled));
        }

        /// <summary>
        /// Enables the layer identified by <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The name of the <see cref="Layer"/> to enable.</param>
        public void EnableChildLayers(string layer)
        {
            if (LayerChildrenVisibilityChangeRequested != null)
                LayerChildrenVisibilityChangeRequested(this, new LayerActionEventArgs(layer, LayerAction.Enabled));
        }

        /// <summary>
        /// Make the layer identified by <paramref name="layer"/> (un)selectable.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="selectable"></param>
        public void SetFeaturesSelectable(string layer, bool selectable)
        {
            if (LayerSelectabilityChangeRequested != null)
                LayerSelectabilityChangeRequested(this,
                                                  new LayerActionEventArgs(layer,
                                                                           selectable
                                                                               ? LayerAction.Selected
                                                                               : LayerAction.Deselected));
        }

        /// <summary>
        /// Gets or sets a list of layers to show as selected.
        /// </summary>
        public IList<string> SelectedLayers
        {
            get
            {
                var lst = new List<string>();
                if (!DesignMode)
                    foreach (ILayer l in _presenter.Map.SelectedLayers)
                        lst.Add(l.LayerName);
                return lst;
            }
            set
            {
                if (!DesignMode)
                    if (LayersSelectionChangeRequested != null)
                        LayersSelectionChangeRequested(this, new LayerActionEventArgs(value, LayerAction.Enabled));
            }
        }

        /// <summary>
        /// Event triggered when the layers selection is requested to be changed
        /// by the associated <see cref="LayersPresenter"/>.
        /// </summary>
        public event EventHandler<LayerActionEventArgs> LayersSelectionChangeRequested;

        /// <summary>
        /// Event triggered when a layer is requested to be enabled or disabled
        /// by the associated <see cref="LayersPresenter"/>.
        /// </summary>
        public event EventHandler<LayerActionEventArgs> LayersEnabledChangeRequested;

        /// <summary>
        /// Event triggered when a layer is requested to have its children be
        /// enabled or disabled by the associated <see cref="LayersPresenter"/>.
        /// </summary>
        public event EventHandler<LayerActionEventArgs> LayerChildrenVisibilityChangeRequested;

        /// <summary>
        /// Event triggered when a layer is requested to have its selectability be
        /// enabled or disabled by the associated <see cref="LayersPresenter"/>.
        /// </summary>
        public event EventHandler<LayerActionEventArgs> LayerSelectabilityChangeRequested;

        /// <summary>
        /// Gets or sets the title to display on the view.
        /// </summary>
        public string Title
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        #endregion

        private void layersTree1_LayerNodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (LayersContextMenu != null)
                {
                    LayersContextMenu.Show(layersTree1, e.X, e.Y);
                }
            }
        }

        private void layersTree1_RequestLayerEnabledChange(object sender, LayerEnabledChangeEventArgs e)
        {
            if (e.Enabled)
                EnableLayer(e.Layer.LayerName);
            else
                DisableLayer(e.Layer.LayerName);
        }
    }
}