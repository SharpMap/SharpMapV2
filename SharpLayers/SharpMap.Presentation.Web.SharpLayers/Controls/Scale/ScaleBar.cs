using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls.Scale
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar",
        "SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar.js")]
    [TargetControlType(typeof (Control))]
    public class ScaleBar : ToolBaseComponent<ScaleBarBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar"; }
        }
    }
}