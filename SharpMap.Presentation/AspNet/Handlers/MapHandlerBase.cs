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


using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Utilities;

namespace SharpMap.Presentation.AspNet.Handlers
{
    public abstract class MapHandlerBase
        : IHttpHandler, IWebMap
    {
        private readonly IGeometryServices geometryServices = new GeometryServices();
        private IMapCacheProvider _cacheProvider;
        private IMapRequestConfig _config;

        private IMapRequestConfigFactory _configFactory;
        private IWebMapRenderer _renderer;
        private IMapRendererConfig _rendererConfig;
        private Boolean disposed;
        private WebMapView mapView;

        public IMapRequestConfigFactory ConfigFactory
        {
            get
            {
                EnsureConfigFactory();
                return _configFactory;
            }
        }

        #region IHttpHandler Members

        public virtual void ProcessRequest(HttpContext context)
        {
            Context = context;
            context.Response.Clear();
            Stream s = null;

            try
            {
                String mime;

                using (s = Render(out mime))
                {
                    if (s != null && context.Response.IsClientConnected)
                    {
                        context.Response.ContentType = mime;
                        SetCacheability(context.Response);
                        s.Position = 0;

                        using (BinaryReader br = new BinaryReader(s))
                        {
                            using (BinaryWriter outStream = new BinaryWriter(context.Response.OutputStream))
                            {
                                outStream.Write(br.ReadBytes((Int32)s.Length), 0, (Int32)s.Length);
                                outStream.Flush();
                            }

                            br.Close();
                        }
                    }
                }
            }
            catch (ClientDisconnectedException)
            {
                Debug.WriteLine("Client Disconnected");
            }
            catch (XmlFormatableExceptionBase ex)
            {
                Debug.WriteLine(String.Format("{0}\n{1}",
                                              ex.InnerException == null
                                                  ? ex.Message
                                                  : ex.InnerException.Message,
                                              ex.StackTrace));

                if (context.Response.IsClientConnected)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 500;
                    context.Response.Status = "500 Internal Server Error";
                    context.Response.StatusDescription = "An error occured.";
                    context.Response.ContentType = "text/xml";
                    using (StreamWriter sw = new StreamWriter(context.Response.OutputStream))
                    {
                        sw.Write(ex.XmlExceptionString);
                        sw.Flush();
                    }
                }
            }
            finally
            {
                try
                {
                    if (s != null)
                    {
                        s.Close();
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }

                try
                {
                    if (Context.Response.OutputStream != null)
                    {
                        Context.Response.OutputStream.Flush();
                        Context.Response.OutputStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }

                try
                {
                    if (_renderer != null)
                    {
                        _renderer.ClearRenderQueue();
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }

                CleanUpRequest();
            }
        }

        public Boolean IsReusable
        {
            get { return true; }
        }

        #endregion

        #region IWebMap Members

        public IGeometryServices GeometryServices
        {
            get { return geometryServices; }
        }

        public IMapCacheProvider CacheProvider
        {
            get
            {
                EnsureCacheProvider();
                return _cacheProvider;
            }

            set { _cacheProvider = value; }
        }


        public IWebMapRenderer MapRenderer
        {
            get
            {
                EnsureRenderer();
                return _renderer;
            }

            set { _renderer = value; }
        }

        public HttpContext Context { get; set; }

        public WebMapView MapView
        {
            get { return mapView; }
            set
            {
                mapView = value;
                MapRenderer.MapView = mapView;
                if (value != null)
                {
                    mapView.WebMapRenderer = MapRenderer;
                    mapView.Map = Map;
                }
            }
        }

        public event EventHandler BeforeCreateMapRequestConfig;
        public event EventHandler MapRequestConfigCreated;
        public event EventHandler BeforeInitMap;
        public event EventHandler MapInitDone;
        public event EventHandler BeforeLoadLayers;
        public event EventHandler LayersLoaded;
        public event EventHandler BeforeLoadMapState;
        public event EventHandler MapStateLoaded;
        public event EventHandler BeforeMapRender;
        public event EventHandler MapRenderDone;

        public void ConfigureMapView()
        {
            MapView = new WebMapView(this);
            MapRequestConfig.ConfigureMapView(MapView);
        }

        public virtual Stream Render(out String mimeType)
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
                Debug.WriteLine(string.Format("Thread {0}: Begin rendering, Map {1})",
                                              System.Threading.Thread.CurrentThread.ManagedThreadId, Map.Guid));
                RaiseMapInitialized();

                RaiseBeforeLoadLayers();
                LoadLayers();
                RaiseLayersLoaded();

                ConfigureMap();

                ConfigureRenderer();

                AssignMonitorToDataProviders();

                RaiseBeforeLoadMapState();
                LoadMapState();
                RaiseMapStateLoaded();

                ConfigureMapView();
                Debug.WriteLine(string.Format("Thread {0}: MapViewConfigured, Map {1}, MapView {2}",
                                              System.Threading.Thread.CurrentThread.ManagedThreadId, Map.Guid,
                                              MapView.Presenter.Guid));

                RaiseBeforeMapRender();

                s = MapView.Render(out mimeType);

                if (s == null)
                {
                    return null;
                }

                try
                {
                    if (String.Compare(mimeType, MapRequestConfig.MimeType, true) == 0)
                    {
                        ///don't cache it if there is a mime type mismatch.
                        ///perhaps we should raise an exception?
                        s.Seek(0, SeekOrigin.Begin);
                        CacheProvider.SaveToCache(MapRequestConfig, s);
                        s.Seek(0, SeekOrigin.Begin);
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
                if (!(s == null))
                {
                    s.Close();
                }

                throw;
            }
            catch (XmlFormatableExceptionBase)
            {
                if (s != null)
                {
                    s.Close();
                }

                throw;
            }
            catch (Exception ex)
            {
                if (s != null)
                {
                    s.Close();
                }

                throw new XmlFormatableExceptionWrapper(ex);
            }
        }

        /// <summary>
        /// Create or retrieve a Map Object.
        /// You can add layers here if you wish.
        /// </summary>
        public virtual void InitMap()
        {
            Map = new Map(GeometryServices[MapRequestConfig.BaseSrid], GeometryServices.CoordinateTransformationFactory);
        }

        /// <summary>
        /// Use to add layers to the map. Not necessary if the layers were added when the map was created or if the map was retrieved from a persistance medium.
        /// </summary>
        public abstract void LoadLayers();

        /// <summary>
        /// this method will configure the map view by calling ConfigureMap on the MapRequestConfig
        /// </summary>
        public virtual void ConfigureMap()
        {
            MapRequestConfig.ConfigureMap(Map);
        }

        /// <summary>
        /// override this method to configure your renderer.
        /// </summary>
        public virtual void ConfigureRenderer()
        {
            IMapRendererConfig config = RendererConfig;
            if (config != null)
            {
                config.ConfigureRenderer(MapRequestConfig, MapRenderer);
            }
        }

        /// <summary>
        /// Use this method to load user state into the map. Or leave empty if not required.
        /// </summary>
        /// <remarks>This was put in for v2 to allow such things as setting feature selections etc. 
        /// Perhaps it should be removed from this version?</remarks> 
        public virtual void LoadMapState()
        {
        }

        /// <summary>
        /// use this method to detach any events. It is called during disposal.
        /// </summary>
        public virtual void UnwireEvents()
        {
        }

        /// <summary>
        /// The Map, Assign a value to WebMapView before assigning a value to Map.
        /// </summary>
        public Map Map { get; set; }

        public IMapRequestConfig MapRequestConfig
        {
            get
            {
                EnsureMapConfiguration();
                return _config;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IMapRendererConfig RendererConfig
        {
            get
            {
                EnsureRendererConfig();
                return _rendererConfig;
            }
            set { _rendererConfig = value; }
        }

        #endregion

        private static void WriteException(Exception ex)
        {
            Debug.WriteLine(String.Format("{0}\n{1}", ex.Message, ex.StackTrace));
        }

        private void EnsureRendererConfig()
        {
            if (Equals(null, _rendererConfig))
            {
                CreateRendererConfig();
            }
        }

        protected virtual void CreateRendererConfig()
        {
            ///do nothing - it is up to child classes to do something
        }

        /// <summary>
        /// Assigns a monitoring method to any AppStateMonitoringFeatureProviders
        /// </summary>
        protected void AssignMonitorToDataProviders()
        {
            Action monitor = delegate
                                 {
                                     if (MapView.ClientDisconnected)
                                     {
                                         throw new ClientDisconnectedException();
                                     }
                                 };

            foreach (ILayer l in Map.Layers)
            {
                if ((l.DataSource is AppStateMonitoringFeatureProvider))
                {
                    ((AppStateMonitoringFeatureProvider)l.DataSource).Monitor = monitor;
                }
                else if (l.DataSource is AsyncFeatureProviderAdapter)
                {
                    if (l.DataSource.HasProperty(AppStateMonitoringFeatureLayerProperties.AppStateMonitor))
                    {
                        l.DataSource.SetPropertyValue(AppStateMonitoringFeatureLayerProperties.AppStateMonitor, monitor);
                    }
                }
            }
        }

        /// <summary>
        /// override this method to control the cacheability headers and caching at the web/proxy server level.
        /// </summary>
        /// <param name="response"></param>
        /// <remarks>By default nothing is set</remarks> 
        public virtual void SetCacheability(HttpResponse response)
        {
        }


        protected void RaiseCreateMapRequestConfig()
        {
            if (BeforeCreateMapRequestConfig != null)
            {
                BeforeCreateMapRequestConfig(this, EventArgs.Empty);
            }
        }

        protected void RaiseMapRequestConfigCreated()
        {
            if (MapRequestConfigCreated != null)
            {
                MapRequestConfigCreated(this, EventArgs.Empty);
            }
        }

        protected void RaiseBeforeInitializeMap()
        {
            if (BeforeInitMap != null)
            {
                BeforeInitMap(this, EventArgs.Empty);
            }
        }

        protected void RaiseMapInitialized()
        {
            if (MapInitDone != null)
            {
                MapInitDone(this, EventArgs.Empty);
            }
        }

        protected void RaiseBeforeLoadLayers()
        {
            if (BeforeLoadLayers != null)
            {
                BeforeLoadLayers(this, EventArgs.Empty);
            }
        }

        protected void RaiseLayersLoaded()
        {
            if (LayersLoaded != null)
            {
                LayersLoaded(this, EventArgs.Empty);
            }
        }

        protected void RaiseBeforeLoadMapState()
        {
            if (BeforeLoadMapState != null)
            {
                BeforeLoadMapState(this, EventArgs.Empty);
            }
        }

        protected void RaiseMapStateLoaded()
        {
            if (MapStateLoaded != null)
            {
                MapStateLoaded(this, EventArgs.Empty);
            }
        }

        protected void RaiseBeforeMapRender()
        {
            if (BeforeMapRender != null)
            {
                BeforeMapRender(this, EventArgs.Empty);
            }
        }

        protected void RaiseMapRenderDone()
        {
            if (MapRenderDone != null)
            {
                MapRenderDone(this, EventArgs.Empty);
            }
        }


        /// <summary>
        /// Create the WebMapWrapper here
        /// </summary>
        /// <returns></returns>
        protected abstract IWebMapRenderer CreateMapRenderer();

        /// <summary>
        /// Create a config factory here
        /// </summary>
        /// <returns></returns>
        protected abstract IMapRequestConfigFactory CreateConfigFactory();


        private void EnsureRenderer()
        {
            if (_renderer == null)
            {
                _renderer = CreateMapRenderer();
            }
        }

        private void EnsureMapConfiguration()
        {
            if (_config == null)
            {
                RaiseCreateMapRequestConfig();
                _config = ConfigFactory.CreateConfig(Context);
                RaiseMapRequestConfigCreated();
            }
        }

        private void EnsureConfigFactory()
        {
            if (_configFactory == null)
            {
                _configFactory = CreateConfigFactory();
            }
        }


        private void EnsureCacheProvider()
        {
            if (_cacheProvider == null)
            {
                _cacheProvider = CreateCacheProvider();
            }
        }

        protected abstract IMapCacheProvider CreateCacheProvider();

        ~MapHandlerBase()
        {
            Dispose(false);
        }


        protected virtual void CleanUpRequest()
        {
            UnwireEvents();

            if (_renderer != null)
            {
                _renderer.Dispose();
            }
            _renderer = null;

            if (MapView != null)
            {
                MapView.Map = null;
                MapView.Dispose();
            }
            MapView = null;

            if (Map != null)
            {
                Map.Dispose();
            }

            Map = null;

            Context = null;
            _config = null;
            _renderer = null;
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!disposed)
            {
                CleanUpRequest();

                disposed = true;
            }
        }
    }
}