using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Styles
{
    public class SldBuilderParams : BuilderParamsBase
    {
        [SharpLayersSerialization(SerializedName = "sldDocumentXml"),
        PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string SldDocumentXml { get; set; }


        [SharpLayersSerialization(SerializationFlags = SharpLayersSerializationFlags.Uri, SerializedName = "sldDocumentUri")]
        public string SldDocumentUri { get; set; }
    }
}