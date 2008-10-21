namespace SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature
{
    public class DrawFeatureToolBuilderParams : ToolBuilderParamsBase
    {
        [SharpLayersSerialization(SerializedName = "layer",
                    SerializationFlags = SharpLayersSerializationFlags.GetComponent)]
        public string EditableLayerId { get; set; }

    }
}