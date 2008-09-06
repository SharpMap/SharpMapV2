
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
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
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
using System.IO;
using NPack.Interfaces;

namespace AGG
{
    static public class DebugFile
    {
        static bool m_FileOpenedOnce = false;

        public static void Print(String message)
        {
            FileStream file;
            if (m_FileOpenedOnce)
            {
                file = new FileStream("test.txt", FileMode.Append, FileAccess.Write);
            }
            else
            {
                file = new FileStream("test.txt", FileMode.Create, FileAccess.Write);
                m_FileOpenedOnce = true;
            }
            StreamWriter sw = new StreamWriter(file);
            sw.Write(message);
            sw.Close();
            file.Close();
        }
    };

    public enum FillingRule
    {
        NonZero,
        EvenOdd
    };

    //----------------------------------------------------poly_subpixel_scale_e
    // These constants determine the subpixel accuracy, to be more precise, 
    // the number of bits of the fractional part of the coordinates. 
    // The possible coordinate capacity in bits can be calculated by formula:
    // sizeof(int) * 8 - poly_subpixel_shift, i.e, for 32-bit integers and
    // 8-bits fractional part the capacity is 24 bits.
    public enum PolySubPixelScale
    {
        Shift = 8,                      //----poly_subpixel_shift
        Scale = 1 << Shift, //----poly_subpixel_scale 
        Mask = Scale - 1,  //----poly_subpixel_mask 
    };


    static public class Basics
    // where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        //----------------------------------------------------------filling_rule_e

        public static unsafe void MemCopy(Byte* pDest, Byte* pSource, int Count)
        {
            for (uint i = 0; i < Count; i++)
            {
                *pDest++ = *pSource++;
            }
        }

        public static unsafe void MemMove(Byte* pDest, Byte* pSource, int Count)
        {
            if (pSource > pDest
                || &pSource[Count] < pDest)
            {
                for (uint i = 0; i < Count; i++)
                {
                    *pDest++ = *pSource++;
                }
            }
            else
            {
                throw new System.NotImplementedException();
            }

        }

        public static unsafe void MemSet(Byte* pDest, byte ByteVal, int Count)
        {
            // the fill is optomized to fill using dwords, you have to pass a valid dword
            uint Val = (uint)((uint)ByteVal << 24) + (uint)(ByteVal << 16) + (uint)(ByteVal << 8) + (uint)ByteVal;

            // dword align to dest
            while (((uint)pDest & 3) != 0
                && Count > 0)
            {
                *pDest++ = ByteVal;
                Count--;
            }

            int NumLongs = Count / 4;

            while (NumLongs-- > 0)
            {
                *((uint*)pDest) = Val;

                pDest += 4;
            }

            switch (Count & 3)
            {
                case 3:
                    pDest[2] = ByteVal;
                    goto case 2;
                case 2:
                    pDest[1] = ByteVal;
                    goto case 1;
                case 1:
                    pDest[0] = ByteVal;
                    break;
            }
        }

        public static unsafe void MemClear(Byte* pDest, int Count)
        {
            // dword align to dest
            while (((uint)pDest & 3) != 0
                && Count > 0)
            {
                *pDest++ = 0;
                Count--;
            }

            int NumLongs = Count / 4;

            while (NumLongs-- > 0)
            {
                *((uint*)pDest) = 0;

                pDest += 4;
            }

            switch (Count & 3)
            {
                case 3:
                    pDest[2] = 0;
                    goto case 2;
                case 2:
                    pDest[1] = 0;
                    goto case 1;
                case 1:
                    pDest[0] = 0;
                    break;
            }
        }

        //------------------------------------------------------------is_equal_eps
        //template<class T> 
        public static bool IsEqualEpsilon(double v1, double v2, double epsilon)
        {
            return Math.Abs(v1 - v2) <= (double)(epsilon);
        }

        //------------------------------------------------------------------deg2rad
        public static double Deg2Rad(double deg)
        {
            return deg * d2r;
        }

        private const double d2r = Math.PI / 180.0;

        //------------------------------------------------------------------rad2deg
        public static double Rad2Deg(double rad)
        {
            return rad * r2d;
        }
        private const double r2d = 180.0 / Math.PI;

        public static int RoundInt<T>(T v)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return v.LessThan(M.Zero<T>()) ? v.Subtract(M.New<T>(0.5)).ToInt() : v.Add(M.New<T>(0.5)).ToInt();
        }

        public static int RoundInt(double v)
        {
            return v < 0 ? (int)(v - 0.5) : (int)(v + 0.5);
        }

        public static int RoundInt<T>(T v, int saturationLimit)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            if (v.LessThan(M.New<T>(-saturationLimit))) return -saturationLimit;
            if (v.GreaterThan(M.New<T>(saturationLimit))) return saturationLimit;
            return RoundInt(v);
        }

        public static uint RoundUint(double v)
        {
            return (uint)(v + 0.5);
        }

        public static uint FloorUint(double v)
        {
            return (uint)(v);
        }


        public static uint CeilingUint<T>(T v)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return (uint)(v.Ceiling().ToInt());
        }


        public static uint CeilingUint(double v)
        {
            return (uint)(Math.Ceiling(v));
        }

    };

    public struct RectInt
    {
        private int m_x1, m_y1, m_x2, m_y2;

        public RectInt(int x1_, int y1_, int x2_, int y2_)
        {
            m_x1 = x1_;
            m_y1 = y1_;
            m_x2 = x2_;
            m_y2 = y2_;
        }

        public void Init(int x1_, int y1_, int x2_, int y2_)
        {
            X1 = x1_;
            Y1 = y1_;
            X2 = x2_;
            Y2 = y2_;
        }

        public int X1
        {
            get
            {
                return m_x1;
            }
            set
            {
                m_x1 = value;
            }
        }

        public int Y1
        {
            get
            {
                return m_y1;
            }
            set
            {
                m_y1 = value;
            }
        }

        public int X2
        {
            get
            {
                return m_x2;
            }
            set
            {
                m_x2 = value;
            }
        }

        public int Y2
        {
            get
            {
                return m_y2;
            }
            set
            {
                m_y2 = value;
            }
        }

        public RectInt Normalize()
        {
            int t;
            if (X1 > X2) { t = X1; X1 = X2; X2 = t; }
            if (Y1 > Y2) { t = Y1; Y1 = Y2; Y2 = t; }
            return this;
        }

        public bool Clip(RectInt r)
        {
            if (X2 > r.X2) X2 = r.X2;
            if (Y2 > r.Y2) Y2 = r.Y2;
            if (X1 < r.X1) X1 = r.X1;
            if (Y1 < r.Y1) Y1 = r.Y1;
            return X1 <= X2 && Y1 <= Y2;
        }

        public bool IsValid()
        {
            return X1 <= X2 && Y1 <= Y2;
        }

        public bool HitTest(int x, int y)
        {
            return (x >= X1 && x <= X2 && y >= Y1 && y <= Y2);
        }

        //-----------------------------------------------------intersect_rectangles
        public void IntersectRectangles(RectInt r1, RectInt r2)
        {
            X1 = r1.X1;
            Y1 = r1.Y1;
            X2 = r1.X2;
            X2 = r1.Y2;
            // First process m_x2,m_y2 because the other order 
            // results in Internal Compiler Error under 
            // Microsoft Visual C++ .NET 2003 69462-335-0000007-18038 in 
            // case of "Maximize Speed" optimization option.
            //-----------------
            if (X2 > r2.X2) X2 = r2.X2;
            if (Y2 > r2.Y2) Y2 = r2.Y2;
            if (X1 < r2.X1) X1 = r2.X1;
            if (Y1 < r2.Y1) Y1 = r2.Y1;
        }


        //---------------------------------------------------------unite_rectangles
        public void UniteRectangles(RectInt r1, RectInt r2)
        {
            X1 = r1.X1;
            Y1 = r1.Y1;
            X2 = r1.X2;
            X2 = r1.Y2;
            if (X2 < r2.X2) X2 = r2.X2;
            if (Y2 < r2.Y2) Y2 = r2.Y2;
            if (X1 > r2.X1) X1 = r2.X1;
            if (Y1 > r2.Y1) Y1 = r2.Y1;
        }
    };

    /// <summary>
    /// todo rename when everything compiles
    /// </summary>
    public struct RectDouble<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        public T x1, y1, x2, y2;

        public RectDouble(T left, T bottom, T right, T top)
        {
            x1 = left;
            y1 = bottom;
            x2 = right;
            y2 = top;
        }

        //public void init(double left, double bottom, double right, double top)
        //{
        //    x1 = left;
        //    y1 = bottom;
        //    x2 = right;
        //    y2 = top;
        //}

        public T Left
        {
            get
            {
                return x1;
            }
            set
            {
                x1 = value;
            }
        }

        public T Bottom
        {
            get
            {
                return y1;
            }
            set
            {
                y1 = value;
            }
        }

        public T Right
        {
            get
            {
                return x2;
            }
            set
            {
                x2 = value;
            }
        }

        public T Top
        {
            get
            {
                return y2;
            }
            set
            {
                y2 = value;
            }
        }

        // This function assumes the rect is normalized
        public T Width
        {
            get
            {
                return Right.Subtract(Left);
            }
        }

        // This function assumes the rect is normalized
        public T Height
        {
            get
            {
                return Top.Subtract(Bottom);
            }
        }

        public RectDouble<T> Normalize()
        {
            T t;
            if (x1.GreaterThan(x2)) { t = x1; x1 = x2; x2 = t; }
            if (y1.GreaterThan(y2)) { t = y1; y1 = y2; y2 = t; }
            return this;
        }

        public bool Clip(RectDouble<T> r)
        {
            if (x2.GreaterThan(r.x2)) x2 = r.x2;
            if (y2.GreaterThan(r.y2)) y2 = r.y2;
            if (x1.LessThan(r.x1)) x1 = r.x1;
            if (y1.LessThan(r.y1)) y1 = r.y1;
            return x1.LessThanOrEqualTo(x2) && y1.LessThanOrEqualTo(y2);
        }

        public bool IsValid()
        {
            return x1.LessThanOrEqualTo(x2) && y1.LessThanOrEqualTo(y2);
        }

        public bool HitTest(T x, T y)
        {
            return (x.GreaterThanOrEqualTo(x1) && x.LessThanOrEqualTo(x2) && y.GreaterThanOrEqualTo(y1) && y.LessThanOrEqualTo(y2));
        }

        //-----------------------------------------------------intersect_rectangles
        public void IntersectRectangles(RectDouble<T> r1, RectDouble<T> r2)
        {
            x1 = r1.x1;
            y1 = r1.y1;
            x2 = r1.x2;
            x2 = r1.y2;
            // First process m_x2,m_y2 because the other order 
            // results in Internal Compiler Error under 
            // Microsoft Visual C++ .NET 2003 69462-335-0000007-18038 in 
            // case of "Maximize Speed" optimization option.
            //-----------------
            if (x2.GreaterThan(r2.x2)) x2 = r2.x2;
            if (y2.GreaterThan(r2.y2)) y2 = r2.y2;
            if (x1.LessThan(r2.x1)) x1 = r2.x1;
            if (y1.LessThanOrEqualTo(r2.y1)) y1 = r2.y1;
        }


        //---------------------------------------------------------unite_rectangles
        public void UniteRectangles(RectDouble<T> r1, RectDouble<T> r2)
        {
            x1 = r1.x1;
            y1 = r1.y1;
            x2 = r1.x2;
            x2 = r1.y2;
            if (x2.LessThan(r2.x2)) x2 = r2.x2;
            if (y2.LessThan(r2.y2)) y2 = r2.y2;
            if (x1.GreaterThan(r2.x1)) x1 = r2.x1;
            if (y1.GreaterThan(r2.y1)) y1 = r2.y1;
        }

        public void Inflate(T inflateSize)
        {
            Left = Left.Subtract(inflateSize);
            Bottom = Bottom.Subtract(inflateSize);
            Right = Right.Add(inflateSize);
            Top = Top.Add(inflateSize);
        }
    };

    //public struct RectDouble
    //{
    //    public double x1, y1, x2, y2;

    //    public RectDouble(double left, double bottom, double right, double top)
    //    {
    //        x1 = left;
    //        y1 = bottom;
    //        x2 = right;
    //        y2 = top;
    //    }

    //    //public void init(double left, double bottom, double right, double top)
    //    //{
    //    //    x1 = left;
    //    //    y1 = bottom;
    //    //    x2 = right;
    //    //    y2 = top;
    //    //}

    //    public double Left
    //    {
    //        get
    //        {
    //            return x1;
    //        }
    //        set
    //        {
    //            x1 = value;
    //        }
    //    }

    //    public double Bottom
    //    {
    //        get
    //        {
    //            return y1;
    //        }
    //        set
    //        {
    //            y1 = value;
    //        }
    //    }

    //    public double Right
    //    {
    //        get
    //        {
    //            return x2;
    //        }
    //        set
    //        {
    //            x2 = value;
    //        }
    //    }

    //    public double Top
    //    {
    //        get
    //        {
    //            return y2;
    //        }
    //        set
    //        {
    //            y2 = value;
    //        }
    //    }

    //    // This function assumes the rect is normalized
    //    public double Width
    //    {
    //        get
    //        {
    //            return Right - Left;
    //        }
    //    }

    //    // This function assumes the rect is normalized
    //    public double Height
    //    {
    //        get
    //        {
    //            return Top - Bottom;
    //        }
    //    }

    //    public RectDouble Normalize()
    //    {
    //        double t;
    //        if (x1 > x2) { t = x1; x1 = x2; x2 = t; }
    //        if (y1 > y2) { t = y1; y1 = y2; y2 = t; }
    //        return this;
    //    }

    //    public bool Clip(RectDouble r)
    //    {
    //        if (x2 > r.x2) x2 = r.x2;
    //        if (y2 > r.y2) y2 = r.y2;
    //        if (x1 < r.x1) x1 = r.x1;
    //        if (y1 < r.y1) y1 = r.y1;
    //        return x1 <= x2 && y1 <= y2;
    //    }

    //    public bool IsValid()
    //    {
    //        return x1 <= x2 && y1 <= y2;
    //    }

    //    public bool HitTest(double x, double y)
    //    {
    //        return (x >= x1 && x <= x2 && y >= y1 && y <= y2);
    //    }

    //    //-----------------------------------------------------intersect_rectangles
    //    public void IntersectRectangles(RectDouble r1, RectDouble r2)
    //    {
    //        x1 = r1.x1;
    //        y1 = r1.y1;
    //        x2 = r1.x2;
    //        x2 = r1.y2;
    //        // First process m_x2,m_y2 because the other order 
    //        // results in Internal Compiler Error under 
    //        // Microsoft Visual C++ .NET 2003 69462-335-0000007-18038 in 
    //        // case of "Maximize Speed" optimization option.
    //        //-----------------
    //        if (x2 > r2.x2) x2 = r2.x2;
    //        if (y2 > r2.y2) y2 = r2.y2;
    //        if (x1 < r2.x1) x1 = r2.x1;
    //        if (y1 < r2.y1) y1 = r2.y1;
    //    }


    //    //---------------------------------------------------------unite_rectangles
    //    public void UniteRectangles(RectDouble r1, RectDouble r2)
    //    {
    //        x1 = r1.x1;
    //        y1 = r1.y1;
    //        x2 = r1.x2;
    //        x2 = r1.y2;
    //        if (x2 < r2.x2) x2 = r2.x2;
    //        if (y2 < r2.y2) y2 = r2.y2;
    //        if (x1 > r2.x1) x1 = r2.x1;
    //        if (y1 > r2.y1) y1 = r2.y1;
    //    }

    //    public void Inflate(double inflateSize)
    //    {
    //        Left = Left - inflateSize;
    //        Bottom = Bottom - inflateSize;
    //        Right = Right + inflateSize;
    //        Top = Top + inflateSize;
    //    }
    //};


    public static class Path
    {
        public enum Commands
        {
            Stop = 0,          //----path_cmd_stop    
            MoveTo = 1,        //----path_cmd_move_to 
            LineTo = 2,        //----path_cmd_line_to 
            Curve3 = 3,        //----path_cmd_curve3  
            Curve4 = 4,        //----path_cmd_curve4  
            CurveN = 5,        //----path_cmd_curveN
            Catrom = 6,        //----path_cmd_catrom
            UbSpline = 7,      //----path_cmd_ubspline
            EndPoly = 0x0F,    //----path_cmd_end_poly
            Mask = 0x0F        //----path_cmd_mask    
        };

        //------------------------------------------------------------path_flags_e
        public enum Flags
        {
            None = 0,         //----path_flags_none 
            CCW = 0x10,       //----path_flags_ccw  
            CW = 0x20,        //----path_flags_cw   
            Close = 0x40,     //----path_flags_close
            Mask = 0xF0       //----path_flags_mask 
        };


        //---------------------------------------------------------------is_vertex
        public static bool IsVertex(uint c)
        {
            return (uint)c >= (uint)Commands.MoveTo
                && (uint)c < (uint)Commands.EndPoly;
        }

        //--------------------------------------------------------------is_drawing
        public static bool IsDrawing(uint c)
        {
            return c >= (uint)Commands.LineTo && c < (uint)Commands.EndPoly;
        }

        //-----------------------------------------------------------------is_stop
        public static bool IsStop(uint c)
        {
            return c == (uint)Commands.Stop;
        }

        //--------------------------------------------------------------is_move_to
        public static bool IsMoveTo(uint c)
        {
            return c == (uint)Commands.MoveTo;
        }

        //--------------------------------------------------------------is_line_to
        public static bool IsLineTo(uint c)
        {
            return c == (uint)Commands.LineTo;
        }

        //----------------------------------------------------------------is_curve
        public static bool IsCurve(uint c)
        {
            return c == (uint)Commands.Curve3
                || c == (uint)Commands.Curve4;
        }

        //---------------------------------------------------------------is_curve3
        public static bool IsCurve3(uint c)
        {
            return c == (uint)Commands.Curve3;
        }

        //---------------------------------------------------------------is_curve4
        public static bool IsCurve4(uint c)
        {
            return c == (uint)Commands.Curve4;
        }

        //-------------------------------------------------------------is_end_poly
        public static bool IsEndPoly(uint c)
        {
            return (c & (uint)Commands.Mask) == (uint)Commands.EndPoly;
        }

        //----------------------------------------------------------------is_close
        public static bool IsClose(uint c)
        {
            return ((int)c & ~(int)(Flags.CW | Flags.CCW)) ==
                   ((uint)Commands.EndPoly | (uint)Flags.Close);
        }

        //------------------------------------------------------------is_next_poly
        public static bool IsNextPoly(uint c)
        {
            return IsStop(c) || IsMoveTo(c) || IsEndPoly(c);
        }

        //-------------------------------------------------------------------is_cw
        public static bool IsCW(uint c)
        {
            return (c & (uint)Flags.CW) != 0;
        }

        //------------------------------------------------------------------is_ccw
        public static bool IsCCW(uint c)
        {
            return (c & (uint)Flags.CCW) != 0;
        }

        //-------------------------------------------------------------is_oriented
        public static bool IsOriented(uint c)
        {
            return (c & ((uint)Flags.CW | (uint)Flags.CCW)) != 0;
        }

        //---------------------------------------------------------------is_closed
        public static bool IsClosed(Flags c)
        {
            return (c & Flags.Close) != 0;
        }

        //----------------------------------------------------------get_close_flag
        public static Flags GetCloseFlag(uint c)
        {
            return (Flags)(c & (uint)Flags.Close);
        }

        //-------------------------------------------------------clear_orientation
        public static Flags ClearOrientation(Flags c)
        {
            return c & ~(Flags.CW | Flags.CCW);
        }

        //---------------------------------------------------------get_orientation
        public static Flags GetOrientation(Flags c)
        {
            return c & (Flags.CW | Flags.CCW);
        }

        /*
        //---------------------------------------------------------set_orientation
        public static path_commands_e set_orientation(uint c, path_flags_e o)
        {
            return clear_orientation(c) | o;
        }
         */

        static public void ShortenPath<T>(VertexSequence<T> vs, T s)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            ShortenPath(vs, s, 0);
        }

        static public void ShortenPath<T>(VertexSequence<T> vs, T s, uint closed)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            //typedef typename VertexSequence::value_type vertex_type;

            if (s.GreaterThan(0.0) && vs.Size() > 1)
            {
                T d;
                int n = (int)(vs.Size() - 2);
                while (n != 0)
                {
                    d = vs[n].Dist;
                    if (d.GreaterThan(s)) break;
                    vs.RemoveLast();
                    s.SubtractEquals(d);
                    --n;
                }
                if (vs.Size() < 2)
                {
                    vs.RemoveAll();
                }
                else
                {
                    n = (int)vs.Size() - 1;
                    VertexDist<T> prev = vs[n - 1];
                    VertexDist<T> last = vs[n];
                    d = prev.Dist.Subtract(s).Divide(prev.Dist);
                    T x = prev.X.Add((last.X.Subtract(prev.X)).Multiply(d));
                    T y = prev.Y.Add((last.Y.Subtract(prev.Y)).Multiply(d));
                    last.X = x;
                    last.Y = y;
                    if (!prev.IsEqual(last)) vs.RemoveLast();
                    vs.Close(closed != 0);
                }
            }
        }

    }
}
