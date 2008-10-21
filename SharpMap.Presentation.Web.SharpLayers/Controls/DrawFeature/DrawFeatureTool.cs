using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

[assembly:
    WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool",
        "SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool.js")]
    [TargetControlType(typeof(Control))]
    public class DrawFeatureTool : ToolBaseComponent<DrawFeatureToolBuilderParams>
    {


        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool"; }
        }
    }
}