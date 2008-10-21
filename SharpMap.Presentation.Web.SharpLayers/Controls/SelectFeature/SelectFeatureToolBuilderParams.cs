namespace SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature
{
    public class SelectFeatureToolBuilderParams : ToolBuilderParamsBase
    {
        [SharpLayersSerialization(SerializedName = "multipleKey")]
        public EventModifierKeys MultipleKey { get; set; }

        [SharpLayersSerialization(SerializedName = "toggleKey")]
        public EventModifierKeys ToggleKey { get; set; }

        [SharpLayersSerialization(SerializedName = "multiple")]
        public bool Multiple { get; set; }

        [SharpLayersSerialization(SerializedName = "clickout")]
        public bool ClickOut { get; set; }

        [SharpLayersSerialization(SerializedName = "toggle")]
        public bool Toggle { get; set; }

        [SharpLayersSerialization(SerializedName = "hover")]
        public bool Hover { get; set; }

        [SharpLayersSerialization(SerializedName = "box")]
        public bool Box { get; set; }


        [SharpLayersSerialization(SerializedName = "onSelect")]
        public string OnSelect { get; set; }

        [SharpLayersSerialization(SerializedName = "onUnselect")]
        public string OnUnselect { get; set; }

        [SharpLayersSerialization(SerializedName = "layer",
            SerializationFlags = SharpLayersSerializationFlags.GetComponent)]
        public string SelectableLayer { get; set; }
    }


    public enum EventModifierKeys
    {
        altKey,
        shiftKey
    }
}