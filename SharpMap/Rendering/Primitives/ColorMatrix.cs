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
    public class ColorMatrix : IViewMatrix, IEquatable<ColorMatrix>
    {
        public readonly static ColorMatrix Identity
            = new ColorMatrix(1, 1, 1, 1, 0, 0, 0);

        public readonly static ColorMatrix Zero
            = new ColorMatrix(0, 0, 0, 0, 0, 0, 0);

        private double _r1, _g2, _b3, _a4;
        private double _w1, _w2, _w3;

        public ColorMatrix()
            : this(Identity) { }

        public ColorMatrix(double redLevel, double greenLevel, double blueLevel, double alphaLevel, 
            double redShift, double greenShift, double blueShift)
        {
            _r1 = redLevel;
            _g2 = greenLevel;
            _b3 = blueLevel;
            _a4 = alphaLevel;

            _w1 = redShift;
            _w2 = greenShift;
            _w3 = blueShift;
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
        }

        public override string ToString()
        {
            return String.Format("[{0}] R: {1}; G: {2}; B: {3}; A: {4}; dxR: {5}; dxG: {6}; dxB: {7}", GetType(), R, G, B, A, RedShift, GreenShift, BlueShift);
        }

        public override int GetHashCode()
        {
            return unchecked(_r1.GetHashCode() + 2352 ^ _g2.GetHashCode() + 235509 ^ _b3.GetHashCode() + 753 ^ _a4.GetHashCode() + 89 ^ _w1.GetHashCode() + 897210 ^ _w2.GetHashCode() + 78595 ^ _w3.GetHashCode() + 9437143);
        }

        #region Equality Computation
        
        public override bool Equals(object obj)
        {
            if (obj is ColorMatrix)
            {
                return Equals(obj as ColorMatrix);
            }

            if (obj is IViewMatrix)
            {
                return Equals(obj as IViewMatrix);
            }

            return false;
        }

        #region IEquatable<ColorMatrix> Members

        public bool Equals(ColorMatrix other)
        {
            return R == other.R &&
                G == other.G &&
                B == other.B &&
                A == other.A &&
                RedShift == other.RedShift &&
                GreenShift == other.GreenShift &&
                BlueShift == other.BlueShift;
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
                lhs[0, 2] == rhs[0, 2] &&
                lhs[0, 3] == rhs[0, 3] &&
                lhs[0, 4] == rhs[0, 4] &&

                lhs[1, 0] == rhs[1, 0] &&
                lhs[1, 1] == rhs[1, 1] &&
                lhs[1, 2] == rhs[1, 2] &&
                lhs[1, 3] == rhs[1, 3] &&
                lhs[1, 4] == rhs[1, 4] &&

                lhs[2, 0] == rhs[2, 0] &&
                lhs[2, 1] == rhs[2, 1] &&
                lhs[2, 2] == rhs[2, 2] &&
                lhs[2, 3] == rhs[2, 3] &&
                lhs[2, 4] == rhs[2, 4] &&

                lhs[3, 0] == rhs[3, 0] &&
                lhs[3, 1] == rhs[3, 1] &&
                lhs[3, 2] == rhs[3, 2] &&
                lhs[3, 3] == rhs[3, 3] &&
                lhs[3, 4] == rhs[3, 4] &&

                lhs[4, 0] == rhs[4, 0] &&
                lhs[4, 1] == rhs[4, 1] &&
                lhs[4, 2] == rhs[4, 2] &&
                lhs[4, 3] == rhs[4, 3] &&
                lhs[4, 4] == rhs[4, 4];
        }

        #endregion
        #endregion

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

        #region IViewMatrix Members

        public void Reset()
        {
            Elements = Identity.Elements;
        }

        public void Invert()
        {
            throw new NotImplementedException();
        }

        public bool IsInvertible
        {
            get { return true; }
        }

        public bool IsEmpty
        {
            get { return false; }
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
                { _w1, _w2, _w3, 0, 1 } };

                return elements;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Rank != 2 || value.GetLength(0) != 5 || value.GetLength(1) != 5)
                {
                    throw new ArgumentException("Elements must be a 5x5 array");
                }

                _r1 = value[0, 0];
                _g2 = value[1, 1];
                _b3 = value[2, 2];
                _a4 = value[3, 3];

                _w1 = value[4, 0];
                _w2 = value[4, 1];
                _w3 = value[4, 2];
            }
        }

        public void Rotate(double degreesTheta)
        {
            throw new NotImplementedException();
        }

        public void RotateAt(double degreesTheta, IViewVector center)
        {
            throw new NotImplementedException();
        }

        public double GetOffset(int dimension)
        {
            throw new NotImplementedException();
        }

        public void Offset(IViewVector offsetVector)
        {
            throw new NotImplementedException();
        }

        public void Multiply(IViewMatrix matrix)
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

        public void Translate(double translationAmount)
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

        public ColorMatrix Clone()
        {
            return new ColorMatrix(this);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
