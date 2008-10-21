namespace SharpMap.Presentation.Web.SharpLayers
{
    public interface IHaveBuilderParams<TBuilderParams> where TBuilderParams : IBuilderParams
    {
        TBuilderParams BuilderParams { get; set; }
    }
}