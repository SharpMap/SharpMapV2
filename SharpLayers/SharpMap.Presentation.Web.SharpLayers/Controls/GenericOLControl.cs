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

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl",
        "SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.js")]
    [TargetControlType(typeof (Control))]
    public class GenericOLControl : ToolBaseComponent<GenericOLToolBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl"; }
        }
    }

    public class GenericOLToolBuilderParams : ToolBuilderParamsBase
    {
        [ClientPropertyName("openLayersClassName")]
            public string OpenLayersClassName { get; set; }
    }
}