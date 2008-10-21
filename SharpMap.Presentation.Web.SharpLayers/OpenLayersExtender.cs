using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.OpenLayers.OpenLayers.js", "text/javascript")]
[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.OpenLayersBehavior.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Designer(typeof (OpenLayersDesigner))]
    [ClientScriptResource(null, "SharpMap.Presentation.Web.SharpLayers.OpenLayers.OpenLayers.js")]
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.OpenLayersBehaviour",
        "SharpMap.Presentation.Web.SharpLayers.OpenLayersBehavior.js")]
    [TargetControlType(typeof (Control))]
    public abstract class OpenLayersExtender : ExtenderControlBase
    {
    }
}