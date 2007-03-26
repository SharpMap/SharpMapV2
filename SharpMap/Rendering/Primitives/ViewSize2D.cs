using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public struct ViewSize2D : IViewVector
    {
        private double _width, _height;
        public static readonly ViewSize2D Zero = new ViewSize2D(0, 0);

        public ViewSize2D(double width, double height)
        {
            _width = width;
            _height = height;
        }

        public double Width
        {
            get { return _width; }
        }

        public double Height
        {
            get { return _height; }
        }

        public override string ToString()
        {
            return String.Format("Size - width: {0}, height: {1}", Width, Height);
        }

        public static bool operator != (ViewSize2D size1, ViewSize2D size2)
        {
            return size1.Width != size2.Width || size1.Height != size2.Height;
        }

        public static bool operator ==(ViewSize2D size1, ViewSize2D size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        public override bool Equals(object obj)
        {
            if (obj is IViewVector)
                return Equals(obj as IViewVector);

            if (!(obj is ViewSize2D))
                return false;

            ViewSize2D other = (ViewSize2D)obj;

            return this == other;
        }

        public override int GetHashCode()
        {
            return unchecked(Width.GetHashCode() ^ Height.GetHashCode());
        }

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
                    return Width;
                else if (element == 1)
                    return Height;
                else
                    throw new ArgumentOutOfRangeException("element", element, "Index must be 0 or 1");
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new ViewSize2D(Width, Height);
        }

        #endregion

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
