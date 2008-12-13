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
using System.Configuration;
using SharpMap.Presentation.AspNet.Demo.Common;
using SharpMap.Presentation.AspNet.WmsServer;
using SharpMap.Presentation.AspNet.WmsServer.Caching;
using SharpMap.Rendering.Web;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Presentation.AspNet.Demo.Wms
{
    public class DemoWmsMapHandler : AsyncWmsHandlerBase
    {
        public override void LoadLayers()
        {
            DemoMapSetupUtility.SetupMap(Context, Map);
        }

        protected override IWebMapRenderer CreateMapRenderer()
        {
            return new GdiImageRenderer();
        }

        /// <summary>
        /// This should return an instance of WmsRequestConfigFactoryBase
        /// </summary>
        /// <returns></returns>
        protected override IMapRequestConfigFactory CreateConfigFactory()
        {
            return new DemoWmsMapConfigFactory();
        }

        protected override IMapCacheProvider CreateCacheProvider()
        {
            return new WmsAwareFileSystemCacheProvider
                       {
                           BaseCacheDir = ConfigurationManager.AppSettings["CacheDirectory"]
                       };
        }

        public override string BaseSrid
        {
            get { return "EPSG:4326"; }
        }
    }
}