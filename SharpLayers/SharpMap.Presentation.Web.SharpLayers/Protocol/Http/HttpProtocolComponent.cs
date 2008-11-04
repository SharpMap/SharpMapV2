using System.Web.UI;
using AjaxControlToolkit;

[assembly:
    WebResource("SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Protocol.Http
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent",
        "SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.js")]
    [TargetControlType(typeof (Control))]
    public class HttpProtocolComponent : ProtocolComponent<HttpProtocolBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent"; }
        }
    }
}