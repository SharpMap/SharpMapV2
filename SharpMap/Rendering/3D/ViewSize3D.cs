using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public struct ViewSize3D
    {
        private double _width, _height, _depth;

        public ViewSize3D(double width, double height, double depth)
        {
            _width = width;
            _height = height;
            _depth = depth;
        }

        public double Width
        {
            get { return _width; }
        }

        public double Height
        {
            get { return _height; }
        }

        public double Depth
        {
            get { return _depth; }
        }

        public override string ToString()
        {
            return String.Format("Size - width: {0}, height: {1}, depth: {1}", Width, Height, Depth);
        }

        public static bool operator !=(ViewSize3D size1, ViewSize3D size2)
        {
            return size1.Width != size2.Width || size1.Height != size2.Height || size1.Depth != size2.Depth;
        }

        public static bool operator ==(ViewSize3D size1, ViewSize3D size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height && size1.Depth == size2.Depth;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ViewSize3D))
                return false;

            ViewSize3D other = (ViewSize3D)obj;

            return this == other;
        }

        public override int GetHashCode()
        {
            return unchecked(Width.GetHashCode() ^ Height.GetHashCode() ^ Depth.GetHashCode());
        }
    }
}
