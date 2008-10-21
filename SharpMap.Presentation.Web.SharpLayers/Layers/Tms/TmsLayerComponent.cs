using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Tms
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent",
        "SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent.js")]
    [TargetControlType(typeof (Control))]
    public class TmsLayerComponent : LayerComponent<TmsLayerBuilderParams>
    {
        //static TmsLayerComponent()
        //{
        //    LayerComponentCollectionBuilder.RegisterLayerType<TmsLayerComponent, TmsLayerBuilderParams>();
        //}

        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent"; }
        }
    }
}