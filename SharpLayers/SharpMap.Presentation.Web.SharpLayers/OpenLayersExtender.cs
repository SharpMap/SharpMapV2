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

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.OpenLayers.OpenLayers.js", "text/javascript")]
[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.OpenLayers.OpenLayers_lf.js", "text/javascript")]
[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.OpenLayersBehavior.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Designer(typeof (OpenLayersDesigner))]
#if DEBUG
    [ClientScriptResource(null, "SharpMap.Presentation.Web.SharpLayers.OpenLayers.OpenLayers_lf.js")]
#else
    [ClientScriptResource(null, "SharpMap.Presentation.Web.SharpLayers.OpenLayers.OpenLayers.js")]
#endif
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.OpenLayersBehaviour",
        "SharpMap.Presentation.Web.SharpLayers.OpenLayersBehavior.js")]
    [TargetControlType(typeof (Control))]
    public abstract class OpenLayersExtender : ExtenderControlBase
    {
    }
}