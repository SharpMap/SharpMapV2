using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Protocol
{
    public interface IProtocolComponent : IScriptControl
    {
        string ID { get; set; }
        IProtocolBuilderParams BuilderParams { get; set; }
    }

    public interface IProtocolComponent<TBuilderParams> : IProtocolComponent
        where TBuilderParams : IProtocolBuilderParams
    {
        new TBuilderParams BuilderParams { get; set; }
    }
}