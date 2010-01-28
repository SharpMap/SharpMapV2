using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Layers.Yahoo.YahooLayerComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Yahoo
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Layers.Yahoo.YahooLayerComponent",
        "SharpMap.Presentation.Web.SharpLayers.Layers.Yahoo.YahooLayerComponent.js")]
    [TargetControlType(typeof (Control))]
    public class YahooLayerComponent : LayerComponent<YahooLayerBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Layers.Yahoo.YahooLayerComponent"; }
        }
    }
}