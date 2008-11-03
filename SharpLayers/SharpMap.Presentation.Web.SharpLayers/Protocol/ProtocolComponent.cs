using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Protocol.ProtocolComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Protocol
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Protocol.ProtocolComponent",
        "SharpMap.Presentation.Web.SharpLayers.Protocol.ProtocolComponent.js")]
    [TargetControlType(typeof(Control))]
    public abstract class ProtocolComponent<TProtocolBuilderParams> : ComponentBase<TProtocolBuilderParams>
        where TProtocolBuilderParams : IProtocolBuilderParams
    {
    }
}