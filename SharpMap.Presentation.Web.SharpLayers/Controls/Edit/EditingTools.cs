using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using AjaxControlToolkit;

[assembly: System.Web.UI.WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls.Edit
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools", "SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.js")]
    [TargetControlType(typeof(Control))]
    public class EditingTools : ToolBaseComponent<EditingToolsBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools"; }
        }
    }
}
