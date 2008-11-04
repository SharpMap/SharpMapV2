using System.Collections.Generic;
using System.Web.UI;
using SharpMap.Presentation.Web.SharpLayers.Format;

namespace SharpMap.Presentation.Web.SharpLayers.Protocol.Http
{
    public class HttpProtocolBuilderParams : BuilderParamsBase, IProtocolBuilderParams
    {
        private readonly Dictionary<string, object> _headers = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _params = new Dictionary<string, object>();

        [SharpLayersSerialization(SerializedName = "url")]
        public string Url { get; set; }

        [SharpLayersSerialization(SerializedName = "headers")]
        public IDictionary<string, object> Headers
        {
            get { return _headers; }
        }

        [SharpLayersSerialization(SerializedName = "params")]
        public IDictionary<string, object> Params
        {
            get { return _params; }
        }

        #region IProtocolBuilderParams Members

        [SharpLayersSerialization(SerializedName = "format"), PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public IFormat Format { get; set; }

        #endregion
    }
}