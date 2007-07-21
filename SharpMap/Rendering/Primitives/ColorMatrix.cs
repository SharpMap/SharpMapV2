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
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    public class ColorMatrix : AffineMatrix<DoubleComponent>, IEquatable<ColorMatrix>
    {
        public new readonly static ColorMatrix Identity
            = new ColorMatrix(1, 1, 1, 1, 0, 0, 0);

        public new readonly static ColorMatrix Zero
            = new ColorMatrix(0, 0, 0, 0, 0, 0, 0);

        public ColorMatrix()
            : this(Identity) { }

        public ColorMatrix(double redLevel, double greenLevel, double blueLevel, double alphaLevel,
            double redShift, double greenShift, double blueShift)
            : base(MatrixFormat.RowMajor, 5)
        {
            this[0, 0] = redLevel;
            this[1, 1] = greenLevel;
            this[2, 2] = blueLevel;
            this[3, 3] = alphaLevel;

            this[4, 0] = redShift;
            this[4, 1] = greenShift;
            this[4, 2] = blueShift;
        }

        public ColorMatrix(IMatrixD matrixToCopy)
            : base(MatrixFormat.RowMajor, 5)
        {
            for (int i = 0; i < RowCount; i++)
            {
                Array.Copy(matrixToCopy.Elements, Elements, matrixToCopy.Elements.Length);
            }
        }

        public override string ToString()
        {
            return String.Format("[{0}] R: {1}; G: {2}; B: {3}; A: {4}; dxR: {5}; dxG: {6}; dxB: {7}", GetType(), R, G, B, A, RedShift, GreenShift, BlueShift);
        }

        public override int GetHashCode()
        {
            return unchecked(R.GetHashCode() + 2352 ^ G.GetHashCode() + 235509 ^ B.GetHashCode() + 753 
                ^ A.GetHashCode() + 89 ^ RedShift.GetHashCode() + 897210 ^ GreenShift.GetHashCode() + 78595 
                ^ BlueShift.GetHashCode() + 9437143);
        }

        #region Equality Computation

        public override bool Equals(object obj)
        {
            if (obj is ColorMatrix)
            {
                return Equals(obj as ColorMatrix);
            }

            if (obj is IMatrixD)
            {
                return Equals(obj as IMatrixD);
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
        #endregion

        public double R
        {
            get { return (double)this[0, 0]; }
            set { this[0, 0] = value; }
        }

        public double G
        {
            get { return (double)this[1, 1]; }
            set { this[1, 1] = value; }
        }

        public double B
        {
            get { return (double)this[2, 2]; }
            set { this[2, 2] = value; }
        }

        public double A
        {
            get { return (double)this[3, 3]; }
            set { this[3, 3] = value; }
        }

        public double RedShift
        {
            get { return (double)this[0, 4]; }
            set { this[0, 4] = value; }
        }

        public double GreenShift
        {
            get { return (double)this[1, 4]; }
            set { this[1, 4] = value; }
        }

        public double BlueShift
        {
            get { return (double)this[2, 4]; }
            set { this[2, 4] = value; }
        }

        public new ColorMatrix Clone()
        {
            return new ColorMatrix(this);
        }
    }
}
