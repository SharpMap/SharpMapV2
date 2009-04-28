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
using System.Web.UI;
using AjaxControlToolkit;
using SharpMap.Presentation.Web.SharpLayers.Controls.Containers;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent",
        "SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.js")]
    [TargetControlType(typeof (Control))]
    public abstract class ToolBaseComponent<TBuilderParams> : ComponentBase<TBuilderParams>,
                                                              IToolComponent<TBuilderParams>
        where TBuilderParams : IToolBuilderParams
    {
        [ExtenderControlProperty]
        [ClientPropertyName("targetMapHost")]
        [ComponentReference]
        public string TargetMapHost
        {
            get
            {
                Control c = GetParent<ToolPanel>(this);
                return c != null ? c.ClientID : GetParent<MapHostExtender>(this).ClientID;
            }
        }

        #region IToolComponent<TBuilderParams> Members

        IToolBuilderParams IToolComponent.BuilderParams
        {
            get { return BuilderParams; }
            set { BuilderParams = (TBuilderParams) value; }
        }

        #endregion

        protected static Control GetParent<TContainer>(Control tool)
        {
            for (Control c = tool; c != null; c = c.Parent)
            {
                if (c is TContainer)
                    return c;
            }
            return null;
        }
    }
}