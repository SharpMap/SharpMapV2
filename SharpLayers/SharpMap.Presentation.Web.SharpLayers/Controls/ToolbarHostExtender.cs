using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost.js", "text/javascript")]

namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    [ClientScriptResource("SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost",
        "SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost.js")]
    [TargetControlType(typeof(Control))]
    public class ToolbarHostExtender : HostBaseExtender<ToolbarBuilderParams>
    {
        private readonly CollectionBase<IToolComponent> _toolComponents =
            new CollectionBase<IToolComponent>((a, b) => a.ID != b.ID);

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public CollectionBase<IToolComponent> ToolComponents
        {
            get { return _toolComponents; }
        }

        [ComponentReference]
        [ClientPropertyName("targetMapId")]
        public string TargetMapId
        {
            get;
            set;
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (obj is IToolComponent)
                ToolComponents.Add((IToolComponent)obj);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            foreach (IToolComponent tool in ToolComponents)
                Controls.Add((Control)tool);
        }

        protected override System.Collections.Generic.IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
        {
            foreach (ScriptDescriptor descriptor in base.GetScriptDescriptors(targetControl))
            {
                if (descriptor is ScriptComponentDescriptor)
                    ((ScriptComponentDescriptor)descriptor).AddComponentProperty("targetMap", TargetMapId);
                yield return descriptor;
            }
        }
    }
}