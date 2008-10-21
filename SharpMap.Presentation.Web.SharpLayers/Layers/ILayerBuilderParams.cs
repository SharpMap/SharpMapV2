namespace SharpMap.Presentation.Web.SharpLayers.Layers
{
    public interface ILayerBuilderParams : IBuilderParams
    {
        bool IsBaseLayer { get; set; }

        bool DisplayInLayerSwitcher { get; set; }

        bool Visibility { get; set; }

        MapUnits Units { get; set; }

        Bounds MaxExtent { get; set; }

        Bounds MinExtent { get; set; }

        bool DisplayOutsideMaxExtent { get; set; }

        bool WrapDateLine { get; set; }

        string Attribution { get; set; }
    }
}