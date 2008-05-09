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

namespace SharpMap.Rendering.Rendering3D
{
    public class Matrix3D : AffineMatrix<DoubleComponent>
    {
        public new static readonly Matrix3D Identity
            = new Matrix3D(1, 0, 0, 0,
                           0, 1, 0, 0,
                           0, 0, 1, 0,
                           0, 0, 0, 1);

        public Matrix3D()
            : this(Identity) { }

        public Matrix3D(Double x1, Double x2, Double x3, Double x4,
                        Double y1, Double y2, Double y3, Double y4,
                        Double z1, Double z2, Double z3, Double z4,
                        Double w1, Double w2, Double w3, Double w4)
            : base(MatrixFormat.RowMajor, 4)
        {
            X1 = x1;
            X2 = x2;
            X3 = x3;
            X4 = x4;
            Y1 = y1;
            Y2 = y2;
            Y3 = y3;
            Y4 = y4;
            Z1 = z1;
            Z2 = z2;
            Z3 = z3;
            Z4 = z4;
            W1 = w1;
            W2 = w2;
            W3 = w3;
            W4 = w4;
        }

        public Matrix3D(IMatrixD matrixToCopy)
            : base(MatrixFormat.RowMajor, 4)
        {
            if (matrixToCopy.ColumnCount != 4)
            {
                throw new ArgumentException("Parameter has an incompatable number columns. " +
                                             "A 3D affine matrix requires 4.", "matrixToCopy");
            }

            if (matrixToCopy.RowCount != 4)
            {
                throw new ArgumentException("Parameter has an incompatable number rows. " +
                                            "A 3D affine matrix requires 4.", "matrixToCopy");
            }

            for (Int32 i = 0; i < 4; i++)
            {
                for (Int32 j = 0; j < 4; j++)
                {
                    this[i, j] = matrixToCopy[i, j];
                }
            }
        }

        public Double X1
        {
            get { return (Double)this[0, 0]; }
            set { this[0, 0] = value; }
        }

        public Double X2
        {
            get { return (Double)this[1, 0]; }
            set { this[1, 0] = value; }
        }

        public Double X3
        {
            get { return (Double)this[2, 0]; }
            set { this[2, 0] = value; }
        }

        public Double X4
        {
            get { return (Double)this[3, 0]; }
            set { this[3, 0] = value; }
        }

        public Double Y1
        {
            get { return (Double)this[0, 1]; }
            set { this[0, 1] = value; }
        }

        public Double Y2
        {
            get { return (Double)this[1, 1]; }
            set { this[1, 1] = value; }
        }

        public Double Y3
        {
            get { return (Double)this[2, 1]; }
            set { this[2, 1] = value; }
        }

        public Double Y4
        {
            get { return (Double)this[3, 1]; }
            set { this[3, 1] = value; }
        }

        public Double Z1
        {
            get { return (Double)this[0, 2]; }
            set { this[0, 2] = value; }
        }

        public Double Z2
        {
            get { return (Double)this[1, 2]; }
            set { this[1, 2] = value; }
        }

        public Double Z3
        {
            get { return (Double)this[2, 2]; }
            set { this[2, 2] = value; }
        }

        public Double Z4
        {
            get { return (Double)this[3, 2]; }
            set { this[3, 2] = value; }
        }

        public Double W1
        {
            get { return (Double)this[0, 3]; }
            set { this[0, 3] = value; }
        }

        public Double W2
        {
            get { return (Double)this[1, 3]; }
            set { this[1, 3] = value; }
        }

        public Double W3
        {
            get { return (Double)this[2, 3]; }
            set { this[2, 3] = value; }
        }

        public Double W4
        {
            get { return (Double)this[3, 3]; }
            set { this[3, 3] = value; }
        }
    }
}