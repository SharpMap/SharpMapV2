using System.ComponentModel;
using System.Windows.Forms;
using SharpMap.Layers;

namespace MapViewer.Controls
{
    public class LayersTree : CustomTreeView
    {
        public LayersTree()
        {
            CheckBoxes = true;
        }

        private BindingList<ILayer> layers;

        public BindingList<ILayer> Layers
        {
            get { return layers; }
            set
            {
                UnwireLayers(layers);
                layers = value;
                if (value != null)
                    WireLayers(layers);
            }
        }

        private void WireLayers(BindingList<ILayer> newLayers)
        {
            if (newLayers != null)
            {
                newLayers.ListChanged += layers_ListChanged;
                foreach (ILayer lyr in newLayers)
                    Nodes.Add(NodeFactory.CreateLayerNode(lyr));
            }
            else
            {
                Nodes.Clear();
            }
        }

        private void layers_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                Nodes.Insert(e.NewIndex, NodeFactory.CreateLayerNode(layers[e.NewIndex]));
                return;
            }

            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                if (e.OldIndex > -1)
                {
                    TreeNode tn = Nodes[e.OldIndex];
                    Nodes.Remove(tn);
                }
            }
        }

        private void UnwireLayers(BindingList<ILayer> layers)
        {
            if (layers != null)
                layers.ListChanged -= layers_ListChanged;
        }


        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Layers = null;
                base.Dispose(disposing);
            }
        }
    }
}