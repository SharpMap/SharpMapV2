using SharpMap.Presentation.AspNet.Demo.NoCache;
using SharpMap.Rendering.Web.Wpf;

namespace SharpMap.Presentation.AspNet.Demo.Wpf
{
    public class DemoWpfMapHandler : DemoMapHandler 
    {
        protected override IWebMapRenderer CreateMapRenderer()
        {
            return new WpfImageRenderer();
        }
    }
}