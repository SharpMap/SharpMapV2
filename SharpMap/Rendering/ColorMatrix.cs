// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
    public class ColorMatrix : AffineMatrix, IEquatable<ColorMatrix>
    {
        private readonly static ColorMatrix _identity
            = new ColorMatrix(1, 1, 1, 1, 0, 0, 0, 0);

        public static new ColorMatrix Identity
        {
            get { return _identity.Clone(); }
        }

        public ColorMatrix()
            : this(Identity) { }

        public ColorMatrix(Double redLevel, Double greenLevel, Double blueLevel, Double alphaLevel,
			Double redShift, Double greenShift, Double blueShift, Double alphaShift)
            : base(MatrixFormat.RowMajor, 5)
        {
            this[0, 0] = new DoubleComponent(redLevel);
            this[1, 1] = new DoubleComponent(greenLevel);
            this[2, 2] = new DoubleComponent(blueLevel);
            this[3, 3] = new DoubleComponent(alphaLevel);

            this[4, 0] = new DoubleComponent(redShift);
            this[4, 1] = new DoubleComponent(greenShift);
            this[4, 2] = new DoubleComponent(blueShift);
			this[4, 3] = new DoubleComponent(alphaShift);
		}

        public ColorMatrix(IMatrixD matrixToCopy)
            : base(MatrixFormat.RowMajor, 5)
        {
            for (Int32 i = 0; i < 5; i++)
            {
                for (Int32 j = 0; j < 5; j++)
                {
                    this[i, j] = matrixToCopy[i, j];
                }
            }
        }

        public override String ToString()
        {
			return String.Format("[{0}] R: {1}; G: {2}; B: {3}; A: {4}; "+
                                 "dxR: {5}; dxG: {6}; dxB: {7}; dxA: {8}", 
                                 GetType(), R, G, B, A, 
                                 RedShift, GreenShift, BlueShift, AlphaShift);
        }

        public override Int32 GetHashCode()
        {
            return unchecked(
                R.GetHashCode() * 17 ^ 
                G.GetHashCode() * 37 ^ 
                B.GetHashCode() * 41 ^ 
                A.GetHashCode() * 43 ^ 
                RedShift.GetHashCode() * 47 ^ 
                GreenShift.GetHashCode() * 53 ^
                BlueShift.GetHashCode() * 59 ^
                AlphaShift.GetHashCode() * 61);
        }

        #region Equality Computation

        public override Boolean Equals(Object obj)
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

        public Boolean Equals(ColorMatrix other)
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

        public Double R
        {
            get { return (Double)this[0, 0]; }
            set { this[0, 0] = value; }
        }

        public Double G
        {
            get { return (Double)this[1, 1]; }
            set { this[1, 1] = value; }
        }

        public Double B
        {
            get { return (Double)this[2, 2]; }
            set { this[2, 2] = value; }
        }

        public Double A
        {
            get { return (Double)this[3, 3]; }
            set { this[3, 3] = value; }
        }

        public Double RedShift
        {
            get { return (Double)this[4, 0]; }
            set { this[4, 0] = value; }
        }

        public Double GreenShift
        {
            get { return (Double)this[4, 1]; }
            set { this[4, 1] = value; }
        }

        public Double BlueShift
        {
            get { return (Double)this[4, 2]; }
            set { this[4, 2] = value; }
        }

		public Double AlphaShift
		{
			get { return (Double)this[4, 3]; }
			set { this[4, 3] = value; }
		}
		
		public new ColorMatrix Clone()
        {
            return new ColorMatrix(this);
        }
    }
}
