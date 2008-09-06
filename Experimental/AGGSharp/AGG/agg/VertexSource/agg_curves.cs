
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
// Copyright (C) 2005 Tony Juricic (tonygeek@yahoo.com)
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
using AGG.Transform;
using NPack.Interfaces;
using NPack;

namespace AGG.VertexSource
{
    public interface ICurve<T> : IVertexSource<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T AngleTolerance { get; set; }
        //void AngleTolerance(double v);
        //void ApproximationMethod(ApproximationMethod v);
        ApproximationMethod ApproximationMethod { get; set; }
        //void CuspLimit(double v);
        T CuspLimit { get; set; }
        void Reset();
    };

    public enum ApproximationMethod
    {
        Inc,
        Div
    };

    public static class Curves
    {
        //--------------------------------------------curve_approximation_method_e


        public const double CurveDistanceEpsilon = 1e-30;
        public const double CurveCollinearityEpsilon = 1e-30;
        public const double CurveAngleToleranceEpsilon = 0.01;
        public enum CurveRecursionLimit { Limit = 32 };

        //-------------------------------------------------------catrom_to_bezier
        public static Curve4Points<T> CatromToBezier<T>(T x1, T y1,
                                                          T x2, T y2,
                                                          T x3, T y3,
                                                          T x4, T y4)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            // Trans. matrix Catmull-Rom to Bezier
            //
            //  0       1       0       0
            //  -1/6    1       1/6     0
            //  0       1/6     1       -1/6
            //  0       0       1       0
            //
            return new Curve4Points<T>(
                x2,
                y2,
                x1.Negative().Add(6).Multiply(x2).Add(x3).Divide(6),
                y1.Negative().Add(6).Multiply(y2).Add(y3).Divide(6),
                x2.Add(6).Multiply(x3).Subtract(x4).Divide(6),
                y2.Add(6).Multiply(y3).Subtract(y4).Divide(6),
                x3,
                y3);
        }

        //-----------------------------------------------------------------------
        public static Curve4Points<T> CatromToBezier<T>(Curve4Points<T> cp)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return CatromToBezier(cp[0], cp[1], cp[2], cp[3],
                                    cp[4], cp[5], cp[6], cp[7]);
        }


        //-----------------------------------------------------ubspline_to_bezier
        public static Curve4Points<T> UBSplineToBezier<T>(T x1, T y1,
                                                T x2, T y2,
                                                T x3, T y3,
                                                T x4, T y4)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            // Trans. matrix Uniform BSpline to Bezier
            //
            //  1/6     4/6     1/6     0
            //  0       4/6     2/6     0
            //  0       2/6     4/6     0
            //  0       1/6     4/6     1/6
            //
            return new Curve4Points<T>(
                x1.Add(4).Multiply(x2).Add(x3).Divide(6),
                y1.Add(4).Multiply(y2).Add(y3).Divide(6),
                x2.Multiply(4).Add(2).Multiply(x3).Divide(6),
                y2.Multiply(4).Add(2).Multiply(y3).Divide(6),
                x2.Multiply(2).Add(4).Multiply(x3).Divide(6),
                y2.Multiply(2).Add(4).Multiply(y3).Divide(6),
                x2.Add(4).Multiply(x3).Add(x4).Divide(6),
                y2.Add(4).Multiply(y3).Add(y4).Divide(6));
        }


        //-----------------------------------------------------------------------
        public static Curve4Points<T> UBSplineToBezier<T>(Curve4Points<T> cp)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return UBSplineToBezier(cp[0], cp[1], cp[2], cp[3],
                                      cp[4], cp[5], cp[6], cp[7]);
        }



        //------------------------------------------------------hermite_to_bezier
        public static Curve4Points<T> HermiteToBezier<T>(T x1, T y1,
                                                           T x2, T y2,
                                                           T x3, T y3,
                                                           T x4, T y4)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            // Trans. matrix Hermite to Bezier
            //
            //  1       0       0       0
            //  1       0       1/3     0
            //  0       1       0       -1/3
            //  0       1       0       0
            //
            return new Curve4Points<T>(
                x1,
                y1,
                x1.Multiply(3).Add(x3).Divide(3),
                y1.Multiply(3).Add(y3).Divide(3),
                x2.Multiply(3).Subtract(x4).Divide(3),
                y2.Multiply(3).Subtract(y4).Divide(3),
                x2,
                y2);
        }



        //-----------------------------------------------------------------------
        public static Curve4Points<T> HermiteToBezier<T>(Curve4Points<T> cp)
             where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return HermiteToBezier(cp[0], cp[1], cp[2], cp[3],
                                     cp[4], cp[5], cp[6], cp[7]);
        }


    };
    // See Implementation agg_curves.cpp

    //--------------------------------------------------------------curve3_inc
    public sealed class Curve3Inc<T> : ICurve<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        int m_num_steps;
        int m_step;
        T m_scale;
        T m_start_x;
        T m_start_y;
        T m_end_x;
        T m_end_y;
        T m_fx;
        T m_fy;
        T m_dfx;
        T m_dfy;
        T m_ddfx;
        T m_ddfy;
        T m_saved_fx;
        T m_saved_fy;
        T m_saved_dfx;
        T m_saved_dfy;

        public Curve3Inc()
        {
            m_num_steps = (0);
            m_step = (0);
            m_scale = (M.One<T>());
        }

        public Curve3Inc(T x1, T y1,
                           T x2, T y2,
                           T x3, T y3)
        {
            m_num_steps = (0);
            m_step = (0);
            m_scale = (M.One<T>());
            Init(x1, y1, x2, y2, x3, y3);
        }

        public void Reset() { m_num_steps = 0; m_step = -1; }

        public void Init(T x1, T y1,
                  T x2, T y2,
                  T x3, T y3)
        {
            m_start_x = x1;
            m_start_y = y1;
            m_end_x = x3;
            m_end_y = y3;

            T dx1 = x2.Subtract(x1);
            T dy1 = y2.Subtract(y1);
            T dx2 = x3.Subtract(x2);
            T dy2 = y3.Subtract(y2);

            //T len = Math.Sqrt(dx1 * dx1 + dy1 * dy1) + Math.Sqrt(dx2 * dx2 + dy2 * dy2);

            T len = dx1.Multiply(dx1)
                .Add(dy1.Multiply(dy1)).Sqrt().Add(dx2.Multiply(dx2).Add(dy2.Multiply(dy2)).Sqrt());


            m_num_steps = (int)len.Multiply(0.25).Multiply(m_scale).Round().ToInt();

            if (m_num_steps < 4)
            {
                m_num_steps = 4;
            }

            T subdivide_step = M.One<T>().Divide(m_num_steps);
            T subdivide_step2 = subdivide_step.Multiply(subdivide_step);

            T tmpx = x1.Subtract(x2).Multiply(2.0).Add(x3).Multiply(subdivide_step2);
            T tmpy = y1.Subtract(y2).Multiply(2.0).Add(y3).Multiply(subdivide_step2);

            m_saved_fx = m_fx = x1;
            m_saved_fy = m_fy = y1;

            m_saved_dfx = m_dfx = tmpx.Add(x2.Subtract(x1)).Multiply(subdivide_step.Multiply(2));
            m_saved_dfy = m_dfy = tmpy.Add(y2.Subtract(y1)).Multiply(subdivide_step.Multiply(2));

            m_ddfx = tmpx.Multiply(2.0);
            m_ddfy = tmpy.Multiply(2.0);

            m_step = m_num_steps;
        }


        // public void ApproximationMethod(ApproximationMethod method) { }
        public ApproximationMethod ApproximationMethod
        {
            get { return ApproximationMethod.Inc; }
            set { }
        }

        //public void ApproximationScale(double s)
        //{
        //    m_scale = s;
        //}

        public T ApproximationScale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
            }
        }

        //public void AngleTolerance(double angle) { }
        public T AngleTolerance { get { return default(T).Zero; } set { } }

        //public void CuspLimit(double limit) { }
        public T CuspLimit { get { return default(T).Zero; } set { } }

        public void Rewind(uint path_id)
        {
            if (m_num_steps == 0)
            {
                m_step = -1;
                return;
            }
            m_step = m_num_steps;
            m_fx = m_saved_fx;
            m_fy = m_saved_fy;
            m_dfx = m_saved_dfx;
            m_dfy = m_saved_dfy;
        }

        public uint Vertex(out T x, out T y)
        {
            if (m_step < 0)
            {
                x = M.Zero<T>();
                y = M.Zero<T>();
                return (uint)Path.Commands.Stop;
            }
            if (m_step == m_num_steps)
            {
                x = m_start_x;
                y = m_start_y;
                --m_step;
                return (uint)Path.Commands.MoveTo;
            }
            if (m_step == 0)
            {
                x = m_end_x;
                y = m_end_y;
                --m_step;
                return (uint)Path.Commands.LineTo;
            }
            m_fx.AddEquals(m_dfx);
            m_fy.AddEquals(m_dfy);
            m_dfx.AddEquals(m_ddfx);
            m_dfy.AddEquals(m_ddfy);
            x = m_fx;
            y = m_fy;
            --m_step;
            return (uint)Path.Commands.LineTo;
        }

    };

    //-------------------------------------------------------------curve3_div
    public sealed class Curve3Div<T> : ICurve<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_approximation_scale;
        T m_distance_tolerance_square;
        T m_angle_tolerance;
        uint m_count;
        VectorPOD<IVector<T>> m_points;

        public Curve3Div()
        {
            m_points = new VectorPOD<IVector<T>>();
            m_approximation_scale = M.One<T>();
            m_angle_tolerance = M.Zero<T>();
            m_count = (0);
        }

        public Curve3Div(T x1, T y1,
                   T x2, T y2,
                   T x3, T y3)
        {
            m_approximation_scale = M.One<T>();
            m_angle_tolerance = M.Zero<T>();
            m_count = (0);
            Init(x1, y1, x2, y2, x3, y3);
        }

        public void Reset() { m_points.RemoveAll(); m_count = 0; }
        public void Init(T x1, T y1,
                  T x2, T y2,
                  T x3, T y3)
        {
            m_points.RemoveAll();
            m_distance_tolerance_square = M.New<T>(0.5).Divide(m_approximation_scale);
            m_distance_tolerance_square.MultiplyEquals(m_distance_tolerance_square);
            Bezier(x1, y1, x2, y2, x3, y3);
            m_count = 0;
        }


        //public void ApproximationMethod(ApproximationMethod method) { }
        public ApproximationMethod ApproximationMethod { get { return ApproximationMethod.Div; } set { } }

        //public void ApproximationScale(double s) { m_approximation_scale = s; }
        public T ApproximationScale { get { return m_approximation_scale; } set { m_approximation_scale = value; } }

        //public void AngleTolerance(double a) { m_angle_tolerance = a; }
        public T AngleTolerance { get { return m_angle_tolerance; } set { m_angle_tolerance = value; } }

        // public void CuspLimit(double limit) { }
        public T CuspLimit { get { return M.Zero<T>(); } set { } }

        public void Rewind(uint idx)
        {
            m_count = 0;
        }

        public uint Vertex(out T x, out T y)
        {
            if (m_count >= m_points.Size())
            {
                x = M.Zero<T>();
                y = M.Zero<T>();
                return (uint)Path.Commands.Stop;
            }

            IVector<T> p = m_points[m_count++];
            x = p[0];
            y = p[1];
            return (m_count == 1) ? (uint)Path.Commands.MoveTo : (uint)Path.Commands.LineTo;
        }

        private void Bezier(T x1, T y1,
                    T x2, T y2,
                    T x3, T y3)
        {
            m_points.Add(MatrixFactory<T>.CreateVector2D(x1, y1));
            RecursiveBezier(x1, y1, x2, y2, x3, y3, 0);
            m_points.Add(MatrixFactory<T>.CreateVector2D(x3, y3));
        }

        private void RecursiveBezier(T x1, T y1,
                                        T x2, T y2,
                                        T x3, T y3,
                                        uint level)
        {
            if (level > (uint)Curves.CurveRecursionLimit.Limit)
            {
                return;
            }

            // Calculate all the mid-points of the line segments
            //----------------------
            T x12 = x1.Add(x2).Divide(2);
            T y12 = y1.Add(y2).Divide(2);
            T x23 = x2.Add(x3).Divide(2);
            T y23 = y2.Add(y3).Divide(2);
            T x123 = x12.Add(x23).Divide(2);
            T y123 = y12.Add(y23).Divide(2);

            T dx = x3.Subtract(x1);
            T dy = y3.Subtract(y1);
            T d = x2.Subtract(x3).Multiply(dy).Subtract(y2.Subtract(y3).Multiply(dx)).Abs();
            T da;

            if (d.GreaterThan(Curves.CurveCollinearityEpsilon))
            {
                // Regular case
                //-----------------
                if (d.Multiply(d).LessThanOrEqualTo(m_distance_tolerance_square.Multiply((dx.Multiply(dx).Add(dy.Multiply(dy))))))
                {
                    // If the curvature doesn't exceed the distance_tolerance Value
                    // we tend to finish subdivisions.
                    //----------------------
                    if (m_angle_tolerance.LessThan(Curves.CurveAngleToleranceEpsilon))
                    {
                        m_points.Add(MatrixFactory<T>.CreateVector2D(x123, y123));
                        return;
                    }

                    // Angle & Cusp Condition
                    //----------------------
                    da = M.Atan2(y3.Subtract(y2), x3.Subtract(x2)).Subtract(M.Atan2(y2.Subtract(y1), x2.Subtract(x1))).Abs();
                    if (da.GreaterThanOrEqualTo(M.PI<T>())) da = M.PI<T>().Multiply(2).Subtract(da);

                    if (da.LessThan(m_angle_tolerance))
                    {
                        // Finally we can stop the recursion
                        //----------------------
                        m_points.Add(MatrixFactory<T>.CreateVector2D(x123, y123));
                        return;
                    }
                }
            }
            else
            {
                // Collinear case
                //------------------
                da = dx.Multiply(dx).Add(dy.Multiply(dy));
                if (da.Equals(0))
                {
                    d = MathUtil.CalcSqDistance(x1, y1, x2, y2);
                }
                else
                {
                    d = x2.Subtract(x1).Multiply(dx).Add(y2.Subtract(y1)).Multiply(dy).Divide(da);
                    if (d.GreaterThan(0) && d.LessThan(1))
                    {
                        // Simple collinear case, 1---2---3
                        // We can leave just two endpoints
                        return;
                    }
                    if (d.LessThanOrEqualTo(0)) d = MathUtil.CalcSqDistance(x2, y2, x1, y1);
                    else if (d.GreaterThanOrEqualTo(1)) d = MathUtil.CalcSqDistance(x2, y2, x3, y3);
                    else d = MathUtil.CalcSqDistance(x2, y2, x1.Add(d.Multiply(dx)), y1.Add(d.Multiply(dy)));
                }
                if (d.LessThan(m_distance_tolerance_square))
                {
                    m_points.Add(MatrixFactory<T>.CreateVector2D(x2, y2));
                    return;
                }
            }

            // Continue subdivision
            //----------------------
            RecursiveBezier(x1, y1, x12, y12, x123, y123, level + 1);
            RecursiveBezier(x123, y123, x23, y23, x3, y3, level + 1);
        }
    };

    //-------------------------------------------------------------curve4_points
    public sealed class Curve4Points<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T[] cp = new T[8];
        public Curve4Points() { }
        public Curve4Points(T x1, T y1,
                      T x2, T y2,
                      T x3, T y3,
                      T x4, T y4)
        {
            cp[0] = x1; cp[1] = y1; cp[2] = x2; cp[3] = y2;
            cp[4] = x3; cp[5] = y3; cp[6] = x4; cp[7] = y4;
        }
        public void init(T x1, T y1,
                  T x2, T y2,
                  T x3, T y3,
                  T x4, T y4)
        {
            cp[0] = x1; cp[1] = y1; cp[2] = x2; cp[3] = y2;
            cp[4] = x3; cp[5] = y3; cp[6] = x4; cp[7] = y4;
        }

        public T this[int i]
        {
            get
            {
                return cp[i];
            }
        }

        //double  operator [] (uint i){ return cp[i]; }
        //double& operator [] (uint i)       { return cp[i]; }
    };


    //-------------------------------------------------------------curve4_inc
    public sealed class Curve4Inc<T> : ICurve<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        int m_num_steps;
        int m_step;
        T m_scale;
        T m_start_x;
        T m_start_y;
        T m_end_x;
        T m_end_y;
        T m_fx;
        T m_fy;
        T m_dfx;
        T m_dfy;
        T m_ddfx;
        T m_ddfy;
        T m_dddfx;
        T m_dddfy;
        T m_saved_fx;
        T m_saved_fy;
        T m_saved_dfx;
        T m_saved_dfy;
        T m_saved_ddfx;
        T m_saved_ddfy;

        public Curve4Inc()
        {
            m_num_steps = (0);
            m_step = (0);
            m_scale = (M.One<T>());
        }

        public Curve4Inc(T x1, T y1,
                           T x2, T y2,
                           T x3, T y3,
                           T x4, T y4)
        {
            m_num_steps = (0);
            m_step = (0);
            m_scale = (M.One<T>());
            Init(x1, y1, x2, y2, x3, y3, x4, y4);
        }

        public Curve4Inc(Curve4Points<T> cp)
        {
            m_num_steps = (0);
            m_step = (0);
            m_scale = (M.One<T>());
            Init(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
        }

        public void Reset() { m_num_steps = 0; m_step = -1; }
        public void Init(T x1, T y1,
                            T x2, T y2,
                            T x3, T y3,
                            T x4, T y4)
        {
            m_start_x = x1;
            m_start_y = y1;
            m_end_x = x4;
            m_end_y = y4;

            T dx1 = x2.Subtract(x1);
            T dy1 = y2.Subtract(y1);
            T dx2 = x3.Subtract(x2);
            T dy2 = y3.Subtract(y2);
            T dx3 = x4.Subtract(x3);
            T dy3 = y4.Subtract(y3);

            T len = M.Length(dx1, dy1).Add(
                    M.Length(dx2, dy2)).Add(
                    M.Length(dx3, dy3)).Multiply(0.25).Multiply(m_scale);

            m_num_steps = (int)len.Round().ToInt();

            if (m_num_steps < 4)
            {
                m_num_steps = 4;
            }

            T subdivide_step = M.One<T>().Divide(m_num_steps);
            T subdivide_step2 = subdivide_step.Multiply(subdivide_step);
            T subdivide_step3 = subdivide_step.Multiply(subdivide_step).Multiply(subdivide_step);

            T pre1 = subdivide_step.Multiply(3);
            T pre2 = subdivide_step2.Multiply(3);
            T pre4 = subdivide_step2.Multiply(6);
            T pre5 = subdivide_step3.Multiply(6);

            T tmp1x = x1.Subtract(x2.Multiply(2.0)).Add(x3);
            T tmp1y = y1.Subtract(y2.Multiply(2.0)).Add(y3);

            T tmp2x = x2.Subtract(x3).Multiply(3.0).Subtract(x1).Add(x4);
            T tmp2y = y2.Subtract(y3).Multiply(3.0).Subtract(y1).Add(y4);

            m_saved_fx = m_fx = x1;
            m_saved_fy = m_fy = y1;

            m_saved_dfx = m_dfx = x2.Subtract(x1).Multiply(pre1).Add(tmp1x.Multiply(pre2)).Add(tmp2x.Multiply(subdivide_step3));
            m_saved_dfy = m_dfy = y2.Subtract(y1).Multiply(pre1).Add(tmp1y.Multiply(pre2)).Add(tmp2y.Multiply(subdivide_step3));

            m_saved_ddfx = m_ddfx = tmp1x.Multiply(pre4).Add(tmp2x.Multiply(pre5));
            m_saved_ddfy = m_ddfy = tmp1y.Multiply(pre4).Add(tmp2y.Multiply(pre5));

            m_dddfx = tmp2x.Multiply(pre5);
            m_dddfy = tmp2y.Multiply(pre5);

            m_step = m_num_steps;
        }


        public void Init(Curve4Points<T> cp)
        {
            Init(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
        }

        //public void ApproximationMethod(ApproximationMethod method) { }
        public ApproximationMethod ApproximationMethod { get { return ApproximationMethod.Inc; } set { } }

        //public void ApproximationScale(double s)
        //{
        //    m_scale = s;
        //}

        public T ApproximationScale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
            }
        }

        //public void AngleTolerance(double angle) { }
        public T AngleTolerance { get { return default(T).Zero; } set { } }

        //public void CuspLimit(double limit) { }
        public T CuspLimit { get { return default(T).Zero; } set { } }

        public void Rewind(uint path_id)
        {
            if (m_num_steps == 0)
            {
                m_step = -1;
                return;
            }
            m_step = m_num_steps;
            m_fx = m_saved_fx;
            m_fy = m_saved_fy;
            m_dfx = m_saved_dfx;
            m_dfy = m_saved_dfy;
            m_ddfx = m_saved_ddfx;
            m_ddfy = m_saved_ddfy;
        }

        public uint Vertex(out T x, out T y)
        {
            if (m_step < 0)
            {
                x = M.Zero<T>();
                y = M.Zero<T>();
                return (uint)Path.Commands.Stop;
            }

            if (m_step == m_num_steps)
            {
                x = m_start_x;
                y = m_start_y;
                --m_step;
                return (uint)Path.Commands.MoveTo;
            }

            if (m_step == 0)
            {
                x = m_end_x;
                y = m_end_y;
                --m_step;
                return (uint)Path.Commands.LineTo;
            }

            m_fx.AddEquals(m_dfx);
            m_fy.AddEquals(m_dfy);
            m_dfx.AddEquals(m_ddfx);
            m_dfy.AddEquals(m_ddfy);
            m_ddfx.AddEquals(m_dddfx);
            m_ddfy.AddEquals(m_dddfy);

            x = m_fx;
            y = m_fy;
            --m_step;
            return (uint)Path.Commands.LineTo;
        }

    };


    //-------------------------------------------------------------curve4_div
    public sealed class Curve4Div<T> : ICurve<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_approximation_scale;
        T m_distance_tolerance_square;
        T m_angle_tolerance;
        T m_cusp_limit;
        uint m_count;
        VectorPOD<IVector<T>> m_points;

        public Curve4Div()
        {
            m_points = new VectorPOD<IVector<T>>();
            m_approximation_scale = (M.One<T>());
            m_angle_tolerance = (M.Zero<T>());
            m_cusp_limit = (M.Zero<T>());
            m_count = (0);
        }

        public Curve4Div(T x1, T y1,
                   T x2, T y2,
                   T x3, T y3,
                   T x4, T y4)
        {
            m_approximation_scale = (M.One<T>());
            m_angle_tolerance = (M.Zero<T>());
            m_cusp_limit = (M.Zero<T>());
            m_count = (0);
            Init(x1, y1, x2, y2, x3, y3, x4, y4);
        }

        public Curve4Div(Curve4Points<T> cp)
        {
            m_approximation_scale = (M.One<T>());
            m_angle_tolerance = (M.Zero<T>());
            m_count = (0);
            Init(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
        }

        public void Reset() { m_points.RemoveAll(); m_count = 0; }
        public void Init(T x1, T y1,
                  T x2, T y2,
                  T x3, T y3,
                  T x4, T y4)
        {
            m_points.RemoveAll();
            m_distance_tolerance_square = M.New<T>(0.5).Divide(m_approximation_scale);
            m_distance_tolerance_square.MultiplyEquals(m_distance_tolerance_square);
            Bezier(x1, y1, x2, y2, x3, y3, x4, y4);
            m_count = 0;
        }


        public void Init(Curve4Points<T> cp)
        {
            Init(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
        }

        //public void ApproximationMethod(ApproximationMethod method) { }

        public ApproximationMethod ApproximationMethod
        {
            get
            {
                return ApproximationMethod.Div;
            }
            set { }
        }

        //public void ApproximationScale(double s) { m_approximation_scale = s; }
        public T ApproximationScale { get { return m_approximation_scale; } set { m_approximation_scale = value; } }

        //public void AngleTolerance(double a) { m_angle_tolerance = a; }
        public T AngleTolerance { get { return m_angle_tolerance; } set { m_angle_tolerance = value; } }

        //public void CuspLimit(double v)
        //{
        //    m_cusp_limit = (v == 0.0) ? 0.0 : Math.PI - v;
        //}

        public T CuspLimit
        {
            get
            {
                return (m_cusp_limit.Equals(0.0)) ? M.Zero<T>() : M.PI<T>().Subtract(m_cusp_limit);
            }
            set
            {
                m_cusp_limit = (value.Equals(0.0)) ? M.Zero<T>() : M.PI<T>().Subtract(value);
            }
        }

        public void Rewind(uint idx)
        {
            m_count = 0;
        }

        public uint Vertex(out T x, out T y)
        {
            if (m_count >= m_points.Size())
            {
                x = M.Zero<T>();
                y = M.Zero<T>();
                return (uint)Path.Commands.Stop;
            }
            IVector<T> p = m_points[m_count++];
            x = p[0];
            y = p[1];
            return (m_count == 1) ? (uint)Path.Commands.MoveTo : (uint)Path.Commands.LineTo;
        }

        private void Bezier(T x1, T y1,
                    T x2, T y2,
                    T x3, T y3,
                    T x4, T y4)
        {
            m_points.Add(MatrixFactory<T>.CreateVector2D(x1, y1));
            RecursiveBezier(x1, y1, x2, y2, x3, y3, x4, y4, 0);
            m_points.Add(MatrixFactory<T>.CreateVector2D(x4, y4));
        }


        private void RecursiveBezier(T x1, T y1,
                              T x2, T y2,
                              T x3, T y3,
                              T x4, T y4,
                              uint level)
        {
            if (level > (uint)Curves.CurveRecursionLimit.Limit)
            {
                return;
            }

            // Calculate all the mid-points of the line segments
            //----------------------
            T x12 = x1.Add(x2).Divide(2);
            T y12 = y1.Add(y2).Divide(2);
            T x23 = x2.Add(x3).Divide(2);
            T y23 = y2.Add(y3).Divide(2);
            T x34 = x3.Add(x4).Divide(2);
            T y34 = y3.Add(y4).Divide(2);
            T x123 = x12.Add(x23).Divide(2);
            T y123 = y12.Add(y23).Divide(2);
            T x234 = x23.Add(x34).Divide(2);
            T y234 = y23.Add(y34).Divide(2);
            T x1234 = x123.Add(x234).Divide(2);
            T y1234 = y123.Add(y234).Divide(2);


            // Try to approximate the full cubic curve by a single straight line
            //------------------
            T dx = x4.Subtract(x1);
            T dy = y4.Subtract(y1);

            T d2 = x2.Subtract(x4).Multiply(dy).Subtract(y2.Subtract(y4).Multiply(dx)).Abs();
            T d3 = x3.Subtract(x4).Multiply(dy).Subtract(y3.Subtract(y4).Multiply(dx)).Abs();
            T da1, da2, k;

            int SwitchCase = 0;
            if (d2.GreaterThan(Curves.CurveCollinearityEpsilon))
            {
                SwitchCase = 2;
            }
            if (d3.GreaterThan(Curves.CurveCollinearityEpsilon))
            {
                SwitchCase++;
            }

            switch (SwitchCase)
            {
                case 0:
                    // All collinear OR p1==p4
                    //----------------------
                    k = dx.Multiply(dx).Add(dy.Multiply(dy));
                    if (k.Equals(0))
                    {
                        d2 = MathUtil.CalcSqDistance(x1, y1, x2, y2);
                        d3 = MathUtil.CalcSqDistance(x4, y4, x3, y3);
                    }
                    else
                    {
                        k = M.New<T>(1).Divide(k);
                        da1 = x2.Subtract(x1);
                        da2 = y2.Subtract(y1);
                        d2 = k.Multiply(da1.Multiply(dx).Add(da2.Multiply(dy)));
                        da1 = x3.Subtract(x1);
                        da2 = y3.Subtract(y1);
                        d3 = k.Multiply(da1.Multiply(dx).Subtract(da2.Multiply(dy)));
                        if (d2.GreaterThan(0) && d2.LessThan(1) && d3.GreaterThan(0) && d3.LessThan(1))
                        {
                            // Simple collinear case, 1---2---3---4
                            // We can leave just two endpoints
                            return;
                        }
                        if (d2.LessThanOrEqualTo(0)) d2 = MathUtil.CalcSqDistance(x2, y2, x1, y1);
                        else if (d2.GreaterThanOrEqualTo(1)) d2 = MathUtil.CalcSqDistance(x2, y2, x4, y4);
                        else d2 = MathUtil.CalcSqDistance(x2, y2, x1.Add(d2.Multiply(dx)), y1.Add(d2.Multiply(dy)));

                        if (d3.LessThanOrEqualTo(0)) d3 = MathUtil.CalcSqDistance(x3, y3, x1, y1);
                        else if (d3.GreaterThanOrEqualTo(1)) d3 = MathUtil.CalcSqDistance(x3, y3, x4, y4);
                        else d3 = MathUtil.CalcSqDistance(x3, y3, x1.Add(d3.Multiply(dx)), y1.Add(d3.Multiply(dy)));
                    }
                    if (d2.GreaterThan(d3))
                    {
                        if (d2.LessThan(m_distance_tolerance_square))
                        {
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x2, y2));
                            return;
                        }
                    }
                    else
                    {
                        if (d3.LessThan(m_distance_tolerance_square))
                        {
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x3, y3));
                            return;
                        }
                    }
                    break;

                case 1:
                    // p1,p2,p4 are collinear, p3 is significant
                    //----------------------
                    if (d3.Multiply(d3).LessThanOrEqualTo(m_distance_tolerance_square.Multiply((dx.Multiply(dx).Add(dy.Multiply(dy))))))
                    {
                        if (m_angle_tolerance.LessThan(Curves.CurveAngleToleranceEpsilon))
                        {
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x23, y23));
                            return;
                        }

                        // Angle Condition
                        //----------------------
                        da1 = M.Atan2(y4.Subtract(y3), x4.Subtract(x3)).Subtract(M.Atan2(y3.Subtract(y2), x3.Subtract(x2))).Abs();
                        if (da1.GreaterThanOrEqualTo(Math.PI)) da1 = M.PI<T>().Multiply(2).Subtract(da1);

                        if (da1.LessThan(m_angle_tolerance))
                        {
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x2, y2));
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x3, y3));
                            return;
                        }

                        if (!m_cusp_limit.Equals(0.0))
                        {
                            if (da1.GreaterThan(m_cusp_limit))
                            {
                                m_points.Add(MatrixFactory<T>.CreateVector2D(x3, y3));
                                return;
                            }
                        }
                    }
                    break;

                case 2:
                    // p1,p3,p4 are collinear, p2 is significant
                    //----------------------
                    if (d2.Multiply(d2).LessThanOrEqualTo(m_distance_tolerance_square.Multiply((dx.Multiply(dx).Add(dy.Multiply(dy))))))
                    {
                        if (m_angle_tolerance.LessThan(Curves.CurveAngleToleranceEpsilon))
                        {
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x23, y23));
                            return;
                        }

                        // Angle Condition
                        //----------------------
                        da1 = M.Atan2(y3.Subtract(y2), x3.Subtract(x2)).Subtract(M.Atan2(y2.Subtract(y1), x2.Subtract(x1))).Abs();
                        if (da1.GreaterThanOrEqualTo(Math.PI)) da1 = M.PI<T>().Multiply(2).Subtract(da1);

                        if (da1.LessThan(m_angle_tolerance))
                        {
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x2, y2));
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x3, y3));
                            return;
                        }

                        if (m_cusp_limit.NotEqual(0.0))
                        {
                            if (da1.GreaterThan(m_cusp_limit))
                            {
                                m_points.Add(MatrixFactory<T>.CreateVector2D(x2, y2));
                                return;
                            }
                        }
                    }
                    break;

                case 3:
                    // Regular case
                    //-----------------
                    if (d2.Add(d3).Multiply(d2.Add(d3)).LessThanOrEqualTo(m_distance_tolerance_square.Multiply(dx.Multiply(dx).Add(dy.Multiply(dy)))))
                    {
                        // If the curvature doesn't exceed the distance_tolerance Value
                        // we tend to finish subdivisions.
                        //----------------------
                        if (m_angle_tolerance.LessThan(Curves.CurveAngleToleranceEpsilon))
                        {
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x23, y23));
                            return;
                        }

                        // Angle & Cusp Condition
                        //----------------------
                        k = M.Atan2(y3.Subtract(y2), x3.Subtract(x2));
                        da1 = k.Subtract(M.Atan2(y2.Subtract(y1), x2.Subtract(x1))).Abs();
                        da2 = M.Atan2(y4.Subtract(y3), x4.Subtract(x3)).Subtract(k).Abs();
                        if (da1.GreaterThanOrEqualTo(Math.PI)) da1 = M.PI<T>().Multiply(2).Subtract(da1);
                        if (da2.GreaterThanOrEqualTo(Math.PI)) da2 = M.PI<T>().Multiply(2).Subtract(da2);

                        if (da1.Add(da2).LessThan(m_angle_tolerance))
                        {
                            // Finally we can stop the recursion
                            //----------------------
                            m_points.Add(MatrixFactory<T>.CreateVector2D(x23, y23));
                            return;
                        }

                        if (m_cusp_limit.NotEqual(0.0))
                        {
                            if (da1.GreaterThan(m_cusp_limit))
                            {
                                m_points.Add(MatrixFactory<T>.CreateVector2D(x2, y2));
                                return;
                            }

                            if (da2.GreaterThan(m_cusp_limit))
                            {
                                m_points.Add(MatrixFactory<T>.CreateVector2D(x3, y3));
                                return;
                            }
                        }
                    }
                    break;
            }

            // Continue subdivision
            //----------------------
            RecursiveBezier(x1, y1, x12, y12, x123, y123, x1234, y1234, level + 1);
            RecursiveBezier(x1234, y1234, x234, y234, x34, y34, x4, y4, level + 1);
        }

    };

    //-----------------------------------------------------------------curve3
    public sealed class Curve3<T> : ICurve<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        Curve3Inc<T> m_curve_inc;
        Curve3Div<T> m_curve_div;
        ApproximationMethod m_approximation_method;

        public Curve3()
        {
            m_curve_inc = new Curve3Inc<T>();
            m_curve_div = new Curve3Div<T>();
            m_approximation_method = ApproximationMethod.Div;
        }
        public Curve3(T x1, T y1,
                       T x2, T y2,
                       T x3, T y3)
            : base()
        {
            m_approximation_method = ApproximationMethod.Div;
            Init(x1, y1, x2, y2, x3, y3);
        }

        public void Reset()
        {
            m_curve_inc.Reset();
            m_curve_div.Reset();
        }

        public void Init(
                    T x1, T y1,
                    T x2, T y2,
                    T x3, T y3)
        {
            if (m_approximation_method == ApproximationMethod.Inc)
            {
                m_curve_inc.Init(x1, y1, x2, y2, x3, y3);
            }
            else
            {
                m_curve_div.Init(x1, y1, x2, y2, x3, y3);
            }
        }

        //public void ApproximationMethod(ApproximationMethod v)
        //{
        //    m_approximation_method = v;
        //}

        public ApproximationMethod ApproximationMethod
        {
            get
            {
                return m_approximation_method;
            }
            set
            {
                m_approximation_method = value;
            }
        }

        //public void ApproximationScale(double s)
        //{
        //    m_curve_inc.ApproximationScale(s);
        //    m_curve_div.ApproximationScale(s);
        //}

        public T ApproximationScale
        {
            get
            {
                return m_curve_inc.ApproximationScale;
            }
            set
            {
                m_curve_inc.ApproximationScale = value;
                m_curve_div.ApproximationScale = value;

            }
        }

        //public void AngleTolerance(double a)
        //{
        //    m_curve_div.AngleTolerance(a);
        //}

        public T AngleTolerance
        {
            get
            {
                return m_curve_div.AngleTolerance;
            }
            set
            {
                m_curve_div.AngleTolerance = value;
            }
        }

        //public void CuspLimit(double v)
        //{
        //    m_curve_div.CuspLimit(v);
        //}

        public T CuspLimit
        {
            get
            {
                return m_curve_div.CuspLimit;
            }
            set
            {
                m_curve_div.CuspLimit = value;
            }
        }

        public void Rewind(uint path_id)
        {
            if (m_approximation_method == ApproximationMethod.Inc)
            {
                m_curve_inc.Rewind(path_id);
            }
            else
            {
                m_curve_div.Rewind(path_id);
            }
        }

        public uint Vertex(out T x, out T y)
        {
            if (m_approximation_method == ApproximationMethod.Inc)
            {
                return m_curve_inc.Vertex(out x, out y);
            }
            return m_curve_div.Vertex(out x, out y);
        }
    };



    //-----------------------------------------------------------------curve4
    public sealed class Curve4<T> : ICurve<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        Curve4Inc<T> m_curve_inc;
        Curve4Div<T> m_curve_div;
        ApproximationMethod m_approximation_method;

        public Curve4()
        {
            m_curve_inc = new Curve4Inc<T>();
            m_curve_div = new Curve4Div<T>();
            m_approximation_method = ApproximationMethod.Div;
        }
        public Curve4(T x1, T y1,
               T x2, T y2,
               T x3, T y3,
               T x4, T y4)
            : base()
        {
            m_approximation_method = ApproximationMethod.Div;
            Init(x1, y1, x2, y2, x3, y3, x4, y4);
        }

        public Curve4(Curve4Points<T> cp)
        {
            m_approximation_method = ApproximationMethod.Div;
            Init(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
        }

        public void Reset()
        {
            m_curve_inc.Reset();
            m_curve_div.Reset();
        }

        public void Init(T x1, T y1,
                  T x2, T y2,
                  T x3, T y3,
                  T x4, T y4)
        {
            if (m_approximation_method == ApproximationMethod.Inc)
            {
                m_curve_inc.Init(x1, y1, x2, y2, x3, y3, x4, y4);
            }
            else
            {
                m_curve_div.Init(x1, y1, x2, y2, x3, y3, x4, y4);
            }
        }

        public void Init(Curve4Points<T> cp)
        {
            Init(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
        }

        //public void ApproximationMethod(ApproximationMethod v)
        //{
        //    m_approximation_method = v;
        //}

        public ApproximationMethod ApproximationMethod
        {
            get
            {
                return m_approximation_method;
            }
            set
            {
                m_approximation_method = value;
            }
        }

        //public void ApproximationScale(double s)
        //{
        //    m_curve_inc.ApproximationScale(s);
        //    m_curve_div.ApproximationScale(s);
        //}
        public T ApproximationScale
        {
            get { return m_curve_inc.ApproximationScale; }
            set
            {
                m_curve_inc.ApproximationScale = value;
                m_curve_div.ApproximationScale = value;
            }
        }

        //public void AngleTolerance(double v)
        //{
        //    m_curve_div.AngleTolerance(v);
        //}

        public T AngleTolerance
        {
            get
            {
                return m_curve_div.AngleTolerance;
            }
            set
            {
                m_curve_div.AngleTolerance = value;
            }
        }

        //public void CuspLimit(double v)
        //{
        //    m_curve_div.CuspLimit(v);
        //}

        public T CuspLimit
        {
            get
            {
                return m_curve_div.CuspLimit;
            }
            set
            {
                m_curve_div.CuspLimit = value;
            }
        }

        public void Rewind(uint path_id)
        {
            if (m_approximation_method == ApproximationMethod.Inc)
            {
                m_curve_inc.Rewind(path_id);
            }
            else
            {
                m_curve_div.Rewind(path_id);
            }
        }

        public uint Vertex(out T x, out T y)
        {
            if (m_approximation_method == ApproximationMethod.Inc)
            {
                return m_curve_inc.Vertex(out x, out y);
            }
            return m_curve_div.Vertex(out x, out y);
        }
    };
}