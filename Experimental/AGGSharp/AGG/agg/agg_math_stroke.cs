
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
//
// Stroke math
//
//----------------------------------------------------------------------------
using System;
using AGG.Transform;
using NPack.Interfaces;
using NPack;

namespace AGG
{

    //-------------------------------------------------------------line_cap_e
    public enum LineCap
    {
        Butt,
        Square,
        Round
    };

    //------------------------------------------------------------line_join_e
    public enum LineJoin
    {
        MiterJoin = 0,
        MiterJoinRevert = 1,
        RoundJoin = 2,
        BevelJoin = 3,
        MiterJoinRound = 4
    };


    //-----------------------------------------------------------inner_join_e
    public enum InnerJoin
    {
        InnerBevel,
        InnerMiter,
        InnerJag,
        InnerRound
    };

    //------------------------------------------------------------math_stroke
    public class MathStroke<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_width;
        T m_width_abs;
        T m_width_eps;
        int m_width_sign;
        T m_miter_limit;
        T m_inner_miter_limit;
        T m_approx_scale;
        LineCap m_line_cap;

        public LineCap LineCap
        {
            get { return m_line_cap; }
            set { m_line_cap = value; }
        }
        LineJoin m_line_join;

        public LineJoin LineJoin
        {
            get { return m_line_join; }
            set { m_line_join = value; }
        }
        InnerJoin m_inner_join;

        public InnerJoin InnerJoin
        {
            get { return m_inner_join; }
            set { m_inner_join = value; }
        }



        public MathStroke()
        {
            m_width = default(T).Set(0.5);
            m_width_abs = default(T).Set(0.5);
            m_width_eps = default(T).Set(0.5).Divide(default(T).Set(1024.0));
            m_width_sign = 1;
            m_miter_limit = default(T).Set(4.0);
            m_inner_miter_limit = default(T).Set(1.01);
            m_approx_scale = default(T).Set(1.0);
            LineCap = LineCap.Butt;
            LineJoin = LineJoin.MiterJoin;
            InnerJoin = InnerJoin.InnerMiter;
        }

        //public void LineCap(LineCap lc)     { m_line_cap = lc; }
        //public void line_join(LineJoin lj) { m_line_join = lj; }
        //public void inner_join(InnerJoin ij) { m_inner_join = ij; }

        //public LineCap line_cap() { return m_line_cap; }
        //public LineJoin line_join() { return m_line_join; }
        //public InnerJoin inner_join() { return m_inner_join; }

        //public void width(double w)
        //{
        //    m_width = w * 0.5;
        //    if (m_width < 0)
        //    {
        //        m_width_abs = -m_width;
        //        m_width_sign = -1;
        //    }
        //    else
        //    {
        //        m_width_abs = m_width;
        //        m_width_sign = 1;
        //    }
        //    m_width_eps = m_width / 1024.0;
        //}

        // public void miter_limit(double ml) { m_miter_limit = ml; }
        //public void miter_limit_theta(double t)
        //{
        //    m_miter_limit = 1.0 / Math.Sin(t * 0.5);
        //}

        // public void inner_miter_limit(double ml) { m_inner_miter_limit = ml; }
        //public void approximation_scale(double aproxScale) { m_approx_scale = aproxScale; }

        public T Width
        {
            get { return m_width.Multiply(2.0); }
            set
            {
                T w = value;
                m_width = w.Multiply(0.5);
                if (m_width.LessThan(0))
                {
                    m_width_abs = m_width.Negative();
                    m_width_sign = -1;
                }
                else
                {
                    m_width_abs = m_width;
                    m_width_sign = 1;
                }
                m_width_eps = m_width.Divide(1024.0);
            }
        }
        public T MiterLimit { get { return m_miter_limit; } set { m_miter_limit = value; } }
        public T MiterLimitTheta { set { m_miter_limit = M.One<T>().Divide(value.Multiply(0.5).Sin()); } }
        public T InnerMiterLimit { get { return m_inner_miter_limit; } set { m_inner_miter_limit = value; } }
        public T ApproximationScale { get { return m_approx_scale; } set { m_approx_scale = value; } }

        public void CalcCap(IVertexDest<T> vc, VertexDist<T> v0, VertexDist<T> v1, T len)
        {
            vc.RemoveAll();

            T dx1 = v1.Y.Subtract(v0.Y).Divide(len);
            T dy1 = v1.X.Subtract(v0.X).Divide(len);
            T dx2 = M.Zero<T>();
            T dy2 = M.Zero<T>();

            dx1.MultiplyEquals(m_width);
            dy1.MultiplyEquals(m_width);

            if (LineCap != LineCap.Round)
            {
                if (LineCap == LineCap.Square)
                {
                    dx2 = dy1.Multiply(m_width_sign);
                    dy2 = dx1.Multiply(m_width_sign);
                }
                AddVertex(vc, v0.X.Subtract(dx1).Subtract(dx2), v0.Y.Add(dy1).Subtract(dy2));
                AddVertex(vc, v0.X.Add(dx1).Subtract(dx2), v0.Y.Subtract(dy1).Subtract(dy2));
            }
            else
            {
                T da = m_width_abs.Divide(m_width_abs.Add(M.New<T>(0.125).Divide(m_approx_scale))).Acos().Multiply(2);
                T a1;
                int i;
                int n = M.PI<T>().Divide(da).ToInt();

                da = M.PI<T>().Divide(n + 1);
                AddVertex(vc, v0.X.Subtract(dx1), v0.Y.Add(dy1));
                if (m_width_sign > 0)
                {
                    a1 = M.Atan2(dy1, dx1.Negative());
                    a1.AddEquals(da);
                    for (i = 0; i < n; i++)
                    {
                        AddVertex(vc, v0.X.Add(a1.Cos().Multiply(m_width)),
                                       v0.Y.Add(a1.Sin().Multiply(m_width)));
                        a1.AddEquals(da);
                    }
                }
                else
                {
                    a1 = M.Atan2(dy1.Negative(), dx1);
                    a1.SubtractEquals(da);
                    for (i = 0; i < n; i++)
                    {
                        AddVertex(vc, v0.X.Add(a1.Cos().Multiply(m_width)),
                                       v0.Y.Add(a1.Sin().Multiply(m_width)));
                        a1.SubtractEquals(da);
                    }
                }
                AddVertex(vc, v0.X.Add(dx1), v0.Y.Subtract(dy1));
            }
        }

        public void CalcJoin(
                            IVertexDest<T> vc,
                            VertexDist<T> v0,
                            VertexDist<T> v1,
                            VertexDist<T> v2,
                            T len1,
                            T len2)
        {
            T dx1 = m_width.Multiply(v1.Y.Subtract(v0.Y)).Divide(len1);
            T dy1 = m_width.Multiply(v1.X.Subtract(v0.X)).Divide(len1);
            T dx2 = m_width.Multiply(v2.Y.Subtract(v1.Y)).Divide(len2);
            T dy2 = m_width.Multiply(v2.X.Subtract(v1.X)).Divide(len2);

            vc.RemoveAll();

            T cp = MathUtil.CrossProduct(v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);
            if (cp.NotEqual(0) && cp.GreaterThan(0) == m_width.GreaterThan(0))
            {
                // Inner join
                //---------------
                T limit = (len1.LessThan(len2) ? len1 : len2).Divide(m_width_abs);
                if (limit.LessThan(m_inner_miter_limit))
                {
                    limit = m_inner_miter_limit;
                }

                switch (InnerJoin)
                {
                    default: // inner_bevel
                        AddVertex(vc, v1.X.Add(dx1), v1.Y.Subtract(dy1));
                        AddVertex(vc, v1.X.Add(dx2), v1.Y.Subtract(dy2));
                        break;

                    case InnerJoin.InnerMiter:
                        CalcMiter(vc,
                                   v0, v1, v2, dx1, dy1, dx2, dy2,
                                   LineJoin.MiterJoinRevert,
                                   limit, M.Zero<T>());
                        break;

                    case InnerJoin.InnerJag:
                    case InnerJoin.InnerRound:
                        cp = M.LengthSquared(dx1.Subtract(dx2), dy1.Subtract(dy2));// (dx1 - dx2) * (dx1 - dx2) + (dy1 - dy2) * (dy1 - dy2);
                        if (cp.LessThan(len1.Multiply(len1)) && cp.LessThan(len2.Multiply(len2)))
                        {
                            CalcMiter(vc,
                                       v0, v1, v2, dx1, dy1, dx2, dy2,
                                       LineJoin.MiterJoinRevert,
                                       limit, M.Zero<T>());
                        }
                        else
                        {
                            if (InnerJoin == InnerJoin.InnerJag)
                            {
                                AddVertex(vc, v1.X.Add(dx1), v1.Y.Subtract(dy1));
                                AddVertex(vc, v1.X, v1.Y);
                                AddVertex(vc, v1.X.Add(dx2), v1.Y.Subtract(dy2));
                            }
                            else
                            {
                                AddVertex(vc, v1.X.Add(dx1), v1.Y.Subtract(dy1));
                                AddVertex(vc, v1.X, v1.Y);
                                CalcArc(vc, v1.X, v1.Y, dx2, dy2.Negative(), dx1, dy1.Negative());
                                AddVertex(vc, v1.X, v1.Y);
                                AddVertex(vc, v1.X.Add(dx2), v1.Y.Add(dy2));
                            }
                        }
                        break;
                }
            }
            else
            {
                // Outer join
                //---------------

                // Calculate the distance between v1 and 
                // the central point of the bevel line segment
                //---------------
                T dx = dx1.Add(dx2).Divide(2);
                T dy = dy1.Add(dy2).Divide(2);
                T dbevel = M.Length(dx, dy);// Math.Sqrt(dx * dx + dy * dy);

                if (LineJoin == LineJoin.RoundJoin || LineJoin == LineJoin.BevelJoin)
                {
                    // This is an optimization that reduces the number of points 
                    // in cases of almost collinear segments. If there's no
                    // visible difference between bevel and miter joins we'd rather
                    // use miter join because it adds only one point instead of two. 
                    //
                    // Here we calculate the middle point between the bevel points 
                    // and then, the distance between v1 and this middle point. 
                    // At outer joins this distance always less than stroke width, 
                    // because it's actually the height of an isosceles triangle of
                    // v1 and its two bevel points. If the difference between this
                    // width and this Value is small (no visible bevel) we can 
                    // add just one point. 
                    //
                    // The constant in the expression makes the result approximately 
                    // the same as in round joins and caps. You can safely comment 
                    // out this entire "if".
                    //-------------------
                    if (m_approx_scale.Multiply(m_width_abs.Subtract(dbevel)).LessThan(m_width_eps))
                    {
                        if (MathUtil.CalcIntersection(v0.X.Add(dx1), v0.Y.Subtract(dy1),
                                             v1.X.Add(dx1), v1.Y.Subtract(dy1),
                                             v1.X.Add(dx2), v1.Y.Subtract(dy2),
                                             v2.X.Add(dx2), v2.Y.Subtract(dy2),
                                             out dx, out dy))
                        {
                            AddVertex(vc, dx, dy);
                        }
                        else
                        {
                            AddVertex(vc, v1.X.Add(dx1), v1.Y.Subtract(dy1));
                        }
                        return;
                    }
                }

                switch (LineJoin)
                {
                    case LineJoin.MiterJoin:
                    case LineJoin.MiterJoinRevert:
                    case LineJoin.MiterJoinRound:
                        CalcMiter(vc,
                                   v0, v1, v2, dx1, dy1, dx2, dy2,
                                   LineJoin,
                                   m_miter_limit,
                                   dbevel);
                        break;

                    case LineJoin.RoundJoin:
                        CalcArc(vc, v1.X, v1.Y, dx1, dy1.Negative(), dx2, dy2.Negative());
                        break;

                    default: // Bevel join
                        AddVertex(vc, v1.X.Add(dx1), v1.Y.Subtract(dy1));
                        AddVertex(vc, v1.X.Add(dx2), v1.Y.Subtract(dy2));
                        break;
                }
            }
        }

        private void AddVertex(IVertexDest<T> vc, T x, T y)
        {
            vc.Add(MatrixFactory<T>.CreateVector2D(x, y));
        }

        void CalcArc(IVertexDest<T> vc,
                      T x, T y,
                      T dx1, T dy1,
                      T dx2, T dy2)
        {
            T a1 = M.Atan2(dy1.Multiply(m_width_sign), dx1.Multiply(m_width_sign));
            T a2 = M.Atan2(dy2.Multiply(m_width_sign), dx2.Multiply(m_width_sign));
            T da = a1.Subtract(a2);
            int i, n;

            da = m_width_abs.Divide(m_width_abs.Add(M.New<T>(0.125).Divide(m_approx_scale))).Acos().Multiply(2);

            AddVertex(vc, x.Add(dx1), y.Add(dy1));
            if (m_width_sign > 0)
            {
                if (a1.GreaterThan(a2)) a2.AddEquals(M.PI<T>().Multiply(2));
                n = (int)a2.Subtract(a1).Divide(da).ToInt();
                da = a2.Subtract(a1).Divide(n + 1);
                a1.AddEquals(da);
                for (i = 0; i < n; i++)
                {
                    AddVertex(vc, x.Add(a1.Cos().Multiply(m_width)), y.Add(a1.Sin().Multiply(m_width)));
                    a1.AddEquals(da);
                }
            }
            else
            {
                if (a1.LessThan(a2)) a2.SubtractEquals(M.PI<T>().Multiply(2));
                n = (int)a1.Subtract(a2).Divide(da).ToInt();
                da = a1.Subtract(a2).Divide(n + 1);
                a1.SubtractEquals(da);
                for (i = 0; i < n; i++)
                {
                    AddVertex(vc, x.Add(a1.Cos().Multiply(m_width)), y.Add(a1.Sin().Multiply(m_width)));
                    a1.SubtractEquals(da);
                }
            }
            AddVertex(vc, x.Add(dx2), y.Add(dy2));
        }

        void CalcMiter(IVertexDest<T> vc,
                        VertexDist<T> v0,
                        VertexDist<T> v1,
                        VertexDist<T> v2,
                        T dx1, T dy1,
                        T dx2, T dy2,
                        LineJoin lj,
                        T mlimit,
                        T dbevel)
        {
            T xi = v1.X;
            T yi = v1.Y;
            T di = M.One<T>();
            T lim = m_width_abs.Multiply(mlimit);
            bool miter_limit_exceeded = true; // Assume the worst
            bool intersection_failed = true; // Assume the worst

            if (MathUtil.CalcIntersection(v0.X.Add(dx1), v0.Y.Subtract(dy1),
                                 v1.X.Add(dx1), v1.Y.Subtract(dy1),
                                 v1.X.Add(dx2), v1.Y.Subtract(dy2),
                                 v2.X.Add(dx2), v2.Y.Subtract(dy2),
                                 out xi, out yi))
            {
                // Calculation of the intersection succeeded
                //---------------------
                di = MathUtil.CalcDistance(v1.X, v1.Y, xi, yi);
                if (di.LessThanOrEqualTo(lim))
                {
                    // Inside the miter limit
                    //---------------------
                    AddVertex(vc, xi, yi);
                    miter_limit_exceeded = false;
                }
                intersection_failed = false;
            }
            else
            {
                // Calculation of the intersection failed, most probably
                // the three points lie one straight line. 
                // First check if v0 and v2 lie on the opposite sides of vector: 
                // (v1.x, v1.y) -> (v1.x+dx1, v1.y-dy1), that is, the perpendicular
                // to the line determined by vertices v0 and v1.
                // This condition determines whether the next line segments continues
                // the previous one or goes back.
                //----------------
                T x2 = v1.X.Add(dx1);
                T y2 = v1.Y.Subtract(dy1);
                if ((MathUtil.CrossProduct(v0.X, v0.Y, v1.X, v1.Y, x2, y2).LessThan(0.0)) ==
                   (MathUtil.CrossProduct(v1.X, v1.Y, v2.X, v2.Y, x2, y2).LessThan(0.0)))
                {
                    // This case means that the next segment continues 
                    // the previous one (straight line)
                    //-----------------
                    AddVertex(vc, v1.X.Add(dx1), v1.Y.Subtract(dy1));
                    miter_limit_exceeded = false;
                }
            }

            if (miter_limit_exceeded)
            {
                // Miter limit exceeded
                //------------------------
                switch (lj)
                {
                    case LineJoin.MiterJoinRevert:
                        // For the compatibility with SVG, PDF, etc, 
                        // we use a simple bevel join instead of
                        // "smart" bevel
                        //-------------------
                        AddVertex(vc, v1.X.Add(dx1), v1.Y.Subtract(dy1));
                        AddVertex(vc, v1.X.Add(dx2), v1.Y.Subtract(dy2));
                        break;

                    case LineJoin.MiterJoinRound:
                        CalcArc(vc, v1.X, v1.Y, dx1, dy1.Negative(), dx2, dy2.Negative());
                        break;

                    default:
                        // If no miter-revert, calculate new dx1, dy1, dx2, dy2
                        //----------------
                        if (intersection_failed)
                        {
                            mlimit.MultiplyEquals(m_width_sign);
                            AddVertex(vc, v1.X.Add(dx1).Add(dy1.Multiply(mlimit)),
                                           v1.Y.Subtract(dy1).Add(dx1.Multiply(mlimit)));
                            AddVertex(vc, v1.X.Add(dx2).Subtract(dy2.Multiply(mlimit)),
                                           v1.Y.Subtract(dy2).Subtract(dx2.Multiply(mlimit)));
                        }
                        else
                        {
                            T x1 = v1.X.Add(dx1);
                            T y1 = v1.Y.Subtract(dy1);
                            T x2 = v1.X.Add(dx2);
                            T y2 = v1.Y.Subtract(dy2);
                            di = lim.Subtract(dbevel).Divide(di.Subtract(dbevel));
                            AddVertex(vc, x1.Add(xi.Subtract(x1).Multiply(di)),
                                           y1.Add(yi.Subtract(y1).Multiply(di)));
                            AddVertex(vc, x2.Add(xi.Subtract(x2).Multiply(di)),
                                           y2.Add(yi.Subtract(y2).Multiply(di)));
                        }
                        break;
                }
            }
        }
    };
}