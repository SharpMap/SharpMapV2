using System.ComponentModel;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Tms
{
    public class TmsLayerBuilderParams : LayerBuilderParamsBase, IGridBasedLayerBuilderParams
    {
        private readonly CollectionBase<ResourceUri> _urls = new CollectionBase<ResourceUri>((a, b) => a.Uri != b.Uri);

        [SharpLayersSerialization(SerializedName = "layername")]
        public string TileCatalogName { get; set; }

        [SharpLayersSerialization(SerializedName = "type")]
        public string ImageExtension { get; set; }

        [SharpLayersSerialization(SerializedName = "url"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public CollectionBase<ResourceUri> TmsServerUrls
        {
            get { return _urls; }
        }

        #region IGridBasedLayerBuilderParams Members

        [SharpLayersSerialization(SerializedName = "tileSize"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Size TileSize { get; set; }

        [SharpLayersSerialization(SerializedName = "transitionEffect")]
        public TransitionEffects TransitionEffect { get; set; }

        #endregion
    }
}