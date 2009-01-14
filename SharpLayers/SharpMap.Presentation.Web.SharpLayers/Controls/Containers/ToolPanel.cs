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
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls.Containers
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel",
        "SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel.js")]
    [TargetControlType(typeof (Control))]
    public class ToolPanel : ToolBaseComponent<ToolPanelBuilderParams>, INamingContainer
    {
        private readonly CollectionBase<IToolComponent> _childTools =
            new CollectionBase<IToolComponent>((o, a) => o.ID != a.ID);

        public ToolPanel()
        {
            _childTools.ItemAdded += _childTools_ItemAdded;
            _childTools.ItemRemoved += _childTools_ItemRemoved;
        }

        [
            SharpLayersSerialization(SerializedName = "childControlHostIds"),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public CollectionBase<IToolComponent> ChildTools
        {
            get { return _childTools; }
        }

        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel"; }
        }

        private void _childTools_ItemAdded(object sender, CollectionBase<IToolComponent>.ItemEventArgs e)
        {
            Controls.Add(e.Item as Control);
        }

        private void _childTools_ItemRemoved(object sender, CollectionBase<IToolComponent>.ItemEventArgs e)
        {
            Controls.Remove(e.Item as Control);
        }

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            foreach (ScriptDescriptor dexc in base.GetScriptDescriptors())
            {
                var descriptor = dexc as ScriptComponentDescriptor;
                if (descriptor != null)
                {
                    var lst = new List<string>();
                    foreach (IToolComponent c in ChildTools)
                        lst.Add(((Control) c).ClientID);

                    string s = "['" + string.Join("','", lst.ToArray()) + "']";

                    descriptor.AddScriptProperty("childControlHostIds", s);
                    yield return descriptor;
                }
                else
                {
                    yield return dexc;
                }
            }
        }
    }
}