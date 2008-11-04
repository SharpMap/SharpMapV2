using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMap.Presentation.Web.SharpLayers.Format
{
    public class GeoJsonFormat : IFormat
    {
        #region IClientClass Members

        public string ClientClassName
        {
            get { return "OpenLayers.Format.GeoJSON"; }
        }

        #endregion
    }
}
