using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent",
        "SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.js")]
    [TargetControlType(typeof(Control))]
    public abstract class ToolBaseComponent<TBuilderParams> : ComponentBase<TBuilderParams>,
                                                              IToolComponent<TBuilderParams>
        where TBuilderParams : IToolBuilderParams
    {
        #region IToolComponent<TBuilderParams> Members

        IToolBuilderParams IToolComponent.BuilderParams
        {
            get { return BuilderParams; }
            set { BuilderParams = (TBuilderParams)value; }
        }

        #endregion


        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            foreach (ScriptDescriptor descriptor in base.GetScriptDescriptors())
            {
                if (descriptor is ScriptComponentDescriptor)
                    ((ScriptComponentDescriptor)descriptor).AddComponentProperty("targetMapHost", Parent.ClientID);
                yield return descriptor;
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            //base.RenderBeginTag(writer);
        }
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            //base.RenderEndTag(writer);
        }
    }
}