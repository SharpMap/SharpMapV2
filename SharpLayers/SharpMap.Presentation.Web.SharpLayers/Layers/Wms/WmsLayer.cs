namespace SharpMap.Presentation.Web.SharpLayers.Layers.Wms
{
    public class WmsLayer : IUICollectionItem
    {
        public string Name { get; set; }



        #region IUICollectionItem Members

        public object GetValue()
        {
            return Name;
        }

        #endregion
    }
}