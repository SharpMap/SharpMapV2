using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent.js", "text/javascript")
]

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Vector
{
    [
        ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent",
            "SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent.js"),
        TargetControlType(typeof (Control))
    ]
    public class VectorLayerComponent : LayerComponent<VectorLayerBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent"; }
        }
    }
}