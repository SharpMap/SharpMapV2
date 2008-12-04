using System.Web.UI;
using AjaxControlToolkit;
using SharpMap.Presentation.Web.SharpLayers.Controls.NavToolbar;

[assembly:
    WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.js",
        "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls.Nav
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar",
        "SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.js")]
    [TargetControlType(typeof(Control))]
    public class NavToolbar : ToolBaseComponent<NavToolbarBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar"; }
        }
    }
}