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
using System.IO;
using System.Reflection;
using System.Text;

using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.Rendering.Thematics;
using SharpMap.CoordinateSystems.Transformations;

namespace SharpMap.Rendering.Rendering2D
{
	/// <summary>
	/// Provides a base class for generating rendered objects from vector shapes.
	/// </summary>
	/// <typeparam name="TRenderObject"></typeparam>
    public abstract class VectorRenderer2D<TRenderObject> : IVectorRenderer2D<TRenderObject>
    {
        private static Symbol2D _defaultSymbol;

        static VectorRenderer2D()
        {
        }

        public static Symbol2D DefaultSymbol
        {
            get 
            {
                if (_defaultSymbol == null)
                {
                    lock (_defaultSymbol)
                    {
                        if (_defaultSymbol == null)
                        {
                            Stream data = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharpMap.Styles.DefaultSymbol.png");
                            _defaultSymbol = new Symbol2D(data, new ViewSize2D(16, 16));
                        }
                    }
                }

                return _defaultSymbol; 
            }
        }

        public VectorRenderer2D()
        {
            this.StyleRenderingMode = StyleRenderingMode.AntiAlias;
        }

        #region IVectorLayerRenderer Members
        public abstract TRenderObject RenderPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        public abstract TRenderObject RenderPath(GraphicsPath2D path, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData);
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, ColorMatrix highlight, ColorMatrix select);
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, Symbol2D highlightSymbolData, Symbol2D selectSymbolData);
        #endregion
    }
}
