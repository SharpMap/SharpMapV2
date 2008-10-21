using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers
{
    [RequiredScript(typeof(OpenLayersExtender))]
    [RequiredScript(typeof(ComponentBase<>))]
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior",
        "SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.js")]
    [TargetControlType(typeof(Control))]
    public class HostBaseExtender<TBuilderParams> : ExtenderControlBase, IHaveBuilderParams<TBuilderParams>
        where TBuilderParams : IBuilderParams
    {
        #region IHaveBuilderParams<TBuilderParams> Members

        [
            ExtenderControlProperty, DefaultValue("{}"), ClientPropertyName("builderParams"),
            Category("Behavior"),
            Description("The Builder Options"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public TBuilderParams BuilderParams
        {
            get { return GetPropertyValue("builderParams", default(TBuilderParams)); }
            set { SetPropertyValue("builderParams", value); }
        }

        #endregion

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new BuilderParamsJavascriptConverter(FindControl) });

            foreach (var descriptor in base.GetScriptDescriptors(targetControl))
            {
                if (descriptor is ScriptComponentDescriptor)
                    ((ScriptComponentDescriptor)descriptor).AddScriptProperty("builderParams", serializer.Serialize(BuilderParams));
                yield return descriptor;
            }
        }

       
    }
}