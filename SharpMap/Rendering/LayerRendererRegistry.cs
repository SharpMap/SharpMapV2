// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
	internal class LayerRendererRegistry
	{
		private static readonly object _initSync = new object();
		private static volatile LayerRendererRegistry _instance;
		readonly Dictionary<RuntimeTypeHandle, object> _renderers = new Dictionary<RuntimeTypeHandle, object>();

		private LayerRendererRegistry()
		{
		}

		public static LayerRendererRegistry Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_initSync)
					{
						if (_instance == null)
						{
							_instance = new LayerRendererRegistry();
						}
					}
				}

				return _instance;
			}
		}

		public void Register(Type layerType, IRenderer renderer)
		{
			_renderers[layerType.TypeHandle] = renderer;
		}

		public TRenderer Get<TRenderer>(Type layerType)
			where TRenderer : class 
		{
			object renderer;
			_renderers.TryGetValue(layerType.TypeHandle, out renderer);

			if (!(renderer is TRenderer))
			{
				return null;
			}

			return renderer as TRenderer;
		}
	}
}
