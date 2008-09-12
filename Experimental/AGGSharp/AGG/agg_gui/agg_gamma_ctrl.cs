using System;
using AGG.Color;
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
// class gamma_ctrl
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
    public class gamma_ctrl_impl<T> : SimpleVertexSourceWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public gamma_ctrl_impl(double x1, double y1, double x2, double y2)
            : this(M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2))
        { }

        public gamma_ctrl_impl(T x1, T y1, T x2, T y2)
            :
            base(x1, y1, x2, y2)
        {
            m_border_width = M.New<T>(2.0);
            m_border_extra = M.New<T>(0.0);
            m_curve_width = M.New<T>(2.0);
            m_grid_width = M.New<T>(0.2);
            m_text_thickness = M.New<T>(1.5);
            m_point_size = M.New<T>(5.0);
            m_text_height = M.New<T>(9.0);
            m_xc1 = (x1);
            m_yc1 = (y1);
            m_xc2 = (x2);
            m_yc2 = y2.Subtract(m_text_height.Multiply(2.0));
            m_xt1 = (x1);
            m_yt1 = y2.Subtract(m_text_height.Multiply(2.0));
            m_xt2 = (x2);
            m_yt2 = (y2);
            m_curve_poly = new ConvStroke<T>(m_gamma_spline);
            m_text_poly = new ConvStroke<T>(m_text);
            m_idx = (0);
            m_vertex = (0);
            m_p1_active = (true);
            m_mouse_point = (0);
            m_pdx = M.New<T>(0.0);
            m_pdy = M.New<T>(0.0);
            calc_spline_box();
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
        public void grid_width(T t) { m_grid_width = t; }
        public void text_thickness(T t) { m_text_thickness = t; }

        public void text_size(double h)
        {
            text_size(M.New<T>(h));
        }
        public void text_size(T h)
        {
            m_text_height = h;
            m_yc2 = Bounds.Top.Subtract(m_text_height.Multiply(2.0));
            m_yt1 = Bounds.Top.Subtract(m_text_height.Multiply(2.0));
            calc_spline_box();
        }

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
            calc_points();

            if (MathUtil.CalcDistance(x, y, m_xp1, m_yp1).LessThanOrEqualTo(m_point_size.Add(1)))
            {
                m_mouse_point = 1;
                m_pdx = m_xp1.Subtract(x);
                m_pdy = m_yp1.Subtract(y);
                m_p1_active = true;
                mouseEvent.Handled = true;
            }

            if (MathUtil.CalcDistance(x, y, m_xp2, m_yp2).LessThanOrEqualTo(m_point_size.Add(1)))
            {
                m_mouse_point = 2;
                m_pdx = m_xp2.Subtract(x);
                m_pdy = m_yp2.Subtract(y);
                m_p1_active = false;
                mouseEvent.Handled = true;
            }
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            if (m_mouse_point != 0)
            {
                m_mouse_point = 0;
                mouseEvent.Handled = true;
            }
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);

            if (m_mouse_point == 1)
            {
                m_xp1 = x.Add(m_pdx);
                m_yp1 = y.Add(m_pdy);
                calc_values();
                mouseEvent.Handled = true;
            }
            if (m_mouse_point == 2)
            {
                m_xp2 = x.Add(m_pdx);
                m_yp2 = y.Add(m_pdy);
                calc_values();
                mouseEvent.Handled = true;
            }
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
            T kx1, ky1, kx2, ky2;
            bool ret = false;
            m_gamma_spline.values(out kx1, out ky1, out kx2, out ky2);
            if (m_p1_active)
            {
                if (keyEvent.KeyCode == Keys.Left) { kx1.SubtractEquals(0.005); ret = true; }
                if (keyEvent.KeyCode == Keys.Right) { kx1.AddEquals(0.005); ret = true; }
                if (keyEvent.KeyCode == Keys.Down) { ky1.SubtractEquals(0.005); ret = true; }
                if (keyEvent.KeyCode == Keys.Up) { ky1.AddEquals(0.005); ret = true; }
            }
            else
            {
                if (keyEvent.KeyCode == Keys.Left) { kx2.AddEquals(0.005); ret = true; }
                if (keyEvent.KeyCode == Keys.Right) { kx2.SubtractEquals(0.005); ret = true; }
                if (keyEvent.KeyCode == Keys.Down) { ky2.AddEquals(0.005); ret = true; }
                if (keyEvent.KeyCode == Keys.Up) { ky2.SubtractEquals(0.005); ret = true; }
            }
            if (ret)
            {
                m_gamma_spline.values(kx1, ky1, kx2, ky2);
                keyEvent.Handled = true;
            }
        }


        public void change_active_point()
        {
            m_p1_active = m_p1_active ? false : true;
        }


        // A copy of agg::gamma_spline interface
        public void values(T kx1, T ky1, T kx2, T ky2)
        {
            m_gamma_spline.values(kx1, ky1, kx2, ky2);
        }

        public void values(out T kx1, out T ky1, out T kx2, out T ky2)
        {
            m_gamma_spline.values(out kx1, out ky1, out kx2, out ky2);
        }

        public byte[] gamma() { return m_gamma_spline.gamma(); }
        public T y(T x) { return m_gamma_spline.y(x); }
        //public double operator() (double x) { return m_gamma_spline.y(x); }
        public gamma_spline<T> get_gamma_spline() { return m_gamma_spline; }

        // Vertex soutce interface
        public override uint num_paths() { return 7; }
        public override void Rewind(uint idx)
        {
            T kx1, ky1, kx2, ky2;
            string tbuf;

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
                    m_vx[8] = m_xc1.Add(m_border_width);
                    m_vy[8] = m_yc2.Subtract(m_border_width.Multiply(0.5));
                    m_vx[9] = m_xc2.Subtract(m_border_width);
                    m_vy[9] = m_yc2.Subtract(m_border_width.Multiply(0.5));
                    m_vx[10] = m_xc2.Subtract(m_border_width);
                    m_vy[10] = m_yc2.Add(m_border_width.Multiply(0.5));
                    m_vx[11] = m_xc1.Add(m_border_width);
                    m_vy[11] = m_yc2.Add(m_border_width.Multiply(0.5));
                    break;

                case 2:                 // Curve
                    m_gamma_spline.box(m_xs1, m_ys1, m_xs2, m_ys2);
                    m_curve_poly.Width = m_curve_width;
                    m_curve_poly.Rewind(0);
                    break;

                case 3:                 // Grid
                    m_vertex = 0;
                    m_vx[0] = m_xs1;
                    m_vy[0] = m_ys1.Add(m_ys2).Multiply(0.5).Subtract(m_grid_width.Multiply(0.5));
                    m_vx[1] = m_xs2;
                    m_vy[1] = m_ys1.Add(m_ys2).Multiply(0.5).Subtract(m_grid_width.Multiply(0.5));
                    m_vx[2] = m_xs2;
                    m_vy[2] = m_ys1.Add(m_ys2).Multiply(0.5).Add(m_grid_width.Multiply(0.5));
                    m_vx[3] = m_xs1;
                    m_vy[3] = m_ys1.Add(m_ys2).Multiply(0.5).Add(m_grid_width.Multiply(0.5));
                    m_vx[4] = m_xs1.Add(m_xs2).Multiply(0.5).Subtract(m_grid_width.Multiply(0.5));
                    m_vy[4] = m_ys1;
                    m_vx[5] = m_xs1.Add(m_xs2).Multiply(0.5).Subtract(m_grid_width.Multiply(0.5));
                    m_vy[5] = m_ys2;
                    m_vx[6] = m_xs1.Add(m_xs2).Multiply(0.5).Add(m_grid_width.Multiply(0.5));
                    m_vy[6] = m_ys2;
                    m_vx[7] = m_xs1.Add(m_xs2).Multiply(0.5).Add(m_grid_width.Multiply(0.5));
                    m_vy[7] = m_ys1;
                    calc_points();
                    m_vx[8] = m_xs1;
                    m_vy[8] = m_yp1.Subtract(m_grid_width.Multiply(0.5));
                    m_vx[9] = m_xp1.Subtract(m_grid_width.Multiply(0.5));
                    m_vy[9] = m_yp1.Subtract(m_grid_width.Multiply(0.5));
                    m_vx[10] = m_xp1.Subtract(m_grid_width.Multiply(0.5));
                    m_vy[10] = m_ys1;
                    m_vx[11] = m_xp1.Add(m_grid_width.Multiply(0.5));
                    m_vy[11] = m_ys1;
                    m_vx[12] = m_xp1.Add(m_grid_width.Multiply(0.5));
                    m_vy[12] = m_yp1.Add(m_grid_width.Multiply(0.5));
                    m_vx[13] = m_xs1;
                    m_vy[13] = m_yp1.Add(m_grid_width.Multiply(0.5));
                    m_vx[14] = m_xs2;
                    m_vy[14] = m_yp2.Add(m_grid_width.Multiply(0.5));
                    m_vx[15] = m_xp2.Add(m_grid_width.Multiply(0.5));
                    m_vy[15] = m_yp2.Add(m_grid_width.Multiply(0.5));
                    m_vx[16] = m_xp2.Add(m_grid_width.Multiply(0.5));
                    m_vy[16] = m_ys2;
                    m_vx[17] = m_xp2.Subtract(m_grid_width.Multiply(0.5));
                    m_vy[17] = m_ys2;
                    m_vx[18] = m_xp2.Subtract(m_grid_width.Multiply(0.5));
                    m_vy[18] = m_yp2.Subtract(m_grid_width.Multiply(0.5));
                    m_vx[19] = m_xs2;
                    m_vy[19] = m_yp2.Subtract(m_grid_width.Multiply(0.5));
                    break;

                case 4:                 // Point1
                    calc_points();
                    if (m_p1_active) m_ellipse.Init(m_xp2, m_yp2, m_point_size, m_point_size, 32);
                    else m_ellipse.Init(m_xp1, m_yp1, m_point_size, m_point_size, 32);
                    break;

                case 5:                 // Point2
                    calc_points();
                    if (m_p1_active) m_ellipse.Init(m_xp1, m_yp1, m_point_size, m_point_size, 32);
                    else m_ellipse.Init(m_xp2, m_yp2, m_point_size, m_point_size, 32);
                    break;

                case 6:                 // Text
                    m_gamma_spline.values(out kx1, out ky1, out kx2, out ky2);
                    tbuf = string.Format("{0:F3} {1:F3} {2:F3} {3:F3}", kx1, ky1, kx2, ky2);
                    m_text.Text = tbuf;
                    m_text.SetFontSize(m_text_height);
                    m_text.StartPoint(m_xt1.Add(m_border_width.Multiply(2.0)), m_yt1.Add(m_yt2).Multiply(0.5).Subtract(m_text_height.Multiply(0.5)));
                    m_text_poly.Width = m_text_thickness;
                    m_text_poly.LineJoin = LineJoin.RoundJoin;
                    m_text_poly.LineCap = LineCap.Round;
                    m_text_poly.Rewind(0);
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
                    if (m_vertex == 0 || m_vertex == 4 || m_vertex == 8) cmd = (uint)Path.Commands.MoveTo;
                    if (m_vertex >= 12) cmd = (uint)Path.Commands.Stop;
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 2:
                    cmd = m_curve_poly.Vertex(out x, out y);
                    break;

                case 3:
                    if (m_vertex == 0 ||
                       m_vertex == 4 ||
                       m_vertex == 8 ||
                       m_vertex == 14) cmd = (uint)Path.Commands.MoveTo;

                    if (m_vertex >= 20) cmd = (uint)Path.Commands.Stop;
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 4:                 // Point1
                case 5:                 // Point2
                    cmd = m_ellipse.Vertex(out x, out y);
                    break;

                case 6:
                    cmd = m_text_poly.Vertex(out x, out y);
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
            m_xs1 = m_xc1.Add(m_border_width);
            m_ys1 = m_yc1.Add(m_border_width);
            m_xs2 = m_xc2.Subtract(m_border_width);
            m_ys2 = m_yc2.Subtract(m_border_width.Multiply(0.5));
        }

        void calc_points()
        {
            T kx1, ky1, kx2, ky2;
            m_gamma_spline.values(out kx1, out ky1, out kx2, out ky2);
            m_xp1 = m_xs1.Add(m_xs2.Subtract(m_xs1).Multiply(kx1).Multiply(0.25));
            m_yp1 = m_ys1.Add(m_ys2.Subtract(m_ys1).Multiply(ky1).Multiply(0.25));
            m_xp2 = m_xs2.Subtract(m_xs2.Subtract(m_xs1).Multiply(kx2).Multiply(0.25));
            m_yp2 = m_ys2.Subtract(m_ys2.Subtract(m_ys1).Multiply(ky2).Multiply(0.25));
        }

        void calc_values()
        {
            T kx1, ky1, kx2, ky2;

            kx1 = m_xp1.Subtract(m_xs1).Multiply(4.0).Divide(m_xs2.Subtract(m_xs1));
            ky1 = m_yp1.Subtract(m_ys1).Multiply(4.0).Divide(m_ys2.Subtract(m_ys1));
            kx2 = m_xs2.Subtract(m_xp2).Multiply(4.0).Divide(m_xs2.Subtract(m_xs1));
            ky2 = m_ys2.Subtract(m_yp2).Multiply(4.0).Divide(m_ys2.Subtract(m_ys1));
            m_gamma_spline.values(kx1, ky1, kx2, ky2);
        }


        gamma_spline<T> m_gamma_spline = new gamma_spline<T>();
        T m_border_width;
        T m_border_extra;
        T m_curve_width;
        T m_grid_width;
        T m_text_thickness;
        T m_point_size;
        T m_text_height;
        T m_xc1;
        T m_yc1;
        T m_xc2;
        T m_yc2;
        T m_xs1;
        T m_ys1;
        T m_xs2;
        T m_ys2;
        T m_xt1;
        T m_yt1;
        T m_xt2;
        T m_yt2;
        ConvStroke<T> m_curve_poly;
        Ellipse<T> m_ellipse = new Ellipse<T>();
        GsvText<T> m_text = new GsvText<T>();
        ConvStroke<T> m_text_poly;
        uint m_idx;
        uint m_vertex;
        T[] m_vx = new T[32];
        T[] m_vy = new T[32];
        T m_xp1;
        T m_yp1;
        T m_xp2;
        T m_yp2;
        bool m_p1_active;
        uint m_mouse_point;
        T m_pdx;
        T m_pdy;
    };

    public class gamma_ctrl<T> : gamma_ctrl_impl<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        RGBA_Bytes m_background_color;
        RGBA_Bytes m_border_color;
        RGBA_Bytes m_curve_color;
        RGBA_Bytes m_grid_color;
        RGBA_Bytes m_inactive_pnt_color;
        RGBA_Bytes m_active_pnt_color;
        RGBA_Bytes m_text_color;
        RGBA_Bytes[] m_colors = new RGBA_Bytes[7];

        public gamma_ctrl(double x1, double y1, double x2, double y2)
            : this(M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2))
        { }

        public gamma_ctrl(T x1, T y1, T x2, T y2) :
            base(x1, y1, x2, y2)
        {
            m_background_color = new RGBA_Bytes(1.0, 1.0, 0.9);
            m_border_color = new RGBA_Bytes(0.0, 0.0, 0.0);
            m_curve_color = new RGBA_Bytes(0.0, 0.0, 0.0);
            m_grid_color = new RGBA_Bytes(0.2, 0.2, 0.0);
            m_inactive_pnt_color = new RGBA_Bytes(0.0, 0.0, 0.0);
            m_active_pnt_color = new RGBA_Bytes(1.0, 0.0, 0.0);
            m_text_color = new RGBA_Bytes(0.0, 0.0, 0.0);

            m_colors[0] = m_background_color;
            m_colors[1] = m_border_color;
            m_colors[2] = m_curve_color;
            m_colors[3] = m_grid_color;
            m_colors[4] = m_inactive_pnt_color;
            m_colors[5] = m_active_pnt_color;
            m_colors[6] = m_text_color;
        }

        // Set colors
        public void background_color(RGBA_Bytes c) { m_background_color = c; }
        public void border_color(RGBA_Bytes c) { m_border_color = c; }
        public void curve_color(RGBA_Bytes c) { m_curve_color = c; }
        public void grid_color(RGBA_Bytes c) { m_grid_color = c; }
        public void inactive_pnt_color(RGBA_Bytes c) { m_inactive_pnt_color = c; }
        public void active_pnt_color(RGBA_Bytes c) { m_active_pnt_color = c; }
        public void text_color(RGBA_Bytes c) { m_text_color = c; }
        public override IColorType color(uint i) { return m_colors[i]; }
    };
}
