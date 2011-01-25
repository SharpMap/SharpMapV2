// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Threading;
using SharpMap.Layers;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
	internal class LayerRendererRegistry
	{
        #region Nested types
        private struct LayerRendererRegistryKey : IEquatable<LayerRendererRegistryKey>
        {
            internal LayerRendererRegistryKey(Type type, String name)
            {
                Type = type == null ? new RuntimeTypeHandle() : type.TypeHandle;
                Name = name;
            }

            internal RuntimeTypeHandle Type;
            internal String Name;

            #region IEquatable<LayerRendererRegistryKey> Members

            public Boolean Equals(LayerRendererRegistryKey other)
            {
                return other.Type.Equals(Type) &&
                    String.Compare(Name, other.Name, StringComparison.CurrentCultureIgnoreCase) == 0;
            }

            #endregion

            public override Int32 GetHashCode()
            {
                return Type.GetHashCode() ^
                    ((Name == null) ? 1247510379 : Name.GetHashCode());
            }

            public override Boolean Equals(Object obj)
            {
                if (obj is LayerRendererRegistryKey)
                {
                    return Equals((LayerRendererRegistryKey)obj);
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion		
        
        private static readonly Object _initSync = new Object();
		private static Object _instance;
        readonly Dictionary<LayerRendererRegistryKey, IRenderer> _renderers
            = new Dictionary<LayerRendererRegistryKey, IRenderer>();

		private LayerRendererRegistry()
		{
		}

		public static LayerRendererRegistry Instance
		{
			get
			{
				if (Thread.VolatileRead(ref _instance) == null)
				{
					lock (_initSync)
					{
                        if (Thread.VolatileRead(ref _instance) == null)
						{
						    Thread.VolatileWrite(ref _instance, new LayerRendererRegistry());
						}
					}
				}

                return _instance as LayerRendererRegistry;
			}
		}

		public void Register(Type layerType, IRenderer renderer)
        {
            LayerRendererRegistryKey key = new LayerRendererRegistryKey(layerType, null);
            _renderers[key] = renderer;
        }

        public void Register(String layerName, IRenderer renderer)
        {
            LayerRendererRegistryKey key = new LayerRendererRegistryKey(null, layerName);
            _renderers[key] = renderer;
        }

        public TRenderer Get<TRenderer, TLayer>()
            where TRenderer : class, IRenderer
        {
            Type layerType = typeof(TLayer);
            LayerRendererRegistryKey key = new LayerRendererRegistryKey(layerType, null);
            IRenderer renderer;

            if (!_renderers.TryGetValue(key, out renderer))
            {
                return null;
            }

            return renderer as TRenderer;
        }

        public TRenderer Get<TRenderer>(String name)
            where TRenderer : class, IRenderer
        {
            LayerRendererRegistryKey key = new LayerRendererRegistryKey(null, name);
            IRenderer renderer;

            if (!_renderers.TryGetValue(key, out renderer))
            {
                return null;
            }

            return renderer as TRenderer;
        }

	    public TRenderer Get<TRenderer>(ILayer layer)
            where TRenderer : class, IRenderer
        {
	        if (layer == null) 
                throw new ArgumentNullException("layer");

	        LayerRendererRegistryKey key = new LayerRendererRegistryKey(null, layer.LayerName);
            IRenderer renderer;

            if (!_renderers.TryGetValue(key, out renderer))
            {
                key = new LayerRendererRegistryKey(layer.GetType(), null);

                if (!_renderers.TryGetValue(key, out renderer))
                {
                    return null;
                }
            }

            return renderer as TRenderer;
        }
	}
}
