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
using System.IO;
using System.Web;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Utilities;

namespace SharpMap.Presentation.AspNet
{
    public interface IWebMap : IDisposable
    {
        HttpContext Context { get; set; }

        IGeometryServices GeometryServices { get; }

        /// <summary>
        /// The Map instance
        /// </summary>
        Map Map { get; set; }

        WebMapView MapView { get; set; }


        /// <summary>
        /// wrapper object which supplies IRenderer instances
        /// </summary>
        IWebMapRenderer MapRenderer { get; set; }

        IMapCacheProvider CacheProvider { get; set; }

        IMapRendererConfig RendererConfig { get; set; }


        ///// <summary>
        ///// An instance of a factory which can create a configuration object from Request parameters
        ///// </summary>
        //IMapRequestConfigFactory<TMapRequestConfig> ConfigFactory { get; }


        /// <summary>
        /// The configuration object which determines the map setup
        /// </summary>
        IMapRequestConfig MapRequestConfig { get; }

        event EventHandler BeforeCreateMapRequestConfig;
        event EventHandler MapRequestConfigCreated;
        event EventHandler BeforeInitMap;
        event EventHandler MapInitDone;
        event EventHandler BeforeLoadLayers;
        //event EventHandler<LayerLoadedEventArgs> LayerLoaded; //- dont think we really need this.
        event EventHandler LayersLoaded;
        event EventHandler BeforeLoadMapState;
        event EventHandler MapStateLoaded;
        event EventHandler BeforeMapRender;
        event EventHandler MapRenderDone;

        //Func<TOutput, Stream> StreamBuilder { get; }


        /// <summary>
        /// Create the SharpMap object - particular implementations may do this afresh/deserialize an existing one/retrieve from session
        /// End result is this.Map should yield a non null value.
        /// </summary>
        void InitMap();

        /// <summary>
        /// Hook used to add layers to the map object. 
        /// Some possible implementations will not use it if the map was deserialized or retrieved from session.
        /// </summary>
        void LoadLayers();

        /// <summary>
        /// default implementation should call MapRequestConfiguration.ConfigureMap(this.Map);
        /// </summary>
        void ConfigureMap();


        void ConfigureMapView();

        /// <summary>
        /// Hook used to configure map state e.g feature selections particular to the context user.
        /// called after ConfigureMap
        /// </summary>
        void LoadMapState();

        /// <summary>
        /// Hook used to configure renderer e.g set compression settings, output format etc
        /// </summary>
        void ConfigureRenderer();


        /// <summary>
        /// unwire any event handlers here before disposal
        /// </summary>
        void UnwireEvents();

        Stream Render(out string mimetype);
    }
}