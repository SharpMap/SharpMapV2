using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMap.Presentation.Web.SharpLayers
{
    public abstract class ValueBase<T> : IUICollectionItem<T>
    {
        #region IUICollectionItem<T> Members

        public T Value
        {
            get;
            set;
        }

      

        #endregion

        #region IUICollectionItem Members

        object IUICollectionItem.Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (T)value;
            }
        }

       
        #endregion
    }
}
