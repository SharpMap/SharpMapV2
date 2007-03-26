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
