namespace SharpMap.Presentation.Web.SharpLayers.Controls
{
    public interface IToolComponent
    {
        string ID { get; set; }
        IToolBuilderParams BuilderParams { get; set; }
    }

    public interface IToolComponent<TToolBuilderParams> : IToolComponent
    {
        new TToolBuilderParams BuilderParams { get; set; }
    }
}