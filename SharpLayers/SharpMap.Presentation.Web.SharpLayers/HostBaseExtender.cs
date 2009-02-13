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
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers
{
    [RequiredScript(typeof (OpenLayersExtender))]
    [RequiredScript(typeof (ComponentBase<>))]
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior",
        "SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.js")]
    [TargetControlType(typeof (Control))]
    public class HostBaseExtender<TBuilderParams> : ExtenderControlBase, IHaveBuilderParams<TBuilderParams>
        where TBuilderParams : IBuilderParams
    {
        #region IHaveBuilderParams<TBuilderParams> Members

        [
            ExtenderControlProperty(true, true),
            ClientPropertyName("builderParams"),
            Category("Behavior"),
            Description("The Builder Options"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public TBuilderParams BuilderParams
        {
            get { return GetPropertyValue("builderParams", default(TBuilderParams)); }
            set { SetPropertyValue("builderParams", value); }
        }

        #endregion

        protected override void RenderScriptAttributes(ScriptBehaviorDescriptor descriptor)
        {
            descriptor.ID = ClientID;
            SharpLayersScriptObjectBuilder.DescribeComponent(this, descriptor, this, this);
        }
    }
}