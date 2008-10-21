namespace SharpMap.Presentation.Web.SharpLayers.Layers.Vector
{
    public class VectorLayerBuilderParams : LayerBuilderParamsBase
    {
        [SharpLayersSerialization(SerializedName = "geometryType")]
        public VectorGeometryType? LimitToGeometryType { get; set; }
    }
}