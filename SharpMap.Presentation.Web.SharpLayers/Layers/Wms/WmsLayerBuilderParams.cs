using System.ComponentModel;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Wms
{
    public class WmsLayerBuilderParams : LayerBuilderParamsBase
    {
        [
            SharpLayersSerialization(SerializedName = "params"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public WmsParameters WmsParameters { get; set; }

        [SharpLayersSerialization(SerializedName = "projection")]
        public string Projection { get; set; }

        [SharpLayersSerialization(SerializedName = "maxResolution")]
        public string MaxResolution { get; set; }

        [
            SharpLayersSerialization(SerializedName = "tileSize"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public Size TileSize { get; set; }

        [SharpLayersSerialization(SerializedName = "transitionEffect")]
        public TransitionEffects TransitionEffect { get; set; }
    }
}