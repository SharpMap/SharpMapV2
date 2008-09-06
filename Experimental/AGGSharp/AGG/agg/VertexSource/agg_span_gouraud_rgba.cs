
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
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
using System;
using AGG.Color;
using AGG.Interpolation;
using AGG.Span;
using NPack.Interfaces;

namespace AGG.VertexSource
{

    //=======================================================span_gouraud_rgba
    public sealed class SpanGouraudRgba<T> : SpanGouraud<T>, ISpanGenerator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        bool m_swap;
        int m_y2;
        RgbaCalc m_rgba1;
        RgbaCalc m_rgba2;
        RgbaCalc m_rgba3;

        public enum SubPixelScale
        {
            Shift = 4,
            Scale = 1 << Shift
        };

        //--------------------------------------------------------------------
        public struct RgbaCalc
        {
            public void Init(SpanGouraud<T>.CoordType c1, SpanGouraud<T>.CoordType c2)
            {
                m_x1 = c1.X.Subtract(0.5);
                m_y1 = c1.Y.Subtract(0.5);
                m_dx = c2.X.Subtract(c1.X);
                T dy = c2.Y.Subtract(c1.Y);
                m_1dy = dy.LessThan(1e-5) ? M.New<T>(1e5) : M.One<T>().Divide(dy);
                m_r1 = (int)c1.Color.R;
                m_g1 = (int)c1.Color.G;
                m_b1 = (int)c1.Color.B;
                m_a1 = (int)c1.Color.A;
                m_dr = (int)c2.Color.R - m_r1;
                m_dg = (int)c2.Color.G - m_g1;
                m_db = (int)c2.Color.B - m_b1;
                m_da = (int)c2.Color.A - m_a1;
            }

            public void Calc(T y)
            {
                T k = y.Subtract(m_y1).Multiply(m_1dy);
                if (k.LessThan(0.0)) k = M.Zero<T>();
                if (k.GreaterThan(M.One<T>())) k = M.One<T>();
                m_r = m_r1 + Basics.RoundInt(M.New<T>(m_dr).Multiply(k));
                m_g = m_g1 + Basics.RoundInt(M.New<T>(m_dg).Multiply(k));
                m_b = m_b1 + Basics.RoundInt(M.New<T>(m_db).Multiply(k));
                m_a = m_a1 + Basics.RoundInt(M.New<T>(m_da).Multiply(k));
                m_x = Basics.RoundInt((m_x1.Add(m_dx.Multiply(k))).Multiply((double)SubPixelScale.Scale));
            }

            public T m_x1;
            public T m_y1;
            public T m_dx;
            public T m_1dy;
            public int m_r1;
            public int m_g1;
            public int m_b1;
            public int m_a1;
            public int m_dr;
            public int m_dg;
            public int m_db;
            public int m_da;
            public int m_r;
            public int m_g;
            public int m_b;
            public int m_a;
            public int m_x;
        };

        //--------------------------------------------------------------------
        public SpanGouraudRgba() { }
        public SpanGouraudRgba(RGBA_Bytes c1,
                          RGBA_Bytes c2,
                          RGBA_Bytes c3,
                          T x1, T y1,
                          T x2, T y2,
                          T x3, T y3)
            : this(c1, c2, c3, x1, y1, x2, y2, x3, y3, M.Zero<T>())
        { }

        public SpanGouraudRgba(RGBA_Bytes c1,
                          RGBA_Bytes c2,
                          RGBA_Bytes c3,
                          T x1, T y1,
                          T x2, T y2,
                          T x3, T y3,
                          T d)
            : base(c1, c2, c3, x1, y1, x2, y2, x3, y3, d)
        { }

        //--------------------------------------------------------------------
        public void Prepare()
        {
            unsafe
            {
                CoordType[] coord = new CoordType[3];
                base.ArrangeVertices(coord);

                m_y2 = (int)(coord[1].Y.ToInt());

                m_swap = MathUtil.CrossProduct(coord[0].X, coord[0].Y,
                                       coord[2].X, coord[2].Y,
                                       coord[1].X, coord[1].Y).LessThan(M.Zero<T>());

                m_rgba1.Init(coord[0], coord[2]);
                m_rgba2.Init(coord[0], coord[1]);
                m_rgba3.Init(coord[1], coord[2]);
            }
        }

        //--------------------------------------------------------------------
        unsafe public void Generate(RGBA_Bytes* span, int x, int y, uint len)
        {
            m_rgba1.Calc(M.New<T>(y));//(m_rgba1.m_1dy > 2) ? m_rgba1.m_y1 : y);
            RgbaCalc pc1 = m_rgba1;
            RgbaCalc pc2 = m_rgba2;

            if (y <= m_y2)
            {
                // Bottom part of the triangle (first subtriangle)
                //-------------------------
                m_rgba2.Calc(M.New<T>(y).Add(m_rgba2.m_1dy));
            }
            else
            {
                // Upper part (second subtriangle)
                m_rgba3.Calc(M.New<T>(y).Subtract(m_rgba3.m_1dy));
                //-------------------------
                pc2 = m_rgba3;
            }

            if (m_swap)
            {
                // It means that the triangle is oriented clockwise, 
                // so that we need to swap the controlling structures
                //-------------------------
                RgbaCalc t = pc2;
                pc2 = pc1;
                pc1 = t;
            }

            // Get the horizontal length with subpixel accuracy
            // and protect it from division by zero
            //-------------------------
            int nlen = Math.Abs(pc2.m_x - pc1.m_x);
            if (nlen <= 0) nlen = 1;

            DdaLineInterpolator r = new DdaLineInterpolator(pc1.m_r, pc2.m_r, (uint)nlen, 14);
            DdaLineInterpolator g = new DdaLineInterpolator(pc1.m_g, pc2.m_g, (uint)nlen, 14);
            DdaLineInterpolator b = new DdaLineInterpolator(pc1.m_b, pc2.m_b, (uint)nlen, 14);
            DdaLineInterpolator a = new DdaLineInterpolator(pc1.m_a, pc2.m_a, (uint)nlen, 14);

            // Calculate the starting point of the gradient with subpixel 
            // accuracy and correct (roll back) the interpolators.
            // This operation will also clip the beginning of the span
            // if necessary.
            //-------------------------
            int start = pc1.m_x - (x << (int)SubPixelScale.Shift);
            r.Prev(start);
            g.Prev(start);
            b.Prev(start);
            a.Prev(start);
            nlen += start;

            int vr, vg, vb, va;
            uint lim = 255;

            // Beginning part of the span. Since we rolled back the 
            // interpolators, the color values may have overflowed.
            // So that, we render the beginning part with checking 
            // for overflow. It lasts until "start" is positive;
            // typically it's 1-2 pixels, but may be more in some cases.
            //-------------------------
            while (len != 0 && start > 0)
            {
                vr = r.Y;
                vg = g.Y;
                vb = b.Y;
                va = a.Y;
                if (vr < 0) vr = 0; if (vr > lim) vr = (int)lim;
                if (vg < 0) vg = 0; if (vg > lim) vg = (int)lim;
                if (vb < 0) vb = 0; if (vb > lim) vb = (int)lim;
                if (va < 0) va = 0; if (va > lim) va = (int)lim;
                //span[0].R = (byte)vr;
                //span[0].G = (byte)vg;
                //span[0].B = (byte)vb;
                //span[0].A = (byte)va;
                span[0] = new RGBA_Bytes((byte)vr, (byte)vg, (byte)vb, (byte)va);
                r.Next((int)SubPixelScale.Scale);
                g.Next((int)SubPixelScale.Scale);
                b.Next((int)SubPixelScale.Scale);
                a.Next((int)SubPixelScale.Scale);
                nlen -= (int)SubPixelScale.Scale;
                start -= (int)SubPixelScale.Scale;
                ++span;
                --len;
            }

            // Middle part, no checking for overflow.
            // Actual spans can be longer than the calculated length
            // because of anti-aliasing, thus, the interpolators can 
            // overflow. But while "nlen" is positive we are safe.
            //-------------------------
            while (len != 0 && nlen > 0)
            {
                //span[0].R = (byte)r.Y;
                //span[0].G = (byte)g.Y;
                //span[0].B = (byte)b.Y;
                //span[0].A = (byte)a.Y;
                span[0] = new RGBA_Bytes((byte)r.Y, (byte)g.Y, (byte)b.Y, (byte)a.Y);
                r.Next((int)SubPixelScale.Scale);
                g.Next((int)SubPixelScale.Scale);
                b.Next((int)SubPixelScale.Scale);
                a.Next((int)SubPixelScale.Scale);
                nlen -= (int)SubPixelScale.Scale;
                ++span;
                --len;
            }

            // Ending part; checking for overflow.
            // Typically it's 1-2 pixels, but may be more in some cases.
            //-------------------------
            while (len != 0)
            {
                vr = r.Y;
                vg = g.Y;
                vb = b.Y;
                va = a.Y;
                if (vr < 0) vr = 0; if (vr > lim) vr = (int)lim;
                if (vg < 0) vg = 0; if (vg > lim) vg = (int)lim;
                if (vb < 0) vb = 0; if (vb > lim) vb = (int)lim;
                if (va < 0) va = 0; if (va > lim) va = (int)lim;
                //span[0].R = (byte)vr;
                //span[0].G = (byte)vg;
                //span[0].B = (byte)vb;
                //span[0].A = (byte)va;
                span[0] = new RGBA_Bytes((byte)vr, (byte)vg, (byte)vb, (byte)va);
                r.Next((int)SubPixelScale.Scale);
                g.Next((int)SubPixelScale.Scale);
                b.Next((int)SubPixelScale.Scale);
                a.Next((int)SubPixelScale.Scale);
                ++span;
                --len;
            }
        }
    };
}
