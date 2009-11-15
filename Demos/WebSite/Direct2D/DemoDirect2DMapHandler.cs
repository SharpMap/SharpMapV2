using SharpMap.Presentation.AspNet.Demo.NoCache;
using SharpMap.Rendering.Web.Direct2D;

namespace SharpMap.Presentation.AspNet.Demo.Direct2D
{
    public class DemoDirect2DMapHandler : DemoMapHandler
    {
        protected override IWebMapRenderer CreateMapRenderer()
        {
            return new Direct2DImageRenderer();
        }
    }
}