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
using System.Runtime.InteropServices;
using System.Text;

namespace SharpMap.Rendering.Rendering2D
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ViewPoint2D : IViewVector
    {
        public static readonly ViewPoint2D Zero = new ViewPoint2D(0, 0);

        private double _x, _y;

        public ViewPoint2D(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public ViewPoint2D(double[] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("value");
            }

            if (elements.Length != 2)
            {
                throw new ArgumentException("Elements array must have only 2 components");
            }

            _x = elements[0];
            _y = elements[1];
        }

        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }

        #region IViewVector Members

        public double[] Elements
        {
            get { return new double[] { _x, _y }; }
        }

        public double this[int element]
        {
            get 
            {
                if (element == 0)
                    return _x;
                if (element == 1)
                    return _y;

                throw new IndexOutOfRangeException("The element index must be either 0 or 1 for a 2D point");
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new ViewPoint2D(_x, _y);
        }

        #endregion

        #region IEquatable<IViewVector> Members

        public bool Equals(IViewVector other)
        {
            if (other == null)
                return false;

            if (Elements.Length != other.Elements.Length)
                return false;

            for (int elementIndex = 0; elementIndex < Elements.Length; elementIndex++)
            {
                if (this[elementIndex] != other[elementIndex])
                    return false;
            }

            return true;
        }

        #endregion

        #region IEnumerable<double> Members

        public IEnumerator<double> GetEnumerator()
        {
            yield return _x;
            yield return _y;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public static bool operator ==(ViewPoint2D vector1, IViewVector vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(ViewPoint2D vector1, IViewVector vector2)
        {
            return !(vector1 == vector2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IViewVector);
        }

        public override int GetHashCode()
        {
            return unchecked((int)_x ^ (int)_y);
        }

        public override string ToString()
        {
            return String.Format("ViewPoint2D - ({0:N3}, {1:N3})", _x, _y);
        }
    }
}
