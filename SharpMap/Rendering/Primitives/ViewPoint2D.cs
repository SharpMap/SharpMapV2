using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpMap.Rendering
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ViewPoint2D : IViewVector
    {
        private double _x, _y;

        public ViewPoint2D(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public ViewPoint2D(double[] elements)
        {
            if (elements == null)
                throw new ArgumentNullException("value");

            if (elements.Length != 2)
                throw new ArgumentException("Elements array must have only 2 components");

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
