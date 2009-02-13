/*
*  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
*  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
*  www.newgrove.com; you can redistribute it and/or modify it under the terms 
*  of the current GNU Lesser General Public License (LGPL) as published by and 
*  available from the Free Software Foundation, Inc., 
*  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
*  This program is distributed without any warranty; 
*  without even the implied warranty of merchantability or fitness for purpose.  
*  See the GNU Lesser General Public License for the full details. 
*  
*  Author: John Diss 2008
* 
*/
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;
using SharpMap.Presentation.Web.SharpLayers.Format;

namespace SharpMap.Presentation.Web.SharpLayers.Protocol.Http
{
    public class HttpProtocolBuilderParams : ProtocolBuilderParamsBase
    {
        private readonly Dictionary<string, object> _headers = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _params = new Dictionary<string, object>();

        [ExtenderControlProperty]
        [UrlProperty]
        [ClientPropertyName("url")]
        public string Url { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("headers")]
        public IDictionary<string, object> Headers
        {
            get { return _headers; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("params")]
        public IDictionary<string, object> Params
        {
            get { return _params; }
        }
    }

    public class ProtocolBuilderParamsBase : BuilderParamsBase, IProtocolBuilderParams
    {
        private readonly CollectionBase<IFormat> _formats = new CollectionBase<IFormat>((a, b) => !Equals(a, b));

        [ExtenderControlProperty]
        [ClientPropertyName("formats")]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public CollectionBase<IFormat> Formats
        {
            get { return _formats; }
        }
    }
}