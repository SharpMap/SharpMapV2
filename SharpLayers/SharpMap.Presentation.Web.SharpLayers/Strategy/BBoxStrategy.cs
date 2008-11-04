using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMap.Presentation.Web.SharpLayers.Strategy
{
    public class BBoxStrategy : IStrategy
    {
        #region IClientClass Members

        public string ClientClassName
        {
            get { return "OpenLayers.Strategy.BBOX"; }
        }

        #endregion
    }
}
