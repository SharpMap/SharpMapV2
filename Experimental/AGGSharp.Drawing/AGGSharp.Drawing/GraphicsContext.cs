using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG.Transform;
using AGG;
using GeoAPI.Geometries;
using AGGSharp.Drawing.Interface;

namespace AGGSharp.Drawing
{
    internal class GraphicsContext : IGraphicsContext
    {
        #region IGraphicsContext Members

        private ITransform _transform = null;
        public ITransform Transform
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
            }
        }

        private IAlphaMask _mask;
        public IAlphaMask Mask
        {
            get
            {
                return _mask;
            }
            set
            {
                _mask = value;
            }
        }

        private IExtents2D _clipBounds;
        public IExtents2D ClipBounds
        {
            get
            {
                return _clipBounds;
            }
            set
            {
                _clipBounds = value;
            }
        }

        #endregion
    }
}
