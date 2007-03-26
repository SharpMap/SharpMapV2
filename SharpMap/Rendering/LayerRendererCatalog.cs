using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;

namespace SharpMap.Rendering
{
    internal class LayerRendererCatalog
    {
        private static readonly object _initSync = new object();
        private static volatile LayerRendererCatalog _instance;
        Dictionary<RuntimeTypeHandle, object> _renderers = new Dictionary<RuntimeTypeHandle, object>();

        private LayerRendererCatalog()
        {
        }

        public static LayerRendererCatalog Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_initSync)
                    {
                        if (_instance == null)
                        {
                            _instance = new LayerRendererCatalog();
                        }       
                    }
                }

                return _instance;
            }
        }

        public void Register<TViewPoint, TViewSize, TViewRectangle, TRenderObject>(Type layerType, IRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject> renderer)
            where TViewPoint : IViewVector
            where TViewSize : IViewVector
            where TViewRectangle : IViewMatrix
        {
            _renderers[layerType.TypeHandle] = renderer;
        }

        public TRenderer Get<TRenderer>(Type layerType)
        {
            object renderer = null;
            _renderers.TryGetValue(layerType.TypeHandle, out renderer);

            if (!(renderer is TRenderer))
                return default(TRenderer);

            return (TRenderer)renderer;
        }
    }
}
