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
using NPack;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// Represents a 2 dimensional affine transform matrix (a 3x3 matrix).
    /// </summary>
    [Serializable]
    public class ViewMatrix2D : AffineMatrix<DoubleComponent>
    {
        public new readonly static ViewMatrix2D Identity
            = new ViewMatrix2D(
                1, 0, 0,
                0, 1, 0,
                0, 0, 1);

        public new readonly static ViewMatrix2D Zero
            = new ViewMatrix2D(
                0, 0, 0,
                0, 0, 0,
                0, 0, 0);

        private double _x1, _x2, _x3;
        private double _y1, _y2, _y3;
        private double _w1, _w2, _w3;

        #region Constructors
        public ViewMatrix2D()
            : this(Identity) { }

        public ViewMatrix2D(double x1, double x2, double x3,
            double y1, double y2, double y3,
            double w1, double w2, double w3)
            :base(3, 3)
        {
            _x1 = x1; _x2 = x2; _x3 = x3;
            _y1 = y1; _y2 = y2; _y3 = y3;
            _w1 = w1; _w2 = w2; _w3 = w3;
        }

        public ViewMatrix2D(ViewMatrix2D matrixToCopy)
            : base(3, 3)
        {
            this._x1 = matrixToCopy._x1;
            this._x2 = matrixToCopy._x2;
            this._x3 = matrixToCopy._x3;

            this._y1 = matrixToCopy._y1;
            this._y2 = matrixToCopy._y2;
            this._y3 = matrixToCopy._y3;

            this._w1 = matrixToCopy._w1;
            this._w2 = matrixToCopy._w2;
            this._w3 = matrixToCopy._w3;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("[ViewMatrix2D] [ [{0:N3}, {1:N3}, {2:N3}], [{3:N3}, {4:N3}, {5:N3}], [{6:N3}, {7:N3}, {8:N3}] ]",
                X1, X2, X3, Y1, Y2, Y3, W1, W2, W3);
        }

        public override int GetHashCode()
        {
            return unchecked(_x1.GetHashCode() + 24243 ^ _x2.GetHashCode() + 7318674 ^ _x3.GetHashCode() + 282 ^ _y1.GetHashCode() + 54645 ^ _y2.GetHashCode() + 42 ^ _y3.GetHashCode() + 244892 ^ _w1.GetHashCode() + 8464 ^ _w1.GetHashCode() + 36565 ^ _w2.GetHashCode() + 3210186 ^ _w3.GetHashCode() + 8373428);
        }

        #region Equality Computation

        public override bool Equals(object obj)
        {
            if (obj is ViewMatrix2D)
            {
                return Equals(obj as ViewMatrix2D);
            }

            if (obj is IViewMatrix)
            {
                return Equals(obj as IViewMatrix);
            }

            return false;
        }

        #region IEquatable<ViewMatrix2D> Members

        public bool Equals(ViewMatrix2D other)
        {
            return X1 == other.X1 &&
                X2 == other.X2 &&
                X3 == other.X3 && 
                Y1 == other.Y1 &&
                Y2 == other.Y2 &&
                Y3 == other.Y3 &&
                W1 == other.W1 &&
                W2 == other.W2 &&
                W3 == other.W3;
        }

        #endregion

        #region IEquatable<IViewMatrix> Members

        public bool Equals(IViewMatrix other)
        {
            if (other == null)
            {
                return false;
            }

            double[,] lhs = this.Elements;
            double[,] rhs = other.Elements;

            if (lhs.Length != rhs.Length)
            {
                return false;
            }

            return lhs[0, 0] == rhs[0, 0] &&
                lhs[0, 1] == rhs[0, 1] &&
                lhs[1, 0] == rhs[1, 0] &&
                lhs[1, 1] == rhs[1, 1];
        }

        #endregion
        #endregion

        #region Properties
        public double X1
        {
            get { return _x1; }
            set { _x1 = value; }
        }

        public double X2
        {
            get { return _x2; }
            set { _x2 = value; }
        }

        public double X3
        {
            get { return _x3; }
            set { _x3 = value; }
        }

        public double Y1
        {
            get { return _y1; }
            set { _y1 = value; }
        }

        public double Y2
        {
            get { return _y2; }
            set { _y2 = value; }
        }

        public double Y3
        {
            get { return _y3; }
            set { _y3 = value; }
        }

        public double W1
        {
            get { return _w1; }
            set { _w1 = value; }
        }

        public double W2
        {
            get { return _w2; }
            set { _w2 = value; }
        }

        public double W3
        {
            get { return _w3; }
            set { _w3 = value; }
        }
        #endregion

        #region IViewMatrix Members

        public double[,] Elements
        {
            get
            {
                double[,] elements = new double[,] {
                { _x1, _x2, _x3 },
                { _y1, _y2, _y3 },
                { _w1, _w2, _w3 } };

                return elements;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Rank != 2 || value.GetLength(0) != 3 || value.GetLength(1) != 3)
                {
                    throw new ArgumentException("Elements must be a 3x3 array");
                }

                _x1 = value[0, 0];
                _x2 = value[0, 1];
                _x3 = value[0, 2];

                _y1 = value[1, 0];
                _y2 = value[1, 1];
                _y3 = value[1, 2];

                _w1 = value[2, 0];
                _w2 = value[2, 1];
                _w3 = value[2, 2];
            }
        }

        public bool IsInvertible
        {
            get
            {
                return true;
            }
        }

        public bool IsEmpty
        {
            get { return false; }
        }

        public void Invert()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            Elements = Identity.Elements;
        }

        public void Multiply(IViewMatrix matrix)
        {
            throw new NotImplementedException();
        }

        public double GetOffset(int dimension)
        {
            if (dimension == 0)
                return W1;
            if (dimension == 1)
                return W2;
            else
                throw new ArgumentOutOfRangeException("dimension", dimension, "Argument must be 0 or 1");
        }

        public void Offset(IViewVector offsetVector)
        {
            throw new NotImplementedException();
        }

        public void Rotate(double degreesTheta)
        {
            throw new NotImplementedException();
        }

        public void RotateAt(double degreesTheta, IViewVector center)
        {
            throw new NotImplementedException();
        }

        public void Scale(double scaleAmount)
        {
            throw new NotImplementedException();
        }

        public void Scale(IViewVector scaleVector)
        {
            throw new NotImplementedException();
        }

        public void Translate(double scaleAmount)
        {
            throw new NotImplementedException();
        }

        public void Translate(IViewVector translationVector)
        {
            throw new NotImplementedException();
        }

        public IViewVector Transform(IViewVector vector)
        {
            throw new NotImplementedException();
        }

        public double[] Transform(params double[] vector)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICloneable Members

        public new ViewMatrix2D Clone()
        {
            return new ViewMatrix2D(this);
        }

        #endregion
    }
}
