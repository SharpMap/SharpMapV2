using SharpMap.Presentation.AspNet.Demo.NoCache;
using SharpMap.Rendering.Web;
using SharpMap.Rendering.Web.Cairo;

namespace SharpMap.Presentation.AspNet.Demo.Cairo
{
    public class DemoCairoMapHandler : DemoMapHandler
    {
        protected override IWebMapRenderer CreateMapRenderer()
        {
            return new CairoImageRenderer();
        }
    }
    public class DemoWpfMapHandler : DemoMapHandler
    {
        protected override IWebMapRenderer CreateMapRenderer()
        {
            return new WpfImageRenderer();
        }
    }

}