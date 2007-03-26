using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public struct ViewPoint3D : IViewVector
    {
        private double _x, _y, _z;

        public ViewPoint3D(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public ViewPoint3D(double[] elements)
        {
            if (elements == null)
                throw new ArgumentNullException("value");

            if (elements.Length != 3)
                throw new ArgumentException("Elements array must have only 3 components");

            _x = elements[0];
            _y = elements[1];
            _z = elements[2];
        }

        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }

        public double Z
        {
            get { return _z; }
        }

        #region IViewVector Members

        public double[] Elements
        {
            get { return new double[] { _x, _y, _z }; }
        }

        public double this[int element]
        {
            get
            {
                if (element == 0)
                    return _x;
                if (element == 1)
                    return _y;
                if (element == 2)
                    return _z;

                throw new IndexOutOfRangeException("The element index must be either 0, 1 or 2 for a 3D point");
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new ViewPoint3D(_x, _y, _z);
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
            yield return _z;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public static bool operator ==(ViewPoint3D vector1, IViewVector vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(ViewPoint3D vector1, IViewVector vector2)
        {
            return !(vector1 == vector2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IViewVector);
        }

        public override int GetHashCode()
        {
            return unchecked((int)_x ^ (int)_y ^ (int)_z);
        }

        public override string ToString()
        {
            return String.Format("ViewPoint3D - ({0:N3}, {1:N3}, {2:N3})", _x, _y, _z);
        }
    }
}
