namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    public class ToolBuilderParamsBase : BuilderParamsBase, IToolBuilderParams
    {
        [SharpLayersSerialization(SerializedName = "div")]
        public string TargetElementId
        {
            get;
            set;
        }
    }
}