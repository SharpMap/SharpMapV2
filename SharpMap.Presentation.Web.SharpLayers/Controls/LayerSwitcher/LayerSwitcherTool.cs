using System.Web.UI;
using AjaxControlToolkit;

[assembly:
    WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool",
        "SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool.js")]
    [TargetControlType(typeof (Control))]
    public class LayerSwitcherTool : ToolBaseComponent<LayerSwitcherToolBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool"; }
        }
    }
}