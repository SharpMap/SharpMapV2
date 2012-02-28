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
using SharpMap.Layers;
using SharpMap.Rendering;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Styles
{
    /// <summary>
    /// Represents a style for a <see cref="RasterLayer"/>.
    /// </summary>
    [Serializable]
    public class RasterStyle : Style
    {
        private ColorMatrix _colorTransform;
        private int _transparency;
        private StyleColor _defaultColor = StyleColor.Transparent;

        private RasterVisualizationMethod _method;
        private string _methodName;

        private RasterTransparency _rasterTransparency;

        /// <summary>
        /// Gets or sets a color transform matrix used to recolor the
        /// raster data.
        /// </summary>
        public ColorMatrix ColorTransform
        {
            get { return _colorTransform; }
            set
            {
                if (_colorTransform == value)
                {
                    return;
                }

                if (_colorTransform == null)
                {
                    _colorTransform = ColorMatrix.Identity;
                }

                _colorTransform = value;
            }
        }

        /// <summary>
        /// Gets or sets the visualization method for the raster
        /// </summary>
        public RasterVisualizationMethod Method
        {
            get { return string.IsNullOrEmpty(_methodName) ? _method : RasterVisualizationMethod.Custom; }
            set
            {
                // Do Not set this specific value!
                if (value == RasterVisualizationMethod.Custom)
                    return;
                _method = value;
            }
        }

        /// <summary>
        /// Gets or sets the overall transparency
        /// </summary>
        public int Transparency
        {
            get { return _transparency; }
            set
            {
                if (value < 0 || value > 255)
                    throw new ArgumentOutOfRangeException("value");
                _transparency = value;
            }
        }

        /// <summary>
        /// Gets the opacity of the layer
        /// </summary>
        public int Opacity
        {
            get { return 255 - _transparency; }
            set
            {
                if (value < 0 || value > 255)
                    throw new ArgumentOutOfRangeException("value");
                _transparency = 255 - value;
            }
        }

        /// <summary>
        /// Gets or sets the default color
        /// </summary>
        public StyleColor DefaultColor
        {
            get { return _defaultColor; }
            set { _defaultColor = value; }
        }

        /// <summary>
        /// Raster visualization routine name. If this value is set, <see cref="Method"/> will always return <see cref="RasterVisualizationMethod.Custom"/>.
        /// </summary>
        public string MethodName
        {
            get { return _methodName; }
            set
            {
                _methodName = value;
            }
        }

        /// <summary>
        /// Gets or sets the raster transparency
        /// </summary>
        public RasterTransparency RasterTransparency
        {
            get { return _rasterTransparency; }
            set { _rasterTransparency = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that the color should be inverted
        /// </summary>
        public bool InvertColor { get; set; }
    }
}