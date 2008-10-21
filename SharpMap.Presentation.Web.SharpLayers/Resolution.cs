using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Serializable]
    public class Resolution : IUICollectionItem
    {
        public double Value { get; set; }

        #region IUICollectionItem Members

        public object GetValue()
        {
            return Value;
        }

        #endregion
    }
}
