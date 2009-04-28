/*
 *  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
 *  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using SharpMap.Presentation.Web.SharpLayers.Controls;
using SharpMap.Presentation.Web.SharpLayers.Layers;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers
{
    [ParseChildren(true, "LayerComponents")]
    [PersistChildren(true)]
    [RequiredScript("SharpMap.Presentation.Web.SharpLayers.OpenLayers.OpenLayers.js")]
    [Designer(typeof (MapHostDesigner))]
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.MapHostBehavior",
        "SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.js")]
    [TargetControlType(typeof (Panel))]
    public class MapHostExtender : HostBaseExtender<MapHostBuilderParams>
    {
        private readonly CollectionBase<ILayerComponent> _layerComponents =
            new CollectionBase<ILayerComponent>(delegate(ILayerComponent a, ILayerComponent b) { return a.ID != b.ID; });

        private readonly CollectionBase<IToolComponent> _toolComponents =
            new CollectionBase<IToolComponent>(delegate(IToolComponent a, IToolComponent b) { return a.ID != b.ID; });

        public MapHostExtender()
        {
            _toolComponents.ItemAdded += _toolComponents_ItemAdded;
            _toolComponents.ItemRemoved += _toolComponents_ItemRemoved;
            _layerComponents.ItemAdded += _layerComponents_ItemAdded;
            _layerComponents.ItemRemoved += _layerComponents_ItemRemoved;
        }

        [
            Category("Behavior"),
            Description("The layer components collection"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public CollectionBase<ILayerComponent> LayerComponents
        {
            get { return _layerComponents; }
        }

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public CollectionBase<IToolComponent> Tools
        {
            get { return _toolComponents; }
        }

        private void _layerComponents_ItemRemoved(object sender, CollectionBase<ILayerComponent>.ItemEventArgs e)
        {
            if (Controls.Contains((Control) e.Item))
                Controls.Remove((Control) e.Item);
        }

        private void _layerComponents_ItemAdded(object sender, CollectionBase<ILayerComponent>.ItemEventArgs e)
        {
            if (!Controls.Contains((Control) e.Item))
                Controls.Add((Control) e.Item);
        }

        private void _toolComponents_ItemRemoved(object sender, CollectionBase<IToolComponent>.ItemEventArgs e)
        {
            if (Controls.Contains((Control) e.Item))
                Controls.Remove((Control) e.Item);
        }

        private void _toolComponents_ItemAdded(object sender, CollectionBase<IToolComponent>.ItemEventArgs e)
        {
            if (!Controls.Contains((Control) e.Item))
                Controls.Add((Control) e.Item);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            foreach (ComponentBase componentBase in LayerComponents)
                Controls.Add(componentBase);

            foreach (IToolComponent toolComponent in Tools)
                Controls.Add((Control) toolComponent);
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (obj as ILayerComponent != null)
                LayerComponents.Add((ILayerComponent) obj);
            else if (obj as IToolComponent != null)
                Tools.Add((IToolComponent) obj);
        }
    }
}