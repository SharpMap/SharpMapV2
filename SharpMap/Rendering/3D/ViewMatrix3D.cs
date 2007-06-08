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
    public class ViewMatrix3D : IViewMatrix
    {
        public readonly static ViewMatrix3D Identity
            = new ViewMatrix3D(1, 0, 0, 0,
                         0, 1, 0, 0,
                         0, 0, 1, 0,
                         0, 0, 0, 1);

        private double _x1, _x2, _x3, _x4;
        private double _y1, _y2, _y3, _y4;
        private double _z1, _z2, _z3, _z4;
        private double _w1, _w2, _w3, _w4;

        public ViewMatrix3D()
            : this(Identity) { }

        public ViewMatrix3D(double x1, double x2, double x3, double x4,
            double y1, double y2, double y3, double y4,
            double z1, double z2, double z3, double z4,
            double w1, double w2, double w3, double w4)
        {
            _x1 = x1; _x2 = x2; _x3 = x3; _x4 = x4;
            _y1 = y1; _y2 = y2; _y3 = y3; _y4 = y4;
            _z1 = z1; _z2 = z2; _z3 = z3; _z4 = z4;
            _w1 = w1; _w2 = w2; _w3 = w3; _w4 = w4;
        }

        public ViewMatrix3D(ViewMatrix3D matrixToCopy)
        {
            this._x1 = matrixToCopy._x1;
            this._x2 = matrixToCopy._x2;
            this._x3 = matrixToCopy._x3;
            this._x4 = matrixToCopy._x4;

            this._y1 = matrixToCopy._y1;
            this._y2 = matrixToCopy._y2;
            this._y3 = matrixToCopy._y3;
            this._y4 = matrixToCopy._y4;

            this._z1 = matrixToCopy._z1;
            this._z2 = matrixToCopy._z2;
            this._z3 = matrixToCopy._z3;
            this._z4 = matrixToCopy._z4;

            this._w1 = matrixToCopy._w1;
            this._w2 = matrixToCopy._w2;
            this._w3 = matrixToCopy._w3;
            this._w4 = matrixToCopy._w4;
        }

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

        public double X4
        {
            get { return _x4; }
            set { _x4 = value; }
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

        public double Y4
        {
            get { return _y4; }
            set { _y4 = value; }
        }
        public double Z1
        {
            get { return _z1; }
            set { _z1 = value; }
        }

        public double Z2
        {
            get { return _z2; }
            set { _z2 = value; }
        }

        public double Z3
        {
            get { return _z3; }
            set { _z3 = value; }
        }

        public double Z4
        {
            get { return _z4; }
            set { _z4 = value; }
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

        public double W4
        {
            get { return _w4; }
            set { _w4 = value; }
        }

        #region IViewMatrix Members

        public double[,] Elements
        {
            get
            {
                double[,] elements = new double[,] {
                { _x1, _x2, _x3, _x4 },
                { _y1, _y2, _y3, _y4 },
                { _z1, _z2, _z3, _z4 },
                { _w1, _w2, _w3, _w4 } };

                return elements;
            }
            set
            {
                if (value.Rank != 2 || value.GetLength(0) != 4 || value.GetLength(1) != 4)
                    throw new ArgumentOutOfRangeException("Elements must be a 4x4 array");

                _x1 = value[0, 0];
                _x2 = value[0, 1];
                _x3 = value[0, 2];
                _x4 = value[0, 3];

                _y1 = value[1, 0];
                _y2 = value[1, 1];
                _y3 = value[1, 2];
                _y4 = value[1, 3];

                _z1 = value[2, 0];
                _z2 = value[2, 1];
                _z3 = value[2, 2];
                _z4 = value[2, 3];

                _w1 = value[3, 0];
                _w2 = value[3, 1];
                _w3 = value[3, 2];
                _w4 = value[3, 3];
            }
        }

        public bool IsInvertible
        {
            get
            {
#warning: returning true here is wrong
                return true;
            }
        }

        public void Invert()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Reset()
        {
            Elements = Identity.Elements;
        }

        public void Multiply(IViewMatrix matrix)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public double GetOffset(int dimension)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Offset(IViewVector offsetVector)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Rotate(double degreesTheta)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RotateAt(double degreesTheta, IViewVector center)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Scale(double scaleAmount)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Scale(IViewVector scaleVector)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Translate(double scaleAmount)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Translate(IViewVector translationVector)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IViewVector Transform(IViewVector vector)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public double[] Transform(params double[] vector)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ICloneable Members

        public ViewMatrix3D Clone()
        {
            return new ViewMatrix3D(this);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region IEquatable<IViewMatrix> Members

        public bool Equals(IViewMatrix other)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
