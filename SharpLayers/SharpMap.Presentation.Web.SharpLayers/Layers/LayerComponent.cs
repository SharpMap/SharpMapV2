using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Layers
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent",
        "SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.js")]
    public abstract class LayerComponent<TLayerBuildParams>
        : ComponentBase<TLayerBuildParams>, ILayerComponent<TLayerBuildParams>
        where TLayerBuildParams : ILayerBuilderParams
    {
        public string Name { get; set; }

        #region ILayerComponent<TLayerBuildParams> Members

        ILayerBuilderParams ILayerComponent.BuilderParams
        {
            get { return BuilderParams; }
            set { BuilderParams = (TLayerBuildParams) value; }
        }

        #endregion

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            //base.RenderBeginTag(writer);
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            //base.RenderEndTag(writer);
        }

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            foreach (ScriptComponentDescriptor componentDescriptor in base.GetScriptDescriptors())
            {
                componentDescriptor.AddComponentProperty("targetMapHost", Parent.ClientID);
                componentDescriptor.AddProperty("name", Name);
                yield return componentDescriptor;
            }
        }


    }
}