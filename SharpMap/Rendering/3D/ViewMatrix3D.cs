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

namespace SharpMap.Rendering.Rendering3D
{
    public class ViewMatrix3D : AffineMatrix<DoubleComponent>
    {
        public new static readonly ViewMatrix3D Identity
            = new ViewMatrix3D(1, 0, 0, 0,
                               0, 1, 0, 0,
                               0, 0, 1, 0,
                               0, 0, 0, 1);

        public ViewMatrix3D()
            : this(Identity)
        {
        }

        public ViewMatrix3D(double x1, double x2, double x3, double x4,
                            double y1, double y2, double y3, double y4,
                            double z1, double z2, double z3, double z4,
                            double w1, double w2, double w3, double w4)
            : base(4, 4)
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

        public ViewMatrix3D(IMatrixD matrixToCopy)
            : base(4, 4)
        {
            if(matrixToCopy.ColumnCount != 4)
            {
                throw new  ArgumentException("Parameter has an incompatable number columns. A 3D affine matrix requires 4.", "matrixToCopy");
            }

            if (matrixToCopy.RowCount != 4)
            {
                throw new ArgumentException("Parameter has an incompatable number rows. A 3D affine matrix requires 4.", "matrixToCopy");
            }

            for (int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    this[i, j] = matrixToCopy[i, j];
                }
            }
        }

        public double X1
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double X2
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double X3
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double X4
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double Y1
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double Y2
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double Y3
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double Y4
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double Z1
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double Z2
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double Z3
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double Z4
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double W1
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double W2
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double W3
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }

        public double W4
        {
            get { return (double)ElementArray[0][0]; }
            set { ElementArray[0][0] = value; }
        }
    }
}