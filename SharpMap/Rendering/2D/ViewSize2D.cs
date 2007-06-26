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

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// A 2 dimensional measure of size.
    /// </summary>
    [Serializable]
    public struct ViewSize2D : IViewVector
    {
        private double _width, _height;
        private bool _hasValue;

        public static readonly ViewSize2D Empty = new ViewSize2D();
		public static readonly ViewSize2D Zero = new ViewSize2D(0, 0);
		public static readonly ViewSize2D Unit = new ViewSize2D(1, 1);

        #region Constructors
        public ViewSize2D(double width, double height)
        {
            _width = width;
            _height = height;
            _hasValue = true;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("[ViewSize2D] Width: {0}, Height: {1}", Width, Height);
        }

        public override int GetHashCode()
        {
            return unchecked(Width.GetHashCode() ^ Height.GetHashCode());
        }

        #region Properties
        public double Width
        {
            get { return _width; }
        }

        public double Height
        {
            get { return _height; }
        }
        #endregion

        #region Equality Testing
        public static bool operator != (ViewSize2D size1, ViewSize2D size2)
        {
            return !(size1.Equals(size2));
        }

        public static bool operator ==(ViewSize2D size1, ViewSize2D size2)
        {
            return size1.Equals(size2);
        }

        public override bool Equals(object obj)
        {
            if (obj is ViewSize2D)
            {
                return Equals((ViewSize2D)obj);
            }

            if (obj is IViewVector)
            {
                return Equals(obj as IViewVector);
            }

            return false;
        }

        public bool Equals(ViewSize2D size)
        {
            return this.IsEmpty == size.IsEmpty && this.Width == size.Width && this.Height == size.Height;
        }

        #region IEquatable<IViewVector> Members

        public bool Equals(IViewVector other)
        {
            double[] myElements = Elements;
            double[] otherElements = other.Elements;

            if (myElements.Length != otherElements.Length)
                return false;

            for (int elementIndex = 0; elementIndex < myElements.Length; elementIndex++)
            {
                if (myElements[elementIndex] != otherElements[elementIndex])
                    return false;
            }

            return true;
        }

        #endregion
        #endregion

        #region IViewVector Members

        public double[] Elements
        {
            get { return new double[] { Width, Height }; }
        }

        public double this[int element]
        {
            get 
            {
                if (element == 0)
                {
                    return Width;
                }
                else if (element == 1)
                {
                    return Height;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("element", element, "Index must be 0 or 1");
                }
            }
        }

        public bool IsEmpty
        {
            get { return !_hasValue; }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new ViewSize2D(Width, Height);
        }

        #endregion

        #region IEnumerable<double> Members

        public IEnumerator<double> GetEnumerator()
        {
            yield return Width;
            yield return Height;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
