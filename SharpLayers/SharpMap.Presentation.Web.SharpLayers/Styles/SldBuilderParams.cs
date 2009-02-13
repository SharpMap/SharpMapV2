using System.Web.UI;
using AjaxControlToolkit;

namespace SharpMap.Presentation.Web.SharpLayers.Styles
{
    public class SldBuilderParams : BuilderParamsBase
    {
        [ExtenderControlProperty]
        [ClientPropertyName("sldDocumentXml"),
         PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
        public string SldDocumentXml { get; set; }

        [ExtenderControlProperty]
        [UrlProperty]
        [ClientPropertyName("sldDocumentUri")]
        public string SldDocumentUri { get; set; }
    }
}