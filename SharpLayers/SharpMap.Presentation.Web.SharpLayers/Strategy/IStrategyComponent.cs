using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Strategy
{
    public interface IStrategyComponent : IScriptControl
    {
        string ID { get; set; }
        IStrategyBuilderParams BuilderParams { get; set; }
    }

    public interface IStrategyComponent<TStrategyBuilderParams> : IStrategyComponent where TStrategyBuilderParams : IStrategyBuilderParams
    {
        new TStrategyBuilderParams BuilderParams { get; set; }
    }
}