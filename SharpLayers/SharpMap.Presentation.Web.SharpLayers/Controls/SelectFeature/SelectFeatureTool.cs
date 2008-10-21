using System.Web.UI;
using AjaxControlToolkit;

[assembly:
    WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool",
        "SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool.js")]
    [TargetControlType(typeof (Control))]
    public class SelectFeatureTool : ToolBaseComponent<SelectFeatureToolBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool"; }
        }
    }
}