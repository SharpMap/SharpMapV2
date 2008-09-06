/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System.Web;
using SharpMap.Presentation.AspNet.Impl;
using SharpMap.Presentation.AspNet.IoC;
using SharpMap.Rendering;

namespace SharpMap.Presentation.AspNet.Demo.Caching
{
    public class DemoCachingWebMap
        : WebMapBase
    {
        public DemoCachingWebMap(HttpContext c)
            : base(c) { }

        protected override IWebMapRenderer CreateMapRenderer()
        {
            return Container.Instance.Resolve<IWebMapRenderer>(); //we will just use the default
        }

        protected override IMapRequestConfigFactory CreateConfigFactory()
        {
            return Container.Instance.Resolve<IMapRequestConfigFactory>();//we will just use the default
        }


        protected override IMapCacheProvider CreateCacheProvider()
        {
            return Container.Instance.Resolve<IMapCacheProvider>("aspNetCacheProvider");
        }

        public override void LoadLayers()
        {
            DemoMapSetupUtility.SetupMap(Context, this.Map);
        }

    }
}
