using System;
using System.ComponentModel;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Serializable]
    public class MapHostBuilderParams : BuilderParamsBase
    {
        private readonly CollectionBase<Resolution> _resolutions = new CollectionBase<Resolution>((a, b) => a != b);

        [SharpLayersSerialization(SerializedName = "tileSize",
            SerializationFlags = SharpLayersSerializationFlags.CreateClientClass),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Size TileSize { get; set; }

        [SharpLayersSerialization(SerializedName = "projection")]
        public string Projection { get; set; }

        [SharpLayersSerialization(SerializedName = "units")]
        public MapUnits Units { get; set; }

        [SharpLayersSerialization(SerializedName = "resolutions"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public CollectionBase<Resolution> Resolutions
        {
            get { return _resolutions; }
        }


        [SharpLayersSerialization(SerializedName = "maxResolution")]
        public double? MaxResolution { get; set; }

        [SharpLayersSerialization(SerializedName = "minResolution")]
        public double? MinResolution { get; set; }

        [SharpLayersSerialization(SerializedName = "maxScale")]
        public double? MaxScale { get; set; }

        [SharpLayersSerialization(SerializedName = "minScale")]
        public double? MinScale { get; set; }


        [SharpLayersSerialization(SerializedName = "maxExtent",
            SerializationFlags = SharpLayersSerializationFlags.CreateClientClass),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MaxExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "minExtent",
            SerializationFlags = SharpLayersSerializationFlags.CreateClientClass),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MinExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "restrictedExtent",
            SerializationFlags = SharpLayersSerializationFlags.CreateClientClass),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds RestrictedExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "numZoomLevels")]
        private int? NumZoomLevels { get; set; }

        [SharpLayersSerialization(SerializedName = "theme")]
        public string Theme { get; set; }

        [SharpLayersSerialization(SerializedName = "fallThrough")]
        public bool FallThrough { get; set; }
    }
}