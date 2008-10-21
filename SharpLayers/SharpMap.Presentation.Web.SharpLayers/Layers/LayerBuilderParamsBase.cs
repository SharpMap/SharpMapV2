using System.ComponentModel;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Layers
{
    public class LayerBuilderParamsBase : ILayerBuilderParams
    {
        //[SharpLayersSerialization(SerializedName = "name")]
        //public string Name { get; set; }

        #region ILayerBuilderParams Members

        [SharpLayersSerialization(SerializedName = "isBaseLayer")]
        public bool IsBaseLayer { get; set; }

        [SharpLayersSerialization(SerializedName = "displayInLayerSwitcher")]
        public bool DisplayInLayerSwitcher { get; set; }

        [SharpLayersSerialization(SerializedName = "visibility")]
        public bool Visibility { get; set; }

        [SharpLayersSerialization(SerializedName = "units")]
        public MapUnits Units { get; set; }

        [SharpLayersSerialization(SerializedName = "maxExtent"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MaxExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "minExtent"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MinExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "displayOutsideMaxExtent")]
        public bool DisplayOutsideMaxExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "wrapDateLine")]
        public bool WrapDateLine { get; set; }

        [SharpLayersSerialization(SerializedName = "attribution")]
        public string Attribution { get; set; }

        #endregion
    }
}