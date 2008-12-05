using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl",
        "SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.js")]
    [TargetControlType(typeof(Control))]
    public class GenericOLControl : ToolBaseComponent<GenericOLToolBuilderParams>
    {
        protected override string ScriptComponentName
        {
            get { return "SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl"; }
        }
    }

    public class GenericOLToolBuilderParams : ToolBuilderParamsBase
    {
        [SharpLayersSerialization(SerializedName = "openLayersClassName")]
        public string OpenLayersClassName { get; set; }
    }
}