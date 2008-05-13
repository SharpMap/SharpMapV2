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
using SharpMap.Data;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// A map layer of feature geometries.
    /// </summary>
    /// <example>
    /// </example>
    public class GeometryLayer : FeatureLayer, ILayer
    {
        #region Instance fields

        #endregion

        #region Object Construction / Disposal

        /// <summary>
        /// Initializes a new, empty vector layer.
        /// </summary>
        public GeometryLayer(IFeatureProvider dataSource)
            : this(String.Empty, dataSource)
        {
        }

        /// <summary>
        /// Initializes a new layer with the given name and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="dataSource">Data source.</param>
        public GeometryLayer(String layername, IFeatureProvider dataSource)
            : this(layername, new VectorStyle(), dataSource)
        {
        }

        /// <summary>
        /// Initializes a new layer with the given name, style and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="style">Style to apply to the layer.</param>
        /// <param name="dataSource">Data source.</param>
        public GeometryLayer(String layername, VectorStyle style, IFeatureProvider dataSource)
            : base(layername, style, dataSource)
        {
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        protected override void Dispose(Boolean disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (DataSource != null)
            {
                DataSource.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the layer style as a VectorStyle.
        /// </summary>
        public new VectorStyle Style
        {
            get { return base.Style as VectorStyle; }
            set { base.Style = value; }
        }
        #endregion

        #region Layer Overrides
        protected override IStyle CreateStyle()
        {
            return new VectorStyle();
        }

        IStyle ILayer.Style
        {
            get { return Style; }
            set
            {
                if (!(value is VectorStyle))
                {
                    throw new ArgumentException("Style value must be of type VectorStyle.", "value");
                }

                Style = value as VectorStyle;
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones the layer.
        /// </summary>
        /// <returns>A copy of the layer.</returns>
        public override Object Clone()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}