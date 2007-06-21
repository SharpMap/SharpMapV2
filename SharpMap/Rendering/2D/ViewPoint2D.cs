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
    /// <summary>
    /// A point in 2 dimensional Cartesian space.
    /// </summary>
    [Serializable]
    public struct ViewPoint2D : IViewVector
    {
        public static readonly ViewPoint2D Empty = new ViewPoint2D();
        public static readonly ViewPoint2D Zero = new ViewPoint2D(0, 0);

        private double _x, _y;
        private bool _hasValue;

        #region Constructors
        public ViewPoint2D(double x, double y)
        {
            _x = x;
            _y = y;
            _hasValue = true;
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
            _hasValue = true;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("[ViewPoint2D] ({0:N3}, {1:N3})", _x, _y);
        }

        public override int GetHashCode()
        {
            return unchecked((int)_x ^ (int)_y);
        }

        #region Properties
        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }
        #endregion

        #region Equality Testing

        public static bool operator ==(ViewPoint2D lhs, IViewVector rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ViewPoint2D lhs, IViewVector rhs)
        {
			return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj)
        {
			if (obj is ViewPoint2D)
			{
				return Equals((ViewPoint2D)obj);
			}

			if (obj is IViewVector)
			{
				return Equals(obj as IViewVector);
			}

			return false;
		}

		public bool Equals(ViewPoint2D point)
		{
			return X == point.X &&
				Y == point.Y &&
				IsEmpty == point.IsEmpty;
		}

        #region IEquatable<IViewVector> Members

        public bool Equals(IViewVector other)
        {
            if (other == null)
            {
                return false;
            }

            if (Elements.Length != other.Elements.Length)
            {
                return false;
            }

            for (int elementIndex = 0; elementIndex < Elements.Length; elementIndex++)
            {
                if (this[elementIndex] != other[elementIndex])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
        #endregion

        #region IViewVector Members

        public double[] Elements
        {
            get 
			{
				if (IsEmpty)
				{
					return new double[0];
				}

				return new double[] { _x, _y }; 
			}
        }

        public double this[int element]
        {
            get 
            {
                if (element == 0)
                {
                    return _x;
                }

                if (element == 1)
                {
                    return _y;
                }

                throw new IndexOutOfRangeException("The element index must be either 0 or 1 for a 2D point");
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
            return new ViewPoint2D(_x, _y);
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
    }
}
