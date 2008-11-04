using SharpMap.Presentation.Web.SharpLayers.Format;

namespace SharpMap.Presentation.Web.SharpLayers.Protocol
{
    public interface IProtocolBuilderParams : IBuilderParams
    {
        [SharpLayersSerialization(SerializedName = "format")]
        IFormat Format { get; set; }
    }
}