using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Renderer;
using System.IO;

namespace SharpMap.Presentation.AspNet
{
    public interface IWebMap<TMapRequestConfig, TOutput> : IDisposable
        where TMapRequestConfig : IMapRequestConfig
    {
        event EventHandler BeforeCreateMapRequestConfig;
        event EventHandler MapRequestConfigCreated;
        event EventHandler BeforeInitMap;
        event EventHandler MapInitDone;
        event EventHandler BeforeLoadLayers;
        event EventHandler<LayerLoadedEventArgs> LayerLoaded;
        event EventHandler LayersLoaded;
        event EventHandler BeforeConfigureMapView;
        event EventHandler MapViewConfigDone;
        event EventHandler BeforeLoadMapState;
        event EventHandler MapStateLoaded;
        event EventHandler BeforeMapRender;
        event EventHandler MapRenderDone;

        Func<TOutput, Stream> StreamBuilder { get; }


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
        void ConfigureMapView();

        /// <summary>
        /// Hook used to configure map state e.g feature selections particular to the context user.
        /// called after ConfigureMap
        /// </summary>
        void LoadMapState();

        /// <summary>
        /// The Map instance
        /// </summary>
        Map Map { get; set; }


        /// <summary>
        /// wrapper object which supplies IRenderer instances
        /// </summary>
        IMapRenderer<TOutput> MapRenderer { get; }


        ///// <summary>
        ///// An instance of a factory which can create a configuration object from Request parameters
        ///// </summary>
        //IMapRequestConfigFactory<TMapRequestConfig> ConfigFactory { get; }


        /// <summary>
        /// The configuration object which determines the map setup
        /// </summary>
        TMapRequestConfig MapRequestConfig { get; }


        /// <summary>
        /// unwire any event handlers here before disposal
        /// </summary>
        void UnwireEvents();

        Stream Render(out string mimetype);


    }
}
