namespace SharpMap.Presentation.Web.SharpLayers.Controls.Edit
{
    public class EditingToolsBuilderParams : ToolBuilderParamsBase
    {
        //TODO : this has to work through the naming containers
        [SharpLayersSerialization(SerializedName = "layer",
            SerializationFlags = SharpLayersSerializationFlags.GetComponent)]
        public string EditableLayerId { get; set; }
    }
}