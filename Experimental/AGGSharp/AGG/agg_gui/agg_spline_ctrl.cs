using System;
using AGG.Color;
using AGG.Interpolation;
using AGG.Transform;
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
// classes spline_ctrl_impl, spline_ctrl
//
//----------------------------------------------------------------------------
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG.UI
{

    //------------------------------------------------------------------------
    // Class that can be used to create an interactive control to set up 
    // gamma arrays.
    //------------------------------------------------------------------------
    public class spline_ctrl_impl<T> : SimpleVertexSourceWidget<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        uint m_num_pnt;
        T[] m_xp = new T[32];
        T[] m_yp = new T[32];
        BSpline<T> m_spline = new BSpline<T>();
        T[] m_spline_values = new T[256];
        byte[] m_spline_values8 = new byte[256];
        T m_border_width;
        T m_border_extra;
        T m_curve_width;
        T m_point_size;
        T m_xs1;
        T m_ys1;
        T m_xs2;
        T m_ys2;
        PathStorage<T> m_curve_pnt;
        ConvStroke<T> m_curve_poly;
        Ellipse<T> m_ellipse;
        uint m_idx;
        uint m_vertex;
        T[] m_vx = new T[32];
        T[] m_vy = new T[32];
        int m_active_pnt;
        int m_move_pnt;
        T m_pdx;
        T m_pdy;
        IAffineTransformMatrix<T> m_mtx = MatrixFactory<T>.NewIdentity(VectorDimension.Two);

        public spline_ctrl_impl(T x1, T y1, T x2, T y2,
                         uint num_pnt)
            : base(x1, y1, x2, y2)
        {
            m_curve_pnt = new PathStorage<T>();
            m_curve_poly = new ConvStroke<T>(m_curve_pnt);
            m_ellipse = new Ellipse<T>();

            m_num_pnt = (num_pnt);
            m_border_width = M.One<T>();
            m_border_extra = M.Zero<T>();
            m_curve_width = M.One<T>();
            m_point_size = M.New<T>(3.0);
            m_curve_poly = new ConvStroke<T>(m_curve_pnt);
            m_idx = (0);
            m_vertex = (0);
            m_active_pnt = (-1);
            m_move_pnt = (-1);
            m_pdx = M.Zero<T>();
            m_pdy = M.Zero<T>();
            if (m_num_pnt < 4) m_num_pnt = 4;
            if (m_num_pnt > 32) m_num_pnt = 32;

            for (int i = 0; i < m_num_pnt; i++)
            {
                m_xp[i] = M.New<T>(i).Divide(m_num_pnt - 1);
                m_yp[i] = M.New<T>(0.5);
            }
            calc_spline_box();
            update_spline();
            {
                m_spline.Init((int)m_num_pnt, m_xp, m_yp);
                for (int i = 0; i < 256; i++)
                {
                    m_spline_values[i] = m_spline.Get(M.New<T>(i).Divide(255.0));
                    if (m_spline_values[i].LessThan(0.0)) m_spline_values[i] = M.Zero<T>();
                    if (m_spline_values[i].GreaterThan(1.0)) m_spline_values[i] = M.One<T>();
                    m_spline_values8[i] = (byte)(m_spline_values[i].ToDouble() * 255.0);
                }
            }

        }


        // Set other parameters
        public void border_width(double t)
        {
            border_width(M.New<T>(t));
        }
        public void border_width(T t)
        {
            border_width(t, M.Zero<T>());
        }

        public void border_width(double t, double extra)
        {
            border_width(M.New<T>(t), M.New<T>(extra));
        }

        public void border_width(T t, T extra)
        {
            m_border_width = t;
            m_border_extra = extra;
            calc_spline_box();
        }

        public void curve_width(T t) { m_curve_width = t; }
        public void point_size(T s) { m_point_size = s; }

        // Event handlers. Just call them if the respective events
        // in your system occure. The functions return true if redrawing
        // is required.
        public override bool InRect(T x, T y)
        {
            GetTransform().Inverse.Transform(ref x, ref y);
            return Bounds.HitTest(x, y);
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            uint i;
            for (i = 0; i < m_num_pnt; i++)
            {
                T xp = calc_xp(i);
                T yp = calc_yp(i);
                if (MathUtil.CalcDistance(x, y, xp, yp).LessThanOrEqualTo(m_point_size.Add(1)))
                {
                    m_pdx = xp.Subtract(x);
                    m_pdy = yp.Subtract(y);
                    m_active_pnt = m_move_pnt = (int)(i);
                    mouseEvent.Handled = true;
                }
            }
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            if (m_move_pnt >= 0)
            {
                m_move_pnt = -1;
                mouseEvent.Handled = true;
            }
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);

            if (m_move_pnt >= 0)
            {
                T xp = x.Add(m_pdx);
                T yp = y.Add(m_pdy);

                set_xp((uint)m_move_pnt, xp.Subtract(m_xs1).Divide(m_xs2.Subtract(m_xs1)));
                set_yp((uint)m_move_pnt, yp.Subtract(m_ys1).Divide(m_ys2.Subtract(m_ys1)));

                update_spline();
                mouseEvent.Handled = true;
            }
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
            T kx = M.Zero<T>();
            T ky = M.Zero<T>();
            bool ret = false;
            if (m_active_pnt >= 0)
            {
                kx = m_xp[m_active_pnt];
                ky = m_yp[m_active_pnt];
                if (keyEvent.KeyCode == Keys.Left) { kx.SubtractEquals(0.001); ret = true; }
                if (keyEvent.KeyCode == Keys.Right) { kx.AddEquals(0.001); ret = true; }
                if (keyEvent.KeyCode == Keys.Down) { ky.SubtractEquals(0.001); ret = true; }
                if (keyEvent.KeyCode == Keys.Up) { ky.AddEquals(0.001); ret = true; }
            }
            if (ret)
            {
                set_xp((uint)m_active_pnt, kx);
                set_yp((uint)m_active_pnt, ky);
                update_spline();
                keyEvent.Handled = true;
            }
        }


        public void active_point(int i)
        {
            m_active_pnt = i;
        }


        public T[] spline() { return m_spline_values; }
        public byte[] spline8() { return m_spline_values8; }
        public T value(T x)
        {
            x = m_spline.Get(x);
            if (x.LessThan(0.0)) x = M.Zero<T>();
            if (x.GreaterThan(1.0)) x = M.One<T>();
            return x;
        }

        public void value(uint idx, double y)
        {
            this.value(idx, M.New<T>(y));
        }

        public void value(uint idx, T y)
        {
            if (idx < m_num_pnt)
            {
                set_yp(idx, y);
            }
        }

        public void point(uint idx, double x, double y)
        {
            point(idx, M.New<T>(x), M.New<T>(y));
        }

        public void point(uint idx, T x, T y)
        {
            if (idx < m_num_pnt)
            {
                set_xp(idx, x);
                set_yp(idx, y);
            }
        }

        public void x(uint idx, T x) { m_xp[idx] = x; }
        public void y(uint idx, T y) { m_yp[idx] = y; }
        public T x(uint idx) { return m_xp[idx]; }
        public T y(uint idx) { return m_yp[idx]; }
        public void update_spline()
        {
            m_spline.Init((int)m_num_pnt, m_xp, m_yp);
            for (int i = 0; i < 256; i++)
            {
                m_spline_values[i] = m_spline.Get(M.New<T>((double)(i) / 255.0));
                if (m_spline_values[i].LessThan(0.0)) m_spline_values[i] = M.Zero<T>();
                if (m_spline_values[i].GreaterThan(1.0)) m_spline_values[i] = M.One<T>();
                m_spline_values8[i] = (byte)(m_spline_values[i].ToDouble() * 255.0);
            }
        }


        // Vertex soutce interface
        public override uint num_paths() { return 5; }
        public override void Rewind(uint idx)
        {
            uint i;

            m_idx = idx;

            switch (idx)
            {
                default:

                case 0:                 // Background
                    m_vertex = 0;
                    m_vx[0] = Bounds.Left.Subtract(m_border_extra);
                    m_vy[0] = Bounds.Bottom.Subtract(m_border_extra);
                    m_vx[1] = Bounds.Right.Add(m_border_extra);
                    m_vy[1] = Bounds.Bottom.Subtract(m_border_extra);
                    m_vx[2] = Bounds.Right.Add(m_border_extra);
                    m_vy[2] = Bounds.Top.Add(m_border_extra);
                    m_vx[3] = Bounds.Left.Subtract(m_border_extra);
                    m_vy[3] = Bounds.Top.Add(m_border_extra);
                    break;

                case 1:                 // Border
                    m_vertex = 0;
                    m_vx[0] = Bounds.Left;
                    m_vy[0] = Bounds.Bottom;
                    m_vx[1] = Bounds.Right;
                    m_vy[1] = Bounds.Bottom;
                    m_vx[2] = Bounds.Right;
                    m_vy[2] = Bounds.Top;
                    m_vx[3] = Bounds.Left;
                    m_vy[3] = Bounds.Top;
                    m_vx[4] = Bounds.Left.Add(m_border_width);
                    m_vy[4] = Bounds.Bottom.Add(m_border_width);
                    m_vx[5] = Bounds.Left.Add(m_border_width);
                    m_vy[5] = Bounds.Top.Subtract(m_border_width);
                    m_vx[6] = Bounds.Right.Subtract(m_border_width);
                    m_vy[6] = Bounds.Top.Subtract(m_border_width);
                    m_vx[7] = Bounds.Right.Subtract(m_border_width);
                    m_vy[7] = Bounds.Bottom.Add(m_border_width);
                    break;

                case 2:                 // Curve
                    calc_curve();
                    m_curve_poly.Width = m_curve_width;
                    m_curve_poly.Rewind(0);
                    break;


                case 3:                 // Inactive points
                    m_curve_pnt.RemoveAll();
                    for (i = 0; i < m_num_pnt; i++)
                    {
                        if ((int)(i) != m_active_pnt)
                        {
                            m_ellipse.Init(calc_xp(i), calc_yp(i),
                                           m_point_size, m_point_size, 32);
                            m_curve_pnt.ConcatPath(m_ellipse);
                        }
                    }
                    m_curve_poly.Rewind(0);
                    break;


                case 4:                 // Active point
                    m_curve_pnt.RemoveAll();
                    if (m_active_pnt >= 0)
                    {
                        m_ellipse.Init(calc_xp((uint)m_active_pnt), calc_yp((uint)m_active_pnt),
                                       m_point_size, m_point_size, 32);

                        m_curve_pnt.ConcatPath(m_ellipse);
                    }
                    m_curve_poly.Rewind(0);
                    break;

            }
        }

        public override uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            uint cmd = (uint)Path.Commands.LineTo;
            switch (m_idx)
            {
                case 0:
                    if (m_vertex == 0) cmd = (uint)Path.Commands.MoveTo;
                    if (m_vertex >= 4) cmd = (uint)Path.Commands.Stop;
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 1:
                    if (m_vertex == 0 || m_vertex == 4) cmd = (uint)Path.Commands.MoveTo;
                    if (m_vertex >= 8) cmd = (uint)Path.Commands.Stop;
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 2:
                    cmd = m_curve_poly.Vertex(out x, out y);
                    break;

                case 3:
                case 4:
                    cmd = m_curve_pnt.Vertex(out x, out y);
                    break;

                default:
                    cmd = (uint)Path.Commands.Stop;
                    break;
            }

            if (!Path.IsStop(cmd))
            {
                GetTransform().Transform(ref x, ref y);
            }

            return cmd;
        }


        private void calc_spline_box()
        {
            m_xs1 = Bounds.Left.Add(m_border_width);
            m_ys1 = Bounds.Bottom.Add(m_border_width);
            m_xs2 = Bounds.Right.Subtract(m_border_width);
            m_ys2 = Bounds.Top.Subtract(m_border_width);
        }

        private void calc_curve()
        {
            int i;
            m_curve_pnt.RemoveAll();
            m_curve_pnt.MoveTo(m_xs1, m_ys1.Add(m_ys2.Subtract(m_ys1).Multiply(m_spline_values[0])));
            for (i = 1; i < 256; i++)
            {
                m_curve_pnt.LineTo(m_xs1.Add(m_xs2.Subtract(m_xs1).Multiply((double)(i) / 255.0)),
                                    m_ys1.Add(m_ys2.Subtract(m_ys1).Multiply(m_spline_values[i])));
            }
        }

        private T calc_xp(uint idx)
        {
            return m_xs1.Add(m_xs2.Subtract(m_xs1).Multiply(m_xp[idx]));
        }

        private T calc_yp(uint idx)
        {
            return m_ys1.Add(m_ys2.Subtract(m_ys1).Multiply(m_yp[idx]));
        }

        private void set_xp(uint idx, T val)
        {
            if (val.LessThan(0.0)) val = M.Zero<T>();
            if (val.GreaterThan(1.0)) val = M.One<T>();

            if (idx == 0)
            {
                val = M.Zero<T>();
            }
            else if (idx == m_num_pnt - 1)
            {
                val = M.One<T>();
            }
            else
            {
                if (val.LessThan(m_xp[idx - 1].Add(0.001))) val = m_xp[idx - 1].Add(0.001);
                if (val.GreaterThan(m_xp[idx + 1].Subtract(0.001))) val = m_xp[idx + 1].Subtract(0.001);
            }
            m_xp[idx] = val;
        }

        private void set_yp(uint idx, T val)
        {
            if (val.LessThan(0.0)) val = M.Zero<T>();
            if (val.GreaterThan(1.0)) val = M.One<T>();
            m_yp[idx] = val;
        }

    };


    //template<class rgba8> 
    public class spline_ctrl<T> : spline_ctrl_impl<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        RGBA_Bytes m_background_color;
        RGBA_Bytes m_border_color;
        RGBA_Bytes m_curve_color;
        RGBA_Bytes m_inactive_pnt_color;
        RGBA_Bytes m_active_pnt_color;

        public spline_ctrl(double x1, double y1, double x2, double y2, uint num_pnt)
            : this(M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2), num_pnt)
        {
        }

        public spline_ctrl(T x1, T y1, T x2, T y2, uint num_pnt) :
            base(x1, y1, x2, y2, num_pnt)
        {
            m_background_color = new RGBA_Bytes(1.0, 1.0, 0.9);
            m_border_color = new RGBA_Bytes(0.0, 0.0, 0.0);
            m_curve_color = new RGBA_Bytes(0.0, 0.0, 0.0);
            m_inactive_pnt_color = new RGBA_Bytes(0.0, 0.0, 0.0);
            m_active_pnt_color = new RGBA_Bytes(1.0, 0.0, 0.0);
        }

        // Set colors
        public void background_color(RGBA_Bytes c) { m_background_color = c; }
        public void border_color(RGBA_Bytes c) { m_border_color = c; }
        public void curve_color(RGBA_Bytes c) { m_curve_color = c; }
        public void inactive_pnt_color(RGBA_Bytes c) { m_inactive_pnt_color = c; }
        public void active_pnt_color(RGBA_Bytes c) { m_active_pnt_color = c; }
        public override IColorType color(uint i)
        {
            switch (i)
            {
                case 0:
                    return m_background_color;

                case 1:
                    return m_border_color;

                case 2:
                    return m_curve_color;

                case 3:
                    return m_inactive_pnt_color;

                case 4:
                    return m_active_pnt_color;

                default:
                    throw new System.IndexOutOfRangeException("You asked for a color out of range.");
            }
        }
    };
}
