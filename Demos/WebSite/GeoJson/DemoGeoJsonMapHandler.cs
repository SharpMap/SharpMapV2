using SharpMap.Presentation.AspNet.Caching;
using SharpMap.Presentation.AspNet.Demo.Common;
using SharpMap.Presentation.AspNet.Handlers;
using SharpMap.Rendering.Web;

namespace SharpMap.Presentation.AspNet.Demo.GeoJson
{
    public class DemoGeoJsonMapHandler : AsyncMapHandlerBase
    {
        public override void LoadLayers()
        {
            DemoMapSetupUtility.SetupMap(Context, Map);
        }

        protected override IWebMapRenderer CreateMapRenderer()
        {
            return new GeoJsonRenderer();
        }

        protected override IMapRequestConfigFactory CreateConfigFactory()
        {
            return new GeoJsonRequestConfigFactory();
        }

        protected override IMapCacheProvider CreateCacheProvider()
        {
            return new NoCacheProvider();
        }

        public override string BaseSrid
        {
            get { return "EPSG:4326"; }
        }
    }
}