namespace SharpMap.Presentation.Web.SharpLayers.Controls.Overview
{
    public class OverviewMapBuilderParams : ToolBuilderParamsBase
    {
        [SharpLayersSerialization(SerializedName = "size", SerializationFlags = SharpLayersSerializationFlags.CreateClientClass)]
        public Size Size { get; set; }

        [SharpLayersSerialization(SerializedName = "minRectSize")]
        public int MinimumRectangleSize { get; set; }

        [SharpLayersSerialization(SerializedName = "minRatio")]
        public int MinRatio { get; set; }

        [SharpLayersSerialization(SerializedName = "maxRatio")]
        public int MaxRatio { get; set; }
    }
}