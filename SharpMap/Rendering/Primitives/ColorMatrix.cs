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
    public class ColorMatrix : IViewMatrix
    {
        public readonly static ColorMatrix Identity
            = new ColorMatrix(1, 1, 1, 1, 0, 0, 0, 0);

        private double _r1, _g2, _b3, _a4;
        private double _w1, _w2, _w3, _w4;

        public ColorMatrix()
            : this(Identity) { }

        public ColorMatrix(double red, double green, double blue, double alpha, 
            double redShift, double greenShift, double blueShift, double alphaShift)
        {
            _r1 = red;
            _g2 = green;
            _b3 = blue;
            _a4 = alpha;

            _w1 = redShift;
            _w2 = greenShift;
            _w3 = blueShift;
            _w4 = alphaShift;
        }

        public ColorMatrix(ColorMatrix matrixToCopy)
        {
            this._r1 = matrixToCopy._r1;
            this._g2 = matrixToCopy._g2;
            this._b3 = matrixToCopy._b3;
            this._a4 = matrixToCopy._a4;

            this._w1 = matrixToCopy._w1;
            this._w2 = matrixToCopy._w2;
            this._w3 = matrixToCopy._w3;
            this._w4 = matrixToCopy._w4;
        }

        public double R
        {
            get { return _r1; }
            set { _r1 = value; }
        }

        public double G
        {
            get { return _g2; }
            set { _g2 = value; }
        }

        public double B
        {
            get { return _b3; }
            set { _b3 = value; }
        }
        
        public double A
        {
            get { return _a4; }
            set { _a4 = value; }
        }

        public double RedShift
        {
            get { return _w1; }
            set { _w1 = value; }
        }

        public double GreenShift
        {
            get { return _w2; }
            set { _w2 = value; }
        }

        public double BlueShift
        {
            get { return _w3; }
            set { _w3 = value; }
        }

        public double AlphaShift
        {
            get { return _w4; }
            set { _w4 = value; }
        }

        #region IViewMatrix Members

        public void Reset()
        {
            Elements = Identity.Elements;
        }

        public void Invert()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsInvertible
        {
            get
            {
#warning: returning true here is wrong
                return true;
            }
        }

        public double[,] Elements
        {
            get
            {
                double[,] elements = new double[,] {
                { _r1, 0, 0, 0, 0 },
                { 0, _g2, 0, 0, 0 },
                { 0, 0, _b3, 0, 0 },
                { 0, 0, 0, _a4, 0 },
                { _w1, _w2, _w3, _w4, 1 } };

                return elements;
            }
            set
            {
                if (value.Rank != 2 || value.GetLength(0) != 5 || value.GetLength(1) != 5)
                    throw new ArgumentOutOfRangeException("Elements must be a 5x5 array");

                _r1 = value[0, 0];
                _g2 = value[1, 1];
                _b3 = value[2, 2];
                _a4 = value[3, 3];

                _w1 = value[4, 0];
                _w2 = value[4, 1];
                _w3 = value[4, 2];
                _w4 = value[4, 3];
            }
        }

        public void Rotate(double degreesTheta)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RotateAt(double degreesTheta, IViewVector center)
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

        public void Multiply(IViewMatrix matrix)
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

        public void Translate(double translationAmount)
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

        public ColorMatrix Clone()
        {
            return new ColorMatrix(this);
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
