/*
 *  The attached / following is part of SharpMap.Presentation.AspNet.WmsServer
 *  SharpMap.Presentation.AspNet.WmsServer is free software © 2008 Newgrove Consultants Limited, 
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
using System;
using System.IO;
using System.Xml;
using SharpMap.Presentation.AspNet.Handlers;
using SharpMap.Presentation.AspNet.MVP;

namespace SharpMap.Presentation.AspNet.WmsServer
{
    public abstract class AsyncWmsHandlerBase
        : AsyncMapHandlerBase
    {
        public new WmsMapRequestConfig MapRequestConfig
        {
            get { return (WmsMapRequestConfig) base.MapRequestConfig; }
        }

        public Capabilities.WmsServiceDescription ServiceDescription
        {
            get { return MapRequestConfig.ServiceDescription; }
        }

        /// <summary>
        /// This should return an instance of WmsRequestConfigFactoryBase
        /// </summary>
        /// <returns></returns>
        protected abstract override IMapRequestConfigFactory CreateConfigFactory();


        public override Stream Render(out string mimeType)
        {
            Stream s = null;
            if (CacheProvider.ExistsInCache(MapRequestConfig))
            {
                mimeType = MapRequestConfig.MimeType;
                s = CacheProvider.RetrieveStream(MapRequestConfig);
                s.Position = 0;
                return s;
            }
            try
            {
                RaiseBeforeInitializeMap();
                InitMap();
                RaiseMapInitialized();

                RaiseBeforeLoadLayers();
                LoadLayers();
                RaiseLayersLoaded();

                ConfigureMap();


                if (MapRequestConfig.WmsMode == WmsMode.Capabilites)
                {
                    XmlDocument caps = Capabilities.GetCapabilities(Context, Map, ServiceDescription);
                    s = new MemoryStream();
                    caps.Save(s);
                    mimeType = "text/xml";
                }
                else
                {
                    ConfigureRenderer();

                    AssignMonitorToDataProviders();

                    RaiseBeforeLoadMapState();
                    LoadMapState();
                    RaiseMapStateLoaded();

                    ConfigureMapView();

                    RaiseBeforeMapRender();
                    s = MapView.Render(out mimeType);
                }
                if (s == null)
                    return null;

                try
                {
                    if (string.Compare(mimeType, MapRequestConfig.MimeType, true) == 0)
                    {
                        ///don't cache it if there is a mime type mismatch.
                        ///perhaps we should raise an exception?
                        s.Position = 0;
                        CacheProvider.SaveToCache(MapRequestConfig, s);
                        s.Position = 0;
                    }
                    RaiseMapRenderDone();
                    return s;
                }
                catch
                {
                    CacheProvider.RemoveFromCache(MapRequestConfig);
                    throw;
                }
            }
            catch (ClientDisconnectedException)
            {
                if (s != null)
                    s.Close();
                throw;
            }
            catch (XmlFormatableExceptionBase)
            {
                if (s != null)
                    s.Close();
                throw;
            }
            catch (Exception ex)
            {
                if (s != null)
                    s.Close();
                throw new XmlFormatableExceptionWrapper(ex);
            }
        }
    }
}