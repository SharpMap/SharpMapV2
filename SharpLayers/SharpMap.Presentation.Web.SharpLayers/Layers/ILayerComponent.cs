using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Layers
{
    public interface ILayerComponent : IScriptControl
    {
        string ID { get; set; }
        ILayerBuilderParams BuilderParams { get; set; }
    }

    public interface ILayerComponent<TBuilderParams> : ILayerComponent
    {
        new TBuilderParams BuilderParams { get; set; }
    }
}