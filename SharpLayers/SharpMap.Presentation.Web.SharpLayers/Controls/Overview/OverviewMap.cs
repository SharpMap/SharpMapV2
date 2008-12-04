using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using AjaxControlToolkit;

[assembly: System.Web.UI.WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls.Overview
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap", "SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap.js")]
    [TargetControlType(typeof(Control))]
    public class OverviewMap : ToolBaseComponent<OverviewMapBuilderParams>
    {

        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap"; }
        }
    }
}
