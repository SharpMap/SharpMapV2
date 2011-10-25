using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using AjaxControlToolkit;

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Yahoo
{
    public class YahooLayerBuilderParams : LayerBuilderParamsBase, IGridBasedLayerBuilderParams
    {

        [ExtenderControlProperty(true, true)]
        [ClientPropertyName("tileSize")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Size TileSize { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("transitionEffect")]
        public TransitionEffects TransitionEffect { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("buffer")]
        public int Buffer { get; set; }


        [ExtenderControlProperty]
        [ClientPropertyName("sphericalMercator")]
        public bool SphericalMercator
        {
            get;
            set;
        }


        [ClientPropertyName("resolutions")]
        public override CollectionBase<DoubleValue> Resolutions
        {
            get
            {
                return base.Resolutions;
            }
        }
        //[ExtenderControlProperty]
        //[ClientPropertyName("mapType")]
        //public YahooMapType MapType //enabling this breaks everything else
        //{
        //    get;
        //    set;
        //}

    }

    //public enum YahooMapType
    //{
    //    YAHOO_MAP_REG = 0,
    //    YAHOO_MAP_HYB,
    //    YAHOO_MAP_SAT
    //}
}