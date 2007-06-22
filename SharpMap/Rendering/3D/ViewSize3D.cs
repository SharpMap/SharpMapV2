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

namespace SharpMap.Rendering.Rendering3D
{
    /// <summary>
    /// A measurement of size in 3 dimensions.
    /// </summary>
    public struct ViewSize3D : IViewVector
    {
        private double _width, _height, _depth;
        private bool _hasValue;

        public static readonly ViewSize3D Empty = new ViewSize3D();
        public static readonly ViewSize3D Zero = new ViewSize3D(0, 0, 0);

        /// <summary>
        /// Creates a new non-empty ViewSize3D with the given values.
        /// </summary>
        /// <param name="width">Width of the measurement.</param>
        /// <param name="height">Height of the measurement.</param>
        /// <param name="depth">Depth of the measurement.</param>
        public ViewSize3D(double width, double height, double depth)
        {
            _width = width;
            _height = height;
            _depth = depth;
            _hasValue = true;
        }

        public override string ToString()
        {
            return String.Format("[ViewSize3D] Width: {0}, Height: {1}, Depth: {1}", Width, Height, Depth);
        }

        public override int GetHashCode()
        {
            return unchecked(Width.GetHashCode() ^ Height.GetHashCode() ^ Depth.GetHashCode());
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

        public override bool Equals(object obj)
        {
            if (!(obj is ViewSize3D))
            {
                return false;
            }

            return Equals((ViewSize3D)obj);
        }

        public bool Equals(ViewSize3D size)
        {
            return this._hasValue == size._hasValue
                && this.Width == size.Width
                && this.Height == size.Height
                && this.Depth == size.Depth;
        }

        #region IViewVector Members

        public double[] Elements
        {
            get { return new double[] { _width, _height, _depth }; }
        }

        public double this[int element]
        {
            get 
            {
                if (element == 0)
                {
                    return _width;
                }
                else if (element == 1)
                {
                    return _height;
                }
                else if (element == 2)
                {
                    return _depth;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("element", element, "Index must be 0 or 1");
                }
            }
        }

        public bool IsEmpty
        {
            get { return _hasValue; }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEquatable<IViewVector> Members

        public bool Equals(IViewVector other)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<double> Members

        public IEnumerator<double> GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public static bool operator !=(ViewSize3D size1, ViewSize3D size2)
        {
            return ! (size1 == size2);
        }

        public static bool operator ==(ViewSize3D size1, ViewSize3D size2)
        {
            return size1.Equals(size2);
        }
    }
}
