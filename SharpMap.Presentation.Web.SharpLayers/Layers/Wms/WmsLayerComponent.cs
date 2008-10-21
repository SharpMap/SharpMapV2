using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Wms
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent",
        "SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent.js")]
    [TargetControlType(typeof (Control))]
    public class WmsLayerComponent : LayerComponent<WmsLayerBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent"; }
        }
    }
}