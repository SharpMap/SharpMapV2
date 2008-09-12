
/*
 *	Portions of this file are  © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 *
 *  Original notices below.
 * 
 */
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;
using NPack.Interfaces;

namespace AGG
{
    //---------------------------------------------------------------line_coord
    public struct LineCoord<T>
                   where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public static int Conv(T x)
        {
            return (int)x.Multiply(LineAABasics.LineSubPixelScale).Round().ToInt();
        }
    };

    //-----------------------------------------------------------line_coord_sat
    public struct LineCoordSat<T>
                   where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public static int Conv(T x)
        {
            return Basics.RoundInt(x.Multiply(LineAABasics.LineSubPixelScale), LineAABasics.LineMaxCoord);
        }
    };

    //==========================================================line_parameters
    public struct LineParameters
    {
        //---------------------------------------------------------------------
        public int X1, Y1, X2, Y2, dX, dY, sX, sY;
        public bool Vertical;
        public int Inc;
        public int Len;
        public int Octant;

        //-------------------------------------------------------------------------
        // The number of the octant is determined as a 3-bit Value as follows:
        // bit 0 = vertical flag
        // bit 1 = sx < 0
        // bit 2 = sy < 0
        //
        // [N] shows the number of the orthogonal quadrant
        // <M> shows the number of the diagonal quadrant
        //               <1>
        //   [1]          |          [0]
        //       . (3)011 | 001(1) .
        //         .      |      .
        //           .    |    . 
        //             .  |  . 
        //    (2)010     .|.     000(0)
        // <2> ----------.+.----------- <0>
        //    (6)110   .  |  .   100(4)
        //           .    |    .
        //         .      |      .
        //       .        |        .
        //         (7)111 | 101(5) 
        //   [2]          |          [3]
        //               <3> 
        //                                                        0,1,2,3,4,5,6,7 
        public static byte[] SOrthogonalQuadrant = { 0, 0, 1, 1, 3, 3, 2, 2 };
        public static byte[] SDiagonalQuadrant = { 0, 1, 2, 1, 0, 3, 2, 3 };

        //---------------------------------------------------------------------
        public LineParameters(int x1_, int y1_, int x2_, int y2_, int len_)
        {
            X1 = (x1_);
            Y1 = (y1_);
            X2 = (x2_);
            Y2 = (y2_);
            dX = (Math.Abs(x2_ - x1_));
            dY = (Math.Abs(y2_ - y1_));
            sX = ((x2_ > x1_) ? 1 : -1);
            sY = ((y2_ > y1_) ? 1 : -1);
            Vertical = (dY >= dX);
            Inc = (Vertical ? sY : sX);
            Len = (len_);
            Octant = ((sY & 4) | (sX & 2) | (Vertical ? 1 : 0));
        }

        //---------------------------------------------------------------------
        public uint OrthogonalQuadrant() { return SOrthogonalQuadrant[Octant]; }
        public uint DiagonalQuadrant() { return SDiagonalQuadrant[Octant]; }

        //---------------------------------------------------------------------
        public bool SameOrthogonalQuadrant(LineParameters lp)
        {
            return SOrthogonalQuadrant[Octant] == SOrthogonalQuadrant[lp.Octant];
        }

        //---------------------------------------------------------------------
        public bool SameDiagonalQuadrant(LineParameters lp)
        {
            return SDiagonalQuadrant[Octant] == SDiagonalQuadrant[lp.Octant];
        }

        //---------------------------------------------------------------------
        public void Divide(LineParameters lp1, LineParameters lp2)
        {
            int xmid = (X1 + X2) >> 1;
            int ymid = (Y1 + Y2) >> 1;
            int len2 = Len >> 1;

            lp1 = this; // it is a struct so this is a copy
            lp2 = this; // it is a struct so this is a copy

            lp1.X2 = xmid;
            lp1.Y2 = ymid;
            lp1.Len = len2;
            lp1.dX = Math.Abs(lp1.X2 - lp1.X1);
            lp1.dY = Math.Abs(lp1.Y2 - lp1.Y1);

            lp2.X1 = xmid;
            lp2.Y1 = ymid;
            lp2.Len = len2;
            lp2.dX = Math.Abs(lp2.X2 - lp2.X1);
            lp2.dY = Math.Abs(lp2.Y2 - lp2.Y1);
        }
    };

    public static class LineAABasics
    {
        public const int LineSubPixelShift = 8;                          //----line_subpixel_shift
        public const int LineSubPixelScale = 1 << LineSubPixelShift;  //----line_subpixel_scale
        public const int LineSubPixelMask = LineSubPixelScale - 1;    //----line_subpixel_mask
        public const int LineMaxCoord = (1 << 28) - 1;              //----line_max_coord
        public const int LineMaxLength = 1 << (LineSubPixelShift + 10); //----line_max_length

        //-------------------------------------------------------------------------
        public const int LineMrSubPixelShift = 4;                           //----line_mr_subpixel_shift
        public const int LineMrSubPixelScale = 1 << LineMrSubPixelShift; //----line_mr_subpixel_scale 
        public const int LineMrSubPixelMask = LineMrSubPixelScale - 1;   //----line_mr_subpixel_mask 

        //------------------------------------------------------------------line_mr
        public static int LineMr(int x)
        {
            return x >> (LineSubPixelShift - LineMrSubPixelShift);
        }

        //-------------------------------------------------------------------line_hr
        public static int LineHr(int x)
        {
            return x << (LineSubPixelShift - LineMrSubPixelShift);
        }

        //---------------------------------------------------------------line_dbl_hr
        public static int LineDblHr(int x)
        {
            return x << LineSubPixelShift;
        }

        //-------------------------------------------------------------------------
        public static void BiSectrix(LineParameters l1,
                   LineParameters l2,
                   out int x, out int y)
        {
            double k = (double)(l2.Len) / (double)(l1.Len);
            double tx = l2.X2 - (l2.X1 - l1.X1) * k;
            double ty = l2.Y2 - (l2.Y1 - l1.Y1) * k;

            //All bisectrices must be on the right of the line
            //If the next point is on the left (l1 => l2.2)
            //then the bisectix should be rotated by 180 degrees.
            if ((double)(l2.X2 - l2.X1) * (double)(l2.Y1 - l1.Y1) <
               (double)(l2.Y2 - l2.Y1) * (double)(l2.X1 - l1.X1) + 100.0)
            {
                tx -= (tx - l2.X1) * 2.0;
                ty -= (ty - l2.Y1) * 2.0;
            }

            // Check if the bisectrix is too short
            double dx = tx - l2.X1;
            double dy = ty - l2.Y1;
            if ((int)Math.Sqrt(dx * dx + dy * dy) < LineSubPixelScale)
            {
                x = (l2.X1 + l2.X1 + (l2.Y1 - l1.Y1) + (l2.Y2 - l2.Y1)) >> 1;
                y = (l2.Y1 + l2.Y1 - (l2.X1 - l1.X1) - (l2.X2 - l2.X1)) >> 1;
                return;
            }

            x = Basics.RoundInt(tx);
            y = Basics.RoundInt(ty);
        }

        //-------------------------------------------fix_degenerate_bisectrix_start
        public static void FixDegenerateBisectrixStart(LineParameters lp,
                                               ref int x, ref int y)
        {
            int d = Basics.RoundInt(((double)(x - lp.X2) * (double)(lp.Y2 - lp.Y1) -
                            (double)(y - lp.Y2) * (double)(lp.X2 - lp.X1)) / lp.Len);
            if (d < LineSubPixelScale / 2)
            {
                x = lp.X1 + (lp.Y2 - lp.Y1);
                y = lp.Y1 - (lp.X2 - lp.X1);
            }
        }

        //---------------------------------------------fix_degenerate_bisectrix_end
        public static void FixDegenerateBisectrixEnd(LineParameters lp,
                                             ref int x, ref int y)
        {
            int d = Basics.RoundInt(((double)(x - lp.X2) * (double)(lp.Y2 - lp.Y1) -
                            (double)(y - lp.Y2) * (double)(lp.X2 - lp.X1)) / lp.Len);
            if (d < LineSubPixelScale / 2)
            {
                x = lp.X2 + (lp.Y2 - lp.Y1);
                y = lp.Y2 - (lp.X2 - lp.X1);
            }
        }
    };
}
