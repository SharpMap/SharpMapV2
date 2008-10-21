using System;

namespace SharpMap.Presentation.Web.SharpLayers
{
    public class ResourceUri : IUICollectionItem
    {
        public Uri Uri { get; set; }



        #region IUICollectionItem Members

        public object GetValue()
        {
            return Uri;
        }

        #endregion
    }
}