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

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent",
        "SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.js")]
    [TargetControlType(typeof(Control))]
    public abstract class ToolBaseComponent<TBuilderParams> : ComponentBase<TBuilderParams>,
                                                              IToolComponent<TBuilderParams>
        where TBuilderParams : IToolBuilderParams
    {
        #region IToolComponent<TBuilderParams> Members

        IToolBuilderParams IToolComponent.BuilderParams
        {
            get { return BuilderParams; }
            set { BuilderParams = (TBuilderParams)value; }
        }

        #endregion


        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            foreach (ScriptDescriptor descriptor in base.GetScriptDescriptors())
            {
                if (descriptor is ScriptComponentDescriptor)
                    ((ScriptComponentDescriptor)descriptor).AddComponentProperty("targetMapHost", Parent.ClientID);
                yield return descriptor;
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            //base.RenderBeginTag(writer);
        }
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            //base.RenderEndTag(writer);
        }
    }
}