// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Styles
{
    /// <summary>
    /// Defines a style used for rendering a lineal geometry.
    /// </summary>
    [Serializable]
    public class LinealGeometryStyle : FeatureStyle
    {
        /// <summary>
        /// Initializes a new VectorStyle with default value.
        /// </summary>
        /// <remarks>
        /// Default style values when initialized:<br/>
        /// <list type="table">
        /// <item>
        /// <term>AreFeaturesSelectable</term>
        /// <description>True</description>
        /// </item>
        /// <item>
        /// <term>LineStyle</term>
        /// <description>1px solid black</description>
        /// </item>
        /// </list>
        /// </remarks>
        public LinealGeometryStyle()
        {
            Line = new StylePen(StyleColor.Random(), 1d);
        }
        
        /// <summary>
        /// Linestyle for line geometries
        /// </summary>
        public StylePen Line { get; set; }
    }

    /// <summary>
    /// Defines a style used for rendering a lineal geometry.
    /// </summary>
    [Serializable]
    public class PolygonalGeometryStyle : LinealGeometryStyle
    {
        /// <summary>
        /// Initializes a new VectorStyle with default values.
        /// </summary>
        /// <remarks>
        /// Default style values when initialized:<br/>
        /// <list type="table">
        /// <item>
        /// <term>AreFeaturesSelectable</term>
        /// <description>True</description>
        /// </item>
        /// <item>
        /// <term>LineStyle</term>
        /// <description>1px solid black</description>
        /// </item>
        /// <item>
        /// <term>FillStyle</term>
        /// <description>Solid black</description>
        /// </item>
        /// </list>
        /// </remarks>
        public PolygonalGeometryStyle()
        {
            var color = new StyleColor(StyleColor.Random(), 127);
            Fill = new SolidStyleBrush(color);
        }
        
        /// <summary>
        /// Fill style for closed geometries.
        /// </summary>
        public StyleBrush Fill { get; set; }
    }

    /// <summary>
    /// Defines a style used for rendering a lineal geometry.
    /// </summary>
    [Serializable]
    public class PuntalGeometryStyle : FeatureStyle
    {
        /// <summary>
        /// Initializes a new VectorStyle with default values.
        /// </summary>
        /// <remarks>
        /// Default style values when initialized:<br/>
        /// <list type="table">
        /// <item>
        /// <term>AreFeaturesSelectable</term>
        /// <description>True</description>
        /// </item>
        /// <item>
        /// <term>Symbol</term>
        /// <description>Null reference (uses the geometry renderer default)</description>
        /// </item>
        /// </list>
        /// </remarks>
// ReSharper disable EmptyConstructor
        public PuntalGeometryStyle()
// ReSharper restore EmptyConstructor
        {
        }
        
        /// <summary>
        /// Linestyle for line geometries
        /// </summary>
        public Symbol2D Symbol { get; set; }
    }

    /// <summary>
    /// Defines a style used for rendering a geometry.
    /// </summary>
    [Serializable]
    public class GeometryStyle : FeatureStyle
    {
        #region Private fields

        private StyleBrush _fillStyle;
        private StylePen _lineStyle;
        private Symbol2D _symbol;

        #endregion Private fields

        /// <summary>
        /// Initializes a new VectorStyle with default values.
        /// </summary>
        /// <remarks>
        /// Default style values when initialized:<br/>
        /// <list type="table">
        /// <item>
        /// <term>AreFeaturesSelectable</term>
        /// <description>True</description>
        /// </item>
        /// <item>
        /// <term>LineStyle</term>
        /// <description>1px solid black</description>
        /// </item>
        /// <item>
        /// <term>FillStyle</term>
        /// <description>Solid black</description>
        /// </item>
        /// <item>
        /// <term>Symbol</term>
        /// <description>Null reference (uses the geometry renderer default)</description>
        /// </item>
        /// </list>
        /// </remarks>
        public GeometryStyle()
        {
            Line = new StylePen(StyleColor.Black, 1);
            Fill = new SolidStyleBrush(StyleColor.Black);
        }

        #region Properties

        /// <summary>
        /// Linestyle for line geometries
        /// </summary>
        public StylePen Line
        {
            get { return _lineStyle; }
            set { _lineStyle = value; }
        }

        /// <summary>
        /// Fill style for closed geometries.
        /// </summary>
        public StyleBrush Fill
        {
            get { return _fillStyle; }
            set { _fillStyle = value; }
        }

        /// <summary>
        /// Gets or sets a symbol used for rendering point features.
        /// </summary>
        public Symbol2D Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        #endregion Properties
    }
}